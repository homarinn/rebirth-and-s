using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// �G�̊Ǘ��N���X
/// </summary>
public class CS_EnemyPlayer : MonoBehaviour
{
    /// <summary>
    /// ���
    /// </summary>
    enum State
    {
        /// <summary> �ҋ@ </summary>
        Idle,

        /// <summary> �ǔ� </summary>
        Chase,

        /// <summary> �s����I�� </summary>
        SelectAction,

        /// <summary> �U�� </summary>
        Attack,

        /// <summary> �K�E�Z </summary>
        Ult,

        /// <summary> ��� </summary>
        Sliding,

        /// <summary> �h�� </summary>
        Guard,

        /// <summary> ���S </summary>
        Dead,
    }

    // ----------------------
    // �v���C���[
    // ----------------------

    /// <summary> �v���C���[ </summary>
    [Header("�v���C���[�擾")]
    [SerializeField] private Transform player;

    // ---------------------------
    // HP
    // ---------------------------

    /// <summary> �ő�HP </summary>
    [Header("�ő�HP")]
    [SerializeField] private float maxHp;

    /// <summary> ���݂�HP </summary>
    private float hp;

    // ---------------------
    // ���x
    // ---------------------

    /// <summary> �ǔ����x </summary>
    [Header("�ǔ����x")]
    [SerializeField] private float chaseSpeed;

    /// <summary> �v���C���[�ɋ߂Â�����̈ړ����x </summary>
    [Header("�v���C���[�ɋ߂Â�����̈ړ����x")]
    [SerializeField] private float moveSpeed;

    // --------------------------
    // �U��
    // --------------------------

    /// <summary> 
    /// �U���̃g���K�[�ƂȂ�v���C���[�Ƃ̋���
    /// </summary>
    [Header("�U���̃g���K�[�ƂȂ�v���C���[�Ƃ̋���")]
    [SerializeField] private float triggerDistance;

    /// <summary> �U���ҋ@���� </summary>
    [Header("�U���ҋ@����")]
    [SerializeField] private float attackInterval;

    /// <summary> �U������m��(%) </summary>
    [Header("�U������m��(%)")]
    [SerializeField] private float attackPercent;

    /// <summary> �U���\�Ȃ�true </summary>
    private bool canAttack = false;

    /// <summary> �U���̊m�����������Ă���true </summary>
    [SerializeField] private bool isAttackPercent = false;

    // ------------------------------
    // �K�E�Z
    // ------------------------------

    /// <summary> �K�E�Z�̃C���^�[�o������ </summary>
    [Header("�K�E�Z�̃C���^�[�o������")]
    [SerializeField] private float ultInterval;

    /// <summary> �K�E�Z���g�p�\�Ȃ�true </summary>
    private bool canUlt = false;

    // ------------------------------
    // ���
    // ------------------------------

    /// <summary> ����C���^�[�o������ </summary>
    [Header("����C���^�[�o������")]
    [SerializeField] private float slidingInterval;

    /// <summary> ����\�Ȃ�true </summary> 
    private bool canSliding = false;

    // ------------------------------
    // �h��
    // ------------------------------

    /// <summary> �h��C���^�[�o������ </summary>
    [Header("�h��C���^�[�o��")]
    [SerializeField] private float guardInterval;

    /// <summary> �h��\�Ȃ�true </summary>
    private bool canGuard = false;

    // ---------------------------
    // ��_���[�W
    // ---------------------------

    // ----------------------------
    // �U�����m�p
    // ----------------------------

    /// <summary> �U�������m�ł��鎞�� </summary>
    [Header("�U�������m�ł��鎞��")]
    [SerializeField] private float attackReceptionTime;

    /// <summary> �K�E�Z�����m�ł��鎞�� </summary>
    [Header("�K�E�Z�����m�ł��鎞��")]
    [SerializeField] private float ultReceptionTime;

    // --------------------
    // ����p
    // --------------------

    /// <summary> ���݂̏�� </summary>
    private State currentState;

    /// <summary> �GAI����p </summary>
    private NavMeshAgent enemyAi;

    // --------------------
    // ����
    // --------------------

    /// <summary> �����p </summary>
    private Rigidbody rd;

    // --------------------------------
    // �A�j���[�V�������m�p
    // --------------------------------

    /// <summary> �v���C���[�̕K�E�Z�A�j���[�V���� </summary>
    [Header("�K�E�Z�̃A�j���[�V����(�v���C���[)")]
    [SerializeField] private AnimationClip playerUltAnim;

    /// <summary> �v���C���[�̍U��1�A�j���[�V���� </summary>
    [Header("�v���C���[�̍U��1�A�j���[�V����")]
    [SerializeField] private AnimationClip playerAttack1Anim;

    /// <summary> �v���C���[�̍U��2�A�j���[�V���� </summary>
    [Header("�v���C���[�̍U��2�A�j���[�V����")]
    [SerializeField] private AnimationClip playerAttack2Anim;

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

        // �v���C���[��
        // �A�j���[�V��������p�R���|�[�l���g���擾
        playerAnimator = player.GetComponent<Animator>();

