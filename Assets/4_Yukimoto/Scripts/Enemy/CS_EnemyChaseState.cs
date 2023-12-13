using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 追尾状態の管理クラス
/// </summary>
public class CS_EnemyChaseState : CS_IEnemyState
{
    /// <summary> 
    /// 攻撃のトリガーとなるプレイヤーとの距離
    /// </summary>
    [Header("攻撃のトリガーとなるプレイヤーとの距離")]
    [SerializeField] private float triggerDistance;

    /// <summary> 追尾速度 </summary>
    [Header("追尾速度")]
    [SerializeField] private float chaseSpeed;

    [Header("攻撃する確率(%)")]
    [SerializeField] private float probability;

    /// <summary> 敵AI操作用 </summary>
    private NavMeshAgent enemyAi;

    /// <summary> 様子見状態の管理クラス </summary>
    private CS_EnemyKeepState keepState;

    private void Awake()
    {
        // 敵AI操作用コンポーネントを取得
        enemyAi = gameObject.GetComponent<NavMeshAgent>();

        // 様子見状態の管理クラスを取得
        keepState = gameObject.GetComponent<CS_EnemyKeepState>();
    }

    /// <summary>
    /// 追尾状態の初期処理
    /// </summary>
    public override void StartState()
    {
        Debug.Log("追尾状態");

        // 追尾速度を設定
        enemyAi.speed = chaseSpeed;
    }

    /// <summary>
    /// 追尾状態の処理を実行する
    /// </summary>
    /// <returns> 次の状態の管理クラス </returns>
    public override CS_IEnemyState RunState(Transform player)
    {
        // 敵AIの操作ができない場合
        if (!enemyAi)
        {
            Debug.Log("NavMeshAgentがありません");

            // nullを返し待機状態にする
            return null;
        }

        // プレイヤーに向かって移動
        enemyAi.SetDestination(player.position);

        // プレイヤーとの距離を確認
        if (CheckDistance(player))
        {
            // プレイヤーに近づいたので
            // 様子見状態に移行
            return keepState;
        }

        // プレイヤーから離れているので
        // 追尾状態を維持
        return this;
    }

    /// <summary>
    /// ターゲットとの距離を調べる
    /// </summary>
    /// <param name="target"> 対象 </param>
    /// <returns>
    /// <para> true : ターゲットとの距離が一定以内 </para>
    /// <para> false : ターゲットとの距離が一定以上 </para>
    /// </returns>
    public bool CheckDistance(Transform target)
    {
        // ターゲットとの距離
        float distance = Vector3.Distance(
            target.position, transform.position);

        // 一定以内にターゲットがいるのか
        return distance < triggerDistance;
    }
}
