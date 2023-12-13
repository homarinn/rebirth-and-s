using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Titan : MonoBehaviour
{
    //State
    private enum State
    {
        IDLE,    //初期
        WALK,    //歩く
        CHARGE,  //溜め
        RUSH,    //突進
        TURN,    //突進時の方向転換
        STOP,    //停止
        DOWN,    //ダウン
        DIE,     //死亡
    }
    private State state = State.IDLE;

    //アニメーター
    [SerializeField, Header("アニメーター")]
    private Animator animator;

    //HP
    [SerializeField, Header("Hpの最大値")] 
    private float hpMax = 0.0f;
    [SerializeField, Header("Hp")]
    private float hp = 0.0f;
    public float Hp{ get{ return hp; } }

    //ターゲット（プレイヤー）の位置情報
    [SerializeField, Header("ターゲット（プレイヤー）の位置情報")] 
    private Transform targetTransform;
    private Vector3 toTargetVector;     //巨人からプレイヤー方向のベクトル

    //--------------------
    //歩き
    //--------------------
    [SerializeField, Header("歩き速度")] 
    private float walkSpeed = 0.0f;
    [SerializeField, Header("ぬるっと方向追尾するときの値（小さいとゆっくり）")] 
    private float gradualTrackingValue = 0.0f;

    //--------------------
    //突進攻撃全体
    //--------------------
    [SerializeField, Header("突進攻撃が始まる距離")] 
    private float attackReactionDistance = 0.0f;
    [SerializeField, Header("突進攻撃全体のインターバル")] 
    private float attackInterval = 0.0f;
    [SerializeField, Header("一定の速度で方向追尾するときの回転速度")] 
    private float constantTrackingAngle = 0.0f;
    private float attackIntervalCount = 0.0f;

    //--------------------
    //溜め
    //--------------------
    [SerializeField, Header("最小溜め時間")] 
    private int chargeTimeMin = 0;
    [SerializeField, Header("最大溜め時間")] 
    private int chargeTimeMax = 0;
    private float chargeTimeCount = 0.0f;

    //突進中は若干追尾するのか突進開始時のプレイヤーの位置に突っ込むか
    //追尾無し
    //攻撃力とHPはfloat

    //--------------------
    //突進
    //--------------------
    [SerializeField, Header("突進一回のインターバル")] 
    private float rushInterval = 0.0f;
    [SerializeField, Header("突進一回の時間")] 
    private float rushTime = 0.0f;
    [SerializeField, Header("突進の初期威力")] 
    private float rushDefaultPower = 0.0f;
    [SerializeField, Header("突進の初期速度")] 
    private float rushDefaultSpeed = 0.0f;
    [SerializeField, Header("溜め時間に比例して増える威力量")] 
    private float rushPowerChargingIncrement = 0;
    [SerializeField, Header("溜め時間に比例して増える突進速度")] 
    private float rushSpeedChargingIncrement = 0;
    [SerializeField, Header("最小突進回数")] 
    private int rushCountMin = 0;
    [SerializeField, Header("最大突進回数")] 
    private int rushCountMax = 0;
    private int rushCount = 0;
    private float rushTimeCount = 0.0f;
    private float rushPower = 0.0f;
    private float rushSpeed = 0.0f;
    private float rushIntervalCount = 0.0f;

    //--------------------
    //停止
    //--------------------
    [SerializeField, Header("停止時間")] 
    private float stoppingTime = 0.0f;
    private float stoppingTimeCount = 0.0f;

    //--------------------
    //ダウン
    //--------------------
    [SerializeField, Header("ダウン時間")] 
    private float downTime = 0.0f;
    private float downTimeCount = 0.0f;
    [SerializeField, Header("弱点を攻撃されたときの追加ダメージ")]
    private float weakPointDamageIncrement = 0.0f;

    //--------------------
    //SE
    //--------------------
    [SerializeField, Header("SE：移動")]
    private AudioSource moveSE;
    [SerializeField, Header("SE：溜め")]
    private AudioSource chargeSE;
    [SerializeField, Header("SE：突進の衝突")]
    private AudioSource clashSE;


    private void Awake()
    {
        hp = hpMax;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartMoving();  //テスト用（最終的にはシーンを管理するスクリプトから呼び出してもらう）
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤー方向へのベクトルを取得
        toTargetVector = targetTransform.position - transform.position;
        //y方向の値は邪魔なので削除
        toTargetVector -= new Vector3(0.0f, toTargetVector.y, 0.0f);

        //Stateごとの処理を行う
        switch (state)
        {
            case State.IDLE:   break;             //初期
            case State.WALK:   Walk();   break;   //歩く
            case State.CHARGE: Charge(); break;   //溜め
            case State.RUSH:   Rush();   break;   //突進
            case State.TURN:   Turn();   break;   //突進時の方向転換
            case State.STOP:   Stop();   break;   //停止
            case State.DOWN:   Down();   break;   //ダウン
            case State.DIE:    break;
            default: 
                break;
        }
    }

    //動き始める関数
    public void StartMoving()
    {
        StartWalk();
    }

    //--------------------------------------------
    //各Stateごとの関数
    //①各Stateを始める「Start～関数」
    //②毎フレームの処理を書いた「～関数」
    //※「～」は各State名
    //--------------------------------------------
    //↓↓↓↓↓↓↓↓↓↓↓↓
    //----------------------------------
    //歩く（Walk）
    //----------------------------------
    private void StartWalk()
    {
        //Stateとアニメーションの遷移
        state = State.WALK;
        animator.SetTrigger("triggerWalk");
        //歩きのSEを再生
        moveSE.Play();
        //攻撃のインターバルをリセット
        attackIntervalCount = attackInterval;
    }
    private void Walk()
    {
        //攻撃のインターバルをカウント
        attackIntervalCount -= Time.deltaTime;
        //攻撃のインターバルが終了 and ターゲットが攻撃範囲内なら
        if (attackIntervalCount <= 0.0f && toTargetVector.magnitude <= attackReactionDistance)
        {
            moveSE.Stop();
            //溜めを始める
            StartCharge();
            return;
        }

        //追尾
        TrackGradualRotation();
        Transfer(walkSpeed);
    }
    //----------------------------------
    //溜め（Charge）
    //----------------------------------
    private void StartCharge()
    {
        //Stateとアニメーションの遷移
        state = State.CHARGE;
        animator.SetTrigger("triggerCharge");
        //溜め用のSEを再生
        chargeSE.Play();
        //溜め時間をランダムで決定
        chargeTimeCount = (float)Random.Range(chargeTimeMin, chargeTimeMax);
        //突進の威力と速度を元に戻しておく
        rushPower = rushDefaultPower;
        rushSpeed = rushDefaultSpeed;
    }
    private void Charge()
    {
        //向きの追尾（一定速度）
        TrackConstantRotation();
        //溜め時間をカウント
        chargeTimeCount -= Time.deltaTime;
        //溜め時間に合わせて威力と速度を上げる
        rushPower += rushPowerChargingIncrement * Time.deltaTime;
        rushSpeed += rushSpeedChargingIncrement * Time.deltaTime;
        //溜めが終了したら
        if (chargeTimeCount <= 0.0f)
        {
            //突進開始
            StartRush();
            //突進の回数をランダムで決定
            rushCount = Random.Range(rushCountMin, rushCountMax + 1);
        }
    }
    //----------------------------------
    //突進（Rush）
    //----------------------------------
    private void StartRush()
    {
        //Stateとアニメーションの遷移
        state = State.RUSH;
        animator.SetTrigger("triggerRush");
        //突進時間をリセット
        rushTimeCount = rushTime;
    }
    private void Rush()
    {
        //移動
        Transfer(rushSpeed);
        //突進時間をカウント
        rushTimeCount -= Time.deltaTime;
        //一回の突進が終了したら
        if (rushTimeCount <= 0.0f)
        {
            //突進回数をカウント
            rushCount--;
            //突進が回数分行われると
            if (rushCount <= 0)
            {
                //停止する
                StartStop();
                return;
            }
            else
            {
                //まだ回数が残っている場合は再度突進するために方向転換を始める
                StartTurn();
            }
        }
    }
    //----------------------------------
    //方向転換（Turn）
    //----------------------------------
    private void StartTurn()
    {
        //Stateとアニメーションの遷移
        state = State.TURN;
        animator.SetTrigger("triggerIdle");
        //突進のインターバルをリセット
        rushIntervalCount = rushInterval;
    }
    private void Turn()
    {
        //方向の追尾（一定速度）
        TrackConstantRotation();
        //突進のインターバルをカウント
        rushIntervalCount -= Time.deltaTime;
        //インターバルが終了したら
        if (rushIntervalCount <= 0.0f)
        {
            //突進開始
            StartRush();
        }
    }
    //----------------------------------
    //停止（Stop）
    //----------------------------------
    private void StartStop()
    {
        //Stateとアニメーションの遷移
        state = State.STOP;
        animator.SetTrigger("triggerIdle");
        //停止時間をリセット
        stoppingTimeCount = stoppingTime;
    }
    private void Stop()
    {
        //停止時間をカウント
        stoppingTimeCount -= Time.deltaTime;
        //停止時間が終了したら
        if(stoppingTimeCount <= 0.0f)
        {
            //再度歩き始める
            StartWalk();
        }
    }
    //----------------------------------
    //ダウン（Down）
    //----------------------------------
    public void StartDown()
    {
        //Stateとアニメーションの遷移
        state = State.DOWN;
        animator.SetTrigger("triggerDown");
        //ダウン時間のリセット
        downTimeCount = downTime;
    }
    private void Down()
    {
        //ダウン時間のカウント
        downTimeCount -= Time.deltaTime;
        //ダウン時間が終了したら
        if(downTimeCount <= 0.0f)
        {
            //再度歩き始める
            StartWalk();
        }
    }
    //----------------------------------
    //死亡（Die）
    //----------------------------------
    public void StartDie()
    {
        //Stateとアニメーションの遷移
        state = State.DIE;
        animator.SetTrigger("triggerDie");
    }
    //↑↑↑↑↑↑↑↑↑↑↑↑↑
    //------------------------------------------------


    //------------------------------------------------
    //攻撃を受けたときの関数
    //（プレイヤー側の衝突判定時に呼び出してもらう）
    //------------------------------------------------
    //シンプルにダメージを受ける
    public void ReceiveDamage(float damage)
    {
        hp -= damage;
        if (hp <= 0.0f)
        {
            if(state != State.DIE)
            {
                StartDie();
            }
            hp = 0.0f;
        }
    }
    //弱点にダメージを受けた時の処理
    public void ReceiveDamageOnWeakPoint(float damage)
    {
        ReceiveDamage(damage + weakPointDamageIncrement);

        //ダウン中でない場合は
        if (state != State.DOWN)
        {
            //ダウンをスタートさせる
            StartDown();
        }
    }

    //---------------------------------
    //プレイヤーに衝突した際の処理
    //---------------------------------
    private void OnCollisionEnter(Collision collision)
    {
        //突進中にプレイヤーに衝突した場合
        if(collision.gameObject.CompareTag("Player") && state == State.RUSH)
        {
            collision.gameObject.GetComponent<CS_Player>().Damage((int)rushPower);
            //衝突した際のSEを再生
            clashSE.Play();
        }
    }


    //--------------------------------------
    //その他の関数
    //--------------------------------------
    //ターゲットの方向を追尾する関数（ぬるっと）
    private void TrackGradualRotation()
    {
        //ターゲットの方向を向いているときの姿勢
        Quaternion lookingRotation = Quaternion.LookRotation(toTargetVector);

        //現在の姿勢とターゲット方向の姿勢で線形補間し、間の姿勢を求める
        lookingRotation = Quaternion.Slerp(transform.rotation, lookingRotation, gradualTrackingValue * Time.deltaTime);
        transform.rotation = lookingRotation;
    }
    //ターゲットの方向を追尾する関数（一定速度）
    private void TrackConstantRotation()
    {
        //現在向いているベクトルからターゲット方向のベクトルまでの角度を求める
        float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(toTargetVector));
        //現在向いているベクトルとターゲット方向のベクトルで外積をとり、回転させる方向を決定する
        Vector3 cross = Vector3.Cross(transform.forward, toTargetVector);
        float sign = (cross.y >= 0.0f) ? 1.0f : -1.0f;

        //もし角度が規定の回転量より小さい場合は
        if (angle <= constantTrackingAngle * Time.deltaTime)
        {
            //角度分だけ回転させる
            transform.rotation *= Quaternion.AngleAxis(sign * angle, Vector3.up);
        }
        else
        {
            //大きい場合は規定の量回転させる
            transform.rotation *= Quaternion.AngleAxis(sign * constantTrackingAngle * Time.deltaTime, Vector3.up);
        }
    }
    //正面方向に移動
    private void Transfer(float speed)
    {
        transform.position += speed * transform.forward * Time.deltaTime;
    }
}
