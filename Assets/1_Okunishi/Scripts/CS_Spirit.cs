using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Spirit : MonoBehaviour
{
    //[SerializeField, Header("����̒�ʒu��Transform���i�[����ϐ�")]
    [SerializeField] Transform spiritPositionTransform;         

    [SerializeField] Transform playerTransform;   //�v���C���[��Transform���i�[����ϐ�
    
    //===����̈ړ�===
    [SerializeField] private float speed = 5.0f;                //����̑��x
    [SerializeField] private float startChaseDistance = 0.5f;   //�ړ����J�n���鋗��
    private bool canMove = true;                                //���삪�����������ł������ǂ����̃t���O

    //===����̉�]===
    [SerializeField] private float rotationSpeed = 180f; //����̉�]���x
    private float startAngle = 0f;      //�����]�J�n���̊p�x
    private float currentAngle = 0f;    //����̌��݂̊p�x

    //===��===
    [SerializeField] private float healCoolTime = 0.0f;  //�񕜂̃N�[���^�C��
    [SerializeField] private float healAmount = 50.0f;   //�񕜗�
    
    //===���̑��ϐ�===
    private float distanceToSpiritPosition = 0.0f;  //����̒�ʒu�ƌ��ݒn�̍�
    private bool healFlag = false;                  //�񕜃t���O


    void Start()
    {
    }

    void Update()
    {
        if (spiritPositionTransform != null)
        {
            
            //�v���C���[�Ɛ���̋������v�Z
            distanceToSpiritPosition = Vector3.Distance(transform.position, spiritPositionTransform.position);

            //��苗���𒴂�����ړ����J�n
            if (distanceToSpiritPosition >= startChaseDistance)
            {
                canMove = true;
            }
            //��ʒu�ɖ߂��Ă�����ړ���~
            if (distanceToSpiritPosition < 0.01f)
            {
                canMove = false;
            }
            //����̒�ʒu�Ɍ������Ĉړ�����
            if (canMove && !healFlag)
            {        
                transform.position = Vector3.MoveTowards(transform.position, spiritPositionTransform.position, speed * Time.deltaTime);
            }


            //�񕜂̍ہA�v���C���[�̎�������]����
            if (healFlag)
            {
                RotateAroundPlayer();
            }

            //�N�[���^�C������
            if (healCoolTime >= 0.0f)
            {
                healCoolTime -= 0.1f * Time.deltaTime;
            }

            //===�f�o�b�O�p===
            if (Input.GetKey(KeyCode.H) && healCoolTime <= 0)
            {
                healFlag = true;
            }
        }
    }

    //�񕜂̍ہA�v���C���[�̎������]����֐�
    void RotateAroundPlayer()
    {
        //��]���x�Ɋ�Â��Ċp�x���X�V
        currentAngle += rotationSpeed * Time.deltaTime;

        //�v���C���[�̎����1���������]���I��
        if (currentAngle - startAngle >= 360f)
        {
            healFlag = false;       //�񕜏I���܂����[
            healCoolTime = 1.0f;    //�񕜃N�[���^�C����ݒ�
            currentAngle = 0;       //���݂̊p�x�����Z�b�g
            return;
        }

        //��]����
        Vector3 rotation = new Vector3(0f, currentAngle, 0f);
        transform.eulerAngles = rotation;
        transform.position = playerTransform.position + Quaternion.Euler(0f, currentAngle, 0f) * new Vector3(0f, 0.5f, -2f);  //�v���C���[�̎���𔼌a2�̉~����ɔz�u
    }

    //�񕜂���Ƃ��ɊO������Ăяo���p
    public void StartHeal()
    {
        healFlag = true;
    }
}
