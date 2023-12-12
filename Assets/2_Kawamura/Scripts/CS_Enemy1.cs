using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CS_Enemy1 : MonoBehaviour
{
    enum AttackType
    {
        Weak,     //弱攻撃
        Strong,   //強攻撃
        BlowOff,  //吹き飛ばし攻撃
    }

    [SerializeField] GameObject player;
    [SerializeField] GameObject magicMissilePrefab;
    [SerializeField] int magicMissileNumber;            //一度に生成する弾の数
    [SerializeField] float halfCircleRadius;            //半円の半径
    [SerializeField] Vector3 spawnPos = new Vector3(0.0f, 0.1f, 1.0f);  //敵1を基準とした弾の生成位置

    [SerializeField] float weakAttackProbability;       //弱攻撃の発生確率
    [SerializeField] float strongAttackProbability;     //強攻撃の発生確率
    [SerializeField] float blowOffAttackProbability;    //吹き飛ばし攻撃の発生確率

    [SerializeField] float maxCreationIntervalSeconds;  //弾を生成する間隔（秒）
    [SerializeField] float maxWeakShootInterval;        //次の弾を発射するまでの間隔（弱攻撃、秒）
    [SerializeField] float maxAttackIntervalSeconds;    //攻撃のインターバル（秒）

    GameObject[] createdMagicMissile;                   //生成した弾
    CS_Enemy1MagicMissile[] script;

    float creationIntervalSeconds;
    float[] weakShootInterval;    //弾を発射するまでの時間
    float attackIntervalSeconds;
    int magicMissileCount = 1;
    int evenCount = 0;  //偶数発目の弾が生成された数
    int oddCount = 0;   //1発目を除く奇数発目の弾が生成された数
    bool isAttack;      //攻撃中か？

    // Start is called before the first frame update
    void Start()
    {
        //変数の初期化
        creationIntervalSeconds = 0.0f;  //1発目は即座に生成するため0
        isAttack = false;
        attackIntervalSeconds = maxAttackIntervalSeconds;
        createdMagicMissile = new GameObject[magicMissileNumber];
        script = new CS_Enemy1MagicMissile[magicMissileNumber];
        weakShootInterval = new float[magicMissileNumber];
        for(int i = 0; i < magicMissileNumber; ++i)
        {
            createdMagicMissile[i] = null;
            script[i] = null;
            weakShootInterval[i] = 0.0f;
        }

        if (magicMissileNumber % 2 == 0)
        {
            Debug.Log("（敵１）偶数発の弾数が設定されています。奇数発の弾数に変更してください。");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーの方向を向く
        LookAtPlayer();

        //攻撃
        if (isAttack)
        {
            Attack();  //弾を発射する

            //攻撃が終了したら次の攻撃の種類を決定
            if (!isAttack)
            {
                
            }
            return;
        }

        //インターバルが経過したら次の攻撃へ移る
        attackIntervalSeconds -= Time.deltaTime;
        if(attackIntervalSeconds <= 0.0f)
        {
            //弾の生成
            creationIntervalSeconds -= Time.deltaTime;
            if (creationIntervalSeconds <= 0.0f)
            {
                CreateMagicMissile();
            }
        }


        //発射したらmagicMissileCountの初期化を忘れずに
    }

    /// <summary>
    /// 弾を発射するまでの時間を設定する
    /// </summary>
    /// <param name="iteration">生成した弾のイテレーション</param>
    void SetValueOfTimeUntilShoot(int iteration)
    {
        weakShootInterval[iteration] = maxWeakShootInterval * (iteration + 1);
    }

    /// <summary>
    /// 繰り出す攻撃の種類を決定する
    /// </summary>
    void DecideAttackType()
    {

    }

    /// <summary>
    /// プレイヤーの方向を向く
    /// </summary>
    void LookAtPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0.0f;  //X軸回転を反映しない
        transform.forward = direction;
    }

    /// <summary>
    /// 弾を生成する
    /// </summary>
    void CreateMagicMissile()
    {
        float angleSpace = 180.0f / magicMissileNumber;  //弾同士の間隔
        const float baseAngle = 90.0f;  //1つ目の弾の配置角度
        float angle = 0.0f;

        //半円のどこに配置するか決定
        if (magicMissileCount == 1)  //1発目
        {
            angle = baseAngle;
        }
        else if (magicMissileCount % 2 == 0)  //偶数発目
        {
            evenCount++;
            angle = baseAngle - evenCount * angleSpace;  //敵からみて左に順に配置
        }
        else  //奇数発目
        {
            oddCount++;
            angle = baseAngle + oddCount * angleSpace;   //敵から見て右に順に配置
        }

        //敵1の回転を考慮して座標決定
        Vector3 magicMissilePos = new Vector3(
            spawnPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * halfCircleRadius,
            spawnPos.y + Mathf.Sin(angle * Mathf.Deg2Rad) * halfCircleRadius,
            spawnPos.z);
        magicMissilePos = transform.TransformPoint(magicMissilePos);

        //生成
        //magicMissileCountは1から始まるため-1
        createdMagicMissile[magicMissileCount - 1] = 
            Instantiate(magicMissilePrefab, magicMissilePos, Quaternion.identity);
        createdMagicMissile[magicMissileCount - 1].transform.SetParent(gameObject.transform);  //敵と弾を親子関係に
        script[magicMissileCount - 1] =
            createdMagicMissile[magicMissileCount - 1].GetComponent<CS_Enemy1MagicMissile>();


        //各変数の更新
        SetValueOfTimeUntilShoot(magicMissileCount - 1);
        magicMissileCount++;
        creationIntervalSeconds = maxCreationIntervalSeconds;

        //全て生成したら生成を止め、攻撃に移る
        if (magicMissileCount > magicMissileNumber)
        {
            isAttack = true;      //攻撃中
            magicMissileCount = 1;
            evenCount = oddCount = 0;
            creationIntervalSeconds = 0.0f;  
        }
    }

    /// <summary>
    /// 弾を発射する
    /// </summary>
    void Attack()
    {
        for(int i = 0; i < magicMissileNumber; ++i)
        {
            if (createdMagicMissile[i] == null)
            {
                continue;
            }

            //発射時間になったら発射
            weakShootInterval[i] -= Time.deltaTime;
            if (weakShootInterval[i] < 0.0f)
            {
                script[i].GetSetIsCanFire = true;

                createdMagicMissile[i] = null;
                script[i] = null;

                //最後の弾を撃ったら攻撃終了
                if(i == magicMissileNumber - 1)
                {
                    isAttack = false;
                    attackIntervalSeconds = maxAttackIntervalSeconds;
                }
            }
        }
    }
}
