using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;

public class CS_Dialogue : MonoBehaviour
{
    [SerializeField, Header("名前テキストUI")]
    TextMeshProUGUI talkernameText;
    [SerializeField, Header("セリフテキストUI")]
    TextMeshProUGUI dialogueText;
    [SerializeField, Header("テキストファイル")]
    TextAsset file;
    [SerializeField, Header("敵の名前テキストカラー")]
    Color nameColor;
    //! @brief 読込用バッファ
    string pBuffer;
    //! @brief 改行で分割した1文
    string[] splitText;
    //! @brief 名前で分割した1文
    string[] nameText;
    //! @brief テキスト配列のインデックス
    int textIndex;
    public int TextIndex
    {
        get { return textIndex; }
    }
    [SerializeField, Header("SE")]
    AudioSource se;
    [SerializeField, Header("次の文字を表示するまでの時間(秒)")]
    float delayDuration;
    [SerializeField, Header("1文表示完了後の待機時間(秒)")]
    float waitSecond;
    //! @brief コルーチン
    Coroutine showCoroutine;
    //! @brief 一文表示終了フラグ
    bool bFinishString;
    //! @brief 一度だけ通るためのフラグ
    bool bOnce = false;

    //! @brief 表示処理が有効かどうか
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
                //! 文字列を一括表示する
                StopCoroutine(showCoroutine);
                dialogueText.maxVisibleCharacters = dialogueText.text.Length;
                bFinishString = true;
            }
            else if (textIndex < splitText.Length - 1)
            {
                if (!string.IsNullOrEmpty(splitText[textIndex]))
                {
                    //! 名前判定
                    int indexName = splitText[textIndex].IndexOf("/");
                    int indexStop = splitText[textIndex].IndexOf("（");
                    int indexSE = splitText[textIndex].IndexOf("#");
                    if (indexName != -1)
                    {
                        if(splitText[textIndex].IndexOf("精霊") != -1)
                        {
                            talkernameText.color = new Color(1.0f, 1.0f, 1.0f);
                        }
                        else
                        {
                            talkernameText.color = nameColor;
                        }
                        //! 名前更新
                        talkernameText.text = splitText[textIndex].Replace("/", "");

                        //! セリフ表示
                        dialogueText.text = splitText[++textIndex];
                        Show();
                        bFinishString = false;
                        textIndex++;
                    }
                    else if (indexStop != -1)
                    {
                        //! 名前非表示
                        talkernameText.text = " ";
                        //! 括弧付きの文字列表示
                        dialogueText.text = " ";
                        Show();
                        bFinishString = false;
                        textIndex++;
                        bEnable = false;
                        bOnce = false;
                    } 
                    else if(indexSE != -1)
                    {
                        talkernameText.text = " ";
                        dialogueText.text = " ";
                        //! 文字列を一括表示する
                        StopCoroutine(showCoroutine);
                        dialogueText.maxVisibleCharacters = dialogueText.text.Length;
                        
                        //! 効果音再生
                        if(se != null)
                        {
                            se.Play();
                        }

                        bFinishString = true;
                        textIndex++;
                    }
                    else
                    {
                        //! セリフのみ更新
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
        splitText = Regex.Split(pBuffer, @"\r?\n\r?\n"); ;
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
        for(int i = 0; i <= length; i++)
        {
            dialogueText.maxVisibleCharacters = i;
            //! 一定時間待機
            yield return delay;
        }

        yield return new WaitForSeconds(waitSecond);

        //! 演出が終わったらすべての文字を表示
        dialogueText.maxVisibleCharacters = length;

        bFinishString = true;

        showCoroutine = null;
    }
}