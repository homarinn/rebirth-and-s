using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CS_Enemy1 : MonoBehaviour
{
    //攻撃の種類
    enum AttackType
    {
        Weak,     //弱攻撃
        Strong,   //強攻撃
        BlowOff,  //吹き飛ばし攻撃
    }
    AttackType attackType;

    [SerializeField] GameObject player;

    [SerializeField] GameObject weakMagicMissile;
    [SerializeField] GameObject strongMagicMissile;

    [SerializeField] int weakMagicMissileNumber;              //一度に生成する弾の数
    [SerializeField] int strongMagicMissileNumber;            //一度に生成する弾の数

    [SerializeField] float halfCircleRadius;            //半円の半径
    [SerializeField] Vector3 spawnPos = new Vector3(0.0f, 0.1f, 1.0f);  //敵1を基準とした弾の生成位置

    [SerializeField] float weakAttackProbability;       //弱攻撃の発生確率
    [SerializeField] float strongAttackProbability;     //強攻撃の発生確率
    [SerializeField] float blowOffAttackProbability;    //吹き飛ばし攻撃の発生確率

    [SerializeField] float weakCreationInterval;  //弾を生成する間隔（秒）
    [SerializeField] float strongCreationInterval;  //弾を生成する間隔（秒）

    [SerializeField] float weakShootInterval;        //次の弾を発射するまでの間隔（弱攻撃、秒）
    [SerializeField] float strongShootInterval;        //次の弾を発射するまでの間隔（弱攻撃、秒）

    [SerializeField] float maxAttackIntervalSeconds;    //攻撃のインターバル（秒）

    //このコード内で使用するための変数
    GameObject[] magicMissile = new GameObject[2]; //弾
    int[] magicMissileNumber = new int[2];         //弾の数
    float[] creationInterval = new float[2];       //弾の生成速度（秒）
    float[] shootInterval;                         //連射速度（秒）
    GameObject[] createdMagicMissile;              //生成した弾
    CS_Enemy1MagicMissile[] script;                //弾のスクリプト

    float attackIntervalSeconds;
    int magicMissileCount = 1;
    int evenCount = 0;  //偶数発目の弾が生成された数
    int oddCount = 0;   //1発目を除く奇数発目の弾が生成された数
    bool isAttack;      //攻撃中か？

    float[] weight = new float[3];  //各攻撃の重み（発生確率）
    float totalWeight;  //3種類の攻撃の重み（発生確率）の総和

    // Start is called before the first frame update
    void Start()
    {
        //変数の初期化
        Initialize();

        //最初の攻撃を設定
        weight[0] = weakAttackProbability;
        weight[1] = strongAttackProbability;
        weight[2] = blowOffAttackProbability;
        for(int i = 0; i < GetEnumLength<AttackType>(); ++i)
        {
            totalWeight += weight[i];
        }
        ChooseAttackType();


        //エラーメッセージ
        for(int i = 0; i < 2; ++i)
        {
            if (magicMissileNumber[i] % 2 == 0)
            {
                AttackType type = (AttackType)i;
                Debug.Log("(" + type + ")偶数発の弾数が設定されています。奇数発の弾数に変更してください。");
            }
        }
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Initialize()
    {
        //弱攻撃
        int num = (int)AttackType.Weak;
        magicMissile[num] = weakMagicMissile;
        magicMissileNumber[num] = weakMagicMissileNumber;
        creationInterval[num] = 0.0f;  //1発目は即座に生成するため0
        //強攻撃
        num = (int)AttackType.Strong;
        magicMissile[num] = strongMagicMissile;
        magicMissileNumber[num] = strongMagicMissileNumber;
        creationInterval[num] = 0.0f;

        //その他変数の初期化
        int max = Math.Max(magicMissileNumber[(int)AttackType.Weak],
                           magicMissileNumber[(int)AttackType.Strong]);
        shootInterval = new float[max];
        createdMagicMissile = new GameObject[max];
        script = new CS_Enemy1MagicMissile[max];
        for (int i = 0; i < max; ++i)
        {
            shootInterval[i] = 0.0f;
            createdMagicMissile[i] = null;
            script[i] = null;
        }
        isAttack = false;
        attackIntervalSeconds = maxAttackIntervalSeconds;
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーの方向を向く
        LookAtPlayer();

        //攻撃
        if (isAttack)
        {
            Shoot(attackType);  //弾を発射する

            //攻撃が終了したら次の攻撃の種類を決定
            if (!isAttack)
            {
                ChooseAttackType();
            }
            return;
        }

        //インターバルが経過したら次の攻撃へ移る
        attackIntervalSeconds -= Time.deltaTime;
        if(attackIntervalSeconds <= 0.0f)
        {
            //弾の生成
            int num = (int)attackType;  //要素番号指定用変数
            creationInterval[num] -= Time.deltaTime;
            if (creationInterval[num] <= 0.0f)
            {
                CreateMagicMissile(attackType);
            }
        }
    }

    /// <summary>
    /// 弾を発射するまでの時間を設定する
    /// </summary>
    /// <param name="type">攻撃の種類（弱・強）</param>
    /// <param name="iteration">生成した弾のイテレーション</param>
    void SetTimeUntilShoot(AttackType type, int iteration)
    {
        if(type == AttackType.Weak)
        {
            shootInterval[iteration] = weakShootInterval * (iteration + 1);
        }
        else
        {
            shootInterval[iteration] = strongShootInterval * (iteration + 1);
        }
    }

    /// <summary>
    /// 繰り出す攻撃の種類を選ぶ
    /// </summary>
    void ChooseAttackType()
    {
        //位置を選ぶ
        float randomPoint = UnityEngine.Random.Range(0, totalWeight);

        //抽選
        float currentWeight = 0.0f;  //現在の重みの位置
        for(int i = 0; i < GetEnumLength<AttackType>(); ++i)
        {
            currentWeight += weight[i];

            if(randomPoint < currentWeight)
            {
                SetAttackType(i);
                return;
            }
        }

        //位置が重みの総和以上なら末尾要素とする
        SetAttackType(weight.Length - 1) ;
    }

    /// <summary>
    /// 攻撃の種類を設定する
    /// </summary>
    /// <param name="iteration">イテレーション</param>
    void SetAttackType(int iteration)
    {
        if(iteration == 0)
        {
            attackType = AttackType.Weak;
        }
        if(iteration == 1)
        {
            attackType = AttackType.Strong;
        }
        if(iteration == 2)
        {
            attackType = AttackType.BlowOff;
        }
        Debug.Log(attackType);
    }

    /// <summary>
    /// enum型の要素数を取得する
    /// </summary>
    /// <typeparam name="T">enum型</typeparam>
    /// <returns>要素数</returns>
    int GetEnumLength<T>()
    {
        return Enum.GetValues(typeof(T)).Length;
    }

    /// <summary>
    /// プレイヤーの方向を向く
    /// </summary>
    void LookAtPlayer()
    {
        if(player == null)
        {
            return;
        }

        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0.0f;  //X軸回転を反映しない
        transform.forward = direction;
    }

    /// <summary>
    /// 弾を生成する
    /// </summary>
    /// <param name="type">攻撃の種類（弱・強）</param>
    void CreateMagicMissile(AttackType type)
    {
        int num = (int)type;  //要素番号指定用変数

        float angleSpace = 180.0f / magicMissileNumber[num];  //弾同士の間隔
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
            Instantiate(magicMissile[num], magicMissilePos, Quaternion.identity);
        createdMagicMissile[magicMissileCount - 1].transform.SetParent(gameObject.transform);  //敵と弾を親子関係に
        script[magicMissileCount - 1] =
            createdMagicMissile[magicMissileCount - 1].GetComponent<CS_Enemy1MagicMissile>();

        //各変数の更新
        SetTimeUntilShoot(type, magicMissileCount - 1);
        magicMissileCount++;
        if(type == AttackType.Weak)
        {
            creationInterval[num] = weakCreationInterval;
        }
        else
        {
            creationInterval[num] = strongCreationInterval;
        }

        //全て生成したら生成を止め、攻撃に移る
        if (magicMissileCount > magicMissileNumber[num])
        {
            isAttack = true;      //攻撃中
            magicMissileCount = 1;
            evenCount = oddCount = 0;
            creationInterval[num] = 0.0f;  
        }
    }

    /// <summary>
    /// 弾を発射する
    /// </summary>
    void Shoot(AttackType type)
    {
        int num = (int)type;  //要素番号指定用変数

        for (int i = 0; i < magicMissileNumber[num]; ++i)
        {
            if (createdMagicMissile[i] == null)
            {
                continue;
            }

            //発射時間になったら発射
            shootInterval[i] -= Time.deltaTime;
            if (shootInterval[i] < 0.0f)
            {
                script[i].GetSetIsCanFire = true;  //発射

                //初期化
                createdMagicMissile[i] = null;
                script[i] = null;

                //最後の弾を撃ったら攻撃終了
                if (i == magicMissileNumber[num] - 1)
                {
                    isAttack = false;
                    attackIntervalSeconds = maxAttackIntervalSeconds;
                }
            }
        }
    }
}


//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//public class CS_Enemy1 : MonoBehaviour
//{
//    //攻撃の種類
//    enum AttackType
//    {
//        Weak,     //弱攻撃
//        Strong,   //強攻撃
//        BlowOff,  //吹き飛ばし攻撃
//    }
//    AttackType type;

//    //弾を撃つ攻撃
//    struct ShootAttack
//    {
//        GameObject magicMissilePrefab;  //弾のプレハブ
//        int magicMissileNumber;         //弾の数
//        //float Probability;              //攻撃の発生確率
//        float creationInterval;         //弾の生成速度（秒）
//        float shootInterval;            //連射速度（秒）
//    }
//    ShootAttack[] shootAttack = new ShootAttack[2];

//    [SerializeField] GameObject player;
//    [SerializeField] GameObject magicMissilePrefab;
//    [SerializeField] int magicMissileNumber;            //一度に生成する弾の数
//    [SerializeField] float halfCircleRadius;            //半円の半径
//    [SerializeField] Vector3 spawnPos = new Vector3(0.0f, 0.1f, 1.0f);  //敵1を基準とした弾の生成位置

//    [SerializeField] float weakAttackProbability;       //弱攻撃の発生確率
//    [SerializeField] float strongAttackProbability;     //強攻撃の発生確率
//    [SerializeField] float blowOffAttackProbability;    //吹き飛ばし攻撃の発生確率

//    [SerializeField] float maxCreationIntervalSeconds;  //弾を生成する間隔（秒）
//    [SerializeField] float maxWeakShootInterval;        //次の弾を発射するまでの間隔（弱攻撃、秒）
//    [SerializeField] float maxAttackIntervalSeconds;    //攻撃のインターバル（秒）

//    GameObject[] createdMagicMissile;                   //生成した弾
//    CS_Enemy1MagicMissile[] script;

//    float creationIntervalSeconds;
//    float[] weakShootInterval;    //弾を発射するまでの時間
//    float attackIntervalSeconds;
//    int magicMissileCount = 1;
//    int evenCount = 0;  //偶数発目の弾が生成された数
//    int oddCount = 0;   //1発目を除く奇数発目の弾が生成された数
//    bool isAttack;      //攻撃中か？

//    float[] weight = new float[3];  //各攻撃の重み（発生確率）
//    float totalWeight;  //3種類の攻撃の重み（発生確率）の総和

//    // Start is called before the first frame update
//    void Start()
//    {
//        //最初の攻撃を設定
//        weight[0] = weakAttackProbability;
//        weight[1] = strongAttackProbability;
//        weight[2] = blowOffAttackProbability;
//        for(int i = 0; i < GetEnumLength<AttackType>(); ++i)
//        {
//            totalWeight += weight[i];
//        }

//        ChooseAttackType();

//        //攻撃の種類に合わせた値を変数に格納する



//        //変数の初期化
//        creationIntervalSeconds = 0.0f;  //1発目は即座に生成するため0
//        isAttack = false;
//        attackIntervalSeconds = maxAttackIntervalSeconds;
//        createdMagicMissile = new GameObject[magicMissileNumber];
//        script = new CS_Enemy1MagicMissile[magicMissileNumber];
//        weakShootInterval = new float[magicMissileNumber];
//        for(int i = 0; i < magicMissileNumber; ++i)
//        {
//            createdMagicMissile[i] = null;
//            script[i] = null;
//            weakShootInterval[i] = 0.0f;
//        }

//        if (magicMissileNumber % 2 == 0)
//        {
//            Debug.Log("（敵１）偶数発の弾数が設定されています。奇数発の弾数に変更してください。");
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        ChooseAttackType();
//        Debug.Log(type);

//        //プレイヤーの方向を向く
//        LookAtPlayer();

//        //攻撃
//        if (isAttack)
//        {
//            Shoot();  //弾を発射する

//            //攻撃が終了したら次の攻撃の種類を決定
//            if (!isAttack)
//            {
                
//            }
//            return;
//        }

//        //インターバルが経過したら次の攻撃へ移る
//        attackIntervalSeconds -= Time.deltaTime;
//        if(attackIntervalSeconds <= 0.0f)
//        {
//            //弾の生成
//            creationIntervalSeconds -= Time.deltaTime;
//            if (creationIntervalSeconds <= 0.0f)
//            {
//                CreateMagicMissile();
//            }
//        }


//        //発射したらmagicMissileCountの初期化を忘れずに
//    }

//    /// <summary>
//    /// 弾を発射するまでの時間を設定する
//    /// </summary>
//    /// <param name="iteration">生成した弾のイテレーション</param>
//    void SetValueOfTimeUntilShoot(int iteration)
//    {
//        weakShootInterval[iteration] = maxWeakShootInterval * (iteration + 1);
//    }

//    /// <summary>
//    /// 繰り出す攻撃の種類を選ぶ
//    /// </summary>
//    void ChooseAttackType()
//    {
//        //位置を選ぶ
//        float randomPoint = UnityEngine.Random.Range(0, totalWeight);

//        //抽選
//        float currentWeight = 0.0f;  //現在の重みの位置
//        for(int i = 0; i < GetEnumLength<AttackType>(); ++i)
//        {
//            currentWeight += weight[i];

//            if(randomPoint < currentWeight)
//            {
//                SetAttackType(i);
//                return;
//            }
//        }

//        //位置が重みの総和以上なら末尾要素とする
//        SetAttackType(weight.Length - 1) ;
//    }

//    /// <summary>
//    /// AttackType型変数に値を設定する
//    /// </summary>
//    /// <param name="iteration">イテレーション</param>
//    void SetAttackType(int iteration)
//    {
//        if(iteration == 0)
//        {
//            type = AttackType.Weak;
//        }
//        if(iteration == 1)
//        {
//            type = AttackType.Strong;
//        }
//        if(iteration == 2)
//        {
//            type = AttackType.BlowOff;
//        }
//    }

//    /// <summary>
//    /// enum型の要素数を取得する
//    /// </summary>
//    /// <typeparam name="T">enum型</typeparam>
//    /// <returns>要素数</returns>
//    int GetEnumLength<T>()
//    {
//        return Enum.GetValues(typeof(T)).Length;
//    }

//    /// <summary>
//    /// プレイヤーの方向を向く
//    /// </summary>
//    void LookAtPlayer()
//    {
//        Vector3 direction = player.transform.position - transform.position;
//        direction.y = 0.0f;  //X軸回転を反映しない
//        transform.forward = direction;
//    }

//    /// <summary>
//    /// 弾を生成する
//    /// </summary>
//    void CreateMagicMissile()
//    {
//        float angleSpace = 180.0f / magicMissileNumber;  //弾同士の間隔
//        const float baseAngle = 90.0f;  //1つ目の弾の配置角度
//        float angle = 0.0f;

//        //半円のどこに配置するか決定
//        if (magicMissileCount == 1)  //1発目
//        {
//            angle = baseAngle;
//        }
//        else if (magicMissileCount % 2 == 0)  //偶数発目
//        {
//            evenCount++;
//            angle = baseAngle - evenCount * angleSpace;  //敵からみて左に順に配置
//        }
//        else  //奇数発目
//        {
//            oddCount++;
//            angle = baseAngle + oddCount * angleSpace;   //敵から見て右に順に配置
//        }

//        //敵1の回転を考慮して座標決定
//        Vector3 magicMissilePos = new Vector3(
//            spawnPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * halfCircleRadius,
//            spawnPos.y + Mathf.Sin(angle * Mathf.Deg2Rad) * halfCircleRadius,
//            spawnPos.z);
//        magicMissilePos = transform.TransformPoint(magicMissilePos);

//        //生成
//        //magicMissileCountは1から始まるため-1
//        createdMagicMissile[magicMissileCount - 1] = 
//            Instantiate(magicMissilePrefab, magicMissilePos, Quaternion.identity);
//        createdMagicMissile[magicMissileCount - 1].transform.SetParent(gameObject.transform);  //敵と弾を親子関係に
//        script[magicMissileCount - 1] =
//            createdMagicMissile[magicMissileCount - 1].GetComponent<CS_Enemy1MagicMissile>();


//        //各変数の更新
//        SetValueOfTimeUntilShoot(magicMissileCount - 1);
//        magicMissileCount++;
//        creationIntervalSeconds = maxCreationIntervalSeconds;

//        //全て生成したら生成を止め、攻撃に移る
//        if (magicMissileCount > magicMissileNumber)
//        {
//            isAttack = true;      //攻撃中
//            magicMissileCount = 1;
//            evenCount = oddCount = 0;
//            creationIntervalSeconds = 0.0f;  
//        }
//    }

//    /// <summary>
//    /// 弾を発射する
//    /// </summary>
//    void Shoot()
//    {
//        for(int i = 0; i < magicMissileNumber; ++i)
//        {
//            if (createdMagicMissile[i] == null)
//            {
//                continue;
//            }

//            //発射時間になったら発射
//            weakShootInterval[i] -= Time.deltaTime;
//            if (weakShootInterval[i] < 0.0f)
//            {
//                script[i].GetSetIsCanFire = true;

//                createdMagicMissile[i] = null;
//                script[i] = null;

//                //最後の弾を撃ったら攻撃終了
//                if(i == magicMissileNumber - 1)
//                {
//                    isAttack = false;
//                    attackIntervalSeconds = maxAttackIntervalSeconds;
//                }
//            }
//        }
//    }
//}
