using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 待機状態の管理クラス(通常は呼ばれない)
/// </summary>
public class CS_EnemyIdleState : CS_IEnemyState
{
    /// <summary> 敵AI操作用 </summary>
    private NavMeshAgent enemyAI;

    /// <summary>
    /// 待機状態の初期処理
    /// </summary>
    public override void StartState()
    {
        // 敵を停止させる
        enemyAI = gameObject.GetComponent<NavMeshAgent>();
        enemyAI.speed = 0.0f;

        Debug.Log("異常が発生したので、待機状態に移行します");
    }

    /// <summary>
    /// 待機状態の処理を実行する
    /// </summary>
    /// <returns> 次の状態の管理クラス </returns>
    public override CS_IEnemyState RunState(Transform player)
    {
        // 待機状態を維持
        return this;
    }
}
