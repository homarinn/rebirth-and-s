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

    [SerializeField,Header("Player�X�N���v�g")]
    CS_Player csPlayer;
    [SerializeField, Header("Enemy��Prefab")]
    GameObject goEnemy;
    //! @brief ���l�̃X�N���v�g
    CS_Titan csTitan = null;
    //! @brief �V���@
    //! @brief Player�~���[
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
                csTitan.StartMoving();
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
        SetEnemyHP();

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
    void SetEnemyHP()
    {
        csTitan = goEnemy.GetComponent<CS_Titan>();
        if (csTitan != null)
        {
            enemyHp = csTitan.Hp;
        }
    }

    //! @brief GameClear/GameOver�̃t���O�ݒ�
    void SetGameFlag()
    {
        //! Todo:PlayerScript����HP�擾(�����ɕϊ����đ��)
        if (csPlayer.Hp <= 0.0f) 
        {
            bGameOver = true;
            csTitan.StopMoving();
            ChangeState(eState.FadeShow);
        }
        //! Todo:EnemyScript����HP�擾(�����ɕϊ����đ��)
        if (enemyHp <= 0.0f)
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
