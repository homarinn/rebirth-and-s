using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CS_Dialogue : MonoBehaviour
{
    [SerializeField, Header("名前テキストUI")]
    TextMeshProUGUI talkernameText;
    [SerializeField, Header("セリフテキストUI")]
    TextMeshProUGUI dialogueText;
    [SerializeField, Header("テキストファイル")]
    TextAsset file;
    //! @brief 読込用バッファ
    string pBuffer;
    //! @brief 改行で分割した1文
    string[] splitText;
    //! @brief 名前で分割した1文
    string[] nameText;
    //! @brief テキスト配列のインデックス
    int textIndex;

    [SerializeField, Header("次の文字を表示するまでの時間(秒)")]
    float delayDuration;
    //! @brief コルーチン
    Coroutine showCoroutine;
    //! @brief 一文表示終了フラグ
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
            
            //if (splitText[textIndex] != "")
            //{
            //    int index = splitText[textIndex].IndexOf("/");
            //    if(index != -1)
            //    {
            //        talkernameText.text = splitText[textIndex];
            //        Show();
            //        //textIndex++;
            //    }
            //    else
            //    {
            //        dialogueText.text = splitText[textIndex];
            //        Show();
            //        //textIndex++;
            //    }

            //    bFinishString = false;
                
            //    if (textIndex + 1< splitText.Length)
            //    {
            //        textIndex++;
            //    }
            //    else
            //    {
            //        //! テキスト表示終了
            //        textIndex = 0;
            //    }
            //}
            //else
            //{
            //    dialogueText.text = "";
            //    textIndex++;
            //}

            if(textIndex < splitText.Length)
            {
                string currentLine = splitText[textIndex];
                if (!string.IsNullOrEmpty(currentLine))
                {
                    int index = currentLine.IndexOf("/");
                    if(index != -1)
                    {
                        talkernameText.text = currentLine;
                        dialogueText.text = "";
                        textIndex++;
                    }
                    else
                    {
                        dialogueText.text = currentLine;
                        Show();
                        bFinishString = false;
                        textIndex++;
                    }

                }

                if (textIndex >= splitText.Length)
                {
                    textIndex = 0;
                }
            }

        }
    }

    //! @brief テキストファイルを読み込む
    bool LoadText()
    {
        pBuffer = file.text;
        if(pBuffer == null)
        {
            Debug.Log("テキストファイルが読み込めません。");
            return false;
        }
        return true;
    }
    //! @brief 改行で分割する
    void SplitString()
    {
        splitText = pBuffer.Split(char.Parse("\n"));
    }

    //! @brief 文字送り演出表示
    void Show()
    {
        if(showCoroutine != null)
        {
            StopCoroutine(showCoroutine);
        }
        //! 1文字ずつ表示するコルーチン実行
        showCoroutine = StartCoroutine(ShowCoroutine());
    }

    IEnumerator ShowCoroutine()
    {
        //! 待機用コルーチン
        var delay = new WaitForSeconds(delayDuration);

        int length = dialogueText.text.Length;

        //! 1文字ずつ表示
        for(int i = 0; i < length; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            //! 一定時間待機
            yield return delay;
        }
        //! 演出が終わったらすべての文字を表示
        dialogueText.maxVisibleCharacters = length;
        bFinishString = true;

        showCoroutine = null;
    }
}
