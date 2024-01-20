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
    [SerializeField] eState state;

    [SerializeField, Header("FadeImage")]
    CanvasGroup cgFade;
    [SerializeField, Header("�t�F�[�h����(�b)")]
    float fadeSpeed = 1.5f;
    [SerializeField, Header("�N���W�b�g01Image")]
    CanvasGroup cgCredit;
    [SerializeField, Header("1���ڂ̃N���W�b�g��\�����鎞��(�b)")]
    float delaySecond;
    [SerializeField, Header("�N���W�b�g�̃t�F�[�h����(�b)")]
    float fadeCreditSpeed = 2.0f;
    [SerializeField, Header("EndingBGM")]
    AudioSource endingBGM;

    //! @brief �V�[���J�ډ\�t���O
    bool bLoadScene;

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
                StartCoroutine(DelayedAction(delaySecond));
                if (bLoadScene)
                {
                    if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
                    {
                        state = eState.FadeShow;
                    }
                }
                break;
            case eState.FadeShow:
                if (endingBGM != null) endingBGM.volume -= 0.1f * Time.deltaTime;
                cgFade.alpha += Time.deltaTime / fadeSpeed;
                if (cgFade.alpha >= 1.0f/* && endingBGM.volume <= 0.0f*/)
                {
                    //titleBGM.Stop();
                    SceneManager.LoadScene("TitleScene");
                }
                break;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            ChangeState(eState.FadeShow);
        }
    }

    //! @brief �w��b���҂��ď������s���R���[�`��
    IEnumerator DelayedAction(float delay)
    {
        yield return new WaitForSeconds(delay);

        //! 1���ڂ�Credit���\���ɂ���
        HideCreditImage();

    }
    //! @brief 1���ڂ̃N���W�b�g���\���ɂ���
    void HideCreditImage()
    {
        cgCredit.alpha -= Time.deltaTime / fadeCreditSpeed;
        //! Fade�I��
        if (cgCredit.alpha <= 0.0f)
        {
            cgCredit.alpha = 0.0f;
            bLoadScene = true;
        }
    }
}
