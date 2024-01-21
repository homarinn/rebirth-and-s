using UnityEngine;

// Player�X�N���v�g
public class CS_PlayerA : MonoBehaviour
{

    // ============ �ړ� =============  //
    [SerializeField, Header("�ړ����x")]
    private float moveSpeed = 5.0f;

    [SerializeField, Header("�v���C���[�̐��񑬓x")]
    private float rotationSpeed = 0.5f;

    // ============ ��� ============= //
    [SerializeField, Header("��𑬓x")]
    private float slidingSpeed = 10.0f;
    [SerializeField, Header("����C���^�[�o��")]
    private float slidingInterval = 1;
    private float slidingTimer = 0;

    // �X���C�f�B���O��/
    private bool slidingNow = false;

    // ============ �U�� ============= //
    [SerializeField, Header("Attack1�U����")]
    private float attack1Power = 10;
    public float Attack1Power
    {
        get
        {
            return attack1Power;
        }
    }
    [SerializeField, Header("Attack1�̃C���^�[�o��")]
    private float attack1Interval = 0.5f;
    [SerializeField, Header("Attack2�U����")]
    private float attack2Power = 20;
    public float Attack2Power
    {
        get
        {
            return attack2Power;
        }
    }
    [SerializeField, Header("Attack2�̃C���^�[�o��")]
    private float attack2Interval = 1.0f;
    private float attackTimer = 0;
    // �U�����H
    private bool attackNow = false;
    private bool attackOk = true;
    private bool isAttack = false;
    public bool IsAttack
    {
        get
        {
            return isAttack;
        }
        set
        {
            isAttack = value;
        }
    }

    // ============ �K�E ============= //
    [SerializeField, Header("�K�E�̈З�")]
    private float ultPower = 30;
    [SerializeField, Header("�K�E�̃C���^�[�o��")]
    private float ultInterval = 3;
    private float ultTimer = 0;
    public float UltTimer
    {
        get
        {
            return Mathf.Clamp(ultTimer, 0, 5);
        }
    }
    // �K�E��?
    private bool ultNow = false;

    // ============= �h�� ============ //
    [SerializeField, Header("�h�䒆�̃_���[�W�J�b�g��")]
    private float defDamgeCut = 0.5f;
    [SerializeField, Header("�h��̃C���^�[�o��")]
    private float gurdInterval = 1;
    private float gurdTimer = 0;

    // �K�[�h��?
    private bool guardNow = false;

    // ========== �X�e�[�^�X ============= //
    [SerializeField, Header("�v���C���[��MaxHP")]
    private int maxHP = 200;
    [SerializeField, Header("�v���C���[��HP")]
    private int hp;
    public int Hp
    {
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
    // �_���[�W
    private float damage = 0;
    public float GetDamage
    {
        get
        {
            return damage;
        }
    }

    // �J�����̈ʒu
    private Transform cameraTransform = null;

    // ����ł��邩�@true=���S : false=�����Ă���
    private bool isDead = false;

    // ========== �R���|�[�l���g ========= //
    private Rigidbody rb;
    private Animator anim;
    private new AudioSource[] audio;

    // =========== Sound ======== //
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
        audio = GetComponents<AudioSource>();
        // �J�����̈ʒu���擾
        cameraTransform = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Transform>();
    }