        // �����p�R���|�[�l���g���擾
        rd = gameObject.GetComponent<Rigidbody>();

        // �ҋ@��Ԃ���n�߂�
        ChangeState(State.Idle);
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
        // ��Ԃɍ��킹�ď���
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

                // �^�C�}�[
                TimerCount();

                // �s���I��
                SelectAction();

                break;

            // ------------------------
            // �U��
            // ------------------------
            case State.Attack:

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
            // ���S
            // ------------------------
            case State.Dead:

                break;
        }

        // �A�j���[�V��������p�ɑ��x��n��
        enemyAnimator.SetFloat("Speed", rd.velocity.magnitude);
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

        // �v���C���[�Ƃ̋��������ȓ�
        if (IsNear(player))
        {
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
        // ------------------------------
        // �_���[�W���[�V��������
        // ------------------------------

        // -----------------------------
        // �U���E�K�E�Z���\��
        // -----------------------------

        // �K�E�Z���g�p�\
        if (canUlt)
        {
            ChangeState(State.Ult);
            return;
        }
        // �K�E�Z���g�p�s�ōU���\
        else if (canAttack)
        {
            // �U����ԂɈڍs
            ChangeState(State.Attack);
            return;
        }

        // -------------------------------------
        // �v���C���[�̕K�E�Z�����m������ 
        // -------------------------------------

        // �v���C���[�̕K�E�Z�����m��
        // �h��\�Ȃ�h�䂷��
        if (CheckPlayerUlt() && canGuard)
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
        if (CheckPlayerAttack() && canSliding)
        {
            ChangeState(State.Sliding);
            return;
        }
        // ���s�\��
        // �h��\�Ȃ�h�䂷��
        else if (CheckPlayerAttack() && canGuard)
        {
            ChangeState(State.Guard);
            return;
        }

        // ----------------------------------
        // �U���̊m�����������Ă���
        // ----------------------------------

        // �U���̊m�����������Ă���U��
        if (isAttackPercent)
        {
            ChangeState(State.Attack);
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
        float rotateSpeed = moveSpeed * 10.0f;

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

    #region �U���C�x���g

    private void AnimAttackOk()
    {
        return;
    }

    /// <summary>
    /// �U��1�̊J�n�C�x���g
    /// </summary>
    private void AnimAttack1()
    {
        LookTarget(player);
    }

    /// <summary>
    /// �U��1�̏I���C�x���g
    /// </summary>
    private void AnimAttack1Faild()
    {
        // �v���C���[���܂��߂��ɂ���ꍇ
        if (IsNear(player))
        {
            return;
        }

        // �U���I��
        enemyAnimator.SetBool(isAttack, false);

        // �^�C�}�[���Z�b�g
        attackTimer = 0;

        // ���̌�ōU�����邩���I����
        isAttackPercent = CheckProbability(attackPercent);

        // �ҋ@��ԂɈڍs
        ChangeState(State.Idle);
    }

    /// <summary>
    /// �U��2�̊J�n�C�x���g
    /// </summary>
    private void AnimAttack2()
    {
        LookTarget(player);
    }

    /// <summary>
    /// �U��2�̏I���C�x���g
    /// </summary>
    private void AnimAttack2Faild()
    {
        // �U���I��
        enemyAnimator.SetBool(isAttack, false);

        // �^�C�}�[���Z�b�g
        attackTimer = 0;

        // ���̌�ōU�����邩���I����
        isAttackPercent = CheckProbability(attackPercent);

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
        // �v���C���[�̕�������
        LookTarget(player);
    }

    /// <summary>
    /// �K�E�Z�̏I���C�x���g
    /// </summary>
    private void AnimUltFailed()
    {
        // �^�C�}�[���Z�b�g
        ultTimer = 0;

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
    /// �h��̏I���C�x���g
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

    }

    #endregion

    #region ��_���[�W

    ///// <summary>
    ///// �_���[�W���󂯂�
    ///// </summary>
    ///// <param name="damage"> �_���[�W�� </param>
    //public void ReceiveDamage(float damage)
    //{
    //    // �h�䒆�Ȃ�true
    //    bool isGuard = currentState == State.Guard;

    //    // �ړ����x��0�ɐݒ�

    //    // �h�䒆
    //    if (isGuard)
    //    {

    //    }
    //    else
    //    {
    //        // �^����ꂽ�_���[�W��HP�����炷
    //        hp -= damage;

    //    }

    //    // HP�������Ȃ����玀�S
    //    if (hp <= 0)
    //    {
    //        ChangeState(State.Dead);
    //        return;
    //    }

    //    // HP���c���Ă���̂�
    //    // �h�䂵�ĂȂ��Ȃ�_���[�W���[�V�����J�n
    //    if (!isGuard)
    //    {
    //        enemyAnimator.SetTrigger(hitTrigger);
    //    }
    //}

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
        if (!enemyAnimator || !playerAnimator)
        {
            Debug.Log("Animator������܂���");
            return false;
        }

        // �����p�R���|�[�l���g���Ȃ�
        if (!rd)
        {
            Debug.Log("Rigidbody������܂���");
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
        return distance < triggerDistance;
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
        // �K�E�Z�̃A�j���[�V�������ݒ肳��Ă��Ȃ�
        if (!playerUltAnim)
        {
            Debug.Log("�v���C���[�̕K�E�Z���ݒ肳��Ă��܂���");
            return false;
        }

        // �v���C���[���K�E�Z�����Ă��Ȃ�
        if (!IsPlayingAnim(playerAnimator, playerUltAnim))
        {
            return false;
        }

        // �v���C���[�̕K�E�Z�̔���J�n����
        float ultStartTime = playerUltAnim.events[0].time;

        // �A�j���[�V�����̌o�ߎ���
        float elapsedTime = GetAnimElapsedTime(playerAnimator);

        // �v���C���[�̕K�E�Z�̃A�j���[�V������
        // ������x�Đ����ꂽ�猟�m
        return elapsedTime >= ultStartTime &&
            elapsedTime < ultStartTime + ultReceptionTime;
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
        // �v���C���[�̍U�����ݒ肳��Ă��Ȃ�
        if (!CheckPlayerAttakAnim())
        {
            return false;
        }

        // �v���C���[���U�����Ă��Ȃ�
        if (!IsPlayingAnim(playerAnimator, playerAttack1Anim) &&
            !IsPlayingAnim(playerAnimator, playerAttack2Anim))
        {
            return false;
        }

        // �U���̌o�ߎ���
        float elapsedTime = GetAnimElapsedTime(playerAnimator);

        // �U�����m�\�Ȏ��Ԃ��o�߂��Ă���̂�
        // �U�������m�ł��Ȃ�
        if (elapsedTime > attackReceptionTime)
        {
            return false;
        }

        return true;
    }

    #endregion

    #region �A�j���[�V����

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
        return playerUltAnim.length * currentTimeRatio;
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
        if (!playerAttack1Anim)
        {
            Debug.Log("�v���C���[�̍U��1���ݒ肳��Ă��܂���");
            return false;
        }

        // �v���C���[�̍U��2���ݒ肳��Ă��Ȃ�
        if (!playerAttack1Anim)
        {
            Debug.Log("�v���C���[�̍U��2���ݒ肳��Ă��܂���");
            return false;
        }

        return true;
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

                // �ҋ@��ԂɈڍs
                currentState = State.Idle;

                break;

            // ----------------------
            // �ǔ���ԂɈڍs
            // ----------------------
            case State.Chase:

                // �ǔ����x��ݒ�
                enemyAi.speed = chaseSpeed;

                // �ǔ���ԂɈڍs
                currentState = State.Chase;

                break;

            // --------------------------
            // �s���I����ԂɈڍs
            // --------------------------

            case State.SelectAction:

                // �s���I�𒆂�NavMesh�œ������Ȃ��̂ŁA
                // speed��0�ɐݒ�
                //enemyAi.speed = 0;

                // �s���I����ԂɈڍs
                currentState = State.SelectAction;

                break;

            // -----------------------
            // �U����ԂɈڍs
            // -----------------------
            case State.Attack:

                // �U���A�j���[�V�����J�n
                enemyAnimator.SetBool(isAttack, true);

                // �U����ԂɈڍs
                currentState = State.Attack;

                break;

            case State.Ult:

                // �K�E�Z�A�j���[�V�����J�n
                enemyAnimator.SetTrigger(ultTirgger);

                // �K�E�Z��ԂɈڍs
                currentState = State.Ult;

                break;

            // -----------------------
            // �����ԂɈڍs
            // -----------------------
            case State.Sliding:

                // ����A�j���[�V�����J�n
                enemyAnimator.SetTrigger(slidingTrigger);

                // �����ԂɈڍs
                currentState = State.Sliding;

                break;

            // ------------------------
            // �h���ԂɈڍs
            // ------------------------
            case State.Guard:

                // �h��A�j���[�V�����J�n
                enemyAnimator.SetTrigger(guardTrigger);

                // �h���ԂɈڍs
                currentState = State.Guard;

                break;

            // ------------------------
            // ���S��ԂɈڍs
            // ------------------------
            case State.Dead:

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

        // �U���\�ɂȂ��ĂȂ��Ȃ�i�߂�
        if (!canAttack) attackTimer += Time.deltaTime;

        // �K�E�Z���g�p�\�ɂȂ��ĂȂ��Ȃ�i�߂�
        if (!canUlt) ultTimer += Time.deltaTime;

        // ����\�ɂȂ��ĂȂ��Ȃ�i�߂�
        if (!canSliding) slidingTimer += Time.deltaTime;

        // �h��\�ɂȂ��ĂȂ��Ȃ�i�߂�
        if (!canGuard) guardTimer += Time.deltaTime;

        // ----------------------------------
        // ��莞�Ԍo�������m�F
        // ----------------------------------

        canAttack = attackTimer >= attackInterval;
        canUlt = ultTimer >= ultInterval;
        canSliding = slidingTimer >= slidingInterval;
        canGuard = guardTimer >= guardInterval;
    }
}