using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Event04Wink : MonoBehaviour
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
        if (index == 8)
        {
            anim.SetBool("bShow", true);
        }
        else if (index == 10)
        {
            anim.SetBool("bHalf", true);
        }
    }
}
