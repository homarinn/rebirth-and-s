using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// �G�̊Ǘ��N���X
/// </summary>
public class CS_EnemyPlayer : MonoBehaviour
{
    // ----------------------------
    // ��Ԑ���p
    // ----------------------------

    /// <summary>
    /// ���
    /// </summary>
    public enum State
    {
        /// <summary> �ҋ@ </summary>
        Idle,

        /// <summary> �ǔ� </summary>
        Chase,

        /// <summary> �s����I�� </summary>
        SelectAction,

        /// <summary> �U��1 </summary>
        Attack1,

        /// <summary> �U��2 </summary>
        Attack2,

        /// <summary> �K�E�Z </summary>
        Ult,

        /// <summary> ��� </summary>
        Sliding,

        /// <summary> �h�� </summary>
        Guard,

        /// <summary> ��_���[�W </summary>
        ReceiveDamage,

        /// <summary> ���S </summary>
        Dead,
    }

    /// <summary> ���݂̏�� </summary>
    [SerializeField] private State currentState;

    /// <summary>
    /// ���݂̏��
    /// </summary>
    public State CurrentState { get { return currentState; } }

    // ----------------------
    // �v���C���[
    // ----------------------

    /// <summary> �v���C���[ </summary>
    [Header("�v���C���[�擾")]
    [SerializeField] private Transform player;

    /// <summary> �v���C���[�Ǘ��p(�^�_���[�W�Ŏg��) </summary>
    private CS_Player playerManager;

    // ---------------------------
    // ��������p
    // ---------------------------

    /// <summary> �������~�߂�ꍇ��true�ɂ��� </summary>
    private bool isStop = false;

    // ---------------------------
    // HP
    // ---------------------------

    /// <summary> �ő�HP </summary>
    [Header("�ő�HP")]
    [SerializeField] private float maxHp;

    /// <summary> ���݂�HP </summary>
    private float hp;

    /// <summary>
    /// ���݂�HP
    /// </summary>
    public float Hp { get { return hp; } }

    // ---------------------
    // ���x
    // ---------------------

    /// <summary> ���x�֌W�̃p�����[�^ </summary>
    [System.Serializable]
    private struct SpeedParameter
    {
        /// <summary> �ǔ����̍ő呬�x </summary>
        [Header("�ǔ����̍ő呬�x")]
        public float chaseMax;

        [Header("�ǔ����̉����x")]
        public float chaseAcceleration;

        /// <summary> �v���C���[�ɋ߂Â�����̈ړ����x </summary>
        [Header("�v���C���[�ɋ߂Â�����̈ړ����x")]
        [NonSerialized] public float nearMove;
    }

    /// <summary> ���x�֌W�̃p�����[�^ </summary>
    [Header("���x�֌W�̃p�����[�^")]
    [SerializeField] private SpeedParameter speedParameter;

    // --------------------------
    // �U��
    // --------------------------

    /// <summary> �U���֌W�̃p�����[�^ </summary>
    [System.Serializable]
    private struct AttackParameter
    {
        /// <summary> �U��1�̍U���� </summary>
        [Header("�U��1�̍U����")]
        public float power_attack1;

        /// <summary> �U��2�̍U���� </summary>
        [Header("�U��2�̍U����")]
        public float power_attack2;

        /// <summary> 
        /// �U���̃g���K�[�ƂȂ�v���C���[�Ƃ̋���
        /// </summary>
        [Header("�U���̃g���K�[�ƂȂ�v���C���[�Ƃ̋���")]
        public float triggerDistance;

        /// <summary> �U���ҋ@����(�b) </summary>
        [Header("�U���ҋ@����(�b)")]
        public float interval;

        /// <summary> �U������m��(%) </summary>
        [Header("�U��������ɍčU������m��(%)")]
        public float percent;

        /// <summary> �U�����ɃX�[�p�[�A�[�}�[�����邩�ǂ��� </summary>
        [Header("�U�����ɃX�[�p�[�A�[�}�[�����邩�ǂ���")]
        public bool hasSuperArmor;

        /// <summary> �U���\�Ȃ�true </summary>
        [NonSerialized] public bool canAttack;

        /// <summary> �U���̊m�����������Ă���true </summary>
        [NonSerialized] public bool isPercent;
    }

    /// <summary> �U���֌W�̃p�����[�^ </summary>
    [Header("�U���֌W�̃p�����[�^")]
    [SerializeField] private AttackParameter attackParameter;

    // ------------------------------
    // �K�E�Z
    // ------------------------------

    /// <summary>
    /// �K�E�Z�֌W�̃p�����[�^
    /// </summary>
    [System.Serializable]
    private struct UltParameter
    {
        /// <summary> �K�E�Z�̈З� </summary>
        [Header("�K�E�Z�̈З�")]
        public float power;

        /// <summary> �K�E�Z�̃C���^�[�o������(�b) </summary>
        [Header("�K�E�Z�̃C���^�[�o������(�b)")]
        public float interval;

        /// <summary> �K�E�Z�̑��x </summary>
        [Header("�K�E�Z�̑��x"), Range(1.0f, 3.0f)]
        public float speed;

        /// <summary> �K�E�Z���g�p�\�Ȃ�true </summary>
        [NonSerialized] public bool canUlt;
    }

    /// <summary>
    /// �K�E�Z�֌W�̃p�����[�^
    /// </summary>
    [Header("�K�E�Z�֌W�̃p�����[�^")]
    [SerializeField] private UltParameter ultParameter;

    // ------------------------------
    // ���
    // ------------------------------

    /// <summary> ����֌W�̃p�����[�^ </summary>
    [System.Serializable]
    private struct SlidingParameter
    {
        /// <summary> ����C���^�[�o������(�b) </summary>
        [Header("����C���^�[�o������(�b)")]
        public float interval;

