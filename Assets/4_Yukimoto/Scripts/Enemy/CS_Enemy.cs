using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEditorInternal;
using UnityEngine;

/// <summary>
/// �G�̊Ǘ��N���X
/// </summary>
public class CS_Enemy : MonoBehaviour
{
    /// <summary> ���݂̏�� </summary>
    private CS_IEnemyState currentState;

    /// <summary> ���̏�� </summary>
    private CS_IEnemyState nextState;

    // Start is called before the first frame update
    void Start()
    {
        // �ҋ@���
        currentState = gameObject.GetComponent<CS_EnemyMoveState>();
    }

    // Update is called once per frame
    void Update()
    {
        // ��Ԃ̏��������s���A
        // ���̏�Ԃ��擾����
        nextState = currentState.RunState();

        // ���̏�Ԃ��w�肳��ĂȂ��ꍇ
        if (!nextState)
        {
            // ���݂̏�Ԃ��ێ�����
            nextState = currentState;
        }

        // ���݂ƈႤ��Ԃ��w�肳�ꂽ�ꍇ
        if (nextState != currentState)
        {
            // ���̏�Ԃֈڍs����
            currentState = nextState;

            // ��������
            currentState.StartState();
        }
    }
}
