using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// �ړ���Ԃ̊Ǘ��N���X
/// </summary>
public class CS_EnemyMoveState : CS_IEnemyState
{
    /// <summary> �v���C���[ </summary>
    [SerializeField] private Transform player;

    /// <summary> 
    /// �U���̃g���K�[�ƂȂ�v���C���[�Ƃ̋���
    /// </summary>
    [SerializeField] private float triggerDistance;

    /// <summary> �ړ����x </summary>
    [SerializeField] private float moveSpeed;

    /// <summary> �GAI����p </summary>
    private NavMeshAgent enemyAI;

    /// <summary> �ҋ@��� </summary>
    private CS_EnemyIdleState idleState;

    /// <summary> �U����� </summary>
    private CS_EnemyAttackState attackState;

    /// <summary> �v���C���[�Ƃ̋��� </summary>
    private float distance = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // �i�r���b�V���擾
        enemyAI = gameObject.GetComponent<NavMeshAgent>();

        // �ҋ@��Ԃ̊Ǘ��N���X���擾
        idleState = gameObject.GetComponent<CS_EnemyIdleState>();

        // �U����Ԃ̊Ǘ��N���X���擾
        attackState = gameObject.GetComponent<CS_EnemyAttackState>();

        // ��������
        StartState();
    }

    /// <summary>
    /// �ړ���Ԃ̏�������
    /// </summary>
    public override void StartState()
    {
        // �G�̈ړ����x��ݒ�
        enemyAI.speed = moveSpeed;
    }

    /// <summary>
    /// �ړ���Ԃ̏��������s����
    /// </summary>
    /// <returns></returns>
    public override CS_IEnemyState RunState()
    {
        // �v���C���[�����Ȃ��ꍇ
        if (!player)
        {
            Debug.Log("�v���C���[�����݂��܂���");

            // �ҋ@��ԂɈڍs
            return idleState;
        }

        // �v���C���[�������X�V
        distance = GetDistance();

        // �v���C���[�Ɍ������Ĉړ�
        enemyAI.SetDestination(player.position);

        // ��苗���v���C���[�ɋ߂Â����ꍇ
        if (distance < triggerDistance)
        {
            // �U����ԂɈڍs
            return attackState;
        }
        // ��苗���v���C���[���痣��Ă���ꍇ
        else
        {
            // �ړ���Ԃ��ێ�
            return this;
        }
    }

    /// <summary>
    /// �v���C���[�Ƃ̋������擾����
    /// </summary>
    /// <returns></returns>
    private float GetDistance()
    {
        return Vector3.Distance(player.position, transform.position);
    }
}