        /// <summary> ����\�Ȃ�true </summary> 
        [NonSerialized] public bool canSliding;
    }

    /// <summary> ����֌W�̃p�����[�^ </summary>
    [Header("����֌W�̃p�����[�^")]
    [SerializeField] private SlidingParameter slidingParameter;

    // ------------------------------
    // �h��
    // ------------------------------

    /// <summary> �h��֌W�̃p�����[�^ </summary>
    [System.Serializable]
    private struct GuardParameter
    {
        /// <summary> �h�䎞�̃J�b�g�� </summary>
        [Header("�h�䎞�̃J�b�g��(%)")]
        public float cutRatio;

        /// <summary> �h��C���^�[�o������(�b) </summary>
        [Header("�h��C���^�[�o������(�b)")]
        public float interval;

        /// <summary> �h��\�Ȃ�true </summary>
        [NonSerialized] public bool canGuard;

        /// <summary> �h�䒆�Ȃ�true </summary>
        [NonSerialized] public bool isGuard;
    }

    /// <summary> �h��֌W�̃p�����[�^ </summary> 
    [Header("�h��֌W�̃p�����[�^")]
    [SerializeField] private GuardParameter guardParameter;

    // ----------------------------
    // �^�_���[�W
    // ----------------------------

    /// <summary> true�̂Ƃ��A���킪������悤�ɂȂ�(���i�h�~) </summary>
    private bool canWeaponHit = false;

    /// <summary>
    /// ���킪�����邩�ǂ���
    /// </summary>
    public bool CanWeaponHit { get { return canWeaponHit; } }

    // ----------------------------
    // ���S
    // ----------------------------

    /// <summary> ���S���[�V�������I��������true </summary>
    private bool isDead = false;

    /// <summary>
    /// ���S�������ǂ���
    /// </summary>
    public bool IsDead { get { return isDead; } }

    // ----------------------------
    // �U�����m�p
    // ----------------------------

    /// <summary> �U���Ȃǂ����m�ł��鎞�� </summary>
    [System.Serializable]
    private struct ReceptionTime
    {
        /// <summary> �U�������m�ł��鎞��(�b) </summary>
        [Header("�U�������m�ł��鎞��(�b)")]
        public float attack;

        /// <summary> �K�E�Z�����m�ł��鎞��(�b) </summary>
        [Header("�K�E�Z�����m�ł��鎞��(�b)")]
        public float ult;
    }

    /// <summary> �U���Ȃǂ����m�ł��鎞�� </summary>
    [Header("�U���Ȃǂ����m�ł��鎞��")]
    [SerializeField] private ReceptionTime receptionTime;

    // --------------------
    // AI����p
    // --------------------

    /// <summary> �GAI����p </summary>
    private NavMeshAgent enemyAi;

    // --------------------------------
    // �A�j���[�V�������m�p
    // --------------------------------

    /// <summary> �v���C���[�̃A�j���[�V�������m�p </summary>
    [System.Serializable]
    private struct PlayerAnimation
    {
        /// <summary> �v���C���[�̕K�E�Z�A�j���[�V���� </summary>
        [Header("�K�E�Z�̃A�j���[�V����(�v���C���[)")]
        public AnimationClip ult;

        /// <summary> �v���C���[�̍U��1�A�j���[�V���� </summary>
        [Header("�v���C���[�̍U��1�A�j���[�V����")]
        public AnimationClip attack1;

        /// <summary> �v���C���[�̍U��2�A�j���[�V���� </summary>
        [Header("�v���C���[�̍U��2�A�j���[�V����")]
        public AnimationClip attack2;

        /// <summary> �v���C���[�̎��S�A�j���[�V���� </summary> 
        [Header("�v���C���[�̎��S�A�j���[�V����")]
        public AnimationClip dead;
    }

    [Header("�v���C���[�̃A�j���[�V�������m�p")]
    [SerializeField] private PlayerAnimation playerAnimation;

    /// <summary> �v���C���[�̃A�j���[�V�����ǂݎ��p </summary>
    private Animator playerAnimator;

    // ---------------------------------
    // �A�j���[�V��������p
    // ---------------------------------

    /// <summary> �G�A�j���[�V��������p </summary>
    private Animator enemyAnimator;

    // ---------------------------------
    // �A�j���[�V�����J�ڗp
    // ---------------------------------

    /// <summary> �_�b�V�����[�V�����J�ڗp </summary>
    private readonly string isRun = "IsRun";

    /// <summary> �U���A�j���[�V�����J�ڗp </summary>
    private readonly string isAttack = "IsAttack";

    /// <summary> �K�E�Z�A�j���[�V�����J�ڗp </summary>
    private readonly string ultTirgger = "UltTrigger";

    /// <summary> ����A�j���[�V�����J�ڗp </summary>
    private readonly string slidingTrigger = "SlidingTrigger";

    /// <summary> �h��A�j���[�V�����J�ڗp </summary>
    private readonly string guardTrigger = "GuardTrigger";

    /// <summary> ��_���[�W�A�j���[�V�����J�ڗp </summary>
    private readonly string hitTrigger = "HitTrigger";

    /// <summary> ���S�A�j���[�V�����J�ڗp </summary>
    private readonly string deadTrigger = "DeadTrigger";

    // -----------------------
    // �T�E���h
    // -----------------------

    /// <summary> �T�E���h </summary>
    [System.Serializable]
    private struct Sound
    {
        /// <summary> �U����SE </summary>
        [Header("�U����SE")]
        public AudioClip attack;

        /// <summary> �U��1�̃q�b�g��SE </summary>
        [Header("�U��1�̃q�b�g��SE")]
        public AudioClip hit_attack1;

