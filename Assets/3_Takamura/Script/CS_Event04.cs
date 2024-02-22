using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Event04 : MonoBehaviour
{
    Animator anim;
    [SerializeField, Header("dialogueのオブジェクト")]
    GameObject goDialogue;
    // Start is called before the first frame update
    void Start()
    {
        anim = this.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        int index = goDialogue.GetComponent<CS_Dialogue>().TextIndex;
        if (index == 3)
        {
            anim.SetBool("bMove04", true);
        }else if (index == 25)
        {
            anim.SetBool("bFall", true);
        }
    }
}
