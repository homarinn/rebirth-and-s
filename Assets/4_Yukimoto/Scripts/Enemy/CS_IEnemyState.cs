using System.Collections;
using System.Collections.Generic;
using System.Resources;
using UnityEngine;

/// <summary>
/// 敵の状態を管理する基底クラス
/// </summary>
public abstract class CS_IEnemyState : MonoBehaviour
{
    /// <summary>
    /// 状態の初期処理
    /// </summary>
    public abstract void StartState();

    /// <summary>
    /// 状態の処理を実行する
    /// </summary>
    /// <returns> 次の状態の管理クラス </returns>
    public abstract CS_IEnemyState RunState();
}
