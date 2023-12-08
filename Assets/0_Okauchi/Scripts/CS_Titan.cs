using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Titan : MonoBehaviour
{
    //State
    private enum State
    {
        IDLE,    //����
        WALK,    //����
        CHARGE,  //����
        RUSH,    //�ːi
        TURN,    //�ːi���̕����]��
        STOP,    //��~
        DOWN,    //�_�E��
    }
    private State state = State.IDLE;

    //�A�j���[�^�[
    [SerializeField, Header("�A�j���[�^�[")]
    private Animator animator;

    //HP
    [SerializeField, Header("Hp�̍ő�l")] 
    private float hpMax = 0.0f;
    [SerializeField, Header("Hp")]
    private float hp = 0.0f;
    public float Hp{ get{ return hp; } }

    //�^�[�Q�b�g�i�v���C���[�j�̈ʒu���
    [SerializeField, Header("�^�[�Q�b�g�i�v���C���[�j�̈ʒu���")] 
    private Transform targetTransform;
    private Vector3 toTargetVector;     //���l����v���C���[�����̃x�N�g��

    //--------------------
    //����
    //--------------------
    [SerializeField, Header("�������x")] 
    private float walkSpeed = 0.0f;
    [SerializeField, Header("�ʂ���ƕ����ǔ�����Ƃ��̒l�i�������Ƃ������j")] 
    private float gradualTrackingValue = 0.0f;

    //--------------------
    //�ːi�U���S��
    //--------------------
    [SerializeField, Header("�ːi�U�����n�܂鋗��")] 
    private float attackReactionDistance = 0.0f;
    [SerializeField, Header("�ːi�U���S�̂̃C���^�[�o��")] 
    private float attackInterval = 0.0f;
    [SerializeField, Header("���̑��x�ŕ����ǔ�����Ƃ��̒l�i��b������̊p�x�j")] 
    private float constantTrackingAngle = 0.0f;
    private float attackIntervalCount = 0.0f;

    //--------------------
    //����
    //--------------------
    [SerializeField, Header("�ŏ����ߎ���")] 
    private int chargeTimeMin = 0;
    [SerializeField, Header("�ő嗭�ߎ���")] 
    private int chargeTimeMax = 0;
    private float chargeTimeCount = 0.0f;

    //�ːi���͎኱�ǔ�����̂��ːi�J�n���̃v���C���[�̈ʒu�ɓ˂����ނ�
    //�ǔ�����
    //�U���͂�HP��float

    //--------------------
    //�ːi
    //--------------------
    [SerializeField, Header("�ːi���̃C���^�[�o��")] 
    private float rushInterval = 0.0f;
    [SerializeField, Header("�ːi���̎���")] 
    private float rushTime = 0.0f;
    [SerializeField, Header("�ːi�̏����З�")] 
    private float rushDefaultPower = 0.0f;
    [SerializeField, Header("�ːi�̏������x")] 
    private float rushDefaultSpeed = 0.0f;
    [SerializeField, Header("���ߎ��Ԃɔ�Ⴕ�đ�����З͗�")] 
    private float rushPowerChargingIncrement = 0;
    [SerializeField, Header("���ߎ��Ԃɔ�Ⴕ�đ�����ːi���x")] 
    private float rushSpeedChargingIncrement = 0;
    [SerializeField, Header("�ŏ��ːi��")] 
    private int rushCountMin = 0;
    [SerializeField, Header("�ő�ːi��")] 
    private int rushCountMax = 0;
    private int rushCount = 0;
    private float rushTimeCount = 0.0f;
    private float rushPower = 0.0f;
    private float rushSpeed = 0.0f;
    private float rushIntervalCount = 0.0f;

    //--------------------
    //��~
    //--------------------
    [SerializeField, Header("��~����")] 
    private float stoppingTime = 0.0f;
    private float stoppingTimeCount = 0.0f;

    //--------------------
    //�_�E��
    //--------------------
    [SerializeField, Header("�_�E������")] 
    private float downTime = 0.0f;
    private float downTimeCount = 0.0f;

    private void Awake()
    {
        hp = hpMax;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartMoving();  //�e�X�g�p�i�ŏI�I�ɂ̓V�[�����Ǘ�����X�N���v�g����Ăяo���Ă��炤�j
        //Okauchi
    }

    // Update is called once per frame
    void Update()
    {
        //�v���C���[�����ւ̃x�N�g�����擾
        toTargetVector = targetTransform.position - transform.position;
        //y�����̒l�͎ז��Ȃ̂ō폜
        toTargetVector -= new Vector3(0.0f, toTargetVector.y, 0.0f);

        //State���Ƃ̏������s��
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

    //�����n�߂�֐�
    public void StartMoving()
    {
        StartWalk();
    }

    //------------------------
    //�eState���Ƃ̊֐�
    //�@�eState���n�߂�uStart�`�֐��v
    //�A���t���[���̏������������u�`�֐��v
    //���u�`�v�͊eState��
    //------------------------
    //������������������������
    //�����iWalk�j
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
