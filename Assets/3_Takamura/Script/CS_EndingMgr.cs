using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_EndingMgr : MonoBehaviour
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

    [SerializeField, Header("EndingBGM")]
    AudioSource endingBGM;

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
                if (endingBGM != null) endingBGM.Play();
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
                if (endingBGM != null) endingBGM.volume -= 0.1f * Time.deltaTime;
                cgFade.alpha += Time.deltaTime / fadeSpeed;
                if (cgFade.alpha >= 1.0f/* && endingBGM.volume <= 0.0f*/)
                {
                    //titleBGM.Stop();
                    SceneManager.LoadScene("Title");
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChangeState(eState.FadeShow);
        }
    }

}
