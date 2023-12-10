using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Enemy1MagicMissile : MonoBehaviour
{
    private Vector3 playerPosition;
    private Vector3 direction;
    private float baseAngle;
    private float radius;
    [SerializeField] GameObject magicMissileSpawnner;
    [SerializeField] GameObject enemy;
    [SerializeField] float speed;

    //ゲッターセッター
    public Vector3 SetPlayerPosition
    {
        set { playerPosition = value; }
    }
    public float SetBaseAngle
    {
        set { baseAngle = value; }
    }
    public float SetRadius
    {
        set { radius = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerPosition = GameObject.Find("Player").transform.position;
        direction = playerPosition - transform.position;

        //弾を発射する
        GetComponent<Rigidbody>().velocity = direction * speed;
    }

    // Update is called once per frame
    void Update()
    {
        //弾の消去
        if(transform.position.y < -5.0f)
        {
            Destroy(gameObject);
        }

        //弾の位置決定
        Vector3 newPos = new Vector3(
             (magicMissileSpawnner.transform.position.x + Mathf.Cos(baseAngle * Mathf.Deg2Rad) * radius) * Mathf.Cos(enemy.transform.rotation.y),
             (magicMissileSpawnner.transform.position.y + Mathf.Sin(baseAngle * Mathf.Deg2Rad) * radius),
             (magicMissileSpawnner.transform.position.z) * Mathf.Sin(enemy.transform.rotation.y));
        transform.position = newPos;
    }
}