        /// <summary> �U��2�̃q�b�g��SE </summary>
        [Header("�U��2�̃q�b�g��SE")]
        public AudioClip hit_attack2;

        /// <summary> �K�E�Z�̃W�����v��SE </summary>
        [Header("�K�E�Z�̃W�����v��SE")]
        public AudioClip ult_jump;

        /// <summary> �K�E�ZSE </summary>
        [Header("�K�E�ZSE")]
        public AudioClip ult;

        /// <summary> ���SE </summary>
        [Header("���SE")]
        public AudioClip sliding;

        /// <summary> �h��J�nSE </summary>
        [Header("�h��J�nSE")]
        public AudioClip guardStart;

        /// <summary> �h��SE </summary>
        [Header("�h��SE")]
        public AudioClip guard;

        /// <summary> ��_���[�WSE </summary>
        [Header("��_���[�WSE")]
        public AudioClip receiveDamage;

        /// <summary> ���SSE </summary>
        [Header("���SSE")]
        public AudioClip dead;
    }

    /// <summary> �T�E���h </summary>
    [Header("�T�E���h")]
    [SerializeField] private Sound sound;

    /// <summary> �T�E���h�p�R���|�[�l���g </summary>
    private AudioSource audioSource;

    // ------------------------------
    // �G�t�F�N�g
    // ------------------------------

    [System.Serializable]
    private struct Effect
    {
        /// <summary> �U��1�G�t�F�N�g </summary>
        [Header("�U��1�G�t�F�N�g")]
        public GameObject attack1;

        /// <summary> �U��2�G�t�F�N�g </summary>
        [Header("�U��2�G�t�F�N�g")]
        public GameObject attack2;

        /// <summary> �h��G�t�F�N�g </summary>
        [Header("�h��G�t�F�N�g")]
        public GameObject guard;

        /// <summary> �h��G�t�F�N�g��t�^����I�u�W�F�N�g </summary>
        [Header("�h��G�t�F�N�g��t�^����I�u�W�F�N�g")]
        public Transform guardParent;
    }

    /// <summary> �G�t�F�N�g </summary>
    [Header("�G�t�F�N�g")]
    [SerializeField] private Effect effect;

    // -----------------------
    // �^�C�}�[
    // -----------------------

    /// <summary> �U���p�^�C�}�[ </summary>
    private float attackTimer = 0;

    /// <summary> �K�E�Z�p�^�C�}�[ </summary>
    private float ultTimer = 0;

    /// <summary> ���p�^�C�}�[ </summary>
    private float slidingTimer = 0;

    /// <summary> �h��p�^�C�}�[ </summary>
    private float guardTimer = 0;

    // ----------------------------
    // �����蔻��
    // ----------------------------

    /// <summary> �����蔻�萧��p </summary>
    private CapsuleCollider capsuleCollider;

    private void Awake()
    {
        // HP���ő�ɂ���
        hp = maxHp;
    }

    // Start is called before the first frame update
    private void Start()
    {
        // AI����p�R���|�[�l���g���擾
        enemyAi = gameObject.GetComponent<NavMeshAgent>();

        // �A�j���[�V��������p�R���|�[�l���g���擾
        enemyAnimator = gameObject.GetComponent<Animator>();

        // �v���C���[�Ǘ��p�R���|�[�l���g���擾
        playerManager = player.GetComponent<CS_Player>();

        // �v���C���[��
        // �A�j���[�V��������p�R���|�[�l���g���擾
        playerAnimator = player.GetComponent<Animator>();

        // �����蔻�萧��p�R���|�[�l���g���擾
        capsuleCollider = gameObject.GetComponent<CapsuleCollider>();

        // �T�E���h�p�R���|�[�l���g���擾
        audioSource = gameObject.GetComponent<AudioSource>();

        // �������~�߂���Ԃ���n�߂�
        Standby();
    }

    // Update is called once per frame
    private void Update()
    {
        // -------------------------------
        // �R���|�[�l���g�m�F
        // -------------------------------

        if (!CheckComponent())
        {
            return;
        }

        // -------------------------------
        // �^�C�}�[
        // -------------------------------

        TimerCount();

        // ------------------------------
        // ���S�m�F
        // ------------------------------

        // HP��0�ɂȂ��Ă���̂�
        // ���S��ԂɂȂ��ĂȂ��Ȃ�ڍs����
        if (hp <= 0 &&
            currentState != State.Dead)
        {
            ChangeState(State.Dead);
            return;
        }

        // -----------------------------
        // �v���C���[�̎��S�m�F
        // -----------------------------

        // �v���C���[�����S�����̂őҋ@
        if (CheckPlayerDead())
        {
            Standby();
        }

        // -------------------------------
        // ��Ԃɍ��킹�čs��
        // -------------------------------

        switch (currentState)
        {
            // ------------------------
            // �ҋ@
            // ------------------------
            case State.Idle:

                Idle();

                break;

            // ---------------------------
            // �v���C���[��ǔ�����
            // ---------------------------
            case State.Chase:

                Chase();

                break;

            // --------------------------------------------
            // �v���C���[�̍s���Ȃǂ���A�G�̍s����I��
            // (�U���E����E�h��E�ړ�)
            // --------------------------------------------
            case State.SelectAction:

                // �s���I��
                SelectAction();

                break;

            // ------------------------
            // �U��1
            // ------------------------
            case State.Attack1:

                break;

            // ------------------------
            // �U��2
            // ------------------------
            case State.Attack2:

                break;

            // ------------------------
            // �K�E�Z
            // ------------------------
            case State.Ult:

                break;

            // ------------------------
            // ���
            // ------------------------
            case State.Sliding:

                break;

            // ------------------------
            // �h��
            // ------------------------
            case State.Guard:

                break;

            // ------------------------
            // ��_���[�W
            // ------------------------
            case State.ReceiveDamage:

                break;

            // ------------------------
            // ���S
            // ------------------------
            case State.Dead:

                break;
        }
    }

