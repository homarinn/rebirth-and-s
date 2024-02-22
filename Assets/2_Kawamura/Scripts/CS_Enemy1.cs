using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
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

    [Header("<=====弾=====>")]
    [Header("弱")]
    [SerializeField] GameObject weakMagicMissile;

    [Header("強")]
    [SerializeField] GameObject strongMagicMissile;
    [Header("==========")]

    [Header("吹き飛ばしエフェクト")]
    [SerializeField] CS_Enemy1BlowOffEffect blowOffEffect;

    [Header("ステージ境界円スクリプト")]
    [SerializeField] CS_StageBoundary stageBoundary;

    [Header("最大HP")]
    [SerializeField] float maxHp;

    [Header("回転速度")]
    [SerializeField] float rotateSpeed;

    [Header("攻撃インターバル（秒）")]
    [SerializeField] float maxAttackInterval;

    [Header("弾生成時の半円半径")]
    [SerializeField] float halfCircleRadius;

    [Header("弾の生成位置（自分基準）")]
    [SerializeField] Vector3 magicMissileSpawnPos = new Vector3(0.0f, 0.1f, 1.0f);

    [Header("<=====攻撃発生確率（%）=====>")]
    [Header("弱")]
    [SerializeField] float weakAttackProbability;   

    [Header("強")]
    [SerializeField] float strongAttackProbability; 

    [Header("吹き飛ばし")]
    [SerializeField] float blowOffAttackProbability;

    [Header("<=====弾の移動速度=====>")]
    [Header("弱")]
    [SerializeField] float weakMoveSpeed;

    [Header("強")]
    [SerializeField] float strongMoveSpeed;

    [Header("<=====弾の威力=====>")]
    [Header("弱")]
    [SerializeField] float weakAttackPower;

    [Header("強")]
    [SerializeField] float strongAttackPower;

    [Header("<=====生成弾数（※奇数のみ）=====>")]
    [Header("弱")]
    [SerializeField] int weakMagicMissileNumber;

    [Header("強")]
    [SerializeField] int strongMagicMissileNumber;

    [Header("<=====生成間隔（秒）=====>")]
    [Header("弱")]
    [SerializeField] float weakCreationInterval;  

    [Header("強")]
    [SerializeField] float strongCreationInterval;

    [Header("<=====発射間隔（秒）=====>")]
    [Header("弱")]
    [SerializeField] float weakShootInterval;   

    [Header("強")]
    [SerializeField] float strongShootInterval;

    [Header("<=====弾発射時のアニメーション速度倍率（0~1）=====>")]
    [Header("弱")]
    [SerializeField] float weakAnimationSpeedRatio;

    [Header("強")]
    [SerializeField] float strongAnimationSpeedRatio;
    [Header("==========")]

    [Header("生成した弾が完全に大きくなるためにかかる時間(秒)")]
    [SerializeField] float timeScaleUpMagicMissileCompletely;

    [Header("弾発射可能な距離割合(プレイヤーの最大移動範囲に対する割合（0~1))")]
    [SerializeField] float maxDistanceRatio;

    [Header("吹き飛ばす力")]
    [SerializeField] float blowOffPower;

    [Header("吹き飛ばし攻撃の生成位置(自分の位置との差)")]
    [SerializeField] Vector3 blowOffEffectPosition;

    [Header("ダウン時間")]
    [SerializeField] float downedTime;

    [Header("ダウンさせるために必要なダメージ量")]
    [SerializeField] float downedDamageAmount;

    [Header("ダウン時、アニメーションを一時停止する時間(秒)")]
    [SerializeField] float timeStopMotion;

    [Header("ダウン終了後、定位置に戻るまでの速さ(秒)")]
    [SerializeField] float timeReturnNormalPos;

    //このコード内で使用するための変数
    Rigidbody myRigidbody;                         //自分のRigidbody
    Animator myAnimator;                             //自分のanimator
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
    bool canFight;                                   //戦闘可能か？
    bool isDead;                                   //死亡したか？

    AudioSource shotAudioSource;                   //弾を撃つ用のAudioSource

    [Header("<=====降下時間（秒）=====>")]
    [Header("ダウン時")]
    [SerializeField] float timeFallToGroundByDown;

    //[Header("死亡時")]
    //[SerializeField] float timeFallToGroundByDeath;
    //[Header("==========")]

    [Header("接地検知オブジェクト")]
    [SerializeField] GameObject groundDetection;

    [Header("もやエフェクト")]
    [SerializeField] ParticleSystem mist;

    [Header("放出もやエフェクト")]
    [SerializeField] ParticleSystem releaseMist;

    [Header("溜めエフェクト")]
    [SerializeField] ParticleSystem trail;

    [Header("土煙エフェクト")]
    [SerializeField] ParticleSystem dustCloud;

    [Header("弾を撃つSE")]
    [SerializeField] private AudioClip shotSE;

    //[Header("曲線軌道にするか？（弱攻撃）")]
    //[SerializeField] bool isCurveWeakMagicMissile;

    //[Header("曲線軌道にするか？（強攻撃）")]
    //[SerializeField] bool isCurveStrongMagicMissile;

    //float blowOffCount;

    //float blowOffDuration;
    //bool isBlowingOff;

    const int defaultPuddleRenderQueue = 2980;
    int addPuddleRenderQueue;

    //float scaleRatioBasedOnY;
    Vector3 scaleRatioBasedOnY;

    float maxCanShootDistance;

    //死亡時の移動用
    //const float moveTimeOfDeath = 0.0f;  //死亡時のみスピードをスクリプト内で隠ぺいする
    float timeArriveGround;
    float totalFallTime;       //落下総合計時間
    Vector3 currentPosition;  //現在の位置
    Vector3 atGroundPosition;   //地面に降りたときの位置
    bool isGround;              //地面に降りたか？
    Collider myCollider;      //自分のコライダー

    //攻撃アニメーション対応用
    bool canShoot;  //発射可能か？
    bool canBlowOff;//吹き飛ばし攻撃可能か？
    //bool canFallByDeath;       //死亡によって落下できるか？

    int downCount = 0;

    bool canFall;  //アニメーション一時停止用

    bool isWaitRise;  //LookAtが作動してしまうため

    bool isStartReadyStandby;  //演出待機の準備開始
    bool isStandby;  //演出待機しているか？
    bool isStartGame;
    bool isFall;  //降下中か？
    bool isWaitFall;  //降下準備中か？
    float timeArriveNormalPos;
    Coroutine waitFall;

    //エフェクト用
    CS_Enemy1Trail trailScript;
    bool isPlayDustCloudEffect;

    //float shotCount = 0;

    //ゲッター
    public float GetHp
    {
        get { return hp; }
    }
    public bool GetIsDead
    {
        get { return isDead; }
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
        myAnimator = GetComponent<Animator>();
        myCollider = GetComponent<Collider>();
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
        canFight = false;
        isDead = false;

        //最初の攻撃を設定
        probability[0] = weakAttackProbability;
        probability[1] = strongAttackProbability;
        probability[2] = blowOffAttackProbability;
        for (int i = 0; i < GetEnumLength<AttackType>(); ++i)
        {
            totalProbability += probability[i];
        }
        SetAttackType(0);  //最初は弱攻撃
        //ChooseAttackType();

        //AudioSourceの取得
        shotAudioSource = GetComponent<AudioSource>();
        //blowOffAudioSource = audioSources[1];

        ////AudioSourceの取得
        //AudioSource[] audioSources = GetComponents<AudioSource>();
        //shotAudioSource = audioSources[0];
        ////blowOffAudioSource = audioSources[1];


        //実験用変数の初期化
        //isBlowingOff = false;
        //blowOffDuration = 0.0f;

        addPuddleRenderQueue = 0;

        //元のプレハブの比率計算
        Vector3 localScale = strongMagicMissile.transform.localScale;
        scaleRatioBasedOnY = new Vector3(
            localScale.x / localScale.y,
            1.0f,  //Yが基準
            localScale.z / localScale.y);
        //scaleRatioBasedOnY = localScale.x / localScale.y;

        //弾を発射できる最大距離を計算
        float boundaryCircleRadius = stageBoundary.GetBoundaryCircleRadius;
        maxCanShootDistance = 
            (boundaryCircleRadius * boundaryCircleRadius) * maxDistanceRatio;

        //降下時のlerp用
        totalFallTime = 0.0f;
        currentPosition = new Vector3(0.0f, 0.0f, 0.0f);
        atGroundPosition = new Vector3(0.0f, 0.0f, 0.0f);
        isGround = false;

        //攻撃アニメーション対応用
        canShoot = false;
        canBlowOff = false;
        //canFallByDeath = false;

        //アニメーション一時停止用
        canFall = false;

        //LookAtが作動するため
        isWaitRise = false;

        //演出待機用
        isStartReadyStandby = false;
        isStandby = true;  //初期状態は待機
        isStartGame = false;
        isFall = false;
        isWaitFall = false;
        timeArriveNormalPos = timeReturnNormalPos;
        waitFall = StartCoroutine(WaitFall());

        //エフェクト用
        trailScript = trail.GetComponent<CS_Enemy1Trail>();
        trail.Stop();
        mist.Stop();
        isPlayDustCloudEffect = false;

        //死亡時用
        groundDetection.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        //if (Input.GetKeyDown(KeyCode.P))
        //{
        //    hp = maxHp * 0.5f;
        //}
        if (Input.GetKeyDown(KeyCode.B))
        {
            hp = 0.0f;
        }
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    ReduceHp(downedDamageAmount + 1.0f);
        //    //hp -= downedDamageAmount + 1.0f;
        //}
        //if (Input.GetKeyDown(KeyCode.C))
        //{
        //    CancelStandby();
        //    hp = maxHp;
        //}

        //if ((!isStandby && !isStartReadyStandby) && hp <= maxHp * 0.5f)
        //{
        //    Standby();
        //}


        //if (shotCount > 0)
        //{
        //    return;
        //}

        //float damage = Time.deltaTime;
        //if (downCount % 2 != 0) { damage *= 3.0f; }
        //ReduceHp(damage);
        //Debug.Log("HP = " + hp);

        //if (Input.GetKeyDown(KeyCode.G))
        //{
        //    ReduceHp(30);
        //    //hp = 0.0f;
        //}

        //Debug.Log("isDead = " + isDead);

        //死亡
        if (hp <= 0.0f)
        {
            Death();
            return;
        }

        //演出待機
        if (isStandby && !isStartReadyStandby)
        {
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
            //Debug.Log("定位置に戻る");
            ReturnNormalPos();
            return;
        }

        //上昇待機中は処理しない
        if (isWaitRise)
        {
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

                //攻撃アニメーション用変数を初期化
                canShoot = false;  //発射不可
                canBlowOff = false;//吹き飛ばし不可
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
            shootInterval[iteration] = weakShootInterval * iteration;
            //shootInterval[iteration] = weakShootInterval * (iteration + 1);
        }
        else
        {
            shootInterval[iteration] = strongShootInterval * iteration;
            //shootInterval[iteration] = strongShootInterval * (iteration + 1);
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

        float angleSpace = 200.0f / magicMissileNumber[num];  //半円の中での弾同士の間隔(元は160.0f)
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
        //magicMissilePos = transform.TransformPoint(magicMissilePos);  //弾の先端を前に向かせるときは消す

        //Quaternion spawnRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        //Quaternion spawnRotation = Quaternion.Euler(0f, 0f, angle) * transform.rotation;
        //Quaternion spawnRotation = Quaternion.Euler(transform.localEulerAngles.y, -45.0f, -90.0f);

        //生成
        //magicMissileCountは1から始まるため-1
        createdMagicMissile[magicMissileCount - 1] =
            Instantiate(magicMissile[num], magicMissilePos, Quaternion.identity);


        //敵と弾を親子関係に
        createdMagicMissile[magicMissileCount - 1].transform.SetParent(gameObject.transform, false);

        script[magicMissileCount - 1] =
            createdMagicMissile[magicMissileCount - 1].GetComponent<CS_Enemy1MagicMissile>();

        //弾のパラメータ設定
        SetMoveSpeed(type, magicMissileCount - 1);
        SetAttackPower(type, magicMissileCount - 1);

        //実験用
        script[magicMissileCount - 1].SetMagicMissileCount = magicMissileCount;
        script[magicMissileCount - 1].SetPuddleRenderQueue = defaultPuddleRenderQueue + addPuddleRenderQueue;
        script[magicMissileCount - 1].SetPlayerTransform = player.transform;

        //実験用2
        Vector3 localScale = strongMagicMissile.transform.localScale;
        //script[magicMissileCount - 1].SetScaleRatioBasedOnY = scaleRatioBasedOnY;

        //弾の種類をセット
        script[magicMissileCount - 1].SetMagicMissileType = SetMagicMissileType(type);


        //弾の目標スケールを計算する
        //親のスケールを反映しないYを計算
        Vector3 myLossyScale = transform.lossyScale;
        float scaleY = createdMagicMissile[magicMissileCount - 1].transform.localScale.y / myLossyScale.y;
        float newScaleX = scaleY * (scaleRatioBasedOnY.x);  //2.0 = 調整
        float newScaleZ = scaleY * (scaleRatioBasedOnY.z);  //2.0 = 調整
        Vector3 target = new Vector3(
            newScaleX,
            scaleY,
            newScaleZ);
        script[magicMissileCount - 1].SetTargetScaleForCreate = target;
        script[magicMissileCount - 1].SetTimeScaleUpCompletely = timeScaleUpMagicMissileCompletely;
        //弾のスケールを0に
        createdMagicMissile[magicMissileCount - 1].transform.localScale = Vector3.zero;

        //Vector3 parentLossyScale = transform.parent.lossyScale;
        //float scaleY = transform.localScale.y / parentLossyScale.y;
        ////XZが同じ比率になるようにYを基準とした比率をYにかけて代入
        //float newScaleX = scaleY * (scaleRatioBasedOnY.x);  //2.0 = 調整
        //float newScaleZ = scaleY * (scaleRatioBasedOnY.z);  //2.0 = 調整
        //transform.localScale = new Vector3(
        //    newScaleX,
        //    scaleY,
        //    newScaleZ);



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
        //実験用
        addPuddleRenderQueue++;
        if(addPuddleRenderQueue >= 20) { addPuddleRenderQueue = 0; }
        //if(addPuddleRenderQueue >= 15) { addPuddleRenderQueue = 0; }

        //全て生成したら生成を止め、攻撃に移る
        if (magicMissileCount > magicMissileNumber[num])
        {
            isAttack = true;
            magicMissileCount = 1;
            evenCount = oddCount = 0;
            creationInterval[num] = 0.0f;

            myAnimator.SetBool("Attack", true);  //モーション発動

            trail.Play();  //エフェクト再生
            trailScript.GetSetIsPlay = true;
            Debug.Log("trail再生生成終わり");
        }
    }

    //void CreateMagicMissile(AttackType type)
    //{
    //    int num = (int)type;  //要素番号指定用変数

    //    float angleSpace = 160.0f / magicMissileNumber[num];  //半円の中での弾同士の間隔
    //    const float baseAngle = 90.0f;  //1つ目の弾の配置角度を半円の真ん中にする
    //    float angle = 0.0f;

    //    //半円のどこに配置するか決定
    //    if (magicMissileCount == 1)  //1発目
    //    {
    //        angle = baseAngle;
    //    }
    //    else if (magicMissileCount % 2 == 0)  //偶数発目
    //    {
    //        evenCount++;
    //        angle = baseAngle - evenCount * angleSpace;  //敵からみて左に順に配置
    //    }
    //    else  //奇数発目
    //    {
    //        oddCount++;
    //        angle = baseAngle + oddCount * angleSpace;   //敵から見て右に順に配置
    //    }

    //    //敵1の回転を考慮して座標決定
    //    Vector3 magicMissilePos = new Vector3(
    //        magicMissileSpawnPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * halfCircleRadius,
    //        magicMissileSpawnPos.y + Mathf.Sin(angle * Mathf.Deg2Rad) * halfCircleRadius,
    //        magicMissileSpawnPos.z);
    //    magicMissilePos = transform.TransformPoint(magicMissilePos);  //弾の先端を前に向かせるときは消す

    //    //生成
    //    //magicMissileCountは1から始まるため-1
    //    createdMagicMissile[magicMissileCount - 1] =
    //        Instantiate(magicMissile[num], magicMissilePos, Quaternion.identity);


    //    //敵と弾を親子関係に
    //    createdMagicMissile[magicMissileCount - 1].transform.SetParent(gameObject.transform);

    //    ////弾の回転値設定
    //    //Vector3 localEulerAngles = createdMagicMissile[magicMissileCount - 1].transform.localEulerAngles;
    //    //localEulerAngles.y += transform.localEulerAngles.y;
    //    //createdMagicMissile[magicMissileCount - 1].transform.localEulerAngles = localEulerAngles;
    //    //Debug.Log(createdMagicMissile[magicMissileCount - 1].transform.localEulerAngles);

    //    script[magicMissileCount - 1] =
    //        createdMagicMissile[magicMissileCount - 1].GetComponent<CS_Enemy1MagicMissile>();

    //    //実験用
    //    script[magicMissileCount - 1].SetMagicMissileCount = magicMissileCount;
    //    script[magicMissileCount - 1].SetPuddleRenderQueue = defaultPuddleRenderQueue + addPuddleRenderQueue;
    //    script[magicMissileCount - 1].SetPlayerTransform = player.transform;

    //    //各変数の更新
    //    SetShootInterval(type, magicMissileCount - 1);
    //    magicMissileCount++;
    //    if(type == AttackType.Weak)
    //    {
    //        creationInterval[num] = weakCreationInterval;
    //    }
    //    else
    //    {
    //        creationInterval[num] = strongCreationInterval;
    //    }
    //    //実験用
    //    addPuddleRenderQueue++;
    //    if(addPuddleRenderQueue >= 15) { addPuddleRenderQueue = 0; }

    //    //全て生成したら生成を止め、攻撃に移る
    //    if (magicMissileCount > magicMissileNumber[num])
    //    {
    //        isAttack = true;
    //        magicMissileCount = 1;
    //        evenCount = oddCount = 0;
    //        creationInterval[num] = 0.0f;  
    //    }
    //}

    void SetMoveSpeed(AttackType type, int iteration)
    {
        if(type == AttackType.Weak)
        {
            script[iteration].SetMoveSpeed = weakMoveSpeed;
        }
        else if(type == AttackType.Strong)
        {
            script[iteration].SetMoveSpeed = strongMoveSpeed;
        }
    }
    void SetAttackPower(AttackType type, int iteration)
    {
        if(type == AttackType.Weak)
        {
            script[iteration].SetAttackPower = weakAttackPower;
        }
        else if(type == AttackType.Strong)
        {
            script[iteration].SetAttackPower = strongAttackPower;
        }
    }

    /// <summary>
    /// 弾の種類を設定する
    /// </summary>
    string SetMagicMissileType(AttackType type)
    {
        string magicMissileType = null;
        if(type == AttackType.Weak)
        {
            magicMissileType = "Weak";
        }
        else if(type == AttackType.Strong)
        {
            magicMissileType = "Strong";
        }

        return magicMissileType;
    }

    /// <summary>
    /// 吹き飛ばし攻撃の準備
    /// </summary>
    void ReadyBlowOff()
    {
        isAttack = true;  //攻撃開始
        myAnimator.SetBool("Attack", true);  //モーション発動

        if (!trailScript.GetSetIsPlay)
        {
            trail.Play();  //エフェクト再生
            trailScript.GetSetIsPlay = true;
        }

        ////blowOffCount -= Time.deltaTime;
        ////if(blowOffCount <= 0.0f)
        ////{
        ////    isAttack = true;
        ////    myAnimator.SetBool("Attack", false);  //モーション停止
        ////}


        //myAnimator.SetBool("Attack", true);  //モーション発動

        //blowOffCount -= Time.deltaTime;
        //if (blowOffCount <= 0.0f)
        //{
        //    isAttack = true;
        //    myAnimator.SetBool("Attack", false);  //モーション停止
        //}
    }

    /// <summary>
    /// AnimationEvent用の関数(弱・強攻撃用)
    /// </summary>
    void AttackEventShoot()
    {
        //吹き飛ばし用ではないので処理しない
        if(attackType == AttackType.BlowOff)
        {
            return;
        }

        //アニメーション速度を変える
        if(attackType == AttackType.Weak)
        {
            myAnimator.SetFloat("AnimationSpeed", weakAnimationSpeedRatio);
        }
        if(attackType == AttackType.Strong)
        {
            myAnimator.SetFloat("AnimationSpeed", strongAnimationSpeedRatio);
        }

        //発射可能
        canShoot = true;  

        //if(attackType == AttackType.Weak ||
        //    attackType == AttackType.Strong)
        //{
        //    canShoot = true;  //発射可能
        //}
    }

    /// <summary>
    /// AnimationEvent用の関数(吹き飛ばし攻撃用)
    /// </summary>
    void AttackEventBlowOff()
    {
        if(attackType == AttackType.BlowOff)
        {
            canBlowOff = true;//吹き飛ばし可能
            trail.Stop();  //エフェクト停止
            trailScript.GetSetIsPlay = false;
        }
    }

    /// <summary>
    /// AnimationEvent用の関数(ダウン時用)
    /// </summary>
    void DownedEvent()
    {
        //演出待機準備中かつダウン中
        if(isStartReadyStandby && isDowned)
        {
            canFall = false;
            //降下準備中の時に演出待機になったとき、強制的に
            if (isFall)
            {
                canFall = true;
            }
            Standby();  //ダウン時の変数処理を使用

            //こっちでいけるかも
            //もしくはStandby()のみ
            //canFall = false;
            //Standby();  //ダウン時の変数処理を使用
        }
    }

    /// <summary>
    /// AnimationEvent用の関数(上昇用)
    /// </summary>
    void RiseEvent()
    {
        //ダウンでなければ(もしくは降下中でなければ)上昇、かつ降下準備中でないとき
        if (!isDowned && !isWaitFall)
        {
            isReturningNormalPos = true;
            isWaitRise = false;
        }

        //通常時かつ上昇時だけエフェクト再生
        if (!isStartReadyStandby && !isStandby && !isDowned)
        {
            PlayDustCloudEffect();
        }
    }

    /// <summary>
    /// AnimationEvent用の関数(死亡用)
    /// </summary>
    void DeathEvent()
    {
        //canFallByDeath = true;

        //重力をつけてFreezePositionYだけ解放する
        myRigidbody.useGravity = true;
        myRigidbody.constraints = RigidbodyConstraints.None;
        myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        //myRigidbody.constraints = RigidbodyConstraints.FreezePositionX;
        //myRigidbody.constraints = RigidbodyConstraints.FreezePositionZ;

    }

    /// <summary>
    /// AnimationEvent用の関数(完全死亡用)
    /// </summary>
    void DeathEventIsDead()
    {
        isDead = true;
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
                //一定距離以内だったら弾を放つ
                if (CheckCanShoot())
                {
                    if (canShoot)
                    {
                        //モーションに合わせて発射
                        Shoot(type);

                        if (trailScript.GetSetIsPlay)
                        {
                            trail.Stop();
                            trailScript.GetSetIsPlay = false;
                        }
                    }
                    else
                    {
                        //範囲内に入ったのでモーションを再度再生
                        myAnimator.SetBool("Attack", true);

                        if (!trailScript.GetSetIsPlay)
                        {
                            trail.Play();
                            trailScript.GetSetIsPlay = true;
                        }
                    }
                }
                //一定距離離れていたら発射とモーション中止
                else
                {
                    canShoot = false;
                    myAnimator.SetBool("Attack", false);
                    myAnimator.SetFloat("AnimationSpeed", 1);  //速度を戻す

                    if (trailScript.GetSetIsPlay)
                    {
                        trail.Stop();
                        trailScript.GetSetIsPlay = false;
                    }
                }

                //if (isShooting) 
                //{
                //    if (CheckCanShoot())
                //    {
                //        Shoot(type);
                //    }
                //    else
                //    {
                //        isShooting = false;  //発射中止
                //        myAnimator.SetBool("Attack", false);  //モーション停止
                //    }
                //}
                break;
            case AttackType.BlowOff:
                if (canBlowOff)
                {
                    BlowOff();
                }
                break;
        }
    }

    /// <summary>
    /// 弾を発射することができるか確認する
    /// </summary>
    /// <returns>発射出来るか？</returns>
    bool CheckCanShoot()
    {
        //ステージの中心からプレイヤーまでの距離
        Vector2 playerDirection = new Vector2(
            player.transform.position.x,
            player.transform.position.z);
        float distance = playerDirection.sqrMagnitude;

        //一定距離以上だと発射不可
        if(distance > maxCanShootDistance)
        {
            return false;
        }

        return true;  //発射可能
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
                //script[i].SetPlayerTransform = player.transform;

                //実験用
                //if(type == AttackType.Weak)
                //{
                //    script[i].SetIsCurve = isCurveWeakMagicMissile;
                //}
                //else
                //{
                //    script[i].SetIsCurve = isCurveStrongMagicMissile;
                //}

                //初期化
                createdMagicMissile[i] = null;
                script[i] = null;

                //音
                shotAudioSource.PlayOneShot(shotSE);
                //shotAudioSource.PlayOneShot(shotAudioSource.clip);

                //最後より一つ前の弾が放たれたらアニメーション速度を元に戻す
                if (i == magicMissileNumber[num] - 2)
                {
                    myAnimator.SetFloat("AnimationSpeed", 1);
                }

                //最後の弾を撃ったら攻撃終了
                if (i == magicMissileNumber[num] - 1)
                {
                    isAttack = false;
                    attackInterval = maxAttackInterval;

                    myAnimator.SetBool("Attack", false);  //モーション停止
                    //shotCount++;
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
            createdMagicMissile[i] = null;
            script[i] = null;
        }
    }

    /// <summary>
    /// 吹き飛ばし攻撃を行う
    /// </summary>
    void BlowOff()
    {
        //エフェクト発生
        Vector3 pos = transform.position + blowOffEffectPosition;
        var effect = Instantiate(blowOffEffect, pos, Quaternion.identity);
        //var effect = Instantiate(blowOffEffect, transform.position, Quaternion.identity);
        effect.SetBlowOffPower = blowOffPower;
        effect.PlayEffect();
        //isBlowingOff = true;

        //blowOffDuration = effect.GetEffectDuration;

        //変数初期化
        isAttack = false;
        //isBlowingOff = false;
        attackInterval = maxAttackInterval;

        //アニメーション用変数初期化
        canBlowOff = false;
        myAnimator.SetBool("Attack", false);  //モーション停止
    }

    /// <summary>
    /// 死亡時処理
    /// </summary>
    void Death()
    {
        if (!canFight)
        {
            //死亡
            canFight = true;
            Debug.Log("死亡");

            //変数の初期化
            isAttack = false;

            //弾を消す
            DestroyMagicMissile();

            //死亡アニメーション再生
            myAnimator.SetTrigger("Death");
            //myAnimator.SetBool("Fall", true);

            //途中から再生？(アニメーションがつながらない）
            //myAnimator.Play("Death", 0, 0.15f);

            //カプセルコライダー無効化
            myCollider.enabled = false;

            //ボーンについているコライダー有効化
            Collider boneCollider = transform.GetChild(3).GetComponent<Collider>();
            boneCollider.enabled = true;

            //接地検知オブジェクト有効化
            groundDetection.SetActive(true);

            //エフェクト停止
            mist.Stop();
            trail.Stop();
            trailScript.GetSetIsPlay = false;

            //エフェクト再生
            Vector3 pos = transform.position;
            pos.y += 2.0f;
            Instantiate(releaseMist, pos, Quaternion.identity);

            //地面に下ろす(重力落下にしたので要らない)
            //目標までの距離から割合を算出してtimeArriveGroundの数値を変える
            // = 降下スピードが変わらない
            //isGround = false;
            //totalFallTime = 0.0f;  //初期化
            //timeArriveGround = timeFallToGroundByDeath * DistanceRatioToGround();
            //currentPosition = transform.position;

            //myRigidbody.useGravity = true;
        }

        //アニメーションが落下タイミングになったら落下させる
        //if (canFallByDeath)
        //{
        //    FallToGround();
        //}
    }

    /// <summary>
    /// ダウン時処理
    /// </summary>
    void Downed()
    {
        //Debug.Log("ダウン中");
        if (!isDowned)
        {
            //上昇中にダウンしたら上昇に関係ある変数を初期化
            //アニメーションも処理を変える
            if (isReturningNormalPos)
            {
                isReturningNormalPos = false;
                totalReturnTime = 0.0f;
                //myAnimator.SetBool("Rise", false);
                myAnimator.SetBool("Fall", true);
            }

            isDowned = true;
            isPlayDustCloudEffect = false;
            //myRigidbody.useGravity = true;

            //変数の初期化（弾関連は絶対）
            isAttack = false;
            attackInterval = maxAttackInterval;
            magicMissileCount = 1;
            evenCount = oddCount = 0;
            for(int i = 0; i < 2; ++i)
            {
                creationInterval[i] = 0.0f;
            }

            //弾を消す
            DestroyMagicMissile();

            //降下アニメーション再生
            myAnimator.SetBool("Fall", true);
            myAnimator.SetBool("Attack", false);
            myAnimator.SetFloat("AnimationSpeed", 1);  //速度を戻す

            //エフェクトの停止
            mist.Stop();
            trail.Stop();
            trailScript.GetSetIsPlay = false;

            //地面に下ろす
            //目標までの距離から割合を算出してtimeArriveGroundの数値を変える
            // = 降下スピードが変わらない
            isGround = false;
            timeArriveGround = timeFallToGroundByDown * DistanceRatioToDestination(atGroundPosition.y);
            currentPosition = transform.position;

            //myRigidbody.useGravity = true;
            downCount++;

            canFall = false;  //初期化
            StartCoroutine(WaitFall());
        }

        if (!canFall)
        {
            //一時停止中で落下できなければ処理しない
            return;
        }

        //地面まで降下する
        if (!isGround)
        {
            FallToGround();
            return;  //降下中はダウン時間をカウントしない
        }

        //ダウン時間が経過したら定位置に戻る
        totalDownedTime += Time.deltaTime;
        if(totalDownedTime > downedTime)
        {
            isDowned = false;
            downedPos = transform.position;
            //myRigidbody.useGravity = false;
            damageAmount = 0.0f;
            totalDownedTime = 0.0f;

            //定位置に戻るアニメーション再生
            myAnimator.SetBool("Rise", true);
            myAnimator.SetBool("Down", false);

            //エフェクトの再生
            mist.Play();

            //定位置に戻す
            //isReturningNormalPos = true;
            isWaitRise = true;
            timeArriveNormalPos = timeReturnNormalPos * DistanceRatioToDestination(normalPos.y);

            //攻撃の種類を決定
            ChooseAttackType();
            canShoot = false;  //発射不可（ダウンするとUpdate()の同様のコードに入らないから)
            canBlowOff = false;//吹き飛ばし不可
        }
    }

    /// <summary>
    /// 一定秒数落下を待つ
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitFall()
    {
        yield return new WaitForSeconds(timeStopMotion);

        canFall = true;
    }

    /// <summary>
    /// 現在の位置が目的地までどれくらいの距離割合なのか計算する
    /// </summary>
    /// <returns>距離割合</returns>
    float DistanceRatioToDestination(float destinationY)
    {
        float currentY = transform.position.y;
        //上昇、降下で処理を変える
        if (isGround)
        {
            if (currentY >= destinationY)
            {
                return 0.0f;
            }

            //最高値が逆になるため割合も逆
            return 1.0f - (currentY / normalPos.y);  
        }
        else
        {
            if (currentY <= destinationY)
            //if(currentY <= atGroundPosition.y)
            {
                return 0.0f;
            }

            return currentY / normalPos.y;  //割合
        }
    }

    /// <summary>
    /// 降下する
    /// </summary>
    void FallToGround()
    {
        isFall = true;

        //移動処理
        totalFallTime += Time.deltaTime;
        float t = Mathf.Clamp01(totalFallTime / timeArriveGround);
        transform.position = Vector3.Lerp(currentPosition, atGroundPosition, t);

        //演出待機準備中の場合、降下させない
        if (isStartReadyStandby)
        {
            t = 1.0f;
        }

        //着地より少し早めにエフェクト再生
        if(t >= 0.96f && t < 1)
        {
            if(!isPlayDustCloudEffect && !isStartReadyStandby)
            {
                //エフェクト生成
                PlayDustCloudEffect();
                isPlayDustCloudEffect = true;
            }
        }
        else if (t >= 1)
        {
            //演出待機準備中以外の時に地面の座標を入れる
            if (!isStartReadyStandby)
            {
                transform.position = atGroundPosition;
                isFall = false;
            }
            isGround = true;
            totalFallTime = 0.0f;

            //死亡していないときダウンモーション再生
            if(hp > 0)
            {
                myAnimator.SetBool("Down", true);
                myAnimator.SetBool("Fall", false);
                myAnimator.SetBool("Rise", false);  //上昇中にダウンしたときのため
            }
            //死亡していたらすべての処理をしないようにする
            //else
            //{
            //    canFallByDeath = false;
            //}
        }
    }

    /// <summary>
    /// 定位置に戻る
    /// </summary>
    void ReturnNormalPos()
    {
        //移動処理
        totalReturnTime += Time.deltaTime;
        float t = Mathf.Clamp01(totalReturnTime / timeArriveNormalPos);
        transform.position = Vector3.Lerp(downedPos, normalPos, t);

        if(t >= 1)
        //if(transform.position == normalPos)
        {
            isReturningNormalPos = false;
            totalReturnTime = 0.0f;
            transform.position = normalPos;
            myAnimator.SetBool("Rise", false);

            //演出待機準備状態なら演出待機を始める
            if (isStartReadyStandby)
            {
                isStartReadyStandby = false;
                isStandby = true;
            }
        }
    }

    /// <summary>
    /// 土煙エフェクトを再生する
    /// </summary>
    void PlayDustCloudEffect()
    {
        Vector3 pos = new Vector3(transform.position.y, 0.2f, transform.position.z);
        ParticleSystem p = Instantiate(dustCloud, pos, Quaternion.Euler(-90.0f, 0.0f, 0.0f));

        p.Play();
    }

    /// <summary>
    /// ダメージ量に加算する
    /// </summary>
    /// <param name="attackPower">プレイヤーの攻撃力</param>
    void AddDamageAmount(float attackPower)
    {
        //damageAmount += attackPower;
        //ダウン中はダウン用のダメージ量変数に加算しない
        if (isDowned)
        {
            return;
        }

        damageAmount += attackPower;


        //if (!isDowned)
        //{
        //    damageAmount += attackPower;
        //}
    }

    /// <summary>
    /// 敵1のHPを減らす
    /// </summary>
    /// <param name="attackPower">攻撃力</param>
    public void ReduceHp(float attackPower)
    {
        //死亡していたら処理しない
        if (canFight)
        {
            return;
        }

        //演出待機中は処理しない
        if(isStandby || isStartReadyStandby)
        {
            return;
        }

        hp -= attackPower;
        AddDamageAmount(attackPower);

        if(hp < 0)
        {
            hp = 0;
        }
    }

    /// <summary>
    /// 演出待機する
    /// </summary>
    public void Standby()
    {
        isStartReadyStandby = true;  //待機準備開始

        //攻撃中もしくは攻撃準備中
        if(isAttack || attackInterval <= 0)
        {
            //変数の初期化
            isAttack = false;
            attackInterval = maxAttackInterval;
            magicMissileCount = 1;
            evenCount = oddCount = 0;
            for (int i = 0; i < 2; ++i)
            {
                creationInterval[i] = 0.0f;
            }

            //弾の消去
            DestroyMagicMissile();

            //アニメーション設定
            myAnimator.SetBool("Attack", false);

            //演出待機開始
            isStartReadyStandby = false;
            isStandby = true;

            Debug.Log("攻撃または攻撃準備中");
        }
        else if(isDowned)
        {
            //降下待機中
            if (!canFall)
            {
                //変数の初期化
                isDowned = false;
                damageAmount = 0.0f;
                StopCoroutine(waitFall);

                //アニメーション設定
                myAnimator.SetBool("Down", false);
                myAnimator.SetBool("Fall", false);

                //演出待機開始
                isStartReadyStandby = false;
                isStandby = true;
                isWaitFall = true;

                Debug.Log("降下待機中");
            }
            //降下中
            else if (!isGround)
            {
                Debug.Log("降下中");
            }
            //ダウン中
            else
            {
                //変数の初期化
                isDowned = false;
                downedPos = transform.position;
                damageAmount = 0.0f;
                totalDownedTime = 0.0f;

                //定位置に戻す
                isWaitRise = true;

                //アニメーション設定
                myAnimator.SetBool("Rise", true);
                myAnimator.SetBool("Down", false);

                Debug.Log("ダウン中");
            }
        }
        //空中待機中
        else if(!isWaitRise && !isReturningNormalPos)
        {
            //変数の初期化
            attackInterval = maxAttackInterval;

            //演出待機開始
            isStartReadyStandby = false;
            isStandby = true;

            Debug.Log("空中待機中");
        }

        //エフェクト停止
        mist.Stop();
        trail.Stop();
        trailScript.GetSetIsPlay = false;

    }

    /// <summary>
    /// 演出待機を解除する
    /// </summary>
    public void CancelStandby()
    {
        isStandby = false;

        //初回以外は攻撃種類を決定
        if (isStartGame)
        {
            ChooseAttackType();
        }
        else
        {
            isStartGame = true;
        }

        //エフェクト再生
        mist.Play();

        damageAmount = 0.0f;
        canShoot = false;  //発射不可
        canBlowOff = false;//吹き飛ばし不可
        isFall = false;  //降下中でない
        isWaitFall = false;  //降下準備中でない
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    //ステージに当たったらダウンモーション再生
    //    if(collision.gameObject.tag == "Stage" && hp > 0)
    //    {
    //        myAnimator.SetBool("Down", true);
    //        myAnimator.SetBool("Fall", false);
    //    }
    //}
}