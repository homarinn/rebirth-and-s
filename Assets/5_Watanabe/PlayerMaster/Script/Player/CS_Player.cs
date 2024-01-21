using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Player : MonoBehaviour
{
    // �X�e�[�g
    enum State
    {
        None,
        Attack,
        Ult,
        Difence,
        Sliding,
        Hit,
        Death
    }
    State state;

    // �J�����̈ʒu
    private Transform cameraTransform = null;

    // HP
    [SerializeField, Header("HP�̍ő�l")]
    private int maxHP = 200;
    private int hp;
    public int Hp{ get{ return hp; }}

    // Move
    [SerializeField, Header("�ړ����x")]
    private float moveSpeed = 0;
    [SerializeField, Header("���񑬓x")]
    private float rotationSpeed = 0;

    private float ultTimer = 0; // �K�E�Z�^�C�}�[
    public float UltTimer{ get{ return Mathf.Clamp(ultTimer, 0, 5); }}

    // Component
    private Rigidbody rb;
    private Animator anim;
    private new AudioSource[] audio;

    // SE
    [SerializeField, Header("�K�ESE")]
    private AudioClip SE_PlayerSpecalAttack;
    [SerializeField, Header("�_���[�WSE")]
    private AudioClip SE_PlayerReceiveDamage;
    [SerializeField, Header("�ړ�SE")]
    private AudioClip SE_PlayerMove;
    [SerializeField, Header("�X���C�f�B���O")]
    private AudioClip SE_PlayerEscape;
    [SerializeField, Header("�K�[�hSE")]
    private AudioClip SE_PlayerGuard;


    void Start()
    {
        // �R���|�[�l���g���擾
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        audio = GetComponents<AudioSource>();
        // �J�����̈ʒu���擾
        cameraTransform = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Transform>();

        // ������
        Initialize();
    }

    /// <summary>
    /// ����������
    /// </summary>
    private void Initialize()
    {
        hp = maxHP;
        ultTimer = 0;
        state = State.None;
    }

    void FixedUpdate()
    {
        Move();
    }

# region �ړ�

    /// <summary>
    /// �ړ�����
    /// </summary>
    private void Move()
    {
        if(state != State.None)
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
        float speed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
        if (anim != null)
        {
            // �A�j���[�V�������Đ�
            anim.SetFloat("Speed", speed);
        }
        // Player�̌����Ă�������ɐi��
        rb.velocity = moveForward.normalized * moveSpeed;

    }

    #endregion

    #region �U��
    #endregion

    #region �K�E
    #endregion

    #region �X���C�f�B���O

    #endregion

    #region �h��
    #endregion

    /// <summary>
    /// �_���[�W�֐�
    /// </summary>
    /// <param name="damage">�^�����</param>
    public void Damage(int damage)
    {

    }
}
