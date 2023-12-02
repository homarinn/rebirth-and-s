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
    [Header("�t�F�[�h����(�b)")]
    float fadeSpeed = 1.5f;

    //[SerializeField]
    //CS_Player csPlayer;
    float playerCurrentHP;
    //[SerializeField]
    //CS_Enemy csEnemy;
    float enemyCurrentHP;

    //! @brief �Q�[���I�[�o�[�t���O
    bool bGameOver;
    //! @brief �Q�[���N���A�t���O
    bool bGameClear;

    //! @brief ���̃V�[����
    string nextScene = "";
    [SerializeField, Header("���̃X�e�[�W��")]
    string nextStage;

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


    //! @brief GameClear/GameOver�̃t���O�ݒ�
    void SetGameFlag()
    {
        //! Todo:PlayerScript����HP�擾(�����ɕϊ����đ��)
        if (playerCurrentHP <= 0.0f) 
        {
            bGameOver = true;
            ChangeState(eState.FadeShow);
        }
        //! Todo:EnemyScript����HP�擾(�����ɕϊ����đ��)
        if (enemyCurrentHP <= 0.0f)
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
                cgFade.alpha += Time.deltaTime / fadeSpeed;
                if (cgFade.alpha >= 1.0f)
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
}
