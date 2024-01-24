using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Spirit : MonoBehaviour
{
    [SerializeField, Header("����̒�ʒu��Transform���i�[����")]
    Transform spiritPositionTransform;

    [SerializeField, Header("�v���C���[��Transform���i�[����")]
    Transform playerTransform;

    [SerializeField, Header("�v���C���[���擾����")]
    private CS_Player player;

    //===����̈ړ�===
    [SerializeField, Header("����̈ړ����x")]
    private float speed = 5.0f;

    [SerializeField, Header("�񕜎��̐����Y�ʒu")]
    private float healSpiritPositionY = 0.5f;

    private float startChaseDistance = 0.5f;   //�ړ����J�n���鋗��
    private bool canMove = true;               //���삪�����������ł������ǂ����̃t���O

    //===����̉�]===
    [SerializeField, Header("����̉�]���x")]
    private float rotationSpeed = 360.0f;

    private float startAngle = 0f;      //�����]�J�n���̊p�x
    private float currentAngle = 0f;    //����̌��݂̊p�x

    //===��===
    [SerializeField, Header("�񕜃N�[���^�C��(�b)")]
    private float healCoolTime = 10.0f;

    [SerializeField, Header("�񕜗�(Player�̍ő�HP��n%�����Z)")]
    private float healPercentage = 50.0f;

    [SerializeField, Header("HP��Player�̍ő�HP�̉�%�܂Ō�������񕜂��邩")]
    private float healTrrigerPercentage = 50.0f;


    //===����===
    [SerializeField, Header("�񕜉���")]
    AudioClip healing_SE;

    AudioSource spiritAudio;    //���g�̉���

    //===���̑��ϐ�===
    private float healAmount = 0.0f;                //�񕜗�
    private float currentCoolTime = 0.0f;           //���݂̃N�[���^�C��
    private bool healFlag = false;                  //�񕜃t���O
    private bool log = true;                        //�f�o�b�O�\���p

    void Start()
    {
        spiritAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (spiritPositionTransform != null)
        {
            //====================
            //=====����̈ړ�=====
            //====================
            {
                //�v���C���[�Ɛ���̋������v�Z
                float distanceToSpiritPosition = Vector3.Distance(
                    transform.position, spiritPositionTransform.position);

                //�㉺�ɂӂ�ӂ킳����
                FloatEffectWhileIdle();

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
                    transform.position = Vector3.MoveTowards(transform.position,
                        spiritPositionTransform.position, speed * Time.deltaTime);
                }
            }

            //==============================
            //=====�v���C���[��HP����=====
            //==============================
            {
                //�v���C���[��HP���w�肵������������������
                if (player.Hp <= player.MaxHP * (healTrrigerPercentage / 100.0f) && currentCoolTime <= 0.0f)
                {
                    //�񕜂���HP�̌v�Z
                    healAmount = player.MaxHP * (healPercentage / 100.0f);

                    if (player.Hp != 0)
                    {
                        healFlag = true;
                    }
                }

                //�񕜂̍ہA�v���C���[�̎�������]����
                if (healFlag)
                {
                    RotateAroundPlayer();
                }

                //�N�[���^�C������
                if (currentCoolTime >= 0.0f)
                {
                    currentCoolTime -= 1.0f * Time.deltaTime;
                }
            }

            //==============================
            //=====�f�o�b�O�p===============
            //=====�������ɕK����������=====
            //==============================
            if (Input.GetKey(KeyCode.H) && healCoolTime <= 0)
            {
                healFlag = true;
            }
        }
        else if (log)
        {
            Debug.Log("SpiritPosition���ݒ肳��Ă��܂���B\n inspector���SpiritPosition���A�^�b�`���Ă��������B");
            log = false;
        }
    }

    //�񕜂̍ہA�v���C���[�̎������]����֐�
    void RotateAroundPlayer()
    {
        //��]���x�Ɋ�Â��Ċp�x���X�V
        currentAngle += rotationSpeed * Time.deltaTime;

        //�v���C���[�̎����4���������]���I��
        if (currentAngle - startAngle >= 1320.0f)
        {
            spiritAudio.PlayOneShot(healing_SE);  //�񕜉�����炷
            player.Hp += healAmount;              //HP��
            healFlag = false;                     //�񕜏I���܂����[
            currentCoolTime = healCoolTime;       //�񕜃N�[���^�C����ݒ�
            currentAngle = 0.0f;                  //���݂̊p�x�����Z�b�g

            //HP���ő�l�𒴂��Ȃ��悤�ɐ���
            player.Hp = Mathf.Min(player.Hp, player.MaxHP);
            return;
        }

        //===��]����===
        Vector3 rotation = new Vector3(0.0f, currentAngle, 0.0f);
        transform.eulerAngles = rotation;
        transform.position = playerTransform.position +
            Quaternion.Euler(0f, currentAngle, 0f) *
            new Vector3(0f, healSpiritPositionY, -2f);
    }

    //������㉺�ɂӂ�ӂ킳���鏈��
    void FloatEffectWhileIdle()
    {
        //�ӂ�ӂ킳���镝�Ƒ����𒲐�
        float yOffset = Mathf.Sin(Time.time * 3f) * 0.2f;
        transform.position = new Vector3(transform.position.x,
            spiritPositionTransform.position.y + yOffset, transform.position.z);
    }

    //�񕜂���Ƃ��ɊO������Ăяo���p
    public void StartHeal()
    {
        healFlag = true;
    }
}
