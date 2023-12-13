using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// 敵の管理クラス
/// </summary>
public class CS_Enemy : MonoBehaviour
{
    /// <summary> プレイヤー </summary>
    [Header("プレイヤー取得")]
    [SerializeField] private Transform player;

    /// <summary> アニメーション操作用 </summary>
    private Animator anim;

    /// <summary> 物理用 </summary>
    private Rigidbody rd;

    /// <summary> 現在の状態 </summary>
    private CS_IEnemyState currentState;

    /// <summary> 次の状態 </summary>
    private CS_IEnemyState nextState;

    /// <summary> 待機状態の管理クラス </summary>
    private CS_EnemyIdleState idleState;

    // Start is called before the first frame update
    void Start()
    {
        // アニメーション操作用コンポーネントを取得
        anim = gameObject.GetComponent<Animator>();

        // 物理用コンポーネントを取得
        rd = gameObject.GetComponent<Rigidbody>();

        // 追尾状態の管理クラスを取得
        currentState = gameObject.GetComponent<CS_EnemyChaseState>();

        // 待機状態の管理クラスを取得
        idleState = gameObject.GetComponent<CS_EnemyIdleState>();

        // 状態の初期処理
        currentState.StartState();
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

        // 状態が設定されているか確認
        if (!currentState)
        {
            Debug.Log("状態が設定されていません");
        }

        // 状態の処理を実行し、
        // 次の状態を取得する
        nextState = currentState.RunState(player);

        anim.SetFloat("Speed", rd.velocity.magnitude);

        // 次の状態が指定されてない場合
        if (!nextState)
        {
            Debug.Log("次の状態が存在しません");

            // 待機状態にする
            currentState = idleState;
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

    /// <summary>
    /// コンポーネントがあるのか確認する
    /// </summary>
    /// <returns>
    /// <para> true : コンポーネントがある </para>
    /// <para> false : コンポーネントがない </para>
    /// </returns>
    private bool CheckComponent()
    {
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
}
