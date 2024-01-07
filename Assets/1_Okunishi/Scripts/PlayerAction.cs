using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    public float speed = 2.0f;
    public float jumpForce = 5.0f; //�W�����v��
    public bool healFlag = false;
    public bool isJump = false;

    void Start()
    {
    }

    void Update()
    {
        //�v���C���[�̈ړ�������
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        //�x�N�g���̐��K��
        Vector3 inputVector = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        //�v���C���[�������L�[�̂ق�������
        if (inputVector != Vector3.zero)
        {
            //Quaternion toRotation = Quaternion.LookRotation(inputVector, Vector3.up);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, Time.deltaTime * 500.0f);
        }

        //�W�����v
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
