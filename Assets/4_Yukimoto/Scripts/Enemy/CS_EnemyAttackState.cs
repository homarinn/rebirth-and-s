using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// �U����Ԃ̊Ǘ��N���X
/// </summary>
public class CS_EnemyAttackState : CS_IEnemyState
{
    /// <summary> �ҋ@��Ԃ̊Ǘ��N���X </summary>
    private CS_EnemyIdleState idleState;

    private void Start()
    {
        // �ҋ@��Ԃ̊Ǘ��N���X���擾
        idleState = gameObject.GetComponent<CS_EnemyIdleState>();
    }

    /// <summary>
    /// �U����Ԃ̏�������
    /// </summary>
    public override void StartState()
    {
        return;
    }

    /// <summary>
    /// �U����Ԃ̏��������s����
    /// </summary>
    /// <returns> ���̏�Ԃ̊Ǘ��N���X </returns>
    public override CS_IEnemyState RunState()
    {
        Debug.Log("�U��");

        if (Input.GetKeyDown(KeyCode.P))
        {
            // �ҋ@��ԂɈڍs
            return idleState;
        }

        // �U����Ԃ��ێ�
        return this;
    }
}
