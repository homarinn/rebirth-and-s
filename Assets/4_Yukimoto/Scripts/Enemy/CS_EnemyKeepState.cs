using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 様子見状態の管理クラス
/// </summary>
public class CS_EnemyKeepState : CS_IEnemyState
{
    /// <summary> 移動速度 </summary>
    [Header("移動速度")]
    [SerializeField] private float moveSpeed;

    [Header("攻撃")]

    /// <summary> 敵AI操作用 </summary>
    private NavMeshAgent enemyAi;

    /// <summary> 追尾状態の管理クラス </summary> 
    private CS_EnemyChaseState chaseState;

    /// <summary> 攻撃状態の管理クラス </summary> 
    private CS_EnemyAttackState attackState;

    private void Awake()
    {
        enemyAi = gameObject.GetComponent<NavMeshAgent>();

        // 追尾状態の管理クラスを取得
        chaseState = gameObject.GetComponent<CS_EnemyChaseState>();

        // 攻撃状態の管理クラスを取得
        attackState = gameObject.GetComponent<CS_EnemyAttackState>();
    }

    /// <summary>
    /// 様子見状態の初期処理
    /// </summary>
    public override void StartState()
    {
        Debug.Log("様子見状態");
        enemyAi.speed = 0;
    }

    /// <summary>
    /// 待機状態の処理を実行する
    /// </summary>
    /// <returns> 次の状態の管理クラス </returns>
    public override CS_IEnemyState RunState(Transform player)
    {
        // プレイヤーを中心に回転移動
        transform.RotateAround(player.position, Vector3.up,
            moveSpeed * Time.deltaTime);

        // プレイヤーとの距離を確認
        if (!chaseState.CheckDistance(player))
        {
            // プレイヤーから離れているので
            // 追尾状態に移行
            return chaseState;
        }

        // 待機状態を維持
        return this;
    }
}
