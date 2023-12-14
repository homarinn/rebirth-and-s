using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// �G�̊Ǘ��N���X
/// </summary>
public class CS_Enemy : MonoBehaviour
{
    /// <summary>
    /// ���
    /// </summary>
    enum State
    {
        Chase,  // �ǔ�
        Keep,   // �l�q��
        Attack, // �U��
        Idle,   // �ҋ@
    }

    /// <summary> �v���C���[ </summary>
    [Header("�v���C���[�擾")]
    [SerializeField] private Transform player;

    /// <summary> 
    /// �U���̃g���K�[�ƂȂ�v���C���[�Ƃ̋���
    /// </summary>
    [Header("�U���̃g���K�[�ƂȂ�v���C���[�Ƃ̋���")]
    [SerializeField] private float triggerDistance;

    /// <summary> �ǔ����x </summary>
    [Header("�ǔ����x")]
    [SerializeField] private float chaseSpeed;

    /// <summary> �v���C���[�ɋ߂Â�����̈ړ����x </summary>
    [Header("�v���C���[�ɋ߂Â�����̈ړ����x")]
    [SerializeField] private float moveSpeed;

    /// <summary> �U������m��(%) </summary>
    [Header("�U������m��(%)")]
    [SerializeField] private float attackProbability;

    /// <summary> ���݂̓G�̏�� </summary>
    private State currentState;

    /// <summary> �GAI����p </summary>
    private NavMeshAgent enemyAi;

    /// <summary> �A�j���[�V��������p </summary>
    private Animator anim;

    /// <summary> �����p </summary>
    private Rigidbody rd;

    private bool canAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        // �GAI����p�R���|�[�l���g���擾
        enemyAi = gameObject.GetComponent<NavMeshAgent>();

        // �A�j���[�V��������p�R���|�[�l���g���擾
        anim = gameObject.GetComponent<Animator>();

        // �����p�R���|�[�l���g���擾
        rd = gameObject.GetComponent<Rigidbody>();

        // �ǔ���Ԃɐݒ�
        currentState = State.Chase;

        enemyAi.speed = chaseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // �v���C���[���ݒ肳��Ă��Ȃ��̂ŁA
        // ���������s���Ȃ��悤�ɂ���
        if (!player)
        {
            Debug.Log("�v���C���[���ݒ肳��Ă��܂���");
            return;
        }

        // �R���|�[�l���g�m�F
        if (!CheckComponent())
        {
            return;
        }

        // �A�j���[�V��������p�̑��x��n��
        anim.SetFloat("Speed", rd.velocity.magnitude);

        switch (currentState)
        {
            // ---------------------
            // �ǔ����
            // ---------------------
            case State.Chase:

                Chase();

                break;

            // -------------------
            // �l�q�����
            // -------------------
            case State.Keep:

                Keep();

                break;

            // --------------------
            // �U�����
            // --------------------
            case State.Attack:

                Attack();

                break;
        }
    }

    #region ��Ԃ̏���

    /// <summary>
    /// �ǔ���Ԃ̏���
    /// </summary>
    private void Chase()
    {
        // �v���C���[�Ɍ������Ĉړ�
        enemyAi.SetDestination(player.position);

        // �v���C���[�Ƃ̋������m�F
        if (CheckDistance(player))
        {
            // �l�q����Ԃł�NavMesh�œ������Ȃ��̂ŁA
            // speed��0�ɐݒ�
            enemyAi.speed = 0;

            // �v���C���[�ɋ߂Â����̂�
            // �l�q����ԂɈڍs
            currentState = State.Keep;
        }
    }

    /// <summary>
    /// �l�q����Ԃ̏���
    /// </summary>
    private void Keep()
    {
        // ��]�ړ��̑��x
        float rotateSpeed = moveSpeed * 10.0f;

        // �v���C���[�𒆐S�ɉ�]�ړ�
        transform.RotateAround(player.position, Vector3.up,
            rotateSpeed * Time.deltaTime);

        // �v���C���[�Ƃ̋������m�F
        if (!CheckDistance(player))
        {
            // �ǔ���Ԃł�NavMesh�œ������̂�
            // speed��ǔ����x�ɐݒ�
            enemyAi.speed = chaseSpeed;

            // �v���C���[���痣��Ă���̂�
            // �ǔ���ԂɈڍs
            currentState = State.Chase;
        }
    }

    /// <summary>
    /// �U����Ԃ̏���
    /// </summary>
    private void Attack()
    {
        anim.SetTrigger("AttackTrigger");

        // �l�q����ԂɈڍs
        currentState = State.Keep;

        canAttack = false;
    }

    #endregion

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
        if (!anim)
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
    /// �^�[�Q�b�g�Ƃ̋����𒲂ׂ�
    /// </summary>
    /// <param name="target"> �Ώ� </param>
    /// <returns>
    /// <para> true : �^�[�Q�b�g�Ƃ̋��������ȓ� </para>
    /// <para> false : �^�[�Q�b�g�Ƃ̋��������ȏ� </para>
    /// </returns>
    private bool CheckDistance(Transform target)
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
    /// <param name="probability"> true�ɂȂ�m��(%) </param>
    /// <returns>
    /// �w�肵���m����true�ɂȂ�
    /// </returns>
    private bool CheckProbability(float probability)
    {
        // ����(0 �` 100.0)
        float randomValue = UnityEngine.Random.value * 100.0f;

        // �m�����l���������ꍇ�́A
        // �m�����������Ɣ��肷��
        return randomValue <= probability;
    }
}
