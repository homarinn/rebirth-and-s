using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public float speed = 2.0f;
    public float jumpForce = 5.0f; //ジャンプ力
    public bool healFlag = false;
    public bool isJump = false;

    void Start()
    {
    }

    void Update()
    {
        //プレイヤーの移動を処理
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //ベクトルの正規化
        Vector3 inputVector = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        //プレイヤーが方向キーのほうを向く
        if (inputVector != Vector3.zero)
        {
            //Quaternion toRotation = Quaternion.LookRotation(inputVector, Vector3.up);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, Time.deltaTime * 500.0f);
        }

        //ジャンプ
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Rigidbody>().velocity = new Vector3(0, jumpForce, 0);
            isJump = true;
        }

        Vector3 movement = inputVector * speed * Time.deltaTime;
        transform.Translate(movement);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Finish")
        {
            isJump = false;
        }
    }
}
