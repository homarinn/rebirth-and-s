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
    private Transform enemyTrs;
    public bool IsHit
    {
        get
        {
            return isHit;
        }
    }

    private void Update()
    {
        if(isHit)
        {
            enemyPos = enemyTrs.position;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.gameObject.tag == "Enemy" || other.gameObject.tag == "MagicMissile")
        {
            isHit = true;
            enemyTrs = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy" || other.gameObject.tag == "MagicMissile")
        {
            isHit = false;
            enemyTrs = null;
        }

    }
}
