using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Event04Sprit : MonoBehaviour
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
        if (index == 5)
        {
            anim.SetBool("bUp01", true);
        }
        else if (index == 6)
        {
            anim.SetBool("bUp02", true);
        }
        else if (index == 7)
        {
            anim.SetBool("bUp03", true);
        }
    }
}
