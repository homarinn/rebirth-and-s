using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_TitanAnimatorEvent : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayWalkingSE()
    {
        CS_Titan titan = GetComponentInParent<CS_Titan>();
        titan.PlayWalkingSE();
    }
}
