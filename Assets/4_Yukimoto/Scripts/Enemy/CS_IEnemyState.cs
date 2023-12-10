using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

/// <summary>
/// �G�̏�Ԃ��Ǘ�������N���X
/// </summary>
public abstract class CS_IEnemyState : MonoBehaviour
{
    /// <summary>
    /// ��Ԃ̏�������
    /// </summary>
    public abstract void StartState();

    /// <summary>
    /// ��Ԃ̏��������s����
    /// </summary>
    /// <returns> ���̏�Ԃ̊Ǘ��N���X </returns>
    public abstract CS_IEnemyState RunState();
}
