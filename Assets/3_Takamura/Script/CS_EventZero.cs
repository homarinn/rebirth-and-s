using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_EventZero : MonoBehaviour
{
    [SerializeField, Header("�Z���t�W")]
    GameObject[] goDialogue;
    [SerializeField, Header("�C�x���g�}�l�[�W���[")]
    GameObject goEventMgr;
    //! @brief �\������UI�̃C���f�b�N�X
    public int index = 0;
    [SerializeField, Header("EventSE")]
    AudioSource eventSE;
    [SerializeField, Header("SE�e�L�X�gUI")]
    GameObject SEtext;
    //! @brief �V�[���J�ڃt���O
    bool bTransition;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < goDialogue.Length; i++)
        {
            goDialogue[i].SetActive(false);
        }
        SEtext.SetActive(false);
        goEventMgr.GetComponent<CS_EventMgr>().State = CS_EventMgr.eState.None;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
        {
            //! SE�Đ��I���ŃV�[���J��
            if (bTransition/* && !eventSE.isPlaying*/)
            {
                SEtext.SetActive(false);
                goEventMgr.GetComponent<CS_EventMgr>().State = CS_EventMgr.eState.FadeHide;
            }
            else
            {
                //! �\������UI�̃C���f�b�N�X���C���N�������g
                index++;
            }
        }

        if(bTransition == true)
        {
            return;
        }
        if (index > goDialogue.Length)
        {
            //! UI��\���ASE�Đ�
            for (int i = 0; i < goDialogue.Length; i++)
            {
                goDialogue[i].SetActive(false);
            }
            if (eventSE != null)
            {
                eventSE.Play();
            }
            SEtext.SetActive(true);
            bTransition = true;
        }
        else
        {
            for (int i = 0; i < index; i++)
            {
                goDialogue[i].SetActive(true);
            }
        }
    }
}
