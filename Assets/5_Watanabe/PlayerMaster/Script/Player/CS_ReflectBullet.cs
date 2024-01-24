using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_ReflectBullet : MonoBehaviour
{
    private Transform targetTrs;

    [SerializeField, Header("’e‚Ì‘¬“x")]
    private float speed = 1;

    void Start()
    {
        targetTrs = GameObject.FindWithTag("Enemy").transform;
        transform.LookAt(targetTrs);
        GetComponent<Rigidbody>().velocity = new Vector3(speed, 0, 0);
    }

}
