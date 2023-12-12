using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_TestPlayer : MonoBehaviour
{
    [SerializeField] float rotateSpeed = 180.0f; //��]���x
    [SerializeField] float forwardSpeed; //�O�i���x
    void Start()
    {
        
    }

    void FixedUpdate()
    {
        //���[�U�[�̑�����擾���Đi�s�������Z�o
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 Dir = new Vector3(0, 0, v);
        //�㉺�L�[�������v���C���[�̌����őO������ɕϊ�
        Dir = transform.TransformDirection(Dir);
        Dir *= forwardSpeed;
        //�v���C���[�̈ړ�
        transform.position += Dir * Time.fixedDeltaTime;
        //�v���C���[�̉�]
        transform.Rotate(0, h * rotateSpeed * Time.fixedDeltaTime, 0);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
