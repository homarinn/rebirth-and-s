using UnityEngine;

/// <summary>
/// Player���C��
/// </summary>
public partial class CS_Player : MonoBehaviour
{
    // =======================
    //
    // �ϐ�
    //
    // =======================

    enum State
    {
        Normal,
        Sliding,
        Attack,
        Difence,
        Damage,
        Ult,
        Death
    }
    State state;

    // �ړ�
    [SerializeField, Header("�ړ����x")]
    private float moveSpeed = 0;
    [SerializeField, Header("�v���C���[�̐��񑬓x")]
    private float rotationSpeed = 0;

    [SerializeField, Header("�����܂��̈ړ����x�ቺ(0�`1)")]
    private float waterOnTheMoveSpeedCut = 0;
    private bool isWaterOnThe = false;
    [SerializeField, Header("�����܂�G�t�F�N�g")]
    private GameObject puddleEffect;
    [SerializeField, Header("�����܂葫��")]
    private Transform lefTrs;

    private bool moveOK = true;    // �ړ�����

    // �X���C�f�B���O
    [SerializeField, Header("�X���C�f�B���O���x")]
    private float slidingSpeed = 0;
    [SerializeField, Header("�X���C�f�B���O�C���^�[�o��")]
    private float slidingInterval = 0;
    private float slidingTimer = 0;

    // �U���З�
    [SerializeField, Header("Auto�U������͈̔̓R���C�_�[")]
    CS_LookCollision csLookCollision;
    [SerializeField, Header("�U���p�̃R���C�_�[")]
    private Collider collider;
    [SerializeField, Header("�U���P�̈З�")]
    private float attack1Power = 0;
    public float Attack1Power
    {
        get
        {
            return attack1Power;
        }
    }
    [SerializeField, Header("�U��2�̈З�")]
    private float attack2Power = 0;
    public float Attack2Power
    {
        get
        {
            return attack2Power;
        }
    }
    private float attackDamage = 0;
    public float AttackDamage
    {
        get
        {
            return attackDamage;
        }
    }
    bool attackOk = true;
    public bool AttackOk
    {
        get
        {
            return attackOk;
        }
        set
        {
            attackOk = value;
        }
    }

    [SerializeField, Header("�U��1�C���^�[�o��")]
    private float attack1Interval = 0;
    [SerializeField, Header("�U��2�C���^�[�o��")]
    private float attack2Interval = 0;
    private float attackTimer = 0;

    // �U���Q�������\��
    private bool attack2Ok = false;

    // �K�E
    [SerializeField, Header("�K�E�Z�̈З�")]
    private float ultPower = 0;
    public float UltPower
    {
        get
        {
            return ultPower;
        }
    }
    [SerializeField, Header("�K�E�Z�̃C���^�[�o��")]
    private float ultInterval = 0;
    private float ultTimer = 0;

    // �h��
    [SerializeField, Header("�h��C���^�[�o��")]
    private float difenceInterval = 0;
    private float difenceTimer = 0;
    [SerializeField, Header("�h�䒆�̃_���[�W�J�b�g%")]
    private float difenceDamageCut = 0;
    [SerializeField]
    private bool isDifence = false;

    [SerializeField, Header("HP�̍ő�l")]
    private float maxHP = 0;    // �ő�HP
    [SerializeField] private float hp;           // ���݂�HP
    private bool isInvisible = false;
    private bool isDeath = false;
    [SerializeField, Header("�_���[�W�A�j���[�V�������Đ�����U��")]
    private float damageAtackOkAttack = 0;
    public bool IsDeath
    {
        get
        {
            return isDeath;
        }
    }

    private Transform cameraTransform = null;       // �J�����̈ʒu
    // �R���|�[�l���g
    private Rigidbody rb;
    private Animator anim;
    private AudioSource audio;

    // SE
    [SerializeField, Header("�ړ�SE")]
    private AudioClip SE_Move;
    [SerializeField, Header("�U��SE")]
    private AudioClip SE_Attack;
    [SerializeField, Header("�X���C�f�B���OSE")]
    private AudioClip SE_Sliding;
    [SerializeField, Header("�h��J�nSE")]
    private AudioClip SE_DifenceStart;
    [SerializeField, Header("�h��SE")]
    private AudioClip SE_Difence;
    [SerializeField, Header("�_���[�WSE")]
    private AudioClip SE_Damage;
    [SerializeField, Header("�K�ESE")]
    private AudioClip SE_Ult;
    [SerializeField, Header("�K�E�W�����vSE")]
    private AudioClip SE_Jump;

