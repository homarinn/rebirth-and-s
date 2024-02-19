using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CS_GameMgr : MonoBehaviour
{
    //! @brief シーン内状態
    enum eState
    {
        None,
        FadeHide,
        TextPart,
        Game,
        FadeShow,
    }
    [SerializeField] eState state;
    
    [SerializeField, Header("FadeImage")]
    CanvasGroup cgFade;
    [SerializeField, Header("フェード時間(秒)")]
    float fadeSpeed = 1.5f;

    [SerializeField, Header("セリフのゲームオブジェクト")]
    GameObject goDialogue;

    [SerializeField,Header("Playerスクリプト")]
    CS_Player csPlayer;
    [SerializeField, Header("EnemyのPrefab")]
    GameObject goEnemy;
    //! @brief 巨人のスクリプト
    CS_Titan csTitan = null;
    //! @brief シヴァ
    CS_Enemy1 csEnemy01 = null;
    //! @brief Playerミラー
    CS_EnemyPlayer csEnPlayer = null;

    float enemyHp;
    float enemyMaxHp;
    [SerializeField, Header("テキストパートに移る体力の割合")]
    float textPartHpParcentage;

    [SerializeField, Header("プレイヤーの初期位置")]
    Vector3 playerInitialPosition;
    [SerializeField, Header("エネミーの初期位置")]
    Vector3 enemyInitialPosition;

    //! @brief ゲームオーバーフラグ
    bool bGameOver;
    //! @brief ゲームクリアフラグ
    bool bGameClear;

    //! @brief 次のシーン名
    string nextScene = "";
    [SerializeField, Header("次のステージ名")]
    string nextStage;

    //! @brief BGM
    [SerializeField, Header("BGM：ステージ")]
    AudioSource stageBGM;

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
            case eState.TextPart:
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
            case eState.TextPart:
                if (goDialogue != null) goDialogue.GetComponent<CS_Dialogue>().Benable = true;
                break;
            case eState.Game:
                if(stageBGM != null) stageBGM.Play();
                if(csTitan != null)csTitan.StartMoving();
                break;
            case eState.FadeShow:
                if (stageBGM != null) stageBGM.Stop();
                cgFade.blocksRaycasts = true;
                cgFade.interactable = true;
                CS_GameOverMgr.SetCurrentSceneName();
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

        enemyMaxHp = GetEnemyHp();

        bGameOver = false;
        bGameClear = false;
    }

    // Update is called once per frame
    void Update()
    {
        SetGameFlag();
        SetNextSceneName();

        StateUpdate();

        //! 終了
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    //! @brief Stageに合ったEnemyのHPを取得
    float GetEnemyHp()
    {
        csEnemy01 = goEnemy.GetComponent<CS_Enemy1>();
        if (csEnemy01 != null)
        {
            return csEnemy01.GetHp;
        }
        csTitan = goEnemy.GetComponent<CS_Titan>();
        if (csTitan != null)
        {
            return csTitan.Hp;
        }
        csEnPlayer = goEnemy.GetComponent<CS_EnemyPlayer>();
        if (csEnPlayer != null)
        {
            return csEnPlayer.Hp;
        }

        Debug.Log("Hp取得できないンゴ");
        return 0.0f;
    }

    //! @brief Stageに合ったEnemyのHPを設定
    bool SetEnemyDeathFlag()
    {
        csEnemy01 = goEnemy.GetComponent<CS_Enemy1>();
        if(csEnemy01 != null)
        {
            return csEnemy01.GetIsDead;
        }
        csTitan = goEnemy.GetComponent<CS_Titan>();
        if (csTitan != null)
        {
            return csTitan.isDead;
        }
        csEnPlayer = goEnemy.GetComponent<CS_EnemyPlayer>();
        if(csEnPlayer != null)
        {
            return  csEnPlayer.IsDead;
        }
        return false;
    }

    //! @brief GameClear/GameOverのフラグ設定
    void SetGameFlag()
    {
        //! PlayerScriptからHP取得
        if (csPlayer.IsDeath) 
        {
            bGameOver = true;
            if(csTitan != null)csTitan.StopMoving();
            ChangeState(eState.FadeShow);
        }
        //! EnemyScriptからHP取得

        bool isDeath = SetEnemyDeathFlag();
        if (isDeath)
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
                    ChangeState(eState.TextPart);
                }
                break;
            case eState.TextPart:
                if (goDialogue != null)
                {
                    if(!goDialogue.GetComponent<CS_Dialogue>().Benable)
                    {
                        ChangeState(eState.Game);
                    }
                }
                break;
            case eState.Game:
                if (GetEnemyHp() / enemyMaxHp <= textPartHpParcentage)
                {
                    ChangeState(eState.TextPart);
                }
                break;
            case eState.FadeShow:
                stageBGM.volume -= 0.1f * Time.deltaTime;
                cgFade.alpha += Time.deltaTime / fadeSpeed;
                if (cgFade.alpha >= 1.0f && stageBGM.volume <= 0.0f)
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

    private void StartEnemy()
    {

    }

    private void StopEnemy()
    {

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
