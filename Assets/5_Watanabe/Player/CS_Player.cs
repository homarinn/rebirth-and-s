using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Player : MonoBehaviour
{
    [SerializeField, Header("�J������Transform���擾")]
    private Transform cameraTransform = null;

    // =========== �ړ� =============== //
    [SerializeField, Header("�ړ������x")]
    private float moveAcceleration = 3.0f;

    [SerializeField, Header("�ړ������x")]
    private float moveDecelerate = 5.0f;

    [SerializeField, Header("�ō������x")]
    private float moveMaxSpeed = 5.0f;

    [SerializeField, Header("�����܂��̍ō����x")]
    private float moveOnTheWaterMaxSpeed = 0.5f;

    [SerializeField, Header("�v���C���[�̐��񑬓x")]
    private float rotationSpeed = 0.5f;

    // �ړ����x
    [SerializeField]
    private float moveSpeed = 0f;
    // �����܂�̏ォ�ǂ���
    private bool isOnTheWater = false;

    // ============�@�U�� ============= //
    [SerializeField, Header("�U���P�̈З�")]
    private int attack1Damage = 1;

    [SerializeField, Header("�U��2�̈З�")]
    private int attack2Damage = 2;

    [SerializeField, Header("�U���A�N�V������́A���̍s���ւ̃C���^�[�o��")]
    private float attackToActionoInterval = 0;

    // �R���{�̉�
    private int attackCount = 0;

    // =========== �K�E ============== // 
    [SerializeField, Header("�K�E�Z�̈З�")]
    private int specalAttackDamage = 10;

    [SerializeField, Header("�K�E�Z�����ւ̃C���^�[�o��")]
    private int specalAttackInterval = 0;

    // =========== �h�� ============= //
    [SerializeField, Header("�h�䒆�_���[�W�J�b�g��")]
    private float damageCut = 0;
    public float DamageCut
    { 
        get
        {
            return damageCut;
        } 
    }

    [SerializeField, Header("�h���́A���ւ̍s���̃C���^�[�o��")]
    private float deffecToActionInterval = 0.0f;


    // ============= ��� ============ //
    [SerializeField, Header("����A�N�V�����̎���")]
    private float avoidTime = 1.0f;

    [SerializeField, Header("����A�N�V�����̑��x")]
    private float avoidSpeed = 1.0f;

    [SerializeField, Header("����A�N�V������́A���ւ̍s���̃C���^�[�o��")]
    private float avoidToActionInterval = 0.0f;    


    // ========== �X�e�[�^�X ============= //
    [SerializeField, Header("�v���C���[��MaxHP")]
    private int maxHP = 200;
    [SerializeField, Header("�v���C���[��HP")]
    private int hp;
    public int Hp {
        get
        {
            return hp;
        }
    }

    // ========== �R���|�[�l���g ========= //
    private Rigidbody rb;
    private Animator anim;

    /// <summary>
    /// �X�^�[�g�C�x���g
    /// </summary>
    private void Start()
    {
        // �R���|�[�l���g���擾
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        // �X�e�[�^�X�̏�����
        hp = maxHP;
    }

    /// <summary>
    /// �X�V�C�x���g
    /// </summary>
    private void Update()
    {
        // �R���|�[�l���g���擾�ł��Ă��Ȃ��ꍇLog���o��
        if(rb == null || anim == null)
        {
            Debug.Log("�R���|�[�l���g���擾�ł��Ă��Ȃ�");
            return;
        }
        // �ړ����͂��擾
        Vector2 moveAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // �ړ�����
        Move(moveAxis.x, moveAxis.y);

        // �U������
        Attack();

        // �h�䏈��
        if(Input.GetMouseButton(1))
        {
            Defence();
        }

        Avoid();

        // �K�E����
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SpecialAttack();
        }
    }

    /// <summary>
    /// �ړ��֐�
    /// </summary>
    private void Move(float horizontal, float vertical)
    {
        // �J������RigidoBody�擾�ł��Ȃ��ꍇ�������Ȃ�
        if(cameraTransform == null)
        {
            Debug.Log("�J�����̈ʒu���擾�ł��Ă��Ȃ�");
            return;
        }

        // �J�����̕�������X-Z�P�ʃx�N�g�����擾
        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveForward = cameraForward * vertical + cameraTransform.right * horizontal;

        // �i�s�����ɉ�]
        if (moveForward != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveForward), rotationSpeed);
        }
        if(moveForward != Vector3.zero)
        {
            // �ړ����͂���Ă���Ƃ��͉�������
            moveSpeed += moveAcceleration * Time.deltaTime;
        }
        else if (vertical == 0 && horizontal == 0)
        {
            // �������Ă��Ȃ��Ƃ��͌�������
            moveSpeed -= moveDecelerate * Time.deltaTime;
        }
        // ���x��␳
        if (!isOnTheWater)
        {
            moveSpeed = Mathf.Clamp(moveSpeed, 0, moveMaxSpeed);
        }
        else
        {
            moveSpeed = Mathf.Clamp(moveSpeed, 0, moveOnTheWaterMaxSpeed);
        }
        if(anim != null)
        {
            anim.SetFloat("Speed", moveSpeed);
        }
        // Player�̌����Ă�������ɐi��
        rb.velocity = transform.forward * moveSpeed;
    }

    /// <summary>
    /// ����֐�
    /// </summary>
    private void Avoid()
    {
        // �������
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            
            rb.AddForce(transform.forward * 50, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// �h��֐�
    /// </summary>
    private void Defence()
    {

    }

    /// <summary>
    /// �U���֐�
    /// </summary>
    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {

            switch (attackCount)
            {
                case 0:
                    break;
                case 1:
                    break;
            }
        }
    }

    /// <summary>
    /// �K�E�Z�֐�
    /// </summary>
    private void SpecialAttack()
    {

    }

    /// <summary>
    /// �_���[�W�֐�
    /// </summary>
    public void Damage()
    {
        // HP���ւ炷
        hp--;
        if(anim != null)
        {
            anim.SetTrigger("Hit");
        }
    }
}
