using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_EventMgr : MonoBehaviour
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

    [SerializeField, Header("FadeImage")]
    CanvasGroup cgFade;
    [SerializeField, Header("�t�F�[�h����(�b)")]
    float fadeSpeed = 1.5f;

    [SerializeField, Header("���̃V�[����")]
    string nextScene;

    [SerializeField, Header("�Z���t�̃Q�[���I�u�W�F�N�g")]
    GameObject goDialogue;

    [SerializeField, Header("EventBGM")]
    AudioSource eventBGM;

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
                goDialogue.GetComponent<CS_Dialogue>().Benable = true;
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

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        eventBGM.volume = 0.0f;
        eventBGM.Play();
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
                if(eventBGM.volume <= 0.3f)
                {
                    eventBGM.volume += 0.2f * Time.deltaTime;
                }
                break;
            case eState.FadeShow:
                if (eventBGM != null) eventBGM.volume -= 0.1f * Time.deltaTime;
                cgFade.alpha += Time.deltaTime / fadeSpeed;
                var flag = true;
                if (eventBGM != null) flag = eventBGM.volume <= 0.0f;
                if (cgFade.alpha >= 1.0f && flag)
                {
                    //titleBGM.Stop();
                    SceneManager.LoadScene(nextScene);
                }
                break;
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
