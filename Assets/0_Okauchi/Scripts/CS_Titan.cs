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
    [SerializeField, Header("一定の速度で方向追尾するときの値（一秒あたりの角度）")] 
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

    private void Awake()
    {
        hp = hpMax;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartMoving();  //テスト用（最終的にはシーンを管理するスクリプトから呼び出してもらう）
        //Okauchi
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
            case State.IDLE:   break;
            case State.WALK:   Walk();   break;
            case State.CHARGE: Charge(); break;
            case State.RUSH:   Rush();   break;
            case State.TURN:   Turn();   break;
            case State.STOP:   Stop();   break;
            case State.DOWN:   Down();   break;
            default: 
                break;
        }
    }

    //動き始める関数
    public void StartMoving()
    {
        StartWalk();
    }

    //------------------------
    //各Stateごとの関数
    //①各Stateを始める「Start～関数」
    //②毎フレームの処理を書いた「～関数」
    //※「～」は各State名
    //------------------------
    //↓↓↓↓↓↓↓↓↓↓↓↓
    //歩く（Walk）
    private void StartWalk()
    {
        state = State.WALK;
        attackIntervalCount = attackInterval;
        animator.SetTrigger("triggerWalk");
    }
    private void Walk()
    {
        attackIntervalCount -= Time.deltaTime;
        if (attackIntervalCount <= 0.0f && toTargetVector.magnitude <= attackReactionDistance)
        {
            StartCharge();
            return;
        }

        TrackGradualRotation();
        Transfer(walkSpeed);
    }
    //
    private void StartCharge()
    {
        state = State.CHARGE;
        chargeTimeCount = (float)Random.Range(chargeTimeMin, chargeTimeMax);
        rushPower = rushDefaultPower;
        rushSpeed = rushDefaultSpeed;
        animator.SetTrigger("triggerCharge");
    }

    private void Charge()
    {
        TrackConstantRotation();
        chargeTimeCount -= Time.deltaTime;
        rushPower += rushPowerChargingIncrement * Time.deltaTime;
        rushPower += rushPowerChargingIncrement * Time.deltaTime;
        if (chargeTimeCount <= 0.0f)
        {
            rushCount = Random.Range(rushCountMin, rushCountMax + 1);
            StartRush();
        }
    }

    private void StartRush()
    {
        state = State.RUSH;
        rushTimeCount = rushTime;
        animator.SetTrigger("triggerRush");
    }

    private void Rush()
    {
        Transfer(rushSpeed);
        rushTimeCount -= Time.deltaTime;
        if (rushTimeCount <= 0.0f)
        {
            rushCount--;
            if (rushCount <= 0)
            {
                StartStop();
                return;
            }
            StartTurn();
        }
    }

    private void StartTurn()
    {
        state = State.TURN;
        rushIntervalCount = rushInterval;
        animator.SetTrigger("triggerIdle");
    }

    private void Turn()
    {
        TrackConstantRotation();
        rushIntervalCount -= Time.deltaTime;
        if (rushIntervalCount <= 0.0f)
        {
            StartRush();
        }
    }

    private void StartStop()
    {
        state = State.STOP;
        rushIntervalCount = 0.0f;
        stoppingTimeCount = stoppingTime;
        animator.SetTrigger("triggerIdle");
    }

    private void Stop()
    {
        stoppingTimeCount -= Time.deltaTime;
        if(stoppingTimeCount <= 0.0f)
        {
            StartWalk();
        }
    }

    public void StartDown()
    {
        if (state == State.DOWN) return;

        state = State.DOWN;
        downTimeCount = downTime;
        animator.SetTrigger("triggerDown");
    }

    private void Down()
    {
        downTimeCount -= Time.deltaTime;
        if(downTimeCount <= 0.0f)
        {
            StartWalk();
        }
    }

    private void TrackGradualRotation()
    {
        Quaternion lookingRotation = Quaternion.LookRotation(toTargetVector);

        lookingRotation = Quaternion.Slerp(transform.rotation, lookingRotation, gradualTrackingValue * Time.deltaTime);
        transform.rotation = lookingRotation;
    }

    private void TrackConstantRotation()
    {
        float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(toTargetVector));
        Vector3 cross = Vector3.Cross(transform.forward, toTargetVector);
        float sign = (cross.y >= 0.0f) ? 1.0f : -1.0f;

        if (angle >= constantTrackingAngle * Time.deltaTime)
        {
            transform.rotation *= Quaternion.AngleAxis(sign * constantTrackingAngle * Time.deltaTime, Vector3.up);
        }
        else
        {
            transform.rotation *= Quaternion.AngleAxis(sign * angle, Vector3.up);
        }
    }

    private void Transfer(float speed)
    {
        transform.position += speed * transform.forward * Time.deltaTime;
    }

    public void ReceiveDamage(int damage)
    {
        hp -= damage;
        if (hp <= 0.0f)
        {
            hp = 0.0f;
        }
    }
}
