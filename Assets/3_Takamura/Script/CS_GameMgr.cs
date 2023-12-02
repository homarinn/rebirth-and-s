using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CS_GameMgr : MonoBehaviour
{
    enum eState
    {
        None,
        FadeHide,
        Game,
        FadeShow,
    }
    eState state;

    [SerializeField]
    [Header("FadeImage")]
    CanvasGroup cgFade;
    [SerializeField]
    [Header("フェード時間(秒)")]
    float fadeSpeed = 1.5f;

    //[SerializeField]
    //CS_Player csPlayer;
    float playerCurrentHP;
    //[SerializeField]
    //CS_Enemy csEnemy;
    float enemyCurrentHP;

    //! @brief ゲームオーバーフラグ
    bool bGameOver;
    //! @brief ゲームクリアフラグ
    bool bGameClear;

    //! @brief 次のシーン名
    string nextScene = "";
    [SerializeField, Header("次のステージ名")]
    string nextStage;

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
            case eState.Game:
                break;
            case eState.FadeShow:
                break;
        }

        switch (nextState)
        {
            case eState.FadeHide:
                break;
            case eState.Game:
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

        //! test
        playerCurrentHP = 1.0f;
        enemyCurrentHP = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        SetGameFlag();
        SetNextSceneName();

        StateUpdate();

        //! test
        //enemyCurrentHP -= 0.001f;
    }


    //! @brief GameClear/GameOverのフラグ設定
    void SetGameFlag()
    {
        //! Todo:PlayerScriptからHP取得(割合に変換して代入)
        if (playerCurrentHP <= 0.0f) 
        {
            bGameOver = true;
            ChangeState(eState.FadeShow);
        }
        //! Todo:EnemyScriptからHP取得(割合に変換して代入)
        if (enemyCurrentHP <= 0.0f)
        {
            bGameClear = true;
            ChangeState(eState.FadeShow);
        }
    }

    //! @brief ステートに伴うUpDate処理
    void StateUpdate()
    {
        switch (state)
        {
            case eState.FadeHide:
                cgFade.alpha -= Time.deltaTime / fadeSpeed;
                //! Fade終了
                if (cgFade.alpha <= 0.0f)
                {
                    ChangeState(eState.Game);
                }
                break;
            case eState.Game:
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

    //! @brief 次のシーン名を設定する
    void SetNextSceneName()
    {
        if (bGameClear)
        {
            nextScene = nextStage;
        }
        else if (bGameOver)
        {
            nextScene = "GameOverScene";
        }
    }
}