    #region ��ԕʂ̏���

    /// <summary>
    /// �ҋ@��Ԃ̏���
    /// </summary>
    private void Idle()
    {
        // �v���C���[�m�F
        if (!player)
        {
            Debug.Log("�v���C���[�����܂���");
            return;
        }

        // �ǔ����x��0�Ȃ�ҋ@
        if (speedParameter.chaseMax <= 0)
        {
            Debug.Log("�ǔ����x��0�ȉ��ɐݒ肳��Ă��܂�");
            return;
        }

        // �������~�߂Ă���̂őҋ@
        if (isStop)
        {
            return;
        }

        // �ǔ���ԂɈڍs
        ChangeState(State.Chase);
    }

    /// <summary>
    /// �ǔ���Ԃ̏���
    /// </summary>
    private void Chase()
    {
        // �v���C���[�Ɍ������Ĉړ�
        enemyAi.SetDestination(player.position);

        // �v���C���[�̕���������
        LookTarget(player);

        // �v���C���[�Ƃ̋��������ȓ�
        if (IsNear(player))
        {
            enemyAi.speed = 0;

            // �s���I����ԂɈڍs
            ChangeState(State.SelectAction);
            return;
        }
    }

    /// <summary>
    /// �s���I��
    /// </summary>
    private void SelectAction()
    {
        // -----------------------------
        // �U���E�K�E�Z���\��
        // -----------------------------

        // �K�E�Z���g�p�\
        if (ultParameter.canUlt)
        {
            ChangeState(State.Ult);
            return;
        }
        // �K�E�Z���g�p�s�ōU���\
        else if (attackParameter.canAttack)
        {
            // �^�C�}�[���Z�b�g
            attackTimer = 0;

            // �U����ԂɈڍs
            ChangeState(State.Attack1);
            return;
        }

        // -------------------------------------
        // �v���C���[�̕K�E�Z�����m������ 
        // -------------------------------------

        // �v���C���[�̕K�E�Z�����m��
        // �h��\�Ȃ�h�䂷��
        if (CheckPlayerUlt() && guardParameter.canGuard)
        {
            // �h���ԂɈڍs
            ChangeState(State.Guard);
            return;
        }

        // -------------------------------------
        // �v���C���[�̍U�������m������
        // -------------------------------------

        // �v���C���[�̍U�������m��
        // ����\�Ȃ�������
        if (CheckPlayerAttack() && slidingParameter.canSliding)
        {
            ChangeState(State.Sliding);
            return;
        }
        // ���s�\��
        // �h��\�Ȃ�h�䂷��
        else if (CheckPlayerAttack() && guardParameter.canGuard)
        {
            ChangeState(State.Guard);
            return;
        }

        // ----------------------------------
        // �U���̊m�����������Ă���
        // ----------------------------------

        // �U���̊m�����������Ă���U��
        if (attackParameter.isPercent)
        {
            ChangeState(State.Attack1);
            return;
        }

        // ---------------------------------
        // �v���C���[�Ƃ̋������m�F
        // ---------------------------------

        // �v���C���[�Ƃ̋��������ȏ�Ȃ�
        // �ǔ���ԂɈڍs
        if (!IsNear(player))
        {
            ChangeState(State.Chase);
            return;
        }

        // ----------------------------------
        // �v���C���[�𒆐S�ɉ�]�ړ�
        // ----------------------------------

        // ���͒ǔ�������
        Chase();
    }

    /// <summary>
    /// �v���C���[�𒆐S�ɉ�]�ړ�
    /// </summary>
    /// <param name="moveLeft"> 
    /// <para> true : �E�ړ� </para>
    /// <para> false : ���ړ� </para>
    /// </param>
    private void MoveAround(bool moveRight = true)
    {
        // ��]�ړ��̑��x
        float rotateSpeed = speedParameter.nearMove * 10.0f;

        // �E�ړ�
        if (moveRight)
        {
            // �ړ��������t�ɂ���
            rotateSpeed *= -1;
        }

        // �v���C���[�𒆐S�ɉ�]�ړ�
        transform.RotateAround(player.position, Vector3.up,
            rotateSpeed * Time.deltaTime);

        return;
    }

    #endregion

    #region �A�j���[�V�����C�x���g

    #region �U���C�x���g

    /// <summary>
    /// �U��1�̊J�n�C�x���g
    /// </summary>
    private void AnimAttack1()
    {
        // ���킪������悤�ɂ���
        canWeaponHit = true;

        // �v���C���[�̕�������
        LookTarget(player);

        // �U��1SE
        PlayOneSound(sound.attack);

        // �U��1�G�t�F�N�g
        CreateEffect(effect.attack1, transform);
    }

    private void AnimAttackOk()
    {
        return;
    }

    /// <summary>
    /// �U��1�̏I���C�x���g
    /// </summary>
    private void AnimAttack1Faild()
    {
        // �v���C���[���܂��߂��ɂ���ꍇ
        if (IsNear(player))
        {
            // �U��2�Ɉڍs
            ChangeState(State.Attack2);
            return;
        }

        // ���킪������Ȃ��悤�ɂ���
        canWeaponHit = false;

        // �U���I��
        enemyAnimator.SetBool(isAttack, false);

        // ���̌�܂��U�����邩���I����
        attackParameter.isPercent = CheckProbability(attackParameter.percent);

        // �ҋ@��ԂɈڍs
        ChangeState(State.Idle);
    }

