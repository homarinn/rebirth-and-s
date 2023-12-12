using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CS_Enemy1 : MonoBehaviour
{
    private GameObject playerObject;
    private GameObject magicMissileSpawnner;
    [SerializeField] GameObject magicMissile;
    [SerializeField] int magicMissileNumber;
    [SerializeField] float radius;

    //試験用
    [SerializeField] float maxAttackInterval;
    private float attackInterval;

    // Start is called before the first frame update
    void Start()
    {
        //オブジェクトの取得
        playerObject = GameObject.Find("Player");
        magicMissileSpawnner = transform.Find("MagicMissileSpawnner").gameObject;

        //変数の初期化
        attackInterval = maxAttackInterval;

        //弾の生成
        for(int i = 0; i < magicMissileNumber; ++i)
        {
            float angle = i * (360.0f / magicMissileNumber);
            Vector3 pos = new Vector3(
                magicMissileSpawnner.transform.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                magicMissileSpawnner.transform.position.y + Mathf.Sin(angle * Mathf.Deg2Rad) * radius,
                magicMissileSpawnner.transform.position.z);
            GameObject go = Instantiate(magicMissile, pos, magicMissileSpawnner.transform.rotation);
            CS_Enemy1MagicMissile script = go.GetComponent<CS_Enemy1MagicMissile>();
            script.SetBaseAngle = angle;
            script.SetRadius = radius;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーの方向を向く
        LookAtPlayer();

        //弾の生成
        //attackInterval -= Time.deltaTime;
        //if(attackInterval < 0)
        //{
        //    Instantiate(magicMissile, 
        //        magicMissileSpawnner.transform.position, Quaternion.identity);
        //    attackInterval = maxAttackInterval;
        //}
    }

    /// <summary>
    /// プレイヤーの方向を向く
    /// </summary>
    void LookAtPlayer()
    {
        Vector3 direction = playerObject.transform.position - transform.position;
        direction.y = 0.0f;  //X軸回転を反映しない
        transform.forward = direction;
    }
}
