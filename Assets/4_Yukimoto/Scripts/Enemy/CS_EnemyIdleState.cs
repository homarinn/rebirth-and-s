using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// �ҋ@��Ԃ̊Ǘ��N���X(�ʏ�͌Ă΂�Ȃ�)
/// </summary>
public class CS_EnemyIdleState : CS_IEnemyState
{
    /// <summary> �GAI����p </summary>
    private NavMeshAgent enemyAI;

    /// <summary>
    /// �ҋ@��Ԃ̏�������
    /// </summary>
    public override void StartState()
    {
        // �G���~������
        enemyAI = gameObject.GetComponent<NavMeshAgent>();
        enemyAI.speed = 0.0f;

        Debug.Log("�ُ킪���������̂ŁA�ҋ@��ԂɈڍs���܂�");
    }

    /// <summary>
    /// �ҋ@��Ԃ̏��������s����
    /// </summary>
    /// <returns> ���̏�Ԃ̊Ǘ��N���X </returns>
    public override CS_IEnemyState RunState(Transform player)
    {
        // �ҋ@��Ԃ��ێ�
        return this;
    }
}
