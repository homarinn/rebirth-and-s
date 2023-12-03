using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Titan : MonoBehaviour
{
    //状態
    private enum State
    {
        IDLE,
        WALK,
        CHARGE,
        RUSH,
        STOP,
        DOWN,
    }
    private State state = State.IDLE;

    private const float STOPPING_DISTANCE = 0.5f;

    //HP
    [SerializeField] private float hp = 0;

    //ターゲット（プレイヤー）の位置情報
    [SerializeField] private Transform targetTransform;   //変更不可な参照ってinspectorから設定できないのか
    private Vector3 toTargetVector;

    //--------------------
    //歩き
    //--------------------
    [SerializeField] private float walkSpeed = 0.0f;
    [SerializeField] private float gradualTrackingValue = 0.0f;

    //--------------------
    //突進攻撃全体
    //--------------------
    [SerializeField] private float attackReactionDistance = 0.0f;
    [SerializeField] private float attackInterval = 0.0f;
    [SerializeField] private float constantTrackingAngle = 0.0f;
    private float attackIntervalCount = 0.0f;

    //--------------------
    //溜め
    //--------------------
    [SerializeField] private int chargeTimeMin = 0;
    [SerializeField] private int chargeTimeMax = 0;
    private float chargeTimeCount = 0.0f;

    //突進中は若干追尾するのか突進開始時のプレイヤーの位置に突っ込むか
    //追尾無し
    //攻撃力とHPはfloat

    //--------------------
    //突進
    //--------------------
    [SerializeField] private float rushInterval = 0.0f;
    [SerializeField] private float rushTime = 0.0f;
    [SerializeField] private float rushDefaultPower = 0.0f;
    [SerializeField] private float rushDefaultSpeed = 0.0f;
    [SerializeField] private float rushPowerChargingIncrement = 0;
    [SerializeField] private float rushSpeedChargingIncrement = 0;
    [SerializeField] private int rushCountMin = 0;
    [SerializeField] private int rushCountMax = 0;
    private int rushCount = 0;
    private float rushTimeCount = 0.0f;
    private float rushPower = 0.0f;
    private float rushSpeed = 0.0f;
    private float rushIntervalCount = 0.0f;

    //--------------------
    //停止
    //--------------------
    [SerializeField] private float stoppingTime = 0.0f;
    private float stoppingTimeCount = 0.0f;

    //--------------------
    //ダウン
    //--------------------
    [SerializeField] private float downTime = 0.0f;
    private float downTimeCount = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        StartMoving();  //テスト用
    }

    // Update is called once per frame
    void Update()
    {
        toTargetVector = targetTransform.position - transform.position;
        toTargetVector -= new Vector3(0.0f, toTargetVector.y, 0.0f);

        switch (state)
        {
            case State.IDLE:   break;
            case State.WALK:   Walk();   break;
            case State.CHARGE: Charge(); break;
            case State.RUSH:   Rush();   break;
            case State.STOP:   Stop();   break;
            case State.DOWN:   Down();   break;
            default: 
                break;
        }
    }

    public void StartMoving()
    {
        StartWalk();
    }

    private void StartWalk()
    {
        state = State.WALK;
        attackIntervalCount = attackInterval;
    }

    private void Walk()
    {
        attackIntervalCount -= Time.deltaTime;
        if (attackIntervalCount <= 0.0f && toTargetVector.magnitude <= attackReactionDistance)
        {
            StartCharge();
        }

        if (toTargetVector.magnitude >= STOPPING_DISTANCE)
        {
            TrackGradualRotation();
            Transfer(walkSpeed);
        }
    }

    private void StartCharge()
    {
        state = State.CHARGE;
        chargeTimeCount = (float)Random.Range(chargeTimeMin, chargeTimeMax);
        rushPower = rushDefaultPower;
        rushSpeed = rushDefaultSpeed;
    }

    private void Charge()
    {
        TrackConstantRotation();
        chargeTimeCount -= Time.deltaTime;
        rushPower += rushPowerChargingIncrement * Time.deltaTime;
        rushPower += rushPowerChargingIncrement * Time.deltaTime;
        if (chargeTimeCount <= 0.0f)
        {
            StartRush();
        }
    }

    private void StartRush()
    {
        state = State.RUSH;
        rushPower = rushDefaultPower;
        rushSpeed = rushDefaultSpeed;
        rushCount = Random.Range(rushCountMin, rushCountMax + 1);
        rushTimeCount = rushTime;
    }

    private void Rush()
    {
        if(rushIntervalCount <= 0.0f)
        {
            Transfer(rushSpeed);
            rushTimeCount -= Time.deltaTime;
            if(rushTimeCount <= 0.0f)
            {
                rushIntervalCount = rushInterval;
                rushCount--;
            }
        }
        else
        {
            TrackConstantRotation();
            rushIntervalCount -= Time.deltaTime;
            if(rushIntervalCount <= 0.0f)
            {
                rushTimeCount = rushTime;
            }
        }

        if(rushCount <= 0)
        {
            StartStop();
        }
    }

    private void StartStop()
    {
        state = State.STOP;
        rushIntervalCount = 0.0f;
        stoppingTimeCount = stoppingTime;
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

        //テスト用
        transform.eulerAngles += new Vector3(90.0f, 0.0f, 0.0f);
    }

    private void Down()
    {
        downTimeCount -= Time.deltaTime;
        if(downTimeCount <= 0.0f)
        {
            StartWalk();
            //テスト用
            transform.eulerAngles -= new Vector3(90.0f, 0.0f, 0.0f);
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
