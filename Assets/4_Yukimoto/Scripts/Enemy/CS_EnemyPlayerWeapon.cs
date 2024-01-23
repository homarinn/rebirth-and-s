using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CS_EnemyPlayerWeapon : MonoBehaviour
{
    /// <summary> プレイヤーのダメージ用 </summary>
    [Header("プレイヤー")]
    [SerializeField] private CS_Player player;

    /// <summary> 状態確認用 </summary>
    private CS_EnemyPlayer enemyPlayer;

    // Start is called before the first frame update
    private void Start()
    {
        enemyPlayer = transform.parent.GetComponent<CS_EnemyPlayer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // 攻撃中・必殺技中でないなら何もしない
        if (enemyPlayer.CurrentState != CS_EnemyPlayer.State.Attack &&
            enemyPlayer.CurrentState != CS_EnemyPlayer.State.Ult)
        {
            return;
        }

        // プレイヤーに命中
        if (other.tag == "Player" && enemyPlayer.canWeaponHit)
        {
            // 多段防止で命中できないようにする
            enemyPlayer.canWeaponHit = false;

            // プレイヤーにダメージ
            player.Damage(1);
        }
    }
}
