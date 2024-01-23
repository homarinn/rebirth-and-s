using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_EndingMgr : MonoBehaviour
{
    //! @brief ステートマシン
    enum eState
    {
        None,
        FadeHide,   //! フェードが明ける
        Standby,    //! 待機状態
        FadeShow,   //! フェードが掛かる
    }
    [SerializeField] eState state;

    [SerializeField, Header("FadeImage")]
    CanvasGroup cgFade;
    [SerializeField, Header("フェード時間(秒)")]
    float fadeSpeed = 1.5f;
    [SerializeField, Header("クレジット01Image")]
    CanvasGroup cgCredit;
    [SerializeField, Header("1枚目のクレジットを表示する時間(秒)")]
    float delaySecond;
    [SerializeField, Header("クレジットのフェード時間(秒)")]
    float fadeCreditSpeed = 2.0f;
    [SerializeField, Header("EndingBGM")]
    AudioSource endingBGM;

    //! @brief シーン遷移可能フラグ
    bool bLoadScene;

    //! @brief ステートの変更
    //! @param nextstate:変更予定のステート
    void ChangeState(eState nextState)
    {
        switch (state)
        {
            case eState.FadeHide:
                cgFade.blocksRaycasts = false;
                cgFade.interactable = false;
                break;
            case eState.Standby:
                break;
            case eState.FadeShow:
                break;
        }

        switch (nextState)
        {
            case eState.FadeHide:
                break;
            case eState.Standby:
                if (endingBGM != null) endingBGM.Play();
                break;
            case eState.FadeShow:

                cgFade.blocksRaycasts = true;
                cgFade.interactable = true;
                break;
        }

        state = nextState;

    }
    // Start is called before the first frame update
    void Start()
    {
        state = eState.FadeHide;

        cgFade.alpha = 1.0f;
        cgFade.blocksRaycasts = true;
        cgFade.interactable = true;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case eState.FadeHide:
                cgFade.alpha -= Time.deltaTime / fadeSpeed;
                //! Fade終了
                if (cgFade.alpha <= 0.0f)
                {
                    ChangeState(eState.Standby);
                }
                break;
            case eState.Standby:
                StartCoroutine(DelayedAction(delaySecond));
                if (bLoadScene)
                {
                    if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
                    {
                        state = eState.FadeShow;
                    }
                }
                break;
            case eState.FadeShow:
                if (endingBGM != null) endingBGM.volume -= 0.1f * Time.deltaTime;
                cgFade.alpha += Time.deltaTime / fadeSpeed;
                if (cgFade.alpha >= 1.0f/* && endingBGM.volume <= 0.0f*/)
                {
                    //titleBGM.Stop();
                    SceneManager.LoadScene("TitleScene");
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChangeState(eState.FadeShow);
        }
    }

    //! @brief 指定秒数待って処理を行うコルーチン
    IEnumerator DelayedAction(float delay)
    {
        yield return new WaitForSeconds(delay);

        //! 1枚目のCreditを非表示にする
        HideCreditImage();

    }
    //! @brief 1枚目のクレジットを非表示にする
    void HideCreditImage()
    {
        cgCredit.alpha -= Time.deltaTime / fadeCreditSpeed;
        //! Fade終了
        if (cgCredit.alpha <= 0.0f)
        {
            cgCredit.alpha = 0.0f;
            bLoadScene = true;
        }
    }
}
