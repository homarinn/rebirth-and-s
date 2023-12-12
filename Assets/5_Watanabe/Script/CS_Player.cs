using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Player : MonoBehaviour
{
    [SerializeField, Header("�J������Transform���擾")]
    private Transform cameraTransform = null;

    // =========== �ړ� =============== //

    [SerializeField, Header("�ړ����x")]
    private float moveSpeed = 5.0f;

    [SerializeField, Header("�v���C���[�̐��񑬓x")]
    private float rotationSpeed = 0.5f;

    // �ړ��ł��邩 true=�\ : false=�s�� 
    private bool isMove = true;
    private void IsMoveOk()
    {
        isMove = true;
        if (isSliding)
        {
            isSliding = false;
        }
        if (isGuard)
        {
            isGuard = false;
        }
        if(!isAttack)
        {
            attackTimer = attackInterval;
            attackPower = 0;
            isAttack = true;
        }
        if (isUlt)
        {
            ultTimer = ultInterval;
            attackPower = 0;
            isUlt = false;
        }
    }

    // ============�@�U�� ============= //
    [SerializeField, Header("Attack1�̍U����")]
    private int attack1Power = 10;
    [SerializeField, Header("Attack2�̍U����")]
    private int attack2Power = 20;
    // ���݂̍U����
    private int attackPower = 0; 
    public int AttackPower
    {
        get
        {
            return attackPower;
        }
    }
    [SerializeField, Header("�U���C���^�[�o��")]
    private float attackInterval = 0.5f;
    private float attackTimer = 0;
    private bool isAttack = true; // �R���{�\��
    public void IsAttackOk()
    {
        isAttack = true;
    }

    // =========== �K�E ============== //
    [SerializeField, Header("�K�E�̍U����")]
    private int ultPower = 50;
    [SerializeField, Header("�K�E�̃C���^�[�o��")]
    private float ultInterval = 5;
    private float ultTimer = 0;
    public float UltTimer
    {
        get
        {
            return ultTimer;
        }
    }
    private bool isUlt = false;

    // =========== �h�� ============= //
    [SerializeField, Header("�h���")]
    private float guardPower = 0.5f;
    [SerializeField, Header("�X���C�f�B���O�C���^�[�o��")]
    private float guardInterval = 1;
    private float guardTimer = 0;
    // �h�䂵�Ă��邩 true=���Ă��� : false=���Ă��Ȃ�
    private bool isGuard = false;
    

    // ============= ��� ============ //
    [SerializeField, Header("�X���C�f�B���O�̑��x")]
    private float slidingSpeed = 10.0f;
    // �X���C�f�B���O���Ă��邩 true=���Ă��� : false=���Ă��Ȃ�
    private bool isSliding = false;

    [SerializeField, Header("�X���C�f�B���O�C���^�[�o��")]
    private float slidingInterval = 1;
    private float slidingTimer = 0;

    // ========= �_���[�W ============= //
    [SerializeField, Header("�_���[�W���󂯂��Ƃ��̖��G����")]
    private float mutekiTime = 1;
    private float mutekiTimer = 0;


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
        set
        {
            hp = value;
        }
    }

    // ����ł��邩�@true=���S : false=�����Ă���
    private bool isDead = false; 

    // ========== �R���|�[�l���g ========= //
    private Rigidbody rb;
    private Animator anim;

    // =========== Sound ======== //
    [SerializeField, Header("�U����U��SE")]
    private AudioClip SE_PlayerAttackMis;
    [SerializeField, Header("�U���P�q�b�gSE")]
    private AudioClip SE_PlayerAttack1Hit;
    [SerializeField, Header("�U��2�q�b�gSE")]
    private AudioClip SE_PlayerAttack2Hit;
    [SerializeField, Header("�K�ESE")]
    private AudioClip SE_PlayerSpecalAttack;


    /// <summary>
    /// ���̉������Ƃ��ɌĂяo�����
    /// </summary>
    private void Awake()
    {
        hp = maxHP;
    }

    /// <summary>
    /// �X�^�[�g�C�x���g
    /// </summary>
    private void Start()
    {
        // �R���|�[�l���g���擾
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        ultTimer = ultInterval;
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

        if(hp <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                anim.SetTrigger("DeadTrigger");

            }
        }

        // �ړ�����
        Move();

        // �������
        Sliding();

        // �U������
        Attack();

        // �h�䏈��
        Guard();

        // �K�E����
        Ult();

        // ���G���Ԃ������ꍇ�^�C�}�[�����炷
        if(mutekiTimer > 0)
        {
            mutekiTimer -= Time.deltaTime;
        }

    }

    /// <summary>
    /// �ړ��֐�
    /// </summary>
    private void Move()
    {
        // �J������RigidoBody�擾�ł��Ȃ��ꍇ�������Ȃ�
        if(cameraTransform == null)
        {
            Debug.Log("�J�����̈ʒu���擾�ł��Ă��Ȃ�");
            return;
        }

        // �ړ��������Ȃ�
        if (!isMove)
        {
            return;
        }

        // �ړ����͂��擾
        Vector2 inputAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // �J�����̕�������X-Z�P�ʃx�N�g�����擾
        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveForward = cameraForward * inputAxis.y + cameraTransform.right * inputAxis.x;

        // �i�s�����ɉ�]
        if (moveForward != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveForward), rotationSpeed);
        }
        if(anim != null)
        {
            anim.SetFloat("Speed", rb.velocity.magnitude);
        }
        // Player�̌����Ă�������ɐi��
        rb.velocity = moveForward * moveSpeed + new Vector3(0,rb.velocity.y,0); 
    }

    /// <summary>
    /// ����֐�
    /// </summary>
    private void Sliding()
    {
        // ���̍s�����Ă����牽�����Ȃ�
        if (!isAttack && isGuard && isUlt)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && slidingTimer <= 0)
        {
            isMove = false;
            isSliding = true;
            slidingTimer = slidingInterval;
            anim.SetTrigger("SlidingTrigger");
        }

        // �����Ă�������Ɉړ�����
        if (isSliding && !isUlt && !isGuard && isAttack)
        {
            rb.velocity = transform.forward * slidingSpeed;
        }

        // �^�C�}�[��0�ȏ�Ȃ猸�炷
        if(slidingTimer > 0 && !isSliding)
        {
            slidingTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// �K�E
    /// </summary>
    void Ult()
    {
        if(Input.GetKeyDown(KeyCode.Space) && ultTimer <= 0)
        {
            isMove = false; // �ړ����Ȃ�
            anim.SetTrigger("UltTrigger");
            ultTimer = ultInterval;
            isAttack = true;
        }

        // �^�C�}�[��0�ȏ�Ȃ猸�炷
        if (ultTimer > 0)
        {
            ultTimer -= Time.deltaTime;
        }
    }

    public void UltDamage()
    {
        attackPower = ultPower;
    }

    /// <summary>
    /// �U���֐�
    /// </summary>
    private void Attack()
    {
        // ���̍s�����Ă����牽�����Ȃ�
        if (isGuard && isSliding)
        {
            return;
        }
        if (Input.GetMouseButtonDown(1) && attackTimer <= 0 && isAttack)
        {
            isMove = false; // �ړ����Ȃ�
            anim.SetTrigger("AttackTrigger");
            isAttack = false;
        }

        // �^�C�}�[��0�ȏ�Ȃ猸�炷
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

    }

    public void Attack1Damage()
    {
        attackPower = attack1Power;
    }
    public void Attack2Damage()
    {
        attackPower = attack2Power;

    }


    /// <summary>
    /// �h��֐�
    /// </summary>
    private void Guard()
    {
        // ���̍s�����Ă����牽�����Ȃ�
        if (!isAttack && isSliding)
        {
            return;
        }
        if(Input.GetMouseButtonDown(0) && guardTimer <= 0)
        {
            isMove = false; // �ړ����Ȃ�
            isGuard = true; // �h�䂵�Ă���
            guardTimer = guardInterval;
            anim.SetTrigger("GuardTrigger");
        }

        // �^�C�}�[��0�ȏ�Ȃ猸�炷
        if (guardTimer > 0 && !isGuard)
        {
            guardTimer -= Time.deltaTime;
        }

    }

    /// <summary>
    /// �_���[�W�֐�
    /// </summary>
    public void Damage(int damage)
    {
        // ���G���Ԃ�������_���[�W���󂯂Ȃ�
        if (mutekiTimer <= 0)
        {
            return;
        }
        // HP���ւ炷
        if (isGuard)
        {
            hp -= (int)(damage * guardPower);
        }
        else
        {
            hp -= damage;
        }
        mutekiTimer = mutekiTime;
        if (anim != null)
        {
            anim.SetTrigger("HitTrigger");
        }
    }

}
