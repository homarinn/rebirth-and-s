using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player�X�N���v�g
public class CS_Player : MonoBehaviour
{

    // ============ �ړ� =============  //
    [SerializeField, Header("�ړ����x")]
    private float moveSpeed = 5.0f;

    [SerializeField, Header("�v���C���[�̐��񑬓x")]
    private float rotationSpeed = 0.5f;

    // ============ ��� ============= //
    [SerializeField, Header("��𑬓x")]
    private float slidingSpeed = 10.0f;

    // �X���C�f�B���O��
    private bool slidingNow = false;

    // ============ �U�� ============= //

    // ============ �K�E ============= //
    [SerializeField, Header("�K�E�̃C���^�[�o��")]
    private float ultInterval = 5;
    private float ultTimer = 0;
    public float UltTimer
    {
        get
        {
            return Mathf.Clamp(ultTimer,0,  5);
        }
    }

    // ============= �h�� ============ //

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

    [SerializeField, Header("���G����")]
    private float invincibleTime = 1;
    // ���G���ԃ^�C�}�[
    private float invincibleTimer = 0;

    // �J�����̈ʒu
    private Transform cameraTransform = null;

    // ����ł��邩�@true=���S : false=�����Ă���
    private bool isDead = false; 

    // ========== �R���|�[�l���g ========= //
    private Rigidbody rb;
    private Animator anim;
    private new AudioSource audio;

    // =========== Sound ======== //
    [SerializeField, Header("�U����U��SE")]
    private AudioClip SE_PlayerAttackMis;
    [SerializeField, Header("�U���P�q�b�gSE")]
    private AudioClip SE_PlayerAttack1Hit;
    [SerializeField, Header("�U��2�q�b�gSE")]
    private AudioClip SE_PlayerAttack2Hit;
    [SerializeField, Header("�K�ESE")]
    private AudioClip SE_PlayerSpecalAttack;
    [SerializeField, Header("�_���[�WSE")]
    private AudioClip SE_PlayerReceiveDamage;
    [SerializeField, Header("�ړ�SE")]
    private AudioClip SE_PlayerMove;
    [SerializeField, Header("�X���C�f�B���O")]
    private AudioClip SE_PlayerEscape;

    /// <summary>
    /// ���̉������Ƃ��ɌĂяo�����
    /// </summary>
    private void Awake()
    {
        // HP��ݒ�
        hp = maxHP;
        // �K�E�C���^�[�o���ݒ�
        ultTimer = ultInterval;
    }

    /// <summary>
    /// �X�^�[�g�C�x���g
    /// </summary>
    private void Start()
    {
        // �R���|�[�l���g���擾
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        // �J�����̈ʒu���擾
        cameraTransform = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Transform>();
    }

    /// <summary>
    /// �X�V�C�x���g
    /// </summary>
    private void Update()
    {
        // HP��0�ȉ��Ȃ玀��ł���
        if(hp <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                if (anim != null)
                {
                    anim.SetTrigger("DeadTrigger");
                }
            }
        }

        // �ړ�����
        Move();

        // �������
        Sliding();
    }

    /// <summary>
    /// �ړ��֐�
    /// </summary>
    private void Move()
    {
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
            // �A�j���[�V�������Đ�
            float speed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
            anim.SetFloat("Speed", speed);
            if(audio != null)
            {
                // �ړ������Đ�
                audio.PlayOneShot(SE_PlayerMove);
            }
        }
        // Player�̌����Ă�������ɐi��
        rb.velocity = moveForward * moveSpeed + new Vector3(0,rb.velocity.y,0); 
    }

    #region ���

    /// <summary>
    /// ����֐�
    /// </summary>
    private void Sliding()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            // �X���C�f�B���O��
            slidingNow = true;
            if(anim != null)
            {
                // �X���C�f�B���O�Đ�
                anim.SetTrigger("SlidingTrigger");
            }
            if(audio != null)
            {
                // �X���C�f�B���O����
                audio.PlayOneShot(SE_PlayerEscape);
            }
        }

        // �X���C�f�B���O��
        if(slidingNow)
        {
            rb.velocity = transform.forward * slidingSpeed;
        }
    }

    /// <summary>
    /// �X���C�f�B���O�A�j���[�V�����̏I������
    /// </summary>
    private void AnimSlidingFiled()
    {
        // �X���C�f�B���O���I��
        slidingNow = false;
    }

    #endregion

    /// <summary>
    /// �_���[�W����
    /// �U�������l�ɓǂ�ł��炤
    /// </summary>
    /// <param name="damage">�^����_���[�W</param>
    public void ReceiveDamage(float _damage)
    {
        // ���G��Ԃ̏ꍇ����
        if (invincibleTimer <= 0)
        {
            return;
        }
        // damage��Hp�����炷
        hp -= (int)(_damage);
        // ���G���Ԃ�����
        invincibleTimer = invincibleTime;

        if(anim != null)
        {            
            // �A�j���[�V�������Đ�
            anim.SetTrigger("HitTrigger");
        }
        if(audio != null)
        {
            // �_���[�W�����Đ�
            audio.PlayOneShot(SE_PlayerReceiveDamage);
        }
    }
}
