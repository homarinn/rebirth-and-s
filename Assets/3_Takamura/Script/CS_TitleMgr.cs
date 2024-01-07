using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_TitleMgr : MonoBehaviour
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

    [SerializeField,Header("TitleBGM")]
    AudioSource titleBGM;

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
                titleBGM.Play();
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
                break;
            case eState.FadeShow:
                titleBGM.volume -= 0.1f * Time.deltaTime;
                cgFade.alpha += Time.deltaTime / fadeSpeed;
                if (cgFade.alpha >= 1.0f && titleBGM.volume <= 0.0f)
                {
                    //titleBGM.Stop();
                    SceneManager.LoadScene("Event01Scene");
                }
                break;
        }
    }

    //! Startボタンが押された時の処理
    public void OnClickStart()
    {
        ChangeState(eState.FadeShow);
    }
    //! Quitボタンが押された時の処理
    public void OnClickQuit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
