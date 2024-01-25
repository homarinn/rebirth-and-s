using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CS_EnemyPlayerWeapon : MonoBehaviour
{
    /// <summary> 状態確認用 </summary>
    private CS_EnemyPlayer enemyPlayer;

    // Start is called before the first frame update
    private void Start()
    {
        enemyPlayer = transform.parent.parent.GetComponent<CS_EnemyPlayer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 持ち主が取得できない
        if (!enemyPlayer)
        {
            Debug.Log("持ち主を取得できていない");
            return;
        }

        // プレイヤーに当たってないなら何もしない
        if(other.tag != "Player")
        {
            return;
        }

        // 攻撃中・必殺技中でないなら何もしない
        if (enemyPlayer.CurrentState != CS_EnemyPlayer.State.Attack &&
            enemyPlayer.CurrentState != CS_EnemyPlayer.State.Ult)
        {
            return;
        }

        // 攻撃が当たるようになっているので与ダメージ処理
        if (enemyPlayer.CanWeaponHit)
        {
            // プレイヤーにダメージ
            enemyPlayer.PlayerDamage();
        }
    }
}
