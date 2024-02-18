using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_EventZero : MonoBehaviour
{
    [SerializeField, Header("セリフ集")]
    GameObject[] goDialogue;
    [SerializeField, Header("イベントマネージャー")]
    GameObject goEventMgr;
    //! @brief 表示するUIのインデックス
    int index = 1;
    [SerializeField, Header("EventSE")]
    AudioSource eventSE;
    [SerializeField, Header("SEテキストUI")]
    GameObject SEtext;
    //! @brief シーン遷移フラグ
    bool bTransition;
    //! @brief タイマー
    float time = 0.0f;
    [SerializeField, Header("セリフを表示し始めるまでの時間")]
    float enableTime;


    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < goDialogue.Length; i++)
        {
            goDialogue[i].SetActive(false);
        }
        SEtext.SetActive(false);
        goEventMgr.GetComponent<CS_Event01>().ChangeState(CS_Event01.eState.None);
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time < 1.0f) return;

        if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
        {
            //! SE再生終了でシーン遷移
            if (bTransition/* && !eventSE.isPlaying*/)
            {
                SEtext.SetActive(false);
                goEventMgr.GetComponent<CS_Event01>().ChangeState(CS_Event01.eState.Standby);
            }
            else
            {
                //! 表示するUIのインデックスをインクリメント
                index++;
            }
        }

        if(bTransition == true)
        {
            return;
        }
        if (index > goDialogue.Length)
        {
            //! UI非表示、SE再生
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
