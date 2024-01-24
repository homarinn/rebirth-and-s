using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_EventMgr : MonoBehaviour
{
    //! @brief ステートマシン
    enum eState
    {
        None,
        FadeHide,   //! フェードが明ける
        Standby,    //! 待機状態
        FadeShow,   //! フェードが掛かる
    }
    eState state;

    [SerializeField, Header("FadeImage")]
    CanvasGroup cgFade;
    [SerializeField, Header("フェード時間(秒)")]
    float fadeSpeed = 1.5f;

    [SerializeField, Header("次のシーン名")]
    string nextScene;

    [SerializeField, Header("セリフのゲームオブジェクト")]
    GameObject goDialogue;

    [SerializeField, Header("EventBGM")]
    AudioSource eventBGM;

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
                goDialogue.GetComponent<CS_Dialogue>().Benable = true;
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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        eventBGM.volume = 0.0f;
        eventBGM.Play();
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
                if(eventBGM.volume <= 0.3f)
                {
                    eventBGM.volume += 0.2f * Time.deltaTime;
                }
                break;
            case eState.FadeShow:
                if (eventBGM != null) eventBGM.volume -= 0.1f * Time.deltaTime;
                cgFade.alpha += Time.deltaTime / fadeSpeed;
                var flag = true;
                if (eventBGM != null) flag = eventBGM.volume <= 0.0f;
                if (cgFade.alpha >= 1.0f && flag)
                {
                    //titleBGM.Stop();
                    SceneManager.LoadScene(nextScene);
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    //! test : ボタンクリックシーン遷移
    public void OnClickButton()
    {
        state = eState.FadeShow;
    }

    //! @brief アプリケーション終了
    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