    /// <summary>
    /// �X�V�C�x���g
    /// </summary>
    private void Update()
    {
        // HP��0�ȉ��Ȃ玀��ł���
        if (hp <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                if (anim != null)
                {
                    anim.SetTrigger("DeadTrigger");
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }
        // ���G���ԂɂȂ����猸�炷
        if (invincibleTimer > 0)
        {
            invincibleTimer -= Time.deltaTime;
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
    }

    /// <summary>
    /// �ړ��֐�
    /// </summary>
    private void Move()
    {
        if (slidingNow)
        {
            return;
        }
        if (attackNow || guardNow || ultNow)
        {
            rb.velocity = Vector3.zero;
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
        rb.velocity = moveForward * moveSpeed + new Vector3(0, rb.velocity.y, 0);
    }

    #region ���

    /// <summary>
    /// ����֐�
    /// </summary>
    private void Sliding()
    {
        // �X���C�f�B���O�C���^�[�o��������Ƃ����炷
        if (slidingTimer > 0)
        {
            slidingTimer -= Time.deltaTime;
        }

        // ���̍s�����͉������Ȃ�
        if (attackNow || guardNow || ultNow)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !slidingNow && slidingTimer <= 0)
        {
            // �X���C�f�B���O��
            slidingNow = true;
            if (anim != null)
            {
                // �X���C�f�B���O�Đ�
                anim.SetTrigger("SlidingTrigger");
            }
            if (audio != null)
            {
                // �X���C�f�B���O����
                audio[0].PlayOneShot(SE_PlayerEscape);
            }
        }

        // �X���C�f�B���O��
        if (slidingNow)
        {
            rb.velocity = transform.forward * slidingSpeed;
        }
    }

    /// <summary>
    /// �X���C�f�B���O�A�j���[�V�����̏I������
    /// </summary>
    private void AnimSlidingFiled()
    {
        // �C���^�[�o��
        slidingTimer = slidingInterval;
        // �X���C�f�B���O���I��
        slidingNow = false;
    }

    #endregion

    #region �U��

    /// <summary>
    /// �U������
    /// </summary>
    private void Attack()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        // ���̍s�����Ȃ�Ȃɂ����Ȃ�
        if (slidingNow || guardNow || ultNow)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0) && attackOk && attackTimer <= 0)
        {
            attackOk = false;
            attackNow = true;
            if (anim != null)
            {
                // �U���g���K�[
                anim.SetTrigger("AttackTrigger");
            }
        }

    }

    /// <summary>
    /// �U���A�j���[�V����1
    /// </summary>
    private void AnimAttack1()
    {
        isAttack = false;
        damage = attack1Power;
    }

    /// <summary>
    /// �U���A�j���[�V����Ok
    /// </summary>
    private void AnimAttackOk()
    {
        attackOk = true;
    }

    /// <summary>
    /// �U���A�j���[�V�����̏I������
    /// </summary>
    private void AnimAttack1Faild()
    {
        damage = 0;
        attackTimer = attack1Interval;
        attackNow = false;
        isAttack = false;
    }

    /// <summary>
    /// �U���A�j���[�V�����Q
    /// </summary>
    private void AnimAttack2()
    {
        isAttack = false;
        damage = attack2Power;
    }

    /// <summary>
    /// �U��2�A�j���[�V�����̏I������
    /// </summary>
    private void AnimAttack2Faild()
    {
        damage = 0;
        attackTimer = attack2Interval;
        attackNow = false;
        attackOk = true;
        isAttack = false;
    }

    #endregion

    #region �h��

    /// <summary>
    ///  �h�䏈��
    /// </summary>
    private void Guard()
    {
        // �K�[�h�C���^�[�o�������炷
        if (gurdTimer > 0)
        {
            gurdTimer -= Time.deltaTime;
        }
        // ���̍s�����Ă��牽�����Ȃ�
        if (attackNow || slidingNow || ultNow)
        {
            return;
        }
        if (Input.GetMouseButtonDown(1) && !guardNow && gurdTimer <= 0)
        {
            // �K�[�h��
            guardNow = true;
            if (anim != null)
            {
                // �K�[�h�A�j���[�V�����Đ�
                anim.SetTrigger("GuardTrigger");
            }
        }
    }

    /// <summary>
    /// �K�[�h�A�j���[�V�����I������
    /// </summary>
    private void AnimGuardFailed()
    {
        guardNow = false;
        gurdTimer = gurdInterval;
    }
    #endregion

    #region �K�E

    /// <summary>
    /// �K�E����
    /// </summary>
    private void Ult()
    {
        // �C���^�[�o�����������ꍇ���炷
        if (ultTimer > 0 && !ultNow)
        {
            ultTimer -= Time.deltaTime;
        }

        // ���̍s�����Ă����ꍇ�������Ȃ�
        if (slidingNow || attackNow || guardNow)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !ultNow && ultTimer <= 0)
        {
            // �K�E��
            ultNow = true;
            ultTimer = ultInterval;
            if (anim != null)
            {
                anim.SetTrigger("UltTrigger");
            }
        }
    }

    /// <summary>
    /// �A�j���[�V�����K�E����
    /// </summary>
    private void AnimUlt()
    {
        isAttack = false;
        damage = ultPower;
    }

    /// <summary>
    /// �A�j���[�V�����K�E�I������
    /// </summary>
    private void AnimUltFailed()
    {
        damage = 0;
        ultNow = false;
        isAttack = false;
    }
    #endregion

    /// <summary>
    /// �_���[�W����
    /// �U�������l�ɓǂ�ł��炤
    /// </summary>
    /// <param name="damage">�^����_���[�W</param>
    public void Damage(float _damage)
    {
        damage = 0;
        // ���G��Ԃ̏ꍇ����
        if (invincibleTimer > 0)
        {
            return;
        }
        if (!guardNow)
        {
            if (audio != null || SE_PlayerReceiveDamage != null)
            {
                audio[0].PlayOneShot(SE_PlayerReceiveDamage);
            }

            // damage��Hp�����炷
            hp -= (int)(_damage);
        }
        else
        {
            if (audio != null || SE_PlayerGuard != null)
            {
                audio[0].PlayOneShot(SE_PlayerGuard);
            }
            hp -= (int)(_damage * defDamgeCut);
        }
        // ���G���Ԃ�����
        invincibleTimer = invincibleTime;

        if (guardNow || ultNow)
        {
            return;
        }

        if (anim != null)
        {
            // �A�j���[�V�������Đ�
            anim.SetTrigger("HitTrigger");
        }
    }

    private void AnimDamgeFailed()
    {
        slidingNow = false;
        attackNow = false;
        guardNow = false;
        ultNow = false;
        attackOk = true;
    }
}
