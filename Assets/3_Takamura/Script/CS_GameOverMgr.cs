using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_GameOverMgr : MonoBehaviour
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

    [SerializeField]
    [Header("FadeImage")]
    CanvasGroup cgFade;
    [SerializeField]
    [Header("フェード時間(秒)")]
    float fadeSpeed = 1.5f;

    //! @brief 次のシーン名
    string nextScene = "";


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
                cgFade.alpha += Time.deltaTime / fadeSpeed;
                if (cgFade.alpha >= 1.0f)
                {
                    SceneManager.LoadScene(nextScene);
                }
                break;
        }
    }

    //! Retryボタンが押された時の処理
    public void OnClickRetry()
    {
        ChangeState(eState.FadeShow);
        //! Todo:ステージ番号取得が必要
        nextScene = "Stage01Scene";
    }
    //! Titleボタンが押された時の処理
    public void OnClickTitle()
    {
        //! アプリケーション終了
        ChangeState(eState.FadeShow);
        nextScene = "TitleScene";
    }
}
