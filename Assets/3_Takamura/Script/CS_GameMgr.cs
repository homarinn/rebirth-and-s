using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CS_GameMgr : MonoBehaviour
{
    //! @brief �V�[�������
    enum eState
    {
        None,
        FadeHide,
        Game,
        FadeShow,
    }
    [SerializeField] eState state;
    
    [SerializeField, Header("FadeImage")]
    CanvasGroup cgFade;
    [SerializeField, Header("�t�F�[�h����(�b)")]
    float fadeSpeed = 1.5f;

    [SerializeField, Header("�Z���t�̃Q�[���I�u�W�F�N�g")]
    GameObject goDialogue;

    [SerializeField,Header("Player�X�N���v�g")]
    CS_Player csPlayer;
    [SerializeField, Header("Enemy��Prefab")]
    GameObject goEnemy;
    //! @brief ���l�̃X�N���v�g
    CS_Titan csTitan = null;
    //! @brief �V���@
    CS_Enemy1 csEnemy01 = null;
    //! @brief Player�~���[
    CS_EnemyPlayer csEnPlayer = null;

    float enemyHp;

    //! @brief �Q�[���I�[�o�[�t���O
    bool bGameOver;
    //! @brief �Q�[���N���A�t���O
    bool bGameClear;

    //! @brief ���̃V�[����
    string nextScene = "";
    [SerializeField, Header("���̃X�e�[�W��")]
    string nextStage;

    //! @brief BGM
    [SerializeField, Header("BGM�F�X�e�[�W")]
    AudioSource stageBGM;

    //! @brief �X�e�[�g�̕ύX
    //! @param nextstate:�ύX�\��̃X�e�[�g
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
                if(stageBGM != null) stageBGM.Play();
                if(csTitan != null)csTitan.StartMoving();
                if(goDialogue != null)goDialogue.GetComponent<CS_Dialogue>().Benable = true;
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

        bGameOver = false;
        bGameClear = false;
    }

    // Update is called once per frame
    void Update()
    {
        SetGameFlag();
        SetNextSceneName();

        StateUpdate();

        //! �I��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    //! @brief Stage�ɍ�����Enemy��HP��ݒ�
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

    //! @brief GameClear/GameOver�̃t���O�ݒ�
    void SetGameFlag()
    {
        //! PlayerScript����HP�擾
        if (csPlayer.IsDeath) 
        {
            bGameOver = true;
            if(csTitan != null)csTitan.StopMoving();
            ChangeState(eState.FadeShow);
        }
        //! EnemyScript����HP�擾

        bool isDeath = SetEnemyDeathFlag();
        if (isDeath)
        {
            bGameClear = true;
            ChangeState(eState.FadeShow);
        }
    }

    //! @brief �X�e�[�g�ɔ���UpDate����
    void StateUpdate()
    {
        switch (state)
        {
            case eState.FadeHide:
                cgFade.alpha -= Time.deltaTime / fadeSpeed;
                //! Fade�I��
                if (cgFade.alpha <= 0.0f)
                {
                    ChangeState(eState.Game);
                }
                break;
            case eState.Game:
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

    //! @brief ���̃V�[������ݒ肷��
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

    //! @brief �A�v���P�[�V�����I��
    public void Quit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}