    /// <summary>
    /// �U��2�̊J�n�C�x���g
    /// </summary>
    private void AnimAttack2()
    {
        // ���킪������悤�ɂ���
        canWeaponHit = true;

        // �v���C���[�̕�������
        LookTarget(player);

        // �U��2SE
        PlayOneSound(sound.attack);

        // �U��2�G�t�F�N�g
        CreateEffect(effect.attack2, transform);
    }

    /// <summary>
    /// �U��2�̏I���C�x���g
    /// </summary>
    private void AnimAttack2Faild()
    {
        // ���킪������Ȃ��悤�ɂ���
        canWeaponHit = false;

        // �U���I��
        enemyAnimator.SetBool(isAttack, false);

        // �^�C�}�[���Z�b�g
        attackTimer = 0;

        // ���̌�ōU�����邩���I����
        attackParameter.isPercent = CheckProbability(attackParameter.percent);

        // �ҋ@��ԂɈڍs
        ChangeState(State.Idle);

        return;
    }

    #endregion

    #region �K�E�Z�C�x���g

    /// <summary>
    /// �K�E�Z�̊J�n�C�x���g
    /// </summary>
    private void AnimUlt()
    {
        // ���킪������悤�ɂ���
        canWeaponHit = true;

        // �v���C���[�̕�������
        LookTarget(player);

        // �v���C���[������ʂ��悤��
        // �Փ˔���𖳂��ɂ���
        capsuleCollider.isTrigger = true;

        // �K�E�Z�T�E���h
        PlayOneSound(sound.ult);
    }

    /// <summary>
    /// �K�E�Z�̔���I���C�x���g
    /// </summary>
    private void UltFinish()
    {
        // ���킪������Ȃ��悤�ɂ���
        canWeaponHit = false;

        // �G���ђʂ��Ȃ��悤��
        // �Փ˔����߂�
        capsuleCollider.isTrigger = false;
    }

    /// <summary>
    /// �K�E�Z�̏I���C�x���g
    /// </summary>
    private void AnimUltFailed()
    {
        // �^�C�}�[���Z�b�g
        ultTimer = 0;

        // �A�j���[�V�������x��������
        enemyAnimator.speed = 1;

        // �ҋ@��ԂɈڍs
        ChangeState(State.Idle);
    }

    #endregion

    #region ����C�x���g

    /// <summary>
    /// ����I���C�x���g
    /// </summary>
    private void AnimSlidingFiled()
    {
        // ���p�^�C�}�[�����Z�b�g
        slidingTimer = 0;

        // �ҋ@��ԂɈڍs
        ChangeState(State.Idle);
    }

    #endregion

    #region �h��C�x���g

    /// <summary>
    /// �h�䔻��̏I���C�x���g
    /// </summary>
    private void GuardFinish()
    {
        // �h��I��
        guardParameter.isGuard = false;
    }

    /// <summary>
    /// �h��A�j���[�V�����̏I���C�x���g
    /// </summary>
    private void AnimGuardFailed()
    {
        // �h��p�^�C�}�[�����Z�b�g
        guardTimer = 0;

        // �ҋ@��ԂɈڍs
        ChangeState(State.Idle);
    }

    #endregion

    #region ��_���[�W�C�x���g

    private void AnimDamgeFailed()
    {
        // �ҋ@��ԂɈڍs
        ChangeState(State.Idle);
    }

    #endregion

    #region ���S�C�x���g

    private void DeadSound()
    {
        PlayOneSound(sound.dead);
    }

    private void AnimDeadFailed()
    {
        isDead = true;
    }

    #endregion

    #endregion

    #region ��_���[�W

    /// <summary>
    /// �_���[�W���󂯂�
    /// </summary>
    /// <param name="damage"> �_���[�W�� </param>
    public void ReceiveDamage(float damage)
    {
        // ���Ɏ��S�����Ȃ牽�����Ȃ�
        if (currentState == State.Dead)
        {
            return;
        }

        // �K�E�Z���̓_���[�W���󂯂Ȃ�
        if (currentState == State.Ult)
        {
            return;
        }

        // ��𒆂̓_���[�W���󂯂Ȃ�
        if (currentState == State.Sliding)
        {
            return;
        }

        // �h�䒆
        if (guardParameter.isGuard)
        {
            // �_���[�W�̌y���l
            float cut = damage * (guardParameter.cutRatio / 100);

            // �_���[�W���y������������
            // HP�����炷
            damage -= cut;
            hp -= damage;

            // �h��SE
            PlayOneSound(sound.guard);

            // �h��G�t�F�N�g
            CreateEffect(effect.guard, effect.guardParent);
        }
        else
        {
            // �_���[�W��HP�����炷
            hp -= damage;

            // ��_���[�WSE
            PlayOneSound(sound.receiveDamage);
        }

        // HP�������Ȃ����玀�S
        if (hp <= 0)
        {
            ChangeState(State.Dead);
            return;
        }

        // �X�[�p�[�A�[�}�[�Ȃ�U������
        // ��_���[�W���[�V�����͂��Ȃ�
        if (currentState == State.Attack1 ||
            currentState == State.Attack2)
        {
            if (attackParameter.hasSuperArmor)
                return;
        }

        // ��_���[�W���[�V�������ɒǉ��Ń��[�V�������N�����Ȃ�
        if (currentState == State.ReceiveDamage)
        {
            return;
        }

        // �h�䒆�Ȃ��_���[�W���[�V�����͂��Ȃ�
        if (guardParameter.isGuard)
        {
            return;
        }

        // HP���c���Ă���̂Ŕ�_���[�W��ԂɈڍs
        ChangeState(State.ReceiveDamage);
    }

    #endregion

