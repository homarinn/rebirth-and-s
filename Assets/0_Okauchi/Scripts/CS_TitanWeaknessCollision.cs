using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_TitanWeaknessCollision : MonoBehaviour
{
    [SerializeField]
    private CS_Titan titan;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            titan.StartDown();
        }
    }
}
