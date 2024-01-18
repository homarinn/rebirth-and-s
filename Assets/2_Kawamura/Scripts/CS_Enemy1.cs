using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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

    [Header("プレイヤー")]
    [SerializeField] GameObject player;

    [Header("弾（弱攻撃）")]
    [SerializeField] GameObject weakMagicMissile;

    [Header("弾（強攻撃）")]
    [SerializeField] GameObject strongMagicMissile;

    [Header("吹き飛ばしエフェクト")]
    [SerializeField] CS_Enemy1BlowOffEffect blowOffEffect;

    [Header("最大HP")]
    [SerializeField] float maxHp;

    [Header("回転速度")]
    [SerializeField] float rotateSpeed;

    [Header("攻撃間のインターバル（秒）")]
    [SerializeField] float maxAttackInterval;

    [Header("弱攻撃の発生確率（%）")]
    [SerializeField] float weakAttackProbability;   

    [Header("強攻撃の発生確率（%）")]
    [SerializeField] float strongAttackProbability; 

    [Header("吹き飛ばし攻撃の発生確率（%）")]
    [SerializeField] float blowOffAttackProbability;

    [Header("半円状に弾を生成する時の半円の半径")]
    [SerializeField] float halfCircleRadius;

    [Header("敵の位置を基準とした弾の生成位置")]
    [SerializeField] Vector3 magicMissileSpawnPos = new Vector3(0.0f, 0.1f, 1.0f);

    [Header("曲線軌道にするか？（弱攻撃）")]
    [SerializeField] bool isCurveWeakMagicMissile;

    [Header("曲線軌道にするか？（強攻撃）")]
    [SerializeField] bool isCurveStrongMagicMissile;

    [Header("1回の攻撃で生成する弾の数（弱攻撃）")]
    [SerializeField] int weakMagicMissileNumber;

    [Header("1回の攻撃で生成する弾の数（強攻撃）")]
    [SerializeField] int strongMagicMissileNumber;

    [Header("弾を生成する間隔（弱攻撃、秒）")]
    [SerializeField] float weakCreationInterval;  

    [Header("弾を生成する間隔（強攻撃、秒）")]
    [SerializeField] float strongCreationInterval;

    [Header("弾の発射間隔（弱攻撃、秒)")]
    [SerializeField] float weakShootInterval;   

    [Header("弾の発射間隔（強攻撃、秒)")]
    [SerializeField] float strongShootInterval;

    [Header("吹き飛ばす力")]
    [SerializeField] float blowOffPower;

    [Header("ダウンさせるために必要なダメージ量")]
    [SerializeField] float downedDamageAmount;

    [Header("ダウン時間")]
    [SerializeField] float downedTime;

    [Header("ダウン終了後、定位置に戻るまでの速さ（秒）")]
    [SerializeField] float timeReturnNormalPos;

    [Header("弾を撃つSE")]
    [SerializeField] private AudioClip shotSE;

    [Header("吹き飛ばし攻撃のSE")]
    [SerializeField] private AudioClip blowOffSE;


    //このコード内で使用するための変数
    Rigidbody myRigidbody;                         //自分のRigidbody
    CS_Enemy1MagicMissile[] script;                //弾のスクリプト

    GameObject[] magicMissile = new GameObject[2]; //弾
    GameObject[] createdMagicMissile;              //生成した弾

    Vector3 normalPos;                             //通常時の位置
    Vector3 downedPos;                             //ダウンしたときの位置
    float hp;                                      //HP
    int[] magicMissileNumber = new int[2];         //弾の数
    float[] creationInterval = new float[2];       //弾の生成速度（秒）
    float[] shootInterval;                         //連射速度（秒）
    float[] probability = new float[3];            //各攻撃の発生確率
    float totalProbability;                        //全攻撃の発生確率の総和
    float damageAmount;                            //ダメージ量
    float totalDownedTime;                         //ダウン時間
    float totalReturnTime;                         //定位置に戻るまでにかかった総合計時間

    float attackInterval;                          //攻撃間のインターバル用変数
    int magicMissileCount = 1;                     //何発目かを表す変数
    int evenCount = 0;                             //偶数発目の弾が生成された数
    int oddCount = 0;                              //1発目を除く奇数発目の弾が生成された数
    bool isAttack;                                 //攻撃中か？
    bool isReturningNormalPos;                     //定位置に戻っている途中か？
    bool isDowned;                                 //ダウン状態か？
    bool isDead;                                   //死亡したか？

    AudioSource shotAudioSource;                   //弾を撃つ用のAudioSource
    //AudioSource blowOffAudioSource;              //吹き飛ばし攻撃用のAudioSource

    //実験用
    [Header("吹き飛ばし攻撃するまでの時間（実験用）")]
    [SerializeField] float maxBlowOffCount;

    float blowOffCount;
    float blowOffDuration;
    bool isBlowingOff;

    //ゲッター
    public float GetHp
    {
        get { return hp; }
    }
    public Vector3 GetLocalEulerAngle
    {
        get { return transform.localEulerAngles; }
    }

    private void Awake()
    {
        hp = maxHp;
    }

    // Start is called before the first frame update
    void Start()
    {
        //初期化処理
        Initialize();

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
        myRigidbody = GetComponent<Rigidbody>();
        myRigidbody.useGravity = false;
        normalPos = transform.position;
        downedPos = new Vector3(0, 0, 0);
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
        damageAmount = 0.0f;
        totalDownedTime = 0.0f;
        totalReturnTime = 0.0f;
        attackInterval = maxAttackInterval;
        isAttack = false;
        isReturningNormalPos = false;
        isDowned = false;
        isDead = false;

        //最初の攻撃を設定
        probability[0] = weakAttackProbability;
        probability[1] = strongAttackProbability;
        probability[2] = blowOffAttackProbability;
        for (int i = 0; i < GetEnumLength<AttackType>(); ++i)
        {
            totalProbability += probability[i];
        }
        ChooseAttackType();

        //AudioSourceの取得
        AudioSource[] audioSources = GetComponents<AudioSource>();
        shotAudioSource = audioSources[0];
        //blowOffAudioSource = audioSources[1];


        //実験用変数の初期化
        blowOffCount = maxBlowOffCount;
        isBlowingOff = false;
        blowOffDuration = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //ReduceHp(Time.deltaTime);
        //Debug.Log("damageAmount = " + damageAmount);

        //死亡
        if (hp <= 0.0f)
        {
            Death();
            return;
        }

        //ダウン
        if(damageAmount > downedDamageAmount)
        {
            Downed();
            return;
        }

        //定位置に戻る
        if (isReturningNormalPos)
        {
            ReturnNormalPos();
            return;
        }

        //プレイヤーの方向を向く
        LookAtPlayer();

        //攻撃
        if (isAttack)
        {
            Attack(attackType);  //攻撃する

            //攻撃が終了したら次の攻撃の種類を決定
            if (!isAttack)
            {
                ChooseAttackType();
            }
            return;
        }

        //インターバルが経過したら次の攻撃へ移る
        attackInterval -= Time.deltaTime;
        if(attackInterval <= 0.0f)
        {
            //攻撃の準備
            AttackReady(attackType);
        }
    }

    /// <summary>
    /// 弾を発射するまでの時間を設定する
    /// </summary>
    /// <param name="type">攻撃の種類（弱・強）</param>
    /// <param name="iteration">生成した弾のイテレーション</param>
    void SetShootInterval(AttackType type, int iteration)
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
        float randomPoint = UnityEngine.Random.Range(0, totalProbability);

        //抽選
        float currentWeight = 0.0f;  //現在の重みの位置
        for(int i = 0; i < GetEnumLength<AttackType>(); ++i)
        {
            currentWeight += probability[i];

            if(randomPoint < currentWeight)
            {
                SetAttackType(i);
                return;
            }
        }

        //位置が重みの総和以上なら末尾要素とする
        SetAttackType(probability.Length - 1) ;
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

        //ターゲット方向を取得
        Vector3 targetDirection = player.transform.position - transform.position;
        targetDirection.y = 0.0f;  //X軸回転を反映しない

        //ターゲット方向の回転を表すQuaternionを取得
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        //滑らかに回転
        transform.rotation = 
            Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 攻撃準備をする
    /// </summary>
    /// <param name="type">攻撃の種類</param>
    void AttackReady(AttackType type)
    {
        switch (type)
        {
        case AttackType.Weak:
        case AttackType.Strong:
            //弾の生成
            int num = (int)attackType;  //要素番号指定用変数
            creationInterval[num] -= Time.deltaTime;
            if (creationInterval[num] <= 0.0f)
            {
                CreateMagicMissile(attackType);
            }
            break;
        case AttackType.BlowOff:
            ReadyBlowOff();
            break;
        }
    }

    /// <summary>
    /// 弾を生成する
    /// </summary>
    /// <param name="type">攻撃の種類（弱・強）</param>
    void CreateMagicMissile(AttackType type)
    {
        int num = (int)type;  //要素番号指定用変数

        float angleSpace = 160.0f / magicMissileNumber[num];  //半円の中での弾同士の間隔
        const float baseAngle = 90.0f;  //1つ目の弾の配置角度を半円の真ん中にする
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
            magicMissileSpawnPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * halfCircleRadius,
            magicMissileSpawnPos.y + Mathf.Sin(angle * Mathf.Deg2Rad) * halfCircleRadius,
            magicMissileSpawnPos.z);
        magicMissilePos = transform.TransformPoint(magicMissilePos);

        //生成
        //magicMissileCountは1から始まるため-1
        createdMagicMissile[magicMissileCount - 1] =
            Instantiate(magicMissile[num], magicMissilePos, Quaternion.identity);
        //敵と弾を親子関係に
        createdMagicMissile[magicMissileCount - 1].transform.SetParent(gameObject.transform);

        //弾の回転値設定
        Vector3 localEulerAngles = createdMagicMissile[magicMissileCount - 1].transform.localEulerAngles;
        localEulerAngles.y += transform.localEulerAngles.y;
        createdMagicMissile[magicMissileCount - 1].transform.localEulerAngles = localEulerAngles;
        //Debug.Log(createdMagicMissile[magicMissileCount - 1].transform.localEulerAngles);

        script[magicMissileCount - 1] =
            createdMagicMissile[magicMissileCount - 1].GetComponent<CS_Enemy1MagicMissile>();

        //実験用
        script[magicMissileCount - 1].SetMagicMissileCount = magicMissileCount;

        //各変数の更新
        SetShootInterval(type, magicMissileCount - 1);
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
            isAttack = true;
            magicMissileCount = 1;
            evenCount = oddCount = 0;
            creationInterval[num] = 0.0f;  
        }
    }

    /// <summary>
    /// 吹き飛ばし攻撃の準備
    /// </summary>
    void ReadyBlowOff()
    {
        blowOffCount -= Time.deltaTime;
        if(blowOffCount <= 0.0f)
        {
            isAttack = true;
        }
    }

    /// <summary>
    /// 攻撃する
    /// </summary>
    /// <param name="type">攻撃の種類</param>
    void Attack(AttackType type)
    {
        switch (type)
        {
            case AttackType.Weak:
            case AttackType.Strong:
                Shoot(type);
                break;
            case AttackType.BlowOff:
                BlowOff();
                break;
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
                script[i].SetPlayerTransform = player.transform;
                //実験用
                if(type == AttackType.Weak)
                {
                    script[i].SetIsCurve = isCurveWeakMagicMissile;
                }
                else
                {
                    script[i].SetIsCurve = isCurveStrongMagicMissile;
                }

                //初期化
                createdMagicMissile[i] = null;
                script[i] = null;

                //音
                shotAudioSource.PlayOneShot(shotSE);
                //shotAudioSource.PlayOneShot(shotAudioSource.clip);

                //最後の弾を撃ったら攻撃終了
                if (i == magicMissileNumber[num] - 1)
                {
                    isAttack = false;
                    attackInterval = maxAttackInterval;
                }
            }
        }
    }

    /// <summary>
    /// 弾を消す
    /// </summary>
    void DestroyMagicMissile()
    {
        int max = Math.Max(magicMissileNumber[(int)AttackType.Weak],
                           magicMissileNumber[(int)AttackType.Strong]);

        for (int i = 0; i < max; ++i)
        {
            if (createdMagicMissile[i] == null)
            {
                continue;
            }
            //発射されていたら消さない（いらない？）
            if (script[i].GetSetIsCanFire)
            {
                continue;
            }

            Destroy(createdMagicMissile[i]);
        }
    }

    /// <summary>
    /// 吹き飛ばし攻撃を行う
    /// </summary>
    void BlowOff()
    {
        //エフェクト発生
        if (!isBlowingOff)
        {
            var effect = Instantiate(blowOffEffect, transform.position, Quaternion.identity);
            effect.SetBlowOffPower = blowOffPower;
            effect.PlayEffect();

            isBlowingOff = true;
            blowOffDuration = effect.GetEffectDuration;
        }

        //攻撃が終わったら変数を初期化
        blowOffDuration -= Time.deltaTime;
        if(blowOffDuration < 0.0f)
        {
            isAttack = false;
            isBlowingOff = false;
            attackInterval = maxAttackInterval;
            blowOffCount = maxBlowOffCount;
        }
    }

    /// <summary>
    /// 死亡時処理
    /// </summary>
    void Death()
    {
        if (!isDead)
        {
            //死亡
            isDead = true;
            Debug.Log("死亡");

            //変数の初期化
            isAttack = false;

            //弾を消す
            DestroyMagicMissile();

            //死亡アニメーション再生

            //地面に下ろす（物理挙動を使わない方法でもよい？）
            myRigidbody.useGravity = true;
        }
    }

    /// <summary>
    /// ダウン時処理
    /// </summary>
    void Downed()
    {
        if (!isDowned)
        {
            isDowned = true;
            myRigidbody.useGravity = true;

            //変数の初期化（弾関連は絶対）
            isAttack = false;
            attackInterval = maxAttackInterval;
            magicMissileCount = 1;
            evenCount = oddCount = 0;
            for(int i = 0; i < 2; ++i)
            {
                creationInterval[i] = 0.0f;
            }
            //吹き飛ばし攻撃
            blowOffCount = maxBlowOffCount;

            //弾を消す
            DestroyMagicMissile();

            //ダウンアニメーション再生

            //地面に下ろす（物理挙動を使わない方法でもよい？）
            myRigidbody.useGravity = true;
        }

        //ダウン時間が経過したら定位置に戻る
        totalDownedTime += Time.deltaTime;
        if(totalDownedTime > downedTime)
        {
            isDowned = false;
            downedPos = transform.position;
            myRigidbody.useGravity = false;
            damageAmount = 0.0f;
            totalDownedTime = 0.0f;

            //定位置に戻るアニメーション再生

            //定位置に戻す
            isReturningNormalPos = true;

            //攻撃の種類を決定
            ChooseAttackType();
        }
    }

    /// <summary>
    /// 定位置に戻る
    /// </summary>
    void ReturnNormalPos()
    {
        //移動処理
        totalReturnTime += Time.deltaTime;
        float t = Mathf.Clamp01(totalReturnTime / timeReturnNormalPos);
        transform.position = Vector3.Lerp(downedPos, normalPos, t);

        if(transform.position == normalPos)
        {
            isReturningNormalPos = false;
            totalReturnTime = 0.0f;
        }
    }

    /// <summary>
    /// ダメージ量に加算する
    /// </summary>
    /// <param name="attackPower">プレイヤーの攻撃力</param>
    void AddDamageAmount(float attackPower)
    {
        if (!isDowned)
        {
            damageAmount += attackPower;
        }
    }

    /// <summary>
    /// 敵1のHPを減らす
    /// </summary>
    /// <param name="attackPower">プレイヤーの攻撃力</param>
    public void ReduceHp(float attackPower)
    {
        hp -= attackPower;
        AddDamageAmount(attackPower);

        if(hp < 0)
        {
            hp = 0;
        }
    }
}