    // Effect
    [SerializeField, Header("�G�t�F�N�g�ʒu")]
    private Transform effectTrs;
    [SerializeField, Header("�h��G�t�F�N�g")]
    private GameObject Eff_Difence;
    [SerializeField, Header("�ʏ�U��01")]
    private GameObject Eff_Attack01;
    [SerializeField, Header("�ʏ�U��02")]
    private GameObject Eff_Attack02;
    [SerializeField, Header("�K�E���n")]
    private GameObject Eff_UltTachi;
    [SerializeField, Header("�K�E�G�t�F�N�g")]
    private GameObject Eff_Ult;

    // =======================
    //
    // �Q�b�^�[�E�Z�b�^�[
    //
    // =======================

    // HP�̍ő�l
    public float MaxHP
    {
        get
        {
            return maxHP;
        }
    }

    // HP
    public float Hp
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

    // �K�E�Z�^�C�}�[
    public float UltTimer
    {
        get
        {
            return Mathf.Clamp(ultTimer, 0, 5);
        }
    }

    private bool action = true;
    public bool Action
    {
        get
        {
            return action;
        }
        set
        {
            action = value;
        }
    }

    // =======================
    //
    // �֐�
    //
    // =======================


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
        if (isDeath || !action)
        {
            return;
        }
        // �C���^�[�o�����X�V
        IntervalUpdate();

