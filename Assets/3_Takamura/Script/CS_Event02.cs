using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Event02 : MonoBehaviour
{
    Animator anim;
    [SerializeField, Header("dialogue�̃I�u�W�F�N�g")]
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
        if(index == 3)
        {
            anim.SetBool("bMove02", true);
        }
    }
}