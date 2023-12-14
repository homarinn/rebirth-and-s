using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敵の管理クラス
/// </summary>
public class CS_Enemy : MonoBehaviour
{
    /// <summary>
    /// 状態
    /// </summary>
    enum State
    {
        Chase,  // 追尾
        Keep,   // 様子見
        Attack, // 攻撃
        Idle,   // 待機
    }

    /// <summary> プレイヤー </summary>
    [Header("プレイヤー取得")]
    [SerializeField] private Transform player;

    /// <summary> 
    /// 攻撃のトリガーとなるプレイヤーとの距離
    /// </summary>
    [Header("攻撃のトリガーとなるプレイヤーとの距離")]
    [SerializeField] private float triggerDistance;

    /// <summary> 追尾速度 </summary>
    [Header("追尾速度")]
    [SerializeField] private float chaseSpeed;

    /// <summary> プレイヤーに近づいた後の移動速度 </summary>
    [Header("プレイヤーに近づいた後の移動速度")]
    [SerializeField] private float moveSpeed;

    /// <summary> 攻撃する確率(%) </summary>
    [Header("攻撃する確率(%)")]
    [SerializeField] private float attackProbability;

    /// <summary> 現在の敵の状態 </summary>
    private State currentState;

    /// <summary> 敵AI操作用 </summary>
    private NavMeshAgent enemyAi;

    /// <summary> アニメーション操作用 </summary>
    private Animator anim;

    /// <summary> 物理用 </summary>
    private Rigidbody rd;

    private bool canAttack = true;

    // Start is called before the first frame update
    void Start()
    {
        // 敵AI操作用コンポーネントを取得
        enemyAi = gameObject.GetComponent<NavMeshAgent>();

        // アニメーション操作用コンポーネントを取得
        anim = gameObject.GetComponent<Animator>();

        // 物理用コンポーネントを取得
        rd = gameObject.GetComponent<Rigidbody>();

        // 追尾状態に設定
        currentState = State.Chase;

        enemyAi.speed = chaseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // プレイヤーが設定されていないので、
        // 処理を実行しないようにする
        if (!player)
        {
            Debug.Log("プレイヤーが設定されていません");
            return;
        }

        // コンポーネント確認
        if (!CheckComponent())
        {
            return;
        }

        // アニメーション制御用の速度を渡す
        anim.SetFloat("Speed", rd.velocity.magnitude);

        switch (currentState)
        {
            // ---------------------
            // 追尾状態
            // ---------------------
            case State.Chase:

                Chase();

                break;

            // -------------------
            // 様子見状態
            // -------------------
            case State.Keep:

                Keep();

                break;

            // --------------------
            // 攻撃状態
            // --------------------
            case State.Attack:

                Attack();

                break;
        }
    }

    #region 状態の処理

    /// <summary>
    /// 追尾状態の処理
    /// </summary>
    private void Chase()
    {
        // プレイヤーに向かって移動
        enemyAi.SetDestination(player.position);

        // プレイヤーとの距離を確認
        if (CheckDistance(player))
        {
            // 様子見状態ではNavMeshで動かさないので、
            // speedを0に設定
            enemyAi.speed = 0;

            // プレイヤーに近づいたので
            // 様子見状態に移行
            currentState = State.Keep;
        }
    }

    /// <summary>
    /// 様子見状態の処理
    /// </summary>
    private void Keep()
    {
        // 回転移動の速度
        float rotateSpeed = moveSpeed * 10.0f;

        // プレイヤーを中心に回転移動
        transform.RotateAround(player.position, Vector3.up,
            rotateSpeed * Time.deltaTime);

        // プレイヤーとの距離を確認
        if (!CheckDistance(player))
        {
            // 追尾状態ではNavMeshで動かすので
            // speedを追尾速度に設定
            enemyAi.speed = chaseSpeed;

            // プレイヤーから離れているので
            // 追尾状態に移行
            currentState = State.Chase;
        }
    }

    /// <summary>
    /// 攻撃状態の処理
    /// </summary>
    private void Attack()
    {
        anim.SetTrigger("AttackTrigger");

        // 様子見状態に移行
        currentState = State.Keep;

        canAttack = false;
    }

    #endregion

    /// <summary>
    /// コンポーネントがあるのか確認する
    /// </summary>
    /// <returns>
    /// <para> true : コンポーネントがある </para>
    /// <para> false : コンポーネントがない </para>
    /// </returns>
    private bool CheckComponent()
    {
        // 敵操作用コンポーネントがない
        if (!enemyAi)
        {
            Debug.Log("NavMeshAgentがありません");
            return false;
        }

        // アニメーション操作用コンポーネントがない
        if (!anim)
        {
            Debug.Log("Animatorがありません");
            return false;
        }

        // 物理用コンポーネントがない
        if (!rd)
        {
            Debug.Log("Rigidbodyがありません");
            return false;
        }

        return true;
    }

    /// <summary>
    /// ターゲットとの距離を調べる
    /// </summary>
    /// <param name="target"> 対象 </param>
    /// <returns>
    /// <para> true : ターゲットとの距離が一定以内 </para>
    /// <para> false : ターゲットとの距離が一定以上 </para>
    /// </returns>
    private bool CheckDistance(Transform target)
    {
        // ターゲットとの距離
        float distance = Vector3.Distance(
            target.position, transform.position);

        // 一定以内にターゲットがいるのか
        return distance < triggerDistance;
    }

    /// <summary>
    /// 確率判定
    /// </summary>
    /// <param name="probability"> trueになる確率(%) </param>
    /// <returns>
    /// 指定した確率でtrueになる
    /// </returns>
    private bool CheckProbability(float probability)
    {
        // 乱数(0 ～ 100.0)
        float randomValue = UnityEngine.Random.value * 100.0f;

        // 確率より値が小さい場合は、
        // 確率を引いたと判定する
        return randomValue <= probability;
    }
}
