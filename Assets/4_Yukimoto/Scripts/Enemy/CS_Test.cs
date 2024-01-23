using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Test : MonoBehaviour
{
    [SerializeField] private GameObject enemy;

    private CS_EnemyPlayer enemyPlayer;

    // Start is called before the first frame update
    void Start()
    {
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
            enemyPlayer.ReceiveDamage(2);
            //Debug.Log(enemyPlayer.Hp);
        }
    }
}
