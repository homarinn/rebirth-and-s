using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// �ҋ@��Ԃ̊Ǘ��N���X
/// </summary>
public class CS_EnemyIdleState : CS_IEnemyState
{
    /// <summary> �U����Ԃ� </summary> 
    private CS_EnemyAttackState attackState;

    private void Start()
    {
        // �U����Ԃ̊Ǘ��N���X���擾
        attackState = gameObject.GetComponent<CS_EnemyAttackState>();
    }

    /// <summary>
    /// �ҋ@��Ԃ̏�������
    /// </summary>
    public override void StartState()
    {
        return;
    }

    /// <summary>
    /// �ҋ@��Ԃ̏��������s����
    /// </summary>
    /// <returns> ���̏�Ԃ̊Ǘ��N���X </returns>
    public override CS_IEnemyState RunState()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // �U����ԂɑJ��
            return attackState;
        }

        // �ҋ@��Ԃ��ێ�
        return this;
    }
}
