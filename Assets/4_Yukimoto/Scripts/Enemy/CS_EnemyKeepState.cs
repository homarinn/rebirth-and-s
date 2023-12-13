using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// �l�q����Ԃ̊Ǘ��N���X
/// </summary>
public class CS_EnemyKeepState : CS_IEnemyState
{
    /// <summary> �ړ����x </summary>
    [Header("�ړ����x")]
    [SerializeField] private float moveSpeed;

    [Header("�U��")]

    /// <summary> �GAI����p </summary>
    private NavMeshAgent enemyAi;

    /// <summary> �ǔ���Ԃ̊Ǘ��N���X </summary> 
    private CS_EnemyChaseState chaseState;

    /// <summary> �U����Ԃ̊Ǘ��N���X </summary> 
    private CS_EnemyAttackState attackState;

    private void Awake()
    {
        enemyAi = gameObject.GetComponent<NavMeshAgent>();

        // �ǔ���Ԃ̊Ǘ��N���X���擾
        chaseState = gameObject.GetComponent<CS_EnemyChaseState>();

        // �U����Ԃ̊Ǘ��N���X���擾
        attackState = gameObject.GetComponent<CS_EnemyAttackState>();
    }

    /// <summary>
    /// �l�q����Ԃ̏�������
    /// </summary>
    public override void StartState()
    {
        Debug.Log("�l�q�����");
        enemyAi.speed = 0;
    }

    /// <summary>
    /// �ҋ@��Ԃ̏��������s����
    /// </summary>
    /// <returns> ���̏�Ԃ̊Ǘ��N���X </returns>
    public override CS_IEnemyState RunState(Transform player)
    {
        // �v���C���[�𒆐S�ɉ�]�ړ�
        transform.RotateAround(player.position, Vector3.up,
            moveSpeed * Time.deltaTime);

        // �v���C���[�Ƃ̋������m�F
        if (!chaseState.CheckDistance(player))
        {
            // �v���C���[���痣��Ă���̂�
            // �ǔ���ԂɈڍs
            return chaseState;
        }

        // �ҋ@��Ԃ��ێ�
        return this;
    }
}
