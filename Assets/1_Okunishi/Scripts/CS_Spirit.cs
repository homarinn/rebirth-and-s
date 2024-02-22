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

    [SerializeField, Header("�񕜎��̃G�t�F�N�g")]
    private GameObject healEffect;

    //===����̈ړ�===
    [SerializeField, Header("����̈ړ����x")]
    private float speed = 5.0f;

    [SerializeField, Header("�񕜎��̐����Y�ʒu")]
    private float healSpiritPositionY = 0.5f;

    private float startChaseDistance = 0.5f;   //�ړ����J�n���鋗��
    private bool canMove = true;               //���삪�����������ł������ǂ����̃t���O

    //===����̉�]===
    [SerializeField, Header("����̉�]���x")]
    private float rotationSpeed = 1.0f;

    private float startAngle = 0f;       //�����]�J�n���̊p�x
    private float currentAngle = 0f;     //����̌��݂̊p�x
    private float endRotation = 1320.0f; //��]�I���l

    //===��===
    [SerializeField, Header("�񕜃N�[���^�C��(�b)")]
    private float healCoolTime = 10.0f;

    [SerializeField, Header("�񕜗�(Player�̍ő�HP��n%�����Z)")]
    private float healPercentage = 50.0f;

    [SerializeField, Header("HP��Player�̍ő�HP�̉�%�܂Ō�������񕜂��邩")]
    private float healTrrigerPercentage = 50.0f;

    private float healAmount = 0.0f;                //�񕜗�
    private bool healFlag = false; //�񕜃t���O

    //===����===
    [SerializeField, Header("�񕜉���")]
    AudioClip healing_SE;

    AudioSource spiritAudio;    //���g�̉���

    //===���̑��ϐ�===
    private float currentCoolTime = 0.0f;       //���݂̃N�[���^�C��
    private float effectDuration = 0.1f;        //�G�t�F�N�g�̎�������
    private float effectPositionYOffset = 2.2f; //�G�t�F�N�g��Y����
    private bool log = true;                    //�f�o�b�O�\���p
    private bool healStop = false;              //�񕜒�~�֐��p

    void Start()
    {
        spiritAudio = GetComponent<AudioSource>();
        healEffect.transform.position = transform.position;
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

                    //�񕜋���
                    healFlag = true;
                }

                //�񕜉\��ԂȂ�񕜂���
                if (healFlag && player.Hp > 0)
                {
                    PlayerHealing();
                    ApplyHealEffect();
                }
                else
                {
                    healFlag = false;
                }

                //�N�[���^�C������
                if (currentCoolTime >= 0.0f)
                {
                    currentCoolTime -= 1.0f * Time.deltaTime;
                }
            }
        }
        else if (log)
        {
            Debug.Log("SpiritPosition���ݒ肳��Ă��܂���B\n inspector���SpiritPosition���A�^�b�`���Ă��������B");
            log = false;
        }
    }

    ///<summary>
    ///�񕜂̍ہA�v���C���[�̎������]����֐�
    ///</summary>
    void PlayerHealing()
    {
        //�񕜂�������Ă��Ȃ��Ȃ��]�������X�L�b�v
        if (healStop)
        {
            healFlag = false;
            currentCoolTime = healCoolTime;
            currentAngle = 0.0f;           
            return;
        }

        //��]���x��␳
        float rSpeed = rotationSpeed * 360.0f;

        //��]���x�Ɋ�Â��Ċp�x���X�V
        currentAngle += rSpeed * Time.deltaTime;

        //�v���C���[�̎����4���������]���I��
        if (currentAngle - startAngle >= endRotation)
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

    ///<summary>
    ///������㉺�ɂӂ�ӂ킳���鏈��
    ///</summary>
    void FloatEffectWhileIdle()
    {
        //�ӂ�ӂ킳���镝�Ƒ����𒲐�
        float yOffset = Mathf.Sin(Time.time * 3f) * 0.2f;
        transform.position = new Vector3(transform.position.x,
            spiritPositionTransform.position.y + yOffset, transform.position.z);
    }

    /// <summary>
    /// �񕜃G�t�F�N�g�𔭓�
    /// </summary>
    public void ApplyHealEffect()
    {
        if (healEffect != null)
        {
            //�G�t�F�N�g�̈ʒu��ݒ�
            Vector3 effectPosition = new Vector3(transform.position.x, 
                transform.position.y - effectPositionYOffset, 
                transform.position.z);

            //�G�t�F�N�g�̃C���X�^���X�𐶐�
            GameObject effectInstance = Instantiate(healEffect, effectPosition, Quaternion.identity);

            //����̎q�I�u�W�F�N�g�ɂ���i�e�q�֌W���`��)
            effectInstance.transform.parent = transform;

            //�G�t�F�N�g�̎������Ԍ�ɍ폜
            Destroy(effectInstance, effectDuration);
        }
        else
        {
            Debug.Log("�q�[���G�t�F�N�g���ݒ肳��Ă��܂���B \n inspecter���healEffect���A�^�b�`���Ă��������B");
        }
    }

    /// <summary>
    /// �C�x���g�V�[�����ŉ񕜂��~����
    /// </summary>
    public void EventHealStop()
    {
        healStop = true;
    }
    /// <summary>
    /// �񕜂��ĊJ����
    /// </summary>
    public void EventHealStart()
    {
        healStop = false;
    }
}
