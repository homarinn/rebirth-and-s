using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class CS_Dialogue : MonoBehaviour
{
    [SerializeField, Header("���O�e�L�X�gUI")]
    TextMeshProUGUI talkernameText;
    [SerializeField, Header("�Z���t�e�L�X�gUI")]
    TextMeshProUGUI dialogueText;
    [SerializeField, Header("�e�L�X�g�t�@�C��")]
    TextAsset file;
    //! @brief �Ǎ��p�o�b�t�@
    string pBuffer;
    //! @brief ���s�ŕ�������1��
    string[] splitText;
    //! @brief ���O�ŕ�������1��
    string[] nameText;
    //! @brief �e�L�X�g�z��̃C���f�b�N�X
    int textIndex;
    public int TextIndex
    {
        get { return textIndex; }
    }
    [SerializeField, Header("���̕�����\������܂ł̎���(�b)")]
    float delayDuration;
    [SerializeField, Header("1���\��������̑ҋ@����(�b)")]
    float waitSecond;
    //! @brief �R���[�`��
    Coroutine showCoroutine;
    //! @brief �ꕶ�\���I���t���O
    bool bFinishString;

    bool bOnce = false;

    //! @brief �\���������L�����ǂ���
    bool bEnable = false;
    public bool Benable
    {
        get { return bEnable; }
        set { bEnable = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        //bEnable = true;
        bFinishString = true;
        LoadText();
        SplitString();
    }

    // Update is called once per frame
    void Update()
    {
        if (bEnable == false) return;

        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return) || bOnce == false) 
        {
            bOnce = true;
            if (!bFinishString)
            {
                //! ��������ꊇ�\������
                StopCoroutine(showCoroutine);
                dialogueText.maxVisibleCharacters = dialogueText.text.Length;
                bFinishString = true;
            }
            else if (textIndex < splitText.Length - 1)
            {
                if (!string.IsNullOrEmpty(splitText[textIndex]))
                {
                    //! ���O����
                    int index = splitText[textIndex].IndexOf("/");
                    int index0 = splitText[textIndex].IndexOf("�i");
                    if (index != -1)
                    {
                        //! ���O�X�V
                        talkernameText.text = splitText[textIndex].Replace("/", "");

                        //! �Z���t�\��
                        dialogueText.text = splitText[++textIndex];
                        Show();
                        bFinishString = false;
                        textIndex++;
                    }
                    else if (index0 != -1)
                    {
                        //! ���O��\��
                        talkernameText.text = " ";
                        //! ���ʕt���̕�����\��
                        dialogueText.text = " ";
                        Show();
                        bFinishString = false;
                        textIndex++;
                        bEnable = false;
                    }
                    else
                    {
                        //! �Z���t�̂ݍX�V
                        dialogueText.text = splitText[textIndex];
                        Show();
                        bFinishString = false;
                        textIndex++;
                    }
                }
            }
            else
            {
                this.gameObject.SetActive(false);
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
        splitText = Regex.Split(pBuffer, @"\r?\n\r?\n"); ;
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
        for(int i = 0; i <= length; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            //! ��莞�ԑҋ@
            yield return delay;
        }

        yield return new WaitForSeconds(waitSecond);

        //! ���o���I������炷�ׂĂ̕�����\��
        dialogueText.maxVisibleCharacters = length;

        bFinishString = true;

        showCoroutine = null;
    }
}