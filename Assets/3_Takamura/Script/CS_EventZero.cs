using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_EventZero : MonoBehaviour
{
    [SerializeField, Header("セリフ集")]
    GameObject[] goDialogue;
    [SerializeField, Header("イベントマネージャー")]
    GameObject goEventMgr;

    public int a = 0;
    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < goDialogue.Length; i++)
        {
            goDialogue[i].SetActive(false);
        }
        goEventMgr.GetComponent<CS_EventMgr>().State = CS_EventMgr.eState.None;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return))
        {
            a++;
        }
        if(a>goDialogue.Length)
        {
            for (int i = 0; i < goDialogue.Length; i++)
            {
                goDialogue[i].SetActive(false);
            }
            goEventMgr.GetComponent<CS_EventMgr>().State = CS_EventMgr.eState.FadeHide;
        }
        if (a < 0 || a > goDialogue.Length) return;
        for(int i = 0; i < a; i++)
        {
            goDialogue[i].SetActive(true);
        }
    }
}
