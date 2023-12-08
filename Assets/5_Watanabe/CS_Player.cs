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

    // ============�@�U�� ============= //

    // =========== �K�E ============== // 

    // =========== �h�� ============= //


    // ============= ��� ============ //B

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

    // ========== �R���|�[�l���g ========= //
    private Rigidbody rb;
    private Animator anim;

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
        anim = GetComponentInChildren<Animator>();
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

        // �ړ�����
        Move();

        // �������
        Avoid();

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
    private void Avoid()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            anim.SetTrigger("SlidingTrigger");
        }
    }

    /// <summary>
    /// �_���[�W�֐�
    /// </summary>
    public void Damage(int i)
    {
        // ���G���Ԃ�������_���[�W���󂯂Ȃ�
        if (mutekiTimer <= 0)
        {
            return;
        }
        // HP���ւ炷
        hp--;
        mutekiTimer = mutekiTime;
        if (anim != null)
        {
            anim.SetTrigger("HitTrigger");
        }
    }

}
