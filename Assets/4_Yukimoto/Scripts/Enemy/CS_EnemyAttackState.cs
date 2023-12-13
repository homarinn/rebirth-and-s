using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 攻撃状態の管理クラス
/// </summary>
public class CS_EnemyAttackState : CS_IEnemyState
{
    /// <summary> 待機状態の管理クラス </summary>
    private CS_EnemyChaseState chaseState;

    private void Awake()
    {
        // 待機状態の管理クラスを取得
        chaseState = gameObject.GetComponent<CS_EnemyChaseState>();
    }

    /// <summary>
    /// 攻撃状態の初期処理
    /// </summary>
    public override void StartState()
    {
        Debug.Log("攻撃");
    }

    /// <summary>
    /// 攻撃状態の処理を実行する
    /// </summary>
    /// <returns> 次の状態の管理クラス </returns>
    public override CS_IEnemyState RunState(Transform player)
    {
        // 追尾状態に移行
        return chaseState;
    }
}
