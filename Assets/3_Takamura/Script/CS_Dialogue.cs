using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CS_Dialogue : MonoBehaviour
{
    [SerializeField, Header("�e�L�X�gUI")]
    TextMeshProUGUI dialogueText;
    [SerializeField, Header("�e�L�X�g�t�@�C��")]
    TextAsset file;
    //! @brief �Ǎ��p�o�b�t�@
    string pBuffer;
    //! @brief ���s�ŕ���
    string[] splitText;
    //! @brief �e�L�X�g�z��̃C���f�b�N�X
    int textIndex;

    [SerializeField, Header("���̕�����\������܂ł̎���(�b)")]
    float delayDuration;
    //! @brief �R���[�`��
    Coroutine showCoroutine;
    //! @brief �ꕶ�\���I���t���O
    bool bFinishString;

    // Start is called before the first frame update
    void Start()
    {
        bFinishString = true;
        LoadText();
        SplitString();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) && bFinishString == true)
        {
            if (splitText[textIndex] != "")
            {
                dialogueText.text = splitText[textIndex];
                Show();

                bFinishString = false;
                textIndex++;
                if (textIndex >= splitText.Length)
                {
                    //! �e�L�X�g�\���I��
                    textIndex = 0;
                }
            }
            else
            {
                dialogueText.text = "";
                textIndex++;
            }

        }
    }

    //! @brief �e�L�X�g�t�@�C����ǂݍ���
    bool LoadText()
    {
        pBuffer = file.text;
        if(pBuffer == null)
        {
            Debug.Log("�e�L�X�g�t�@�C�����ǂݍ��߂܂���B");
            return false;
        }
        return true;
    }
    //! @brief ���s�ŕ�������
    void SplitString()
    {
        splitText = pBuffer.Split(char.Parse("\n"));
    }

    //! @brief �������艉�o�\��
    void Show()
    {
        if(showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
        }
        //! 1�������\������R���[�`�����s
        showCoroutine = StartCoroutine(ShowCoroutine());
    }
    IEnumerator ShowCoroutine()
    {
        //! �ҋ@�p�R���[�`��
        var delay = new WaitForSeconds(delayDuration);

        int length = dialogueText.text.Length;

        //! 1�������\��
        for(int i = 0; i < length; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            //! ��莞�ԑҋ@
            yield return delay;
        }
        //! ���o���I������炷�ׂĂ̕�����\��
        dialogueText.maxVisibleCharacters = length;
        bFinishString = true;

        showCoroutine = null;
    }
}
