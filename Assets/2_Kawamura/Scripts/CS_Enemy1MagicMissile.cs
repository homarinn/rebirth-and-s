using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Enemy1MagicMissile : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    Transform playerTransform;
    Rigidbody rd;
    Vector3 direction;
    bool isCanFire;  //射出可能か？

    //ゲッターセッター
    public bool GetSetIsCanFire
    {
        get { return isCanFire; }
        set { isCanFire = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        rd = GetComponent<Rigidbody>();
        playerTransform = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーに向けて弾を発射する
        if (isCanFire)
        {
            direction = playerTransform.position - transform.position;
            direction.Normalize();
            rd.velocity = direction * moveSpeed;

            isCanFire = false;
        }

        //弾の消去
        if (transform.position.y < -5.0f)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
