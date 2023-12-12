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
/// 敵の管理クラス
/// </summary>
public class CS_Enemy : MonoBehaviour
{
    /// <summary> 現在の状態 </summary>
    private CS_IEnemyState currentState;

    /// <summary> 次の状態 </summary>
    private CS_IEnemyState nextState;

    // Start is called before the first frame update
    void Start()
    {
        // 待機状態
        currentState = gameObject.GetComponent<CS_EnemyMoveState>();
    }

    // Update is called once per frame
    void Update()
    {
        // 状態の処理を実行し、
        // 次の状態を取得する
        nextState = currentState.RunState();

        // 次の状態が指定されてない場合
        if (!nextState)
        {
            // 現在の状態を維持する
            nextState = currentState;
        }

        // 現在と違う状態が指定された場合
        if (nextState != currentState)
        {
            // 次の状態へ移行する
            currentState = nextState;

            // 初期処理
            currentState.StartState();
        }
    }
}
