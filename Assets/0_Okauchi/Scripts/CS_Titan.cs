using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CS_Titan : MonoBehaviour
{
    //State
    private enum State
    {
        IDLE,       //����
        WALK,       //����
        CHARGE,     //����
        RUSH,       //�ːi
        TURN,       //�ːi���̕����]��
        STOP,       //�ːi��̒�~
        DOWN,       //�_�E��
        DIE,        //���S
    }
    private State state = State.IDLE;

    //�A�j���[�^�[
    [SerializeField, Header("�A�j���[�^�[")]
    private Animator animator;
    [SerializeField, Header("���S�A�j���[�V�����N���b�v")]
    private AnimationClip dieClip;

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
    [SerializeField, Header("�ŏ����ߎ��ԁi2�b�����̓o�O�邩���j")] 
    private int chargeTimeMin = 0;
    [SerializeField, Header("�ő嗭�ߎ���")] 
    private int chargeTimeMax = 0;
    private float chargeTimeCount = 0.0f;
    private float chargeTime = 0.0f;
    [SerializeField, Header("���ߒ��̃_���[�W�J�b�g���i0.0�`1.0�j")]
    private float damageCutPercentage = 0.0f;
    //�A�j���[�V�������x�̒���
    private const float animationSpeedChangingTime = 0.3f;
    private const float animationSpeedResetRemainingTime = 0.633f;
    private bool changedAnimationSpeed = false;

    //�ːi���͎኱�ǔ�����̂��ːi�J�n���̃v���C���[�̈ʒu�ɓ˂����ނ�
    //�ǔ�����
    //�U���͂�HP��float

    //--------------------
    //�ːi
    //--------------------
    private enum RushState
    {
        SPEED_UP,    //���x�㏸
        SPEED_MAX,   //���x�ő�
        SPEED_DOWN,  //���x����
        FINISH,      //�I��
    }
    private RushState rushState = RushState.FINISH;
    [SerializeField, Header("�ːi���̃C���^�[�o��")] 
    private float rushInterval = 0.0f;
    [SerializeField, Header("�ːi�̎������ԁi�ő呬�x�̎��ԁj")] 
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
    [SerializeField, Header("���x�̌W���i���x�㏸���j")]
    private float rushSpeedUpCoefficient = 0.0f;
    [SerializeField, Header("���x�̌W���i���x�������j")]
    private float rushSpeedDownCoefficient = 0.0f;
    private float rushSpeedLimit = 0.0f;

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
    [SerializeField, Header("�_�E���ォ������n�߂�܂ł̎���")]
    private float afterDownTime = 0.0f;
    private float afterDownTimeCount = 0.0f;
    //�R���C�_�[������p
    private CapsuleCollider collider;
    private const float colliderChangingTime = 1.0f;
    private float colliderRadius = 0.0f;
    private float colliderHeight = 0.0f;
    private Vector3 downColliderCenter = new Vector3(0.4f, 0.44f, 0.05f);
    private const float downColliderRadius = 0.44f;
    private const float downColliderHeight = 0.5f;

    //--------------------
    //���S
    //--------------------
    private float dieTimeCount = 0.0f;
    [SerializeField, Header("���S����ɂ���܂ł̎���")]
    private float dieTime = 0.0f; 

    //--------------------
    //SE
    //--------------------
    [SerializeField, Header("SE�F�ړ�")]
    private AudioSource moveSE;
    [SerializeField, Header("SE�F����")]
    private AudioSource chargeSE;
    private const float chargeSETime = 4.3f;
    [SerializeField, Header("SE�F�ːi�̏Փ�")]
    private AudioSource clashSE;

    //--------------------
    //�G�t�F�N�g
    //--------------------
    [SerializeField, Header("���߃G�t�F�N�g�i�n�ʂ���o�Ă��j")]
    private GameObject chargeEffectGround;
    [SerializeField, Header("���߃G�t�F�N�g�i�V�[���h���ۂ���j")]
    private GameObject chargeEffectShield;
    [SerializeField, Header("�ːi�G�t�F�N�g�i�Ռ��g���ۂ���j")]
    private GameObject rushEffect;
    [SerializeField, Header("�ːi�G�t�F�N�g�̐���y���W")]
    private float rushEffectPositionY;
    [SerializeField, Header("�ːi�G�t�F�N�g�̐������ԊԊu")]
    private float rushEffectGeneratingSpan;
    private float rushEffectTimeCount;
    [SerializeField, Header("�v���C���[�ɑ΂���q�b�g�G�t�F�N�g")]
    private GameObject hitEffect;
    [SerializeField, Header("��_�ɍU�����ꂽ���̃q�b�g�G�t�F�N�g")]
    private GameObject weaknessEffect;
    [SerializeField, Header("��_�̃R���C�_�[���W")]
    private Transform weaknessTransform;

    //���S���܂���
    public bool isDead = false;


    private void Awake()
    {
        hp = hpMax;
    }

    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<CapsuleCollider>();

        if(SceneManager.GetActiveScene().name == "OkauchiScene")
        {
            StartMoving();  //�e�X�g�p�i�ŏI�I�ɂ̓V�[�����Ǘ�����X�N���v�g����Ăяo���Ă��炤�j
        }

        colliderRadius = collider.radius;
        colliderHeight = collider.height;
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
            case State.STOP:   Stop();   break;   //�ːi��̒�~
            case State.DOWN:   Down();   break;   //�_�E��
            case State.DIE:    Die();    break;
            default: 
                break;
        }
    }

    //�����n�߂�֐�
    public void StartMoving()
    {
        StartWalk();
    }

    //�~�߂�֐�
    public void StopMoving()
    {
        StartIdle();
    }

    //--------------------------------------------
    //�eState���Ƃ̊֐�
    //�@�eState���n�߂�uStart�`�֐��v
    //�A���t���[���̏������������u�`�֐��v
    //���u�`�v�͊eState��
    //--------------------------------------------
    //������������������������
    //----------------------------------
    //������ԁiIdle�j
    //----------------------------------
    private void StartIdle()
    {
        state = State.IDLE;
        animator.SetTrigger("triggerIdle");
    }
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
        //���ߎ��Ԃ������_���Ō���
        chargeTime = (float)Random.Range(chargeTimeMin, chargeTimeMax);
        chargeTimeCount = chargeTime;
        //���ߗp��SE���Đ�
        chargeSE.pitch = chargeSETime / chargeTime;
        chargeSE.Play();
        //�ːi�̈З͂Ƒ��x�����ɖ߂��Ă���
        rushPower = rushDefaultPower;
        rushSpeedLimit = rushDefaultSpeed;
        //�G�t�F�N�g����
        GenerateChargeEffect(chargeTime);
    }
    private void Charge()
    {
        //�����̒ǔ��i��葬�x�j
        TrackConstantRotation();
        //���ߎ��Ԃ��J�E���g
        chargeTimeCount -= Time.deltaTime;
        //���ߎ��Ԃɍ��킹�ĈЗ͂Ƒ��x���グ��
        rushPower += rushPowerChargingIncrement * Time.deltaTime;
        rushSpeedLimit += rushSpeedChargingIncrement * Time.deltaTime;
        //�A�j���[�V�������x�𒲐�
        if(chargeTime - chargeTimeCount >= animationSpeedChangingTime && chargeTime - chargeTimeCount < 1.0f && !changedAnimationSpeed)
        {
            float chargeRemainingTime = chargeTimeCount - animationSpeedResetRemainingTime;
            float animationChargeRemainingTime = 1.0f - animationSpeedChangingTime;
            animator.speed = animationChargeRemainingTime / chargeRemainingTime;
            changedAnimationSpeed = true;
        }
        if(chargeTimeCount <= animationSpeedResetRemainingTime && changedAnimationSpeed)
        {
            animator.speed = 1.0f;
            changedAnimationSpeed = false;
        }
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
        rushTimeCount = 0.0f;
        //���x�����Z�b�g
        rushSpeed = 0.0f;
        rushState = RushState.SPEED_UP;
        rushEffectTimeCount = 0;
    }
    private void Rush()
    {
        //�ːi���Ԃ��J�E���g
        rushTimeCount += Time.deltaTime;
        //�ːi���̑��x�𒲐�
        switch(rushState)
        {
            case RushState.SPEED_UP:
                rushSpeed = rushSpeedUpCoefficient * Mathf.Pow(rushTimeCount, 3.0f);
                if (rushSpeed > rushSpeedLimit)
                {
                    rushSpeed = rushSpeedLimit;
                    rushTimeCount = 0.0f;
                    rushState = RushState.SPEED_MAX;
                }
                break;
            case RushState.SPEED_MAX:
                if(rushTimeCount >= rushTime)
                {
                    float speedDownTime = Mathf.Pow(rushSpeed / rushSpeedDownCoefficient, 1.0f / 3.0f);
                    rushTimeCount = -speedDownTime;
                    rushState = RushState.SPEED_DOWN;
                }
                //�G�t�F�N�g����
                rushEffectTimeCount -= Time.deltaTime;
                if (rushEffectTimeCount <= 0.0f)
                {
                    GenerateRushEffect();
                    rushEffectTimeCount = rushEffectGeneratingSpan;
                }
                break;
            case RushState.SPEED_DOWN:
                rushSpeed = -1.0f * rushSpeedDownCoefficient * Mathf.Pow(rushTimeCount, 3.0f);
                if (rushTimeCount >= 0.0f)
                {
                    rushSpeed = 0.0f;
                    rushState = RushState.FINISH;
                }
                break;
            default: 
                break;
        }
        //�ړ�
        Transfer(rushSpeed);
        //�ːi��񕪂��I�������Ƃ��̏���
        if (rushState == RushState.FINISH)
        {
            //�ːi�񐔂��J�E���g
            rushCount--;
            //�ːi���񐔕��s�����
            if (rushCount <= 0)
            {
                //��~����
                StartStop();
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
        moveSE.Stop();
    }
    private void Down()
    {
        if(downTimeCount >= 0.0f)
        {
            //�_�E�����Ԃ̃J�E���g
            downTimeCount -= Time.deltaTime;
            //collider�̒���
            if (downTime - downTimeCount >= colliderChangingTime)
            {
                SetDownColliderParameter();
            }
            //�_�E�����Ԃ��I��������
            if (downTimeCount < 0.0f)
            {
                animator.SetTrigger("triggerIdle");
                afterDownTimeCount = afterDownTime;

                //collider�߂�
                SetDefaultColliderParameter();
            }
        }
        else
        {
            //�_�E����̎��ԃJ�E���g
            afterDownTimeCount -= Time.deltaTime;
            if (afterDownTimeCount <= 0.0f)
            {
                StartWalk();
            }
        }
    }
    //----------------------------------
    //���S�iDie�j
    //----------------------------------
    private void StartDie()
    {
        //State�ƃA�j���[�V�����̑J��
        state = State.DIE;
        animator.SetTrigger("triggerDie");
        moveSE.Stop();
    }
    private void Die()
    {
        dieTimeCount += Time.deltaTime;
        if (dieTimeCount >= colliderChangingTime)
        {
            SetDownColliderParameter();
        }
        if(dieTimeCount >= dieTime)
        {
            isDead = true;
            UnityEditor.EditorApplication.isPaused = true;
        }
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
        if(state == State.CHARGE)
        {
            damage *= 1.0f - damageCutPercentage;
        }
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
    public void ReceiveDamageOnWeakPoint()
    {
        if (state == State.CHARGE) return;
        ReceiveDamage(weakPointDamageIncrement);
        //�G�t�F�N�g
        GameObject effect = Instantiate(weaknessEffect, weaknessTransform.position, Quaternion.identity);

        //�_�E����or���S���łȂ��ꍇ��
        if (state != State.DOWN && state != State.DIE)
        {
            //�_�E�����X�^�[�g������
            StartDown();
        }
    }

    //----------------------------------
    //�G�t�F�N�g�𐶐�����
    //----------------------------------
    private void GenerateChargeEffect(float _chargeTime)
    {
        //�n�ʂ���o�Ă���
        //����
        //GameObject effectGround = Instantiate(chargeEffectGround, transform.position, Quaternion.identity) as GameObject;
        ////�q���擾
        //int childCount = effectGround.transform.childCount;
        //ParticleSystem[] psGround = new ParticleSystem[childCount];
        ////���ԕύX
        //for(int i = 0; i < childCount; i++)
        //{
        //    psGround[i] = effectGround.transform.GetChild(i).gameObject.GetComponent<ParticleSystem>();
        //    psGround[i].Stop();
        //    var main = psGround[i].main;
        //    main.duration = _chargeTime;
        //    psGround[i].Play();
        //}

        //�V�[���h���ۂ����
        GameObject effectShield = Instantiate(chargeEffectShield, transform.position, Quaternion.identity) as GameObject;
        int childCount = effectShield.transform.childCount;
        ParticleSystem[] psShield = new ParticleSystem[childCount + 1];
        psShield[0] = effectShield.GetComponent<ParticleSystem>();
        for (int i = 1; i < childCount + 1; i++)
        {
            psShield[i] = effectShield.transform.GetChild(i - 1).gameObject.GetComponent<ParticleSystem>();
        }
        //���ԕύX
        for (int i = 0; i < childCount + 1; i++)
        {
            psShield[i].Stop();
            var main = psShield[i].main;
            if(i == 0)
            {
                main.startLifetime = _chargeTime - 0.2f;
            }
            else if(i == childCount)
            {
                main.duration = _chargeTime - 1.0f;
            }
            else
            {
                main.startLifetime = _chargeTime;
            }
            psShield[i].Play();
        }
    }

    private void GenerateRushEffect()
    {
        //����
        Vector3 generatingPosition = new Vector3(transform.position.x, rushEffectPositionY, transform.position.z);
        float generatingRotationY = -1.0f * transform.rotation.eulerAngles.y * Mathf.Deg2Rad;
        GameObject effect = Instantiate(rushEffect, generatingPosition, Quaternion.identity) as GameObject;
        //�p�x����
        ParticleSystem ps = effect.GetComponent<ParticleSystem>();
        ps.Stop();
        var main = ps.main;
        main.startRotationY = generatingRotationY;
        main.startRotationYMultiplier = generatingRotationY;
        ps.Play();
    }

    //---------------------------------
    //�v���C���[�ɏՓ˂����ۂ̏���
    //---------------------------------
    private void OnCollisionEnter(Collision collision)
    {
        //�ːi���Ƀv���C���[�ɏՓ˂����ꍇ
        if(collision.gameObject.CompareTag("Player") && state == State.RUSH)
        {
            collision.gameObject.GetComponent<CS_Player>().ReceiveDamage((int)rushPower);
            //�Փ˂����ۂ�SE���Đ�
            clashSE.Play();
            //�G�t�F�N�g
            if(collision.contacts[0].point != null)
            {
                GameObject effect = Instantiate(hitEffect, collision.contacts[0].point, Quaternion.identity);
            }
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
    //collider�����p
    private void SetDefaultColliderParameter()
    {
        collider.center = new Vector3(0.0f, colliderHeight / 2.0f, 0.0f);
        collider.radius = colliderRadius;
        collider.height = colliderHeight;
    }
    private void SetDownColliderParameter()
    {
        collider.center = downColliderCenter;
        collider.radius = downColliderRadius;
        collider.height = downColliderHeight;
    }

    /// <summary>
    /// �Đ����̃A�j���[�V�������擾����
    /// </summary>
    /// <param name="animator"> �A�j���[�^�[ </param>
    /// <returns> �Đ����̃A�j���[�V���� </returns>
    private AnimatorStateInfo GetCurrentAnim(Animator animator, int layerIndex = 0)
    {
        // �Đ����̃A�j���[�V����
        return animator.GetCurrentAnimatorStateInfo(layerIndex);
    }

    /// <summary>
    /// �w�肵���A�j���[�V�������Đ������m�F����
    /// </summary>
    /// <param name="animator"> �A�j���[�^�[ </param>
    /// <param name="anim"> �m�F����A�j���[�V���� </param>
    /// <returns>
    /// <para> true : �Đ��� </para>
    /// <para> false : �Đ����Ă��Ȃ� </para>
    /// </returns>
    private bool IsPlayingAnim(Animator animator, AnimationClip anim)
    {
        // �Đ����̃A�j���[�V����
        var currentAnim = GetCurrentAnim(animator);

        // �Đ����̃A�j���[�V�����̖��O��
        // �w�肵�����O�ƈ�v����Ȃ�true
        return currentAnim.IsName(anim.name);
    }
}
