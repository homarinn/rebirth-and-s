using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_ATest : MonoBehaviour
{
    [SerializeField] private GameObject enemy;

    [SerializeField] private GameObject effect;

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
        
        if(Input.GetKeyDown(KeyCode.P))
        {
            var effectObject = Instantiate(effect, enemyPlayer.transform);
            Destroy(effectObject, 1.0f);
        } 
        
        if(Input.GetKeyDown(KeyCode.F))
        {
            enemyPlayer.CancelStandby();
        }
    }
}
