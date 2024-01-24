using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_ATest : MonoBehaviour
{
    [SerializeField] private GameObject enemy;

    [SerializeField] private float damage;

    [SerializeField] private float damageCutRatio;

    private CS_EnemyPlayer enemyPlayer;

    // Start is called before the first frame update
    void Start()
    {
        if (enemy)
            enemyPlayer = enemy.GetComponent<CS_EnemyPlayer>();
        //Debug.Log(enemyPlayer.Hp);
    }

    // Update is called once per frame
    void Update()
    {
        if (!enemyPlayer)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            // ダメージの軽減値
            float cut = damage * (damageCutRatio / 100);
            float d = damage - cut;
            enemyPlayer.ReceiveDamage(d);
            //Debug.Log(enemyPlayer.Hp);
        }
    }
}
