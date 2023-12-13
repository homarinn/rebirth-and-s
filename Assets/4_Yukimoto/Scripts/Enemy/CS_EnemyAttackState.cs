using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �U����Ԃ̊Ǘ��N���X
/// </summary>
public class CS_EnemyAttackState : CS_IEnemyState
{
    /// <summary> �ҋ@��Ԃ̊Ǘ��N���X </summary>
    private CS_EnemyChaseState chaseState;

    private void Awake()
    {
        // �ҋ@��Ԃ̊Ǘ��N���X���擾
        chaseState = gameObject.GetComponent<CS_EnemyChaseState>();
    }

    /// <summary>
    /// �U����Ԃ̏�������
    /// </summary>
    public override void StartState()
    {
        Debug.Log("�U��");
    }

    /// <summary>
    /// �U����Ԃ̏��������s����
    /// </summary>
    /// <returns> ���̏�Ԃ̊Ǘ��N���X </returns>
    public override CS_IEnemyState RunState(Transform player)
    {
        // �ǔ���ԂɈڍs
        return chaseState;
    }
}
