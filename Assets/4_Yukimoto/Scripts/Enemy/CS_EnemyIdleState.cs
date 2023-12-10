using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 待機状態の管理クラス
/// </summary>
public class CS_EnemyIdleState : CS_IEnemyState
{
    /// <summary> 攻撃状態の </summary> 
    private CS_EnemyAttackState attackState;

    private void Start()
    {
        // 攻撃状態の管理クラスを取得
        attackState = gameObject.GetComponent<CS_EnemyAttackState>();
    }

    /// <summary>
    /// 待機状態の初期処理
    /// </summary>
    public override void StartState()
    {
        return;
    }

    /// <summary>
    /// 待機状態の処理を実行する
    /// </summary>
    /// <returns> 次の状態の管理クラス </returns>
    public override CS_IEnemyState RunState()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // 攻撃状態に遷移
            return attackState;
        }

        // 待機状態を維持
        return this;
    }
}
