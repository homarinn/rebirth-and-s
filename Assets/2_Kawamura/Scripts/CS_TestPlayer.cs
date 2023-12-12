using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_TestPlayer : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 180.0f; //回転速度
    [SerializeField] float forwardSpeed; //前進速度
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        //ユーザーの操作を取得して進行方向を算出
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 Dir = new Vector3(0, 0, v);
        //上下キー方向をプレイヤーの向きで前後方向に変換
        Dir = transform.TransformDirection(Dir);
        Dir *= forwardSpeed;
        //プレイヤーの移動
        transform.position += Dir * Time.fixedDeltaTime;
        //プレイヤーの回転
        transform.Rotate(0, h * rotateSpeed * Time.fixedDeltaTime, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
