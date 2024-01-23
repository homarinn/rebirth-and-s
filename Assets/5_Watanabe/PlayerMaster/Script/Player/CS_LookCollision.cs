using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_LookCollision : MonoBehaviour
{

    private Vector3 enemyPos = Vector3.zero;
    public Vector3 EnemyPos
    {
        get
        {
            return enemyPos;
        }
    }
    private bool isHit = false;
    public bool IsHit
    {
        get
        {
            return isHit;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            isHit = true;
            enemyPos = other.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            isHit = false;
            enemyPos = Vector3.zero;
        }

    }
}
