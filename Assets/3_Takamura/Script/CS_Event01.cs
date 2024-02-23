using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_Event01 : MonoBehaviour
{
    //! @brief �X�e�[�g�}�V��
    public enum eState
    {
        None,
        FadeHide,   //! �t�F�[�h��������
        Standby,    //! �ҋ@���
        FadeShow,   //! �t�F�[�h���|����
    }
    [SerializeField]
    eState state = eState.None;

    [SerializeField, Header("�܂΂���Object")]
    GameObject goMabataki;
    [SerializeField, Header("FadeImage")]
    CanvasGroup cgFade;
    [SerializeField, Header("�t�F�[�h����(�b)")]
    float fadeSpeed = 1.5f;

    //! @brief ���̃V�[����
    string nextScene = "Stage01Scene";

    [SerializeField, Header("�Z���t�̃Q�[���I�u�W�F�N�g")]
    GameObject goDialogue;

    [SerializeField, Header("EventBGM")]
    AudioSource eventBGM;
    [SerializeField]
    float test;

    bool isAudio = false;

    //! @brief �X�e�[�g�̕ύX
    //! @param nextstate:�ύX�\��̃X�e�[�g
    public void ChangeState(eState nextState)
    {
        switch (state)
        {
            case eState.FadeHide:
                break;
            case eState.Standby:
                break;
            case eState.FadeShow:
                break;
            default:
                break;
        }

        switch (nextState)
        {
            case eState.FadeHide:
                cgFade.alpha = 0.0f;
                goMabataki.GetComponent<Animator>().SetBool("bHide", true);
                break;
            case eState.Standby:
                cgFade.alpha = 0.0f;
                break;
            case eState.FadeShow:
                cgFade.blocksRaycasts = true;
                cgFade.interactable = true;
                break;
            default:
                break;
        }

        state = nextState;

    }

    // Start is called before the first frame update
    void Awake()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case eState.FadeHide:
                ChangeState(eState.Standby);
                break;
            case eState.Standby:
                if (!isAudio)
                {
                    isAudio = true;
                    eventBGM.Play();
                }

                Animator animator = goMabataki.GetComponent<Animator>();
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("FadeHide") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    goDialogue.GetComponent<CS_Dialogue>().Benable = true;
                }
                if (eventBGM.volume <= 0.3f)
                {
                    eventBGM.volume += 0.2f * Time.deltaTime;
                }
                break;
            case eState.FadeShow:
                if (eventBGM != null) 
                { 
                    eventBGM.volume -= 0.1f * Time.deltaTime;
                }
                cgFade.alpha += Time.deltaTime / fadeSpeed;
                bool tmp = eventBGM != null ? cgFade.alpha >= 1.0f && eventBGM.volume <= 0.0f : cgFade.alpha >= 1.0f;
                if (cgFade.alpha >= 1.0f && tmp)
                {
                    //titleBGM.Stop();
                    SceneManager.LoadScene(nextScene);
                }
                break;
        }

        if (goDialogue.activeSelf == false)
        {
            ChangeState(eState.FadeShow);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Quit();
        }
    }

    //! test : �{�^���N���b�N�V�[���J��
    public void OnClickButton()
    {
        state = eState.FadeShow;
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
