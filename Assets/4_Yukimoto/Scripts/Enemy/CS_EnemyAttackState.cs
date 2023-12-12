using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// 攻撃状態の管理クラス
/// </summary>
public class CS_EnemyAttackState : CS_IEnemyState
{
    /// <summary> 待機状態の管理クラス </summary>
    private CS_EnemyIdleState idleState;

    private void Start()
    {
        // 待機状態の管理クラスを取得
        idleState = gameObject.GetComponent<CS_EnemyIdleState>();
    }

    /// <summary>
    /// 攻撃状態の初期処理
    /// </summary>
    public override void StartState()
    {
        return;
    }

    /// <summary>
    /// 攻撃状態の処理を実行する
    /// </summary>
    /// <returns> 次の状態の管理クラス </returns>
    public override CS_IEnemyState RunState()
    {
        Debug.Log("攻撃");

        if (Input.GetKeyDown(KeyCode.P))
        {
            // 待機状態に移行
            return idleState;
        }

        // 攻撃状態を維持
        return this;
    }
}