    #region �^�_���[�W

    /// <summary>
    /// �v���C���[�Ƀ_���[�W��^����
    /// </summary>
    public void PlayerDamage()
    {
        // ���i�h�~�ōU����������Ȃ��悤�ɂ���
        canWeaponHit = false;

        // �U��1
        if (currentState == State.Attack1)
        {
            // �U��1�̍U���͂��Q��
            playerManager.ReceiveDamage(attackParameter.power_attack1);

            // �U��1�̃T�E���h
            PlayOneSound(sound.hit_attack1);
        }

        // �U��2
        if (currentState == State.Attack2)
        {
            // �U��2�̍U���͂��Q��
            playerManager.ReceiveDamage(attackParameter.power_attack2);

            // �U��2�T�E���h
            PlayOneSound(sound.hit_attack2);
        }

        // �K�E�Z
        if (currentState == State.Ult)
        {
            // �K�E�Z�̈З͂��Q��
            playerManager.ReceiveDamage(ultParameter.power);
        }
    }

    #endregion

    #region ��������p

    /// <summary>
    /// �������~�߂�
    /// </summary>
    public void Standby()
    {
        ChangeState(State.Idle);
        isStop = true;
    }

    /// <summary>
    /// ��~��Ԃ���������
    /// </summary>
    public void CancelStandby()
    {
        ChangeState(State.Idle);
        isStop = false;
    }

    #endregion

    #region ����p

    /// <summary>
    /// �R���|�[�l���g������̂��m�F����
    /// </summary>
    /// <returns>
    /// <para> true : �R���|�[�l���g������ </para>
    /// <para> false : �R���|�[�l���g���Ȃ� </para>
    /// </returns>
    private bool CheckComponent()
    {
        // �G����p�R���|�[�l���g���Ȃ�
        if (!enemyAi)
        {
            Debug.Log("NavMeshAgent������܂���");
            return false;
        }

        // �A�j���[�V��������p�R���|�[�l���g���Ȃ�
        if (!enemyAnimator)
        {
            Debug.Log("�G��Animator������܂���");
            return false;
        }

        // �R���C�_�[���m�F
        if (!capsuleCollider)
        {
            Debug.Log("�R���C�_�[������܂���");
            return false;
        }

        return true;
    }

    /// <summary>
    /// �^�[�Q�b�g���߂��ɂ��邩
    /// </summary>
    /// <param name="target"> �Ώ� </param>
    /// <returns>
    /// <para> true : �^�[�Q�b�g�Ƃ̋��������ȓ� </para>
    /// <para> false : �^�[�Q�b�g�Ƃ̋��������ȏ� </para>
    /// </returns>
    private bool IsNear(Transform target)
    {
        // �^�[�Q�b�g�Ƃ̋���
        float distance = Vector3.Distance(
            target.position, transform.position);

        // ���ȓ��Ƀ^�[�Q�b�g������̂�
        return distance < attackParameter.triggerDistance;
    }

    /// <summary>
    /// �m������
    /// </summary>
    /// <param name="percent"> true�ɂȂ�m��(%) </param>
    /// <returns>
    /// �w�肵���m����true�ɂȂ�
    /// </returns>
    private bool CheckProbability(float percent)
    {
        // ����(0 �` 100.0)
        float randomValue = UnityEngine.Random.value * 100.0f;

        // �m�����l���������ꍇ�́A
        // �m�����������Ɣ��肷��
        return randomValue < percent;
    }

    #endregion

    #region �v���C���[�̍s�����m�p

    /// <summary>
    /// �v���C���[�̕K�E�Z�����m�������m�F����
    /// </summary>
    /// <returns> 
    /// <para> true : �K�E�Z�����m���� </para>
    /// <para> false : �K�E�Z�����m���Ȃ����� </para>
    /// </returns>
    private bool CheckPlayerUlt()
    {
        // �v���C���[�̃A�j���[�^�[���Ȃ�
        if (!playerAnimator)
        {
            Debug.Log("�v���C���[�̃A�j���[�^�[������܂���");
            return false;
        }

        // �K�E�Z�̃A�j���[�V�������ݒ肳��Ă��Ȃ�
        if (!playerAnimation.ult)
        {
            Debug.Log("�v���C���[�̕K�E�Z�A�j���[�V�������ݒ肳��Ă��܂���");
            return false;
        }

        // �v���C���[���K�E�Z�����Ă��Ȃ�
        if (!IsPlayingAnim(playerAnimator, playerAnimation.ult))
        {
            return false;
        }

        // �v���C���[�̕K�E�Z�̔���J�n����
        float ultStartTime = playerAnimation.ult.events[0].time;

        // �A�j���[�V�����̌o�ߎ���
        float elapsedTime = GetAnimElapsedTime(playerAnimator);

        // �v���C���[�̕K�E�Z�̃A�j���[�V������
        // ������x�Đ����ꂽ�猟�m
        return elapsedTime >= ultStartTime &&
            elapsedTime < ultStartTime + receptionTime.ult;
    }

