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
        DIE,     //���S
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
    [SerializeField, Header("���̑��x�ŕ����ǔ�����Ƃ��̉�]���x")] 
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
    [SerializeField, Header("��_���U�����ꂽ�Ƃ��̒ǉ��_���[�W")]
    private float weakPointDamageIncrement = 0.0f;

    //--------------------
    //SE
    //--------------------
    [SerializeField, Header("SE�F�ړ�")]
    private AudioSource moveSE;
    [SerializeField, Header("SE�F����")]
    private AudioSource chargeSE;
    [SerializeField, Header("SE�F�ːi�̏Փ�")]
    private AudioSource clashSE;


    private void Awake()
    {
        hp = hpMax;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartMoving();  //�e�X�g�p�i�ŏI�I�ɂ̓V�[�����Ǘ�����X�N���v�g����Ăяo���Ă��炤�j
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
            case State.IDLE:   break;             //����
            case State.WALK:   Walk();   break;   //����
            case State.CHARGE: Charge(); break;   //����
            case State.RUSH:   Rush();   break;   //�ːi
            case State.TURN:   Turn();   break;   //�ːi���̕����]��
            case State.STOP:   Stop();   break;   //��~
            case State.DOWN:   Down();   break;   //�_�E��
            case State.DIE:    break;
            default: 
                break;
        }
    }

    //�����n�߂�֐�
    public void StartMoving()
    {
        StartWalk();
    }

    //--------------------------------------------
    //�eState���Ƃ̊֐�
    //�@�eState���n�߂�uStart�`�֐��v
    //�A���t���[���̏������������u�`�֐��v
    //���u�`�v�͊eState��
    //--------------------------------------------
    //������������������������
    //----------------------------------
    //�����iWalk�j
    //----------------------------------
    private void StartWalk()
    {
        //State�ƃA�j���[�V�����̑J��
        state = State.WALK;
        animator.SetTrigger("triggerWalk");
        //������SE���Đ�
        moveSE.Play();
        //�U���̃C���^�[�o�������Z�b�g
        attackIntervalCount = attackInterval;
    }
    private void Walk()
    {
        //�U���̃C���^�[�o�����J�E���g
        attackIntervalCount -= Time.deltaTime;
        //�U���̃C���^�[�o�����I�� and �^�[�Q�b�g���U���͈͓��Ȃ�
        if (attackIntervalCount <= 0.0f && toTargetVector.magnitude <= attackReactionDistance)
        {
            moveSE.Stop();
            //���߂��n�߂�
            StartCharge();
            return;
        }

        //�ǔ�
        TrackGradualRotation();
        Transfer(walkSpeed);
    }
    //----------------------------------
    //���߁iCharge�j
    //----------------------------------
    private void StartCharge()
    {
        //State�ƃA�j���[�V�����̑J��
        state = State.CHARGE;
        animator.SetTrigger("triggerCharge");
        //���ߗp��SE���Đ�
        chargeSE.Play();
        //���ߎ��Ԃ������_���Ō���
        chargeTimeCount = (float)Random.Range(chargeTimeMin, chargeTimeMax);
        //�ːi�̈З͂Ƒ��x�����ɖ߂��Ă���
        rushPower = rushDefaultPower;
        rushSpeed = rushDefaultSpeed;
    }
    private void Charge()
    {
        //�����̒ǔ��i��葬�x�j
        TrackConstantRotation();
        //���ߎ��Ԃ��J�E���g
        chargeTimeCount -= Time.deltaTime;
        //���ߎ��Ԃɍ��킹�ĈЗ͂Ƒ��x���グ��
        rushPower += rushPowerChargingIncrement * Time.deltaTime;
        rushSpeed += rushSpeedChargingIncrement * Time.deltaTime;
        //���߂��I��������
        if (chargeTimeCount <= 0.0f)
        {
            //�ːi�J�n
            StartRush();
            //�ːi�̉񐔂������_���Ō���
            rushCount = Random.Range(rushCountMin, rushCountMax + 1);
        }
    }
    //----------------------------------
    //�ːi�iRush�j
    //----------------------------------
    private void StartRush()
    {
        //State�ƃA�j���[�V�����̑J��
        state = State.RUSH;
        animator.SetTrigger("triggerRush");
        //�ːi���Ԃ����Z�b�g
        rushTimeCount = rushTime;
    }
    private void Rush()
    {
        //�ړ�
        Transfer(rushSpeed);
        //�ːi���Ԃ��J�E���g
        rushTimeCount -= Time.deltaTime;
        //���̓ːi���I��������
        if (rushTimeCount <= 0.0f)
        {
            //�ːi�񐔂��J�E���g
            rushCount--;
            //�ːi���񐔕��s�����
            if (rushCount <= 0)
            {
                //��~����
                StartStop();
                return;
            }
            else
            {
                //�܂��񐔂��c���Ă���ꍇ�͍ēx�ːi���邽�߂ɕ����]�����n�߂�
                StartTurn();
            }
        }
    }
    //----------------------------------
    //�����]���iTurn�j
    //----------------------------------
    private void StartTurn()
    {
        //State�ƃA�j���[�V�����̑J��
        state = State.TURN;
        animator.SetTrigger("triggerIdle");
        //�ːi�̃C���^�[�o�������Z�b�g
        rushIntervalCount = rushInterval;
    }
    private void Turn()
    {
        //�����̒ǔ��i��葬�x�j
        TrackConstantRotation();
        //�ːi�̃C���^�[�o�����J�E���g
        rushIntervalCount -= Time.deltaTime;
        //�C���^�[�o�����I��������
        if (rushIntervalCount <= 0.0f)
        {
            //�ːi�J�n
            StartRush();
        }
    }
    //----------------------------------
    //��~�iStop�j
    //----------------------------------
    private void StartStop()
    {
        //State�ƃA�j���[�V�����̑J��
        state = State.STOP;
        animator.SetTrigger("triggerIdle");
        //��~���Ԃ����Z�b�g
        stoppingTimeCount = stoppingTime;
    }
    private void Stop()
    {
        //��~���Ԃ��J�E���g
        stoppingTimeCount -= Time.deltaTime;
        //��~���Ԃ��I��������
        if(stoppingTimeCount <= 0.0f)
        {
            //�ēx�����n�߂�
            StartWalk();
        }
    }
    //----------------------------------
    //�_�E���iDown�j
    //----------------------------------
    public void StartDown()
    {
        //State�ƃA�j���[�V�����̑J��
        state = State.DOWN;
        animator.SetTrigger("triggerDown");
        //�_�E�����Ԃ̃��Z�b�g
        downTimeCount = downTime;
    }
    private void Down()
    {
        //�_�E�����Ԃ̃J�E���g
        downTimeCount -= Time.deltaTime;
        //�_�E�����Ԃ��I��������
        if(downTimeCount <= 0.0f)
        {
            //�ēx�����n�߂�
            StartWalk();
        }
    }
    //----------------------------------
    //���S�iDie�j
    //----------------------------------
    public void StartDie()
    {
        //State�ƃA�j���[�V�����̑J��
        state = State.DIE;
        animator.SetTrigger("triggerDie");
    }
    //��������������������������
    //------------------------------------------------


    //------------------------------------------------
    //�U�����󂯂��Ƃ��̊֐�
    //�i�v���C���[���̏Փ˔��莞�ɌĂяo���Ă��炤�j
    //------------------------------------------------
    //�V���v���Ƀ_���[�W���󂯂�
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
    //��_�Ƀ_���[�W���󂯂����̏���
    public void ReceiveDamageOnWeakPoint(float damage)
    {
        ReceiveDamage(damage + weakPointDamageIncrement);

        //�_�E�����łȂ��ꍇ��
        if (state != State.DOWN)
        {
            //�_�E�����X�^�[�g������
            StartDown();
        }
    }

    //---------------------------------
    //�v���C���[�ɏՓ˂����ۂ̏���
    //---------------------------------
    private void OnCollisionEnter(Collision collision)
    {
        //�ːi���Ƀv���C���[�ɏՓ˂����ꍇ
        if(collision.gameObject.CompareTag("Player") && state == State.RUSH)
        {
            collision.gameObject.GetComponent<CS_Player>().Damage((int)rushPower);
            //�Փ˂����ۂ�SE���Đ�
            clashSE.Play();
        }
    }


    //--------------------------------------
    //���̑��̊֐�
    //--------------------------------------
    //�^�[�Q�b�g�̕�����ǔ�����֐��i�ʂ���Ɓj
    private void TrackGradualRotation()
    {
        //�^�[�Q�b�g�̕����������Ă���Ƃ��̎p��
        Quaternion lookingRotation = Quaternion.LookRotation(toTargetVector);

        //���݂̎p���ƃ^�[�Q�b�g�����̎p���Ő��`��Ԃ��A�Ԃ̎p�������߂�
        lookingRotation = Quaternion.Slerp(transform.rotation, lookingRotation, gradualTrackingValue * Time.deltaTime);
        transform.rotation = lookingRotation;
    }
    //�^�[�Q�b�g�̕�����ǔ�����֐��i��葬�x�j
    private void TrackConstantRotation()
    {
        //���݌����Ă���x�N�g������^�[�Q�b�g�����̃x�N�g���܂ł̊p�x�����߂�
        float angle = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(toTargetVector));
        //���݌����Ă���x�N�g���ƃ^�[�Q�b�g�����̃x�N�g���ŊO�ς��Ƃ�A��]��������������肷��
        Vector3 cross = Vector3.Cross(transform.forward, toTargetVector);
        float sign = (cross.y >= 0.0f) ? 1.0f : -1.0f;

        //�����p�x���K��̉�]�ʂ�菬�����ꍇ��
        if (angle <= constantTrackingAngle * Time.deltaTime)
        {
            //�p�x��������]������
            transform.rotation *= Quaternion.AngleAxis(sign * angle, Vector3.up);
        }
        else
        {
            //�傫���ꍇ�͋K��̗ʉ�]������
            transform.rotation *= Quaternion.AngleAxis(sign * constantTrackingAngle * Time.deltaTime, Vector3.up);
        }
    }
    //���ʕ����Ɉړ�
    private void Transfer(float speed)
    {
        transform.position += speed * transform.forward * Time.deltaTime;
    }
}