        switch (state)
        {
            case State.Normal:

                // ������
                if (Input.GetKeyDown(KeyCode.LeftShift) && slidingTimer <= 0)
                {
                    state = State.Sliding;
                    audio.PlayOneShot(SE_Sliding);      // ���ʉ���炷
                    anim.SetTrigger("SlidingTrigger");  // �A�j���[�V�������Đ�
                }

                // �U������
                if (Input.GetMouseButtonDown(0) && attackTimer <= 0)
                {
                    if (csLookCollision.IsHit)
                    {
                        Vector3 pos = Vector3.Scale(csLookCollision.EnemyPos, new Vector3(1, 0, 1));
                        transform.LookAt(pos);
                    }
                    else
                    {
                        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
                        // �i�s�����ɉ�]
                        if (cameraForward != Vector3.zero)
                        {
                            transform.rotation = Quaternion.LookRotation(cameraForward);
                        }

                    }

                    rb.velocity = Vector3.zero;
                    state = State.Attack;
                    rb.velocity = Vector3.zero;
                    anim.SetTrigger("AttackTrigger");  // �A�j���[�V�������Đ�
                }

                // �h�����
                if (Input.GetMouseButtonDown(1) && difenceTimer <= 0)
                {
                    state = State.Difence;
                    rb.velocity = Vector3.zero;
                    anim.SetTrigger("GuardTrigger");  // �A�j���[�V�������Đ�
                }

                // �K�E����
                if (Input.GetKeyDown(KeyCode.Space) && ultTimer <= 0)
                {
                    Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
                    // �i�s�����ɉ�]
                    if (cameraForward != Vector3.zero)
                    {
                        transform.rotation = Quaternion.LookRotation(cameraForward);
                    }
                    if (csLookCollision.IsHit)
                    {
                        Vector3 pos = Vector3.Scale(csLookCollision.EnemyPos, new Vector3(1, 0, 1));
                        transform.LookAt(pos);
                    }

                    state = State.Ult;
                    rb.velocity = Vector3.zero;
                    anim.SetTrigger("UltTrigger");
                }
                if (hp <= 0)
                {
                    state = State.Death;
                    anim.SetTrigger("DeadTrigger");
                    hp = 0;
                }

                // �ړ�����
                Move();
                break;
            case State.Sliding:
                // �������
                Sliding();
                break;
            case State.Attack:

                // �U��2�̓���
                if (Input.GetMouseButtonDown(0) && attack2Ok)
                {
                    if (csLookCollision.IsHit)
                    {
                        Vector3 pos = Vector3.Scale(csLookCollision.EnemyPos, new Vector3(1, 0, 1));
                        transform.LookAt(pos);
                    }
                    attack2Ok = false;
                    anim.SetTrigger("AttackTrigger");  // �A�j���[�V�������Đ�
                }
                break;
        }
    }

    /// <summary>
    /// �C���^�[�o������
    /// </summary>
    private void IntervalUpdate()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // �X���C�f�B���O�̃C���^�[�o���^�C�}�[�����炷
        if (slidingInterval > 0)
        {
            slidingTimer -= Time.deltaTime;
        }

        // �f�B�t�F���X�̃C���^�[�o���^�C�}�[�����炷
        if (difenceTimer > 0)
        {
            difenceTimer -= Time.deltaTime;
        }

        // �K�E�̃C���^�[�o���^�C�}�[�����炷
        if (ultTimer > 0)
        {
            ultTimer -= Time.deltaTime;
        }
    }

    #region �ړ�

    /// <summary>
    /// �ړ��֐�
    /// </summary>
    private void Move()
    {
        if (!moveOK && state != State.Normal)
        {
            return; // �ړ��s��
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
        if (anim != null)
        {
            // ���x���擾
            float speed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
            // �A�j���[�V�������Đ�
            anim.SetFloat("Speed", speed);
        }
        if (isWaterOnThe)
        {
            rb.velocity = moveForward.normalized * (moveSpeed * waterOnTheMoveSpeedCut);
        }
        else
        {
            // Player�̌����Ă�������ɐi��
            rb.velocity = moveForward.normalized * moveSpeed;
        }
    }

    /// <summary>
    /// �������Ȃ���
    /// </summary>
    private void AnimMoveAudio()
    {
        if (isWaterOnThe)
        {
            var eff = Instantiate(puddleEffect, lefTrs);
            Destroy(eff, 1);
        }
        else
        {
            audio.PlayOneShot(SE_Move);
        }
    }

    #endregion

    #region �X���C�f�B���O

    /// <summary>
    /// �X���C�f�B���O�֐�
    /// </summary>
    private void Sliding()
    {
        if (isWaterOnThe)
        {
            rb.velocity = transform.forward * (slidingSpeed * waterOnTheMoveSpeedCut);
        }
        else
        {
            rb.velocity = transform.forward * slidingSpeed;
        }
    }

    /// <summary>
    /// �X���C�f�B���O�A�j���[�V�����I���̎��ɌĂяo��
    /// </summary>
    private void AnimSlidingFailed()
    {
        state = State.Normal;
        slidingTimer = slidingInterval;
        //rb.velocity = Vector3.zero;
    }

    #endregion

    #region �U��

    /// <summary>
    /// �U��1�̈З͐ݒ�
    /// </summary>
    private void AnimAttackSetPower()
    {
        attackDamage = attackDamage == 0 ? attack1Power : 0;
        if (attackDamage == 0)
        {
            attackOk = false;
            collider.enabled = false;
        }
        else if (attackDamage != 0)
        {
            collider.enabled = true;
            attackOk = true;
            var eff = Instantiate(Eff_Attack01, transform);
            Destroy(eff, 1);
        }
    }

    /// <summary>
    /// �U���A�j���[�V�����I���̎��ɌĂяo��
    /// </summary>
    private void AnimAttack1Failied()
    {
        state = State.Normal;
        attackTimer = attack1Interval;
        attack2Ok = false;
    }

    /// <summary>
    /// �U���Q�𔭓��\�ɂ���
    /// </summary>
    private void AnimAttack2OK()
    {
        attack2Ok = true;
    }

    /// <summary>
    /// �U���T�E���h��炷
    /// </summary>
    private void AnimAttackAudio()
    {
        audio.PlayOneShot(SE_Attack);      // ���ʉ���炷
    }

    /// <summary>
    /// �U��2�̈З͐ݒ�
    /// </summary>
    private void AnimAttack2SetPower()
    {
        attackDamage = attackDamage == 0 ? attack2Power : 0;
        if (attackDamage == 0)
        {
            collider.enabled = false;
            attackOk = false;
        }
        else if (attackDamage != 0)
        {
            collider.enabled = true;
            attackOk = true;
            var eff = Instantiate(Eff_Attack02, transform);
            Destroy(eff, 1);
        }

    }

    /// <summary>
    /// �U��2�A�j���[�V�����I���̎��ɌĂяo��
    /// </summary>
    private void AnimAttack2Failied()
    {
        state = State.Normal;
        attackTimer = attack2Interval;
    }


    #endregion

    #region �h��

    private void AnimDifenceStart()
    {
        audio.PlayOneShot(SE_DifenceStart);
    }

    /// <summary>
    /// �h�䒆�̐ݒ�
    /// </summary>
    private void SetDifence()
    {
        isDifence = isDifence == true ? false : true;
    }

    private void AnimDifenceFailed()
    {
        difenceTimer = difenceInterval;
        state = State.Normal;
    }

    #endregion

    #region �K�E

    /// <summary>
    /// �K�E�̈З͐ݒ�
    /// </summary>
    private void AnimUltAttackSetPower()
    {
        attackDamage = attackDamage == 0 ? ultPower : 0;
        if (attackDamage == 0)
        {
            attackOk = false;
            collider.enabled = false;
        }
        else if (attackDamage != 0)
        {
            attackOk = true;
            collider.enabled = true;
            var eff = Instantiate(Eff_Ult, effectTrs);
            Destroy(eff, 2);
        }
    }

    private void AnimUltAudioJump()
    {
        audio.PlayOneShot(SE_Jump);
    }

    private void AnimUltAudio()
    {
        audio.PlayOneShot(SE_Ult);
    }

    /// <summary>
    /// �K�E�A�j���[�V�����I���̎��ɌĂяo��
    /// </summary>
    private void AnimUltFailed()
    {
        state = State.Normal;
        ultTimer = ultInterval;
    }

    private void AnimUltTachi()
    {
        var eff = Instantiate(Eff_UltTachi, lefTrs);
        Destroy(eff, 2);
    }

    #endregion

    /// <summary>
    /// �_���[�W����
    /// �U�������l�ɓǂ�ł��炤
    /// </summary>
    /// <param name="_damage">�^����_���[�W</param>
    public void ReceiveDamage(float _damage)
    {
        // ���G�������牽�����Ȃ�
        if (isInvisible || hp <= 0 || state == State.Sliding || state == State.Ult)
        {
            return;
        }

        if (isDifence)
        {
            var eff = Instantiate(Eff_Difence, effectTrs);
            Destroy(eff, 1);
            audio.PlayOneShot(SE_Difence);
            // �K�[�h���_���[�W����
            hp -= _damage * difenceDamageCut;
        }
        else
        {
            audio.PlayOneShot(SE_Damage);
            isInvisible = true;
            hp -= _damage;
        }

        if (hp <= 0)
        {
            hp = 0;
        }

        if (state == State.Difence || state == State.Ult)
        {
            isInvisible = false;
            return;
        }
        if (_damage >= damageAtackOkAttack)
        {
            attackDamage = 0;
            state = State.Damage;
            anim.SetTrigger("DamageTrigger");
        }
        else
        {
            isInvisible = false;
        }
    }

    /// <summary>
    /// ������΂�
    /// </summary>
    /// <param name="direcion">��΂�����</param>
    /// <param name="power">��΂��З�</param>
    public void BlowOff(Vector3 direcion, float power)
    {
        if (state == State.Difence || state == State.Ult)
        {
            return;
        }
        state = State.Damage;
        anim.SetTrigger("BlowOffTrigger");
    }

    /// <summary>
    /// �_���[�W�A�j���[�V�����I���̎��ɌĂяo��
    /// </summary>
    private void AnimDamageFailed()
    {
        state = State.Normal;
        isInvisible = false;
        attack2Ok = false;
        attackDamage = 0;
    }

    private void AnimDead()
    {
        isDeath = true;
    }


    /// <summary>
    /// �R���W�����ƐڐG�����Ƃ�
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // �����܂�
        if (other.gameObject.tag == "Puddle")
        {
            isWaterOnThe = true;
        }
    }

    /// <summary>
    /// �R���W�����Ɨ��ꂽ�Ƃ�
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        // �����܂�
        if (other.gameObject.tag == "Puddle")
        {
            isWaterOnThe = false;
        }
    }
}