    /// <summary>
    /// �v���C���[�̍U��1�E�U��2�����m�������m�F����
    /// </summary>
    /// <returns>
    /// <para> true : ���m���� </para>
    /// <para> false : ���m���Ȃ����� </para>
    /// </returns>
    private bool CheckPlayerAttack()
    {
        // �v���C���[�̃A�j���[�^�[���Ȃ�
        if (!playerAnimator)
        {
            Debug.Log("�v���C���[�̃A�j���[�^�[������܂���");
            return false;
        }

        // �v���C���[�̍U�����ݒ肳��Ă��Ȃ�
        if (!CheckPlayerAttakAnim())
        {
            return false;
        }

        // �v���C���[���U�����Ă��Ȃ�
        if (!IsPlayingAnim(playerAnimator, playerAnimation.attack1) &&
            !IsPlayingAnim(playerAnimator, playerAnimation.attack2))
        {
            return false;
        }

        // �U���̌o�ߎ���
        float elapsedTime = GetAnimElapsedTime(playerAnimator);

        // �U�����m�\�Ȏ��Ԃ��o�߂��Ă���̂�
        // �U�������m�ł��Ȃ�
        if (elapsedTime > receptionTime.attack)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// �v���C���[�̎��S�����m�������m�F����
    /// </summary>
    /// <returns>
    /// <para> true : ���m���� </para>
    /// <para> false : ���m���Ȃ����� </para>
    /// </returns>
    private bool CheckPlayerDead()
    {
        // �v���C���[�̃A�j���[�^�[���Ȃ�
        if (!playerAnimator)
        {
            Debug.Log("�v���C���[�̃A�j���[�^�[������܂���");
            return false;
        }

        // ���S�A�j���[�V�������ݒ肳��Ă��Ȃ�
        if (!playerAnimation.dead)
        {
            Debug.Log("�v���C���[�̎��S�A�j���[�V�������ݒ肳��Ă��܂���");
            return false;
        }

        // �v���C���[�����S���Ă��Ȃ�
        if (!IsPlayingAnim(playerAnimator, playerAnimation.dead))
        {
            return false;
        }

        return true;
    }

    #endregion

    #region �A�j���[�V����

    /// <summary>
    /// �A�j���[�V�����̌o�ߎ��Ԃ��擾����
    /// </summary>
    /// <param name="animator"> �A�j���[�^�[ </param>
    /// <returns> �A�j���[�V�����̌o�ߎ��� </returns>
    private float GetAnimElapsedTime(Animator animator)
    {
        // ���݂̌o�ߊ���(0 �` 1.0)
        float currentTimeRatio = GetCurrentAnim(animator).normalizedTime;

        // �A�j���[�V�����̍Đ����ԂɊ|����
        // �o�ߎ��Ԃ����߂�
        return playerAnimation.ult.length * currentTimeRatio;
    }

    /// <summary>
    /// �Đ����̃A�j���[�V�������擾����
    /// </summary>
    /// <param name="animator"> �A�j���[�^�[ </param>
    /// <returns> �Đ����̃A�j���[�V���� </returns>
    private AnimatorStateInfo GetCurrentAnim(
        Animator animator, int layerIndex = 0)
    {
        // �Đ����̃A�j���[�V����
        return animator.GetCurrentAnimatorStateInfo(layerIndex);
    }

    /// <summary>
    /// �w�肵���A�j���[�V�������Đ������m�F����
    /// </summary>
    /// <param name="animator"> �A�j���[�^�[ </param>
    /// <param name="anim"> �m�F����A�j���[�V���� </param>
    /// <returns>
    /// <para> true : �Đ��� </para>
    /// <para> false : �Đ����Ă��Ȃ� </para>
    /// </returns>
    private bool IsPlayingAnim(
        Animator animator, AnimationClip anim)
    {
        // �Đ����̃A�j���[�V����
        var currentAnim = GetCurrentAnim(animator);

        // �Đ����̃A�j���[�V�����̖��O��
        // �w�肵�����O�ƈ�v����Ȃ�true
        return currentAnim.IsName(anim.name);
    }

    /// <summary>
    /// �v���C���[�̍U���A�j���[�V�������ݒ肳��Ă��邩�m�F����
    /// </summary>
    /// <returns>
    /// <para> true : �ݒ肳��Ă��� </para>
    /// <para> false : �ݒ肳��Ă��Ȃ� </para>
    /// </returns>
    private bool CheckPlayerAttakAnim()
    {
        // �v���C���[�̍U��1���ݒ肳��Ă��Ȃ�
        if (!playerAnimation.attack1)
        {
            Debug.Log("�v���C���[�̍U��1�A�j���[�V�������ݒ肳��Ă��܂���");
            return false;
        }

        // �v���C���[�̍U��2���ݒ肳��Ă��Ȃ�
        if (!playerAnimation.attack1)
        {
            Debug.Log("�v���C���[�̍U��2�A�j���[�V�������ݒ肳��Ă��܂���");
            return false;
        }

        return true;
    }

    #endregion

    #region �T�E���h

    /// <summary>
    /// �T�E���h���ݒ肳��Ă��邩�m�F����
    /// </summary>
    /// <param name="clip"> �m�F����T�E���h </param>
    /// <returns>
    /// <para> true : �T�E���h������ </para>
    /// <para> true : �T�E���h�܂��́A�T�E���h�p�R���|�[�l���g���Ȃ� </para>
    /// </returns>
    bool CheckSound(AudioClip clip)
    {
        // �T�E���h�p�R���|�[�l���g
        // �܂��̓T�E���h�N���b�v���ݒ肳��Ă��Ȃ�
        if (!audioSource || !clip)
        {
            Debug.Log("SE�܂��́AAudioSorce�R���|�[�l���g������܂���");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 1�񂾂��T�E���h��炷
    /// </summary>
    /// <param name="clip"> �炵�����T�E���h </param>
    void PlayOneSound(AudioClip clip)
    {
        if (CheckSound(clip))
        {
            audioSource.PlayOneShot(clip);
        }
    }

    #endregion

    #region �G�t�F�N�g

    /// <summary>
    /// �G�t�F�N�g�𐶐�����
    /// </summary>
    /// <param name="e"> ��������G�t�F�N�g </param>
    /// <param name="effectParent"> �G�t�F�N�g��t�^����I�u�W�F�N�g </param>
    private void CreateEffect(GameObject e, Transform effectParent)
    {
        var effectObject = Instantiate(e, effectParent);
        Destroy(effectObject, 1.0f);
    }

    #endregion

    /// <summary>
    /// ��Ԃ��ڍs����
    /// </summary>
    /// <param name="state"> �ڍs��̏�� </param>
    private void ChangeState(State state)
    {
        // �w�肳�ꂽ��ԂɈڍs����
        switch (state)
        {
            // -------------------------
            // �ҋ@��ԂɈڍs
            // -------------------------
            case State.Idle:

                // �_�b�V�����[�V�����I��
                enemyAnimator.SetBool(isRun, false);

                enemyAi.speed = 0;

                // �ҋ@��ԂɈڍs
                currentState = State.Idle;

                break;

            // ----------------------
            // �ǔ���ԂɈڍs
            // ----------------------
            case State.Chase:

                // �ő呬�x��ݒ�
                enemyAi.speed = speedParameter.chaseMax;

                // �����x��ݒ�
                enemyAi.acceleration = speedParameter.chaseAcceleration;

                // �_�b�V�����[�V�����J�n
                enemyAnimator.SetBool(isRun, true);

                // �ǔ���ԂɈڍs
                currentState = State.Chase;

                break;

            // --------------------------
            // �s���I����ԂɈڍs
            // --------------------------

            case State.SelectAction:

                // �s���I����ԂɈڍs
                currentState = State.SelectAction;

                break;

            // -----------------------
            // �U��1��ԂɈڍs
            // -----------------------
            case State.Attack1:

                // �U��1�A�j���[�V�����J�n
                enemyAnimator.SetBool(isAttack, true);

                // �U��1��ԂɈڍs
                currentState = State.Attack1;

                break;

            // -----------------------
            // �U��2��ԂɈڍs
            // -----------------------
            case State.Attack2:

                // �U��2��ԂɈڍs
                currentState = State.Attack2;

                break;

            // -----------------------
            // �K�E�Z��ԂɈڍs
            // -----------------------
            case State.Ult:

                // �K�E�Z�̑��x��ݒ�
                enemyAnimator.speed = ultParameter.speed;

                // �K�E�Z�A�j���[�V�����J�n
                enemyAnimator.SetTrigger(ultTirgger);

                // �K�E�Z�̃W�����v��SE
                PlayOneSound(sound.ult_jump);

                // �K�E�Z��ԂɈڍs
                currentState = State.Ult;

                break;

            // -----------------------
            // �����ԂɈڍs
            // -----------------------
            case State.Sliding:

                // ����A�j���[�V�����J�n
                enemyAnimator.SetTrigger(slidingTrigger);

                // ���SE
                PlayOneSound(sound.sliding);

                // �����ԂɈڍs
                currentState = State.Sliding;

                break;

            // ------------------------
            // �h���ԂɈڍs
            // ------------------------
            case State.Guard:

                // �h��J�n
                guardParameter.isGuard = true;

                // �h��A�j���[�V�����J�n
                enemyAnimator.SetTrigger(guardTrigger);

                // �h��SE
                PlayOneSound(sound.guardStart);

                // �h���ԂɈڍs
                currentState = State.Guard;

                break;

            // ---------------------------
            // ��_���[�W��ԂɈڍs
            // ---------------------------
            case State.ReceiveDamage:

                // �ړ����Ȃ��悤�ɂ���
                enemyAi.speed = 0;

                // ��_���[�W���[�V�����J�n
                enemyAnimator.SetTrigger(hitTrigger);

                // ��_���[�W��ԂɈڍs
                currentState = State.ReceiveDamage;

                break;

            // ------------------------
            // ���S��ԂɈڍs
            // ------------------------
            case State.Dead:

                // �ړ����Ȃ��悤�ɂ���
                enemyAi.speed = 0;

                // ���S�A�j���[�V�����J�n
                enemyAnimator.SetTrigger(deadTrigger);

                // ���S��ԂɈڍs
                currentState = State.Dead;

                break;
        }
    }

    /// <summary>
    /// �^�[�Q�b�g�̕�������(Y����]�̂�)
    /// </summary>
    /// <param name="target"> �Ώ� </param>
    private void LookTarget(Transform target)
    {
        // �^�[�Q�b�g�̍��W
        Vector3 targetPos = target.position;

        // �㉺�̉�]�����Ȃ��悤��
        // Y���W�𓯂��ɂ���
        targetPos.y = transform.position.y;

        // �v���C���[�̕��������悤�ɉ�]
        transform.LookAt(targetPos);
    }

    /// <summary>
    /// �e��^�C�}�[��i�߂�
    /// </summary>
    private void TimerCount()
    {
        // ----------------------------
        // �^�C�}�[��i�߂�
        // ----------------------------

        // �U�����Ă���̎��Ԃ��v��
        if (!attackParameter.canAttack) attackTimer += Time.deltaTime;

        // �K�E�Z���g�p���Ă���̎��Ԃ��v��
        if (!ultParameter.canUlt) ultTimer += Time.deltaTime;

        // ������Ă���̎��Ԃ��v��
        if (!slidingParameter.canSliding) slidingTimer += Time.deltaTime;

        // �h�䂵�Ă���̎��Ԃ��v��
        if (!guardParameter.canGuard) guardTimer += Time.deltaTime;

        // ----------------------------------
        // ��莞�Ԍo�������m�F
        // ----------------------------------

        attackParameter.canAttack = attackTimer >= attackParameter.interval;
        ultParameter.canUlt = ultTimer >= ultParameter.interval;
        slidingParameter.canSliding = slidingTimer >= slidingParameter.interval;
        guardParameter.canGuard = guardTimer >= guardParameter.interval;
    }
}
