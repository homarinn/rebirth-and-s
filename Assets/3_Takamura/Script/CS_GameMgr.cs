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
        FirstTextPart,
        Game,
        HalfFadeShow,
        HalfFadeHide,
        SecondTextPart,
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
    [SerializeField, Header("PlayerCamera�̃I�u�W�F�N�g")]
    GameObject goPlayerCamera;
    [SerializeField, Header("Enemy��Prefab")]
    GameObject goEnemy;
    //! @brief ���l�̃X�N���v�g
    CS_Titan csTitan = null;
    //! @brief �V���@
    CS_Enemy1 csEnemy01 = null;
    //! @brief Player�~���[
    CS_EnemyPlayer csEnPlayer = null;

    float enemyHp;
    float enemyMaxHp;
    [SerializeField, Header("�e�L�X�g�p�[�g�Ɉڂ�̗͂̊���")]
    float textPartHpParcentage;
    [SerializeField, Header("�e�L�X�g�p�[�g�Ɉڂ�ۂ̑҂�����")]
    float textPartWaitTime;
    float textPartWaitTimeCount;
    bool finishedTextPart = false;

    //�v���C���[�ƃG�l�~�[�̏����z�u
    Vector3 playerInitialPosition;
    Vector3 enemyInitialPosition;
    Quaternion playerInitialRotation;
    Quaternion enemyInitialRotation;

    [SerializeField, Header("PlayerUI��CanvasGroup")]
    CanvasGroup cgPlayerUI;
    [SerializeField, Header("EnemyUI��CanvasGroup")]
    CanvasGroup cgEnemyUI;
    [SerializeField, Header("UI�̃��l��Fade�����鑬�x")]
    float fadeSpeedUIAlpha;

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
            case eState.FirstTextPart:  break;
            case eState.Game:           break;
            case eState.HalfFadeShow:   break;
            case eState.HalfFadeHide:
                cgFade.blocksRaycasts = false;
                cgFade.interactable = false;
                break;
            case eState.SecondTextPart:
                finishedTextPart = true;
                break;
            case eState.FadeShow:       break;
            default:
                break;
        }

        switch (nextState)
        {
            case eState.FadeHide:
                break;
            case eState.FirstTextPart:
                if (goDialogue != null) goDialogue.GetComponent<CS_Dialogue>().Benable = true;
                break;
            case eState.Game:
                if (stageBGM != null)
                {
                    stageBGM.volume = 0.1f;
                    stageBGM.Play();
                }
                StartEnemy();
                break;
            case eState.HalfFadeShow:
                StopEnemy();
                if (stageBGM != null) stageBGM.Stop();
                cgFade.blocksRaycasts = true;
                cgFade.interactable = true;
                break;
            case eState.HalfFadeHide:
                cgPlayerUI.alpha = 0.0f;
                cgEnemyUI.alpha = 0.0f;
                InitializeTransform();
                break;
            case eState.SecondTextPart:
                if (goDialogue != null) goDialogue.GetComponent<CS_Dialogue>().Benable = true;
                break;
            case eState.FadeShow:
                if (stageBGM != null) stageBGM.Stop();
                cgFade.blocksRaycasts = true;
                cgFade.interactable = true;
                CS_GameOverMgr.SetCurrentSceneName();
                break;
            default:
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

        cgPlayerUI.alpha = 0.0f;
        cgEnemyUI.alpha = 0.0f;

        //�v���C���[�ƃG�l�~�[�̏����z�u���o���Ă���
        SetInitialTransform();

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

        //! �I��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    //! @brief Stage�ɍ�����Enemy��HP���擾
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

        Debug.Log("Hp�擾�ł��Ȃ����S");
        return 0.0f;
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
                    ChangeState(eState.FirstTextPart);
                }
                break;
            case eState.FirstTextPart:
                if (goDialogue != null)
                {
                    if (!goDialogue.GetComponent<CS_Dialogue>().Benable)
                    {
                        ChangeState(eState.Game);
                    }
                }
                break;
            case eState.Game:
                if(cgPlayerUI.alpha < 1.0f)
                {
                    cgPlayerUI.alpha += fadeSpeedUIAlpha * Time.deltaTime;
                    cgEnemyUI.alpha += fadeSpeedUIAlpha * Time.deltaTime;
                }               
                if (GetEnemyHp() / enemyMaxHp <= textPartHpParcentage && !finishedTextPart)
                {
                    ChangeState(eState.HalfFadeShow);
                }
                break;
            case eState.HalfFadeShow:
                stageBGM.volume -= 0.1f * Time.deltaTime;
                cgFade.alpha += Time.deltaTime / fadeSpeed;
                if (cgFade.alpha >= 1.0f && stageBGM.volume <= 0.0f)
                {
                    textPartWaitTimeCount += Time.deltaTime;
                    if(textPartWaitTimeCount >= textPartWaitTime)
                    {
                        ChangeState(eState.HalfFadeHide);
                    }
                }
                break;
            case eState.HalfFadeHide:
                cgFade.alpha -= Time.deltaTime / fadeSpeed;
                //! Fade�I��
                if (cgFade.alpha <= 0.0f)
                {
                    ChangeState(eState.SecondTextPart);
                }
                break;
            case eState.SecondTextPart:
                if (goDialogue != null)
                {
                    if (!goDialogue.GetComponent<CS_Dialogue>().Benable)
                    {
                        ChangeState(eState.Game);
                    }
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
            default:
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

    //�G�l�~�[�𓮂���
    private bool StartEnemy()
    {
        csEnemy01 = goEnemy.GetComponent<CS_Enemy1>();
        if (csEnemy01 != null)
        {
            return true;
        }
        csTitan = goEnemy.GetComponent<CS_Titan>();
        if (csTitan != null)
        {
            csTitan.StartMoving();
            return true;
        }
        csEnPlayer = goEnemy.GetComponent<CS_EnemyPlayer>();
        if (csEnPlayer != null)
        {
            return true;
        }
        Debug.Log("�G�l�~�[�����Ȃ����S");
        return false;
    }

    //�G�l�~�[���~�߂�
    private bool StopEnemy()
    {
        csEnemy01 = goEnemy.GetComponent<CS_Enemy1>();
        if (csEnemy01 != null)
        {
            return true;
        }
        csTitan = goEnemy.GetComponent<CS_Titan>();
        if (csTitan != null)
        {
            csTitan.StopMoving();
            return true;
        }
        csEnPlayer = goEnemy.GetComponent<CS_EnemyPlayer>();
        if (csEnPlayer != null)
        {
            return true;
        }
        Debug.Log("�G�l�~�[�~�܂�Ȃ����S");
        return false;
    }

    //�ʒu�Ǝp�����o���Ă���
    private void SetInitialTransform()
    {
        playerInitialPosition = csPlayer.gameObject.transform.position;
        playerInitialRotation = csPlayer.gameObject.transform.rotation;
        enemyInitialPosition = goEnemy.transform.position;
        enemyInitialRotation = goEnemy.transform.rotation;
    }

    //�ʒu�Ǝp���������̏�Ԃɖ߂�
    private void InitializeTransform()
    {
        csPlayer.gameObject.transform.position = playerInitialPosition;
        csPlayer.gameObject.transform.rotation = playerInitialRotation;
        goEnemy.transform.position = enemyInitialPosition;
        goEnemy.transform.rotation = enemyInitialRotation;
        goPlayerCamera.transform.rotation = Quaternion.identity;
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
