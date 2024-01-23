using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_GameOverMgr : MonoBehaviour
{
    //! @brief �X�e�[�g�}�V��
    enum eState
    {
        None,
        FadeHide,   //! �t�F�[�h��������
        Standby,    //! �ҋ@���
        FadeShow,   //! �t�F�[�h���|����
    }
    eState state;

    [SerializeField]
    [Header("FadeImage")]
    CanvasGroup cgFade;
    [SerializeField]
    [Header("�t�F�[�h����(�b)")]
    float fadeSpeed = 1.5f;

    [SerializeField, Header("GameOverBGM")]
    AudioSource gameoverBGM;

    //! @brief ���̃V�[����
    string nextScene = "";

    //! @brief ���O�Ƀv���C�����V�[����
    public static string preScene;

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
                //! Fade�I��
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

        //! �I��
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    //! Retry�{�^���������ꂽ���̏���
    public void OnClickRetry()
    {
        ChangeState(eState.FadeShow);
        //! Todo:�X�e�[�W�ԍ��擾���K�v
        nextScene = preScene;
    }
    //! Title�{�^���������ꂽ���̏���
    public void OnClickTitle()
    {
        //! �A�v���P�[�V�����I��
        ChangeState(eState.FadeShow);
        nextScene = "TitleScene";
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

    //! @brief �v���C���̃V�[������ۑ�����
    public static void SetCurrentSceneName()
    {
        preScene = SceneManager.GetActiveScene().name;
    }
}
