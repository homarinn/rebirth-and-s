using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_ReflectBullet : MonoBehaviour
{
    private Transform targetTrs;
    const float adjustPositionY = 0.3f;

    [SerializeField, Header("弾の速度")]
    private float speed = 100;
    [SerializeField, Header("ダメージ")]
    private float damage = 10;
    [SerializeField]
    private GameObject hitEffect;

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

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Enemy")
        {
            if(other.GetComponent<CS_Enemy1>() != null)
            {
                other.GetComponent<CS_Enemy1>().ReduceHp(damage);
                //エフェクト出す
                Vector3 instancePosition = other.transform.position;
                instancePosition.y += adjustPositionY * 2.5f;
                Instantiate(hitEffect, instancePosition, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

}
