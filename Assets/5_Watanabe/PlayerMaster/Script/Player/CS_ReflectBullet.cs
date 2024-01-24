using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_ReflectBullet : MonoBehaviour
{
    private Transform targetTrs;

    [SerializeField, Header("’e‚Ì‘¬“x")]
    private float speed = 100;

    void Start()
    {
        gameObject.transform.parent = null;
        targetTrs = GameObject.FindGameObjectWithTag("Enemy").transform;
        var dir = targetTrs.position - transform.position;
        var lookAtRotataion = Quaternion.LookRotation(dir, Vector3.up);
        var offsetRotation = Quaternion.FromToRotation(Vector3.right, Vector3.forward);
        transform.rotation = lookAtRotataion * offsetRotation;
        GetComponent<Rigidbody>().velocity = transform.right * speed;
    }

}
