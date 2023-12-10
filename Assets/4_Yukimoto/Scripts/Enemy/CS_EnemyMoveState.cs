using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 移動状態の管理クラス
/// </summary>
public class CS_EnemyMoveState : CS_IEnemyState
{
    /// <summary> プレイヤー </summary>
    [SerializeField] private Transform player;

    /// <summary> 
    /// 攻撃のトリガーとなるプレイヤーとの距離
    /// </summary>
    [SerializeField] private float triggerDistance;

    /// <summary> 移動速度 </summary>
    [SerializeField] private float moveSpeed;

    /// <summary> 敵AI操作用 </summary>
    private NavMeshAgent enemyAI;

    /// <summary> 待機状態 </summary>
    private CS_EnemyIdleState idleState;

    /// <summary> 攻撃状態 </summary>
    private CS_EnemyAttackState attackState;

    /// <summary> プレイヤーとの距離 </summary>
    private float distance = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        // ナビメッシュ取得
        enemyAI = gameObject.GetComponent<NavMeshAgent>();

        // 待機状態の管理クラスを取得
        idleState = gameObject.GetComponent<CS_EnemyIdleState>();

        // 攻撃状態の管理クラスを取得
        attackState = gameObject.GetComponent<CS_EnemyAttackState>();

        // 初期処理
        StartState();
    }

    /// <summary>
    /// 移動状態の初期処理
    /// </summary>
    public override void StartState()
    {
        // 敵の移動速度を設定
        enemyAI.speed = moveSpeed;
    }

    /// <summary>
    /// 移動状態の処理を実行する
    /// </summary>
    /// <returns></returns>
    public override CS_IEnemyState RunState()
    {
        // プレイヤーがいない場合
        if (!player)
        {
            Debug.Log("プレイヤーが存在しません");

            // 待機状態に移行
            return idleState;
        }

        // プレイヤー距離を更新
        distance = GetDistance();

        // プレイヤーに向かって移動
        enemyAI.SetDestination(player.position);

        // 一定距離プレイヤーに近づいた場合
        if (distance < triggerDistance)
        {
            // 攻撃状態に移行
            return attackState;
        }
        // 一定距離プレイヤーから離れている場合
        else
        {
            // 移動状態を維持
            return this;
        }
    }

    /// <summary>
    /// プレイヤーとの距離を取得する
    /// </summary>
    /// <returns></returns>
    private float GetDistance()
    {
        return Vector3.Distance(player.position, transform.position);
    }
}
