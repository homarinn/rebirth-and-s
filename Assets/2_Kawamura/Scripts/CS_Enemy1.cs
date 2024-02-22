using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CS_Enemy1 : MonoBehaviour
{
    //�U���̎��
    enum AttackType
    {
        Weak,     //��U��
        Strong,   //���U��
        BlowOff,  //������΂��U��
    }
    AttackType attackType;

    [Header("�v���C���[")]
    [SerializeField] GameObject player;

    [Header("<=====�e=====>")]
    [Header("��")]
    [SerializeField] GameObject weakMagicMissile;

    [Header("��")]
    [SerializeField] GameObject strongMagicMissile;
    [Header("==========")]

    [Header("������΂��G�t�F�N�g")]
    [SerializeField] CS_Enemy1BlowOffEffect blowOffEffect;

    [Header("�X�e�[�W���E�~�X�N���v�g")]
    [SerializeField] CS_StageBoundary stageBoundary;

    [Header("�ő�HP")]
    [SerializeField] float maxHp;

    [Header("��]���x")]
    [SerializeField] float rotateSpeed;

    [Header("�U���C���^�[�o���i�b�j")]
    [SerializeField] float maxAttackInterval;

    [Header("�e�������̔��~���a")]
    [SerializeField] float halfCircleRadius;

    [Header("�e�̐����ʒu�i������j")]
    [SerializeField] Vector3 magicMissileSpawnPos = new Vector3(0.0f, 0.1f, 1.0f);

    [Header("<=====�U�������m���i%�j=====>")]
    [Header("��")]
    [SerializeField] float weakAttackProbability;   

    [Header("��")]
    [SerializeField] float strongAttackProbability; 

    [Header("������΂�")]
    [SerializeField] float blowOffAttackProbability;

    [Header("<=====�e�̈ړ����x=====>")]
    [Header("��")]
    [SerializeField] float weakMoveSpeed;

    [Header("��")]
    [SerializeField] float strongMoveSpeed;

    [Header("<=====�e�̈З�=====>")]
    [Header("��")]
    [SerializeField] float weakAttackPower;

    [Header("��")]
    [SerializeField] float strongAttackPower;

    [Header("<=====�����e���i����̂݁j=====>")]
    [Header("��")]
    [SerializeField] int weakMagicMissileNumber;

    [Header("��")]
    [SerializeField] int strongMagicMissileNumber;

    [Header("<=====�����Ԋu�i�b�j=====>")]
    [Header("��")]
    [SerializeField] float weakCreationInterval;  

    [Header("��")]
    [SerializeField] float strongCreationInterval;

    [Header("<=====���ˊԊu�i�b�j=====>")]
    [Header("��")]
    [SerializeField] float weakShootInterval;   

    [Header("��")]
    [SerializeField] float strongShootInterval;

    [Header("<=====�e���ˎ��̃A�j���[�V�������x�{���i0~1�j=====>")]
    [Header("��")]
    [SerializeField] float weakAnimationSpeedRatio;

    [Header("��")]
    [SerializeField] float strongAnimationSpeedRatio;
    [Header("==========")]

    [Header("���������e�����S�ɑ傫���Ȃ邽�߂ɂ����鎞��(�b)")]
    [SerializeField] float timeScaleUpMagicMissileCompletely;

    [Header("�e���ˉ\�ȋ�������(�v���C���[�̍ő�ړ��͈͂ɑ΂��銄���i0~1))")]
    [SerializeField] float maxDistanceRatio;

    [Header("������΂���")]
    [SerializeField] float blowOffPower;

    [Header("������΂��U���̐����ʒu(�����̈ʒu�Ƃ̍�)")]
    [SerializeField] Vector3 blowOffEffectPosition;

    [Header("�_�E������")]
    [SerializeField] float downedTime;

    [Header("�_�E�������邽�߂ɕK�v�ȃ_���[�W��")]
    [SerializeField] float downedDamageAmount;

    [Header("�_�E�����A�A�j���[�V�������ꎞ��~���鎞��(�b)")]
    [SerializeField] float timeStopMotion;

    [Header("�_�E���I����A��ʒu�ɖ߂�܂ł̑���(�b)")]
    [SerializeField] float timeReturnNormalPos;

    //���̃R�[�h���Ŏg�p���邽�߂̕ϐ�
    Rigidbody myRigidbody;                         //������Rigidbody
    Animator myAnimator;                             //������animator
    CS_Enemy1MagicMissile[] script;                //�e�̃X�N���v�g

    GameObject[] magicMissile = new GameObject[2]; //�e
    GameObject[] createdMagicMissile;              //���������e

    Vector3 normalPos;                             //�ʏ펞�̈ʒu
    Vector3 downedPos;                             //�_�E�������Ƃ��̈ʒu
    float hp;                                      //HP
    int[] magicMissileNumber = new int[2];         //�e�̐�
    float[] creationInterval = new float[2];       //�e�̐������x�i�b�j
    float[] shootInterval;                         //�A�ˑ��x�i�b�j
    float[] probability = new float[3];            //�e�U���̔����m��
    float totalProbability;                        //�S�U���̔����m���̑��a
    float damageAmount;                            //�_���[�W��
    float totalDownedTime;                         //�_�E������
    float totalReturnTime;                         //��ʒu�ɖ߂�܂łɂ������������v����

    float attackInterval;                          //�U���Ԃ̃C���^�[�o���p�ϐ�
    int magicMissileCount = 1;                     //�����ڂ���\���ϐ�
    int evenCount = 0;                             //�������ڂ̒e���������ꂽ��
    int oddCount = 0;                              //1���ڂ���������ڂ̒e���������ꂽ��
    bool isAttack;                                 //�U�������H
    bool isReturningNormalPos;                     //��ʒu�ɖ߂��Ă���r�����H
    bool isDowned;                                 //�_�E����Ԃ��H
    bool canFight;                                   //�퓬�\���H
    bool isDead;                                   //���S�������H

    AudioSource shotAudioSource;                   //�e�����p��AudioSource

    [Header("<=====�~�����ԁi�b�j=====>")]
    [Header("�_�E����")]
    [SerializeField] float timeFallToGroundByDown;

    //[Header("���S��")]
    //[SerializeField] float timeFallToGroundByDeath;
    //[Header("==========")]

    [Header("�ڒn���m�I�u�W�F�N�g")]
    [SerializeField] GameObject groundDetection;

    [Header("����G�t�F�N�g")]
    [SerializeField] ParticleSystem mist;

    [Header("���o����G�t�F�N�g")]
    [SerializeField] ParticleSystem releaseMist;

    [Header("���߃G�t�F�N�g")]
    [SerializeField] ParticleSystem trail;

    [Header("�y���G�t�F�N�g")]
    [SerializeField] ParticleSystem dustCloud;

    [Header("�e������SE")]
    [SerializeField] private AudioClip shotSE;

    //[Header("�Ȑ��O���ɂ��邩�H�i��U���j")]
    //[SerializeField] bool isCurveWeakMagicMissile;

    //[Header("�Ȑ��O���ɂ��邩�H�i���U���j")]
    //[SerializeField] bool isCurveStrongMagicMissile;

    //float blowOffCount;

    //float blowOffDuration;
    //bool isBlowingOff;

    const int defaultPuddleRenderQueue = 2980;
    int addPuddleRenderQueue;

    //float scaleRatioBasedOnY;
    Vector3 scaleRatioBasedOnY;

    float maxCanShootDistance;

    //���S���̈ړ��p
    //const float moveTimeOfDeath = 0.0f;  //���S���̂݃X�s�[�h���X�N���v�g���ŉB�؂�����
    float timeArriveGround;
    float totalFallTime;       //���������v����
    Vector3 currentPosition;  //���݂̈ʒu
    Vector3 atGroundPosition;   //�n�ʂɍ~�肽�Ƃ��̈ʒu
    bool isGround;              //�n�ʂɍ~�肽���H
    Collider myCollider;      //�����̃R���C�_�[

    //�U���A�j���[�V�����Ή��p
    bool canShoot;  //���ˉ\���H
    bool canBlowOff;//������΂��U���\���H
    //bool canFallByDeath;       //���S�ɂ���ė����ł��邩�H

    int downCount = 0;

    bool canFall;  //�A�j���[�V�����ꎞ��~�p

    bool isWaitRise;  //LookAt���쓮���Ă��܂�����

    bool isStartReadyStandby;  //���o�ҋ@�̏����J�n
    bool isStandby;  //���o�ҋ@���Ă��邩�H
    bool isStartGame;
    bool isFall;  //�~�������H
    bool isWaitFall;  //�~�����������H
    float timeArriveNormalPos;
    Coroutine waitFall;

    //�G�t�F�N�g�p
    CS_Enemy1Trail trailScript;
    bool isPlayDustCloudEffect;

    //float shotCount = 0;

    //�Q�b�^�[
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
        //����������
        Initialize();

        //�G���[���b�Z�[�W
        for(int i = 0; i < 2; ++i)
        {
            if (magicMissileNumber[i] % 2 == 0)
            {
                AttackType type = (AttackType)i;
                Debug.Log("(" + type + ")�������̒e�����ݒ肳��Ă��܂��B����̒e���ɕύX���Ă��������B");
            }
        }
    }

    /// <summary>
    /// ����������
    /// </summary>
    private void Initialize()
    {
        //��U��
        int num = (int)AttackType.Weak;
        magicMissile[num] = weakMagicMissile;
        magicMissileNumber[num] = weakMagicMissileNumber;
        creationInterval[num] = 0.0f;  //1���ڂ͑����ɐ������邽��0
        //���U��
        num = (int)AttackType.Strong;
        magicMissile[num] = strongMagicMissile;
        magicMissileNumber[num] = strongMagicMissileNumber;
        creationInterval[num] = 0.0f;

        //���̑��ϐ��̏�����
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

        //�ŏ��̍U����ݒ�
        probability[0] = weakAttackProbability;
        probability[1] = strongAttackProbability;
        probability[2] = blowOffAttackProbability;
        for (int i = 0; i < GetEnumLength<AttackType>(); ++i)
        {
            totalProbability += probability[i];
        }
        SetAttackType(0);  //�ŏ��͎�U��
        //ChooseAttackType();

        //AudioSource�̎擾
        shotAudioSource = GetComponent<AudioSource>();
        //blowOffAudioSource = audioSources[1];

        ////AudioSource�̎擾
        //AudioSource[] audioSources = GetComponents<AudioSource>();
        //shotAudioSource = audioSources[0];
        ////blowOffAudioSource = audioSources[1];


        //�����p�ϐ��̏�����
        //isBlowingOff = false;
        //blowOffDuration = 0.0f;

        addPuddleRenderQueue = 0;

        //���̃v���n�u�̔䗦�v�Z
        Vector3 localScale = strongMagicMissile.transform.localScale;
        scaleRatioBasedOnY = new Vector3(
            localScale.x / localScale.y,
            1.0f,  //Y���
            localScale.z / localScale.y);
        //scaleRatioBasedOnY = localScale.x / localScale.y;

        //�e�𔭎˂ł���ő勗�����v�Z
        float boundaryCircleRadius = stageBoundary.GetBoundaryCircleRadius;
        maxCanShootDistance = 
            (boundaryCircleRadius * boundaryCircleRadius) * maxDistanceRatio;

        //�~������lerp�p
        totalFallTime = 0.0f;
        currentPosition = new Vector3(0.0f, 0.0f, 0.0f);
        atGroundPosition = new Vector3(0.0f, 0.0f, 0.0f);
        isGround = false;

        //�U���A�j���[�V�����Ή��p
        canShoot = false;
        canBlowOff = false;
        //canFallByDeath = false;

        //�A�j���[�V�����ꎞ��~�p
        canFall = false;

        //LookAt���쓮���邽��
        isWaitRise = false;

        //���o�ҋ@�p
        isStartReadyStandby = false;
        isStandby = true;  //������Ԃ͑ҋ@
        isStartGame = false;
        isFall = false;
        isWaitFall = false;
        timeArriveNormalPos = timeReturnNormalPos;
        waitFall = StartCoroutine(WaitFall());

        //�G�t�F�N�g�p
        trailScript = trail.GetComponent<CS_Enemy1Trail>();
        trail.Stop();
        mist.Stop();
        isPlayDustCloudEffect = false;

        //���S���p
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

        //���S
        if (hp <= 0.0f)
        {
            Death();
            return;
        }

        //���o�ҋ@
        if (isStandby && !isStartReadyStandby)
        {
            return;
        }

        //�_�E��
        if(damageAmount > downedDamageAmount)
        {
            Downed();
            return;
        }

        //��ʒu�ɖ߂�
        if (isReturningNormalPos)
        {
            //Debug.Log("��ʒu�ɖ߂�");
            ReturnNormalPos();
            return;
        }

        //�㏸�ҋ@���͏������Ȃ�
        if (isWaitRise)
        {
            return;
        }

        //�v���C���[�̕���������
        LookAtPlayer();

        //�U��
        if (isAttack)
        {
            Attack(attackType);  //�U������

            //�U�����I�������玟�̍U���̎�ނ�����
            if (!isAttack)
            {
                ChooseAttackType();

                //�U���A�j���[�V�����p�ϐ���������
                canShoot = false;  //���˕s��
                canBlowOff = false;//������΂��s��
            }
            return;
        }

        //�C���^�[�o�����o�߂����玟�̍U���ֈڂ�
        attackInterval -= Time.deltaTime;
        if(attackInterval <= 0.0f)
        {
            //�U���̏���
            AttackReady(attackType);
        }
    }

    /// <summary>
    /// �e�𔭎˂���܂ł̎��Ԃ�ݒ肷��
    /// </summary>
    /// <param name="type">�U���̎�ށi��E���j</param>
    /// <param name="iteration">���������e�̃C�e���[�V����</param>
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
    /// �J��o���U���̎�ނ�I��
    /// </summary>
    void ChooseAttackType()
    {
        //�ʒu��I��
        float randomPoint = UnityEngine.Random.Range(0, totalProbability);

        //���I
        float currentWeight = 0.0f;  //���݂̏d�݂̈ʒu
        for(int i = 0; i < GetEnumLength<AttackType>(); ++i)
        {
            currentWeight += probability[i];

            if(randomPoint < currentWeight)
            {
                SetAttackType(i);
                return;
            }
        }

        //�ʒu���d�݂̑��a�ȏ�Ȃ疖���v�f�Ƃ���
        SetAttackType(probability.Length - 1) ;
    }

    /// <summary>
    /// �U���̎�ނ�ݒ肷��
    /// </summary>
    /// <param name="iteration">�C�e���[�V����</param>
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
    /// enum�^�̗v�f�����擾����
    /// </summary>
    /// <typeparam name="T">enum�^</typeparam>
    /// <returns>�v�f��</returns>
    int GetEnumLength<T>()
    {
        return Enum.GetValues(typeof(T)).Length;
    }

    /// <summary>
    /// �v���C���[�̕���������
    /// </summary>
    void LookAtPlayer()
    {
        if(player == null)
        {
            return;
        }

        //�^�[�Q�b�g�������擾
        Vector3 targetDirection = player.transform.position - transform.position;
        targetDirection.y = 0.0f;  //X����]�𔽉f���Ȃ�

        //�^�[�Q�b�g�����̉�]��\��Quaternion���擾
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);

        //���炩�ɉ�]
        transform.rotation = 
            Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    /// <summary>
    /// �U������������
    /// </summary>
    /// <param name="type">�U���̎��</param>
    void AttackReady(AttackType type)
    {
        switch (type)
        {
        case AttackType.Weak:
        case AttackType.Strong:
            //�e�̐���
            int num = (int)attackType;  //�v�f�ԍ��w��p�ϐ�
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
    /// �e�𐶐�����
    /// </summary>
    /// <param name="type">�U���̎�ށi��E���j</param>
    void CreateMagicMissile(AttackType type)
    {
        int num = (int)type;  //�v�f�ԍ��w��p�ϐ�

        float angleSpace = 200.0f / magicMissileNumber[num];  //���~�̒��ł̒e���m�̊Ԋu(����160.0f)
        const float baseAngle = 90.0f;  //1�ڂ̒e�̔z�u�p�x�𔼉~�̐^�񒆂ɂ���
        float angle = 0.0f;

        //���~�̂ǂ��ɔz�u���邩����
        if (magicMissileCount == 1)  //1����
        {
            angle = baseAngle;
        }
        else if (magicMissileCount % 2 == 0)  //��������
        {
            evenCount++;
            angle = baseAngle - evenCount * angleSpace;  //�G����݂č��ɏ��ɔz�u
        }
        else  //�����
        {
            oddCount++;
            angle = baseAngle + oddCount * angleSpace;   //�G���猩�ĉE�ɏ��ɔz�u
        }

        //�G1�̉�]���l�����č��W����
        Vector3 magicMissilePos = new Vector3(
            magicMissileSpawnPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * halfCircleRadius,
            magicMissileSpawnPos.y + Mathf.Sin(angle * Mathf.Deg2Rad) * halfCircleRadius,
            magicMissileSpawnPos.z);
        //magicMissilePos = transform.TransformPoint(magicMissilePos);  //�e�̐�[��O�Ɍ�������Ƃ��͏���

        //Quaternion spawnRotation = Quaternion.LookRotation(transform.forward, Vector3.up);
        //Quaternion spawnRotation = Quaternion.Euler(0f, 0f, angle) * transform.rotation;
        //Quaternion spawnRotation = Quaternion.Euler(transform.localEulerAngles.y, -45.0f, -90.0f);

        //����
        //magicMissileCount��1����n�܂邽��-1
        createdMagicMissile[magicMissileCount - 1] =
            Instantiate(magicMissile[num], magicMissilePos, Quaternion.identity);


        //�G�ƒe��e�q�֌W��
        createdMagicMissile[magicMissileCount - 1].transform.SetParent(gameObject.transform, false);

        script[magicMissileCount - 1] =
            createdMagicMissile[magicMissileCount - 1].GetComponent<CS_Enemy1MagicMissile>();

        //�e�̃p�����[�^�ݒ�
        SetMoveSpeed(type, magicMissileCount - 1);
        SetAttackPower(type, magicMissileCount - 1);

        //�����p
        script[magicMissileCount - 1].SetMagicMissileCount = magicMissileCount;
        script[magicMissileCount - 1].SetPuddleRenderQueue = defaultPuddleRenderQueue + addPuddleRenderQueue;
        script[magicMissileCount - 1].SetPlayerTransform = player.transform;

        //�����p2
        Vector3 localScale = strongMagicMissile.transform.localScale;
        //script[magicMissileCount - 1].SetScaleRatioBasedOnY = scaleRatioBasedOnY;

        //�e�̎�ނ��Z�b�g
        script[magicMissileCount - 1].SetMagicMissileType = SetMagicMissileType(type);


        //�e�̖ڕW�X�P�[�����v�Z����
        //�e�̃X�P�[���𔽉f���Ȃ�Y���v�Z
        Vector3 myLossyScale = transform.lossyScale;
        float scaleY = createdMagicMissile[magicMissileCount - 1].transform.localScale.y / myLossyScale.y;
        float newScaleX = scaleY * (scaleRatioBasedOnY.x);  //2.0 = ����
        float newScaleZ = scaleY * (scaleRatioBasedOnY.z);  //2.0 = ����
        Vector3 target = new Vector3(
            newScaleX,
            scaleY,
            newScaleZ);
        script[magicMissileCount - 1].SetTargetScaleForCreate = target;
        script[magicMissileCount - 1].SetTimeScaleUpCompletely = timeScaleUpMagicMissileCompletely;
        //�e�̃X�P�[����0��
        createdMagicMissile[magicMissileCount - 1].transform.localScale = Vector3.zero;

        //Vector3 parentLossyScale = transform.parent.lossyScale;
        //float scaleY = transform.localScale.y / parentLossyScale.y;
        ////XZ�������䗦�ɂȂ�悤��Y����Ƃ����䗦��Y�ɂ����đ��
        //float newScaleX = scaleY * (scaleRatioBasedOnY.x);  //2.0 = ����
        //float newScaleZ = scaleY * (scaleRatioBasedOnY.z);  //2.0 = ����
        //transform.localScale = new Vector3(
        //    newScaleX,
        //    scaleY,
        //    newScaleZ);



        //�e�ϐ��̍X�V
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
        //�����p
        addPuddleRenderQueue++;
        if(addPuddleRenderQueue >= 20) { addPuddleRenderQueue = 0; }
        //if(addPuddleRenderQueue >= 15) { addPuddleRenderQueue = 0; }

        //�S�Đ��������琶�����~�߁A�U���Ɉڂ�
        if (magicMissileCount > magicMissileNumber[num])
        {
            isAttack = true;
            magicMissileCount = 1;
            evenCount = oddCount = 0;
            creationInterval[num] = 0.0f;

            myAnimator.SetBool("Attack", true);  //���[�V��������

            trail.Play();  //�G�t�F�N�g�Đ�
            trailScript.GetSetIsPlay = true;
            Debug.Log("trail�Đ������I���");
        }
    }

    //void CreateMagicMissile(AttackType type)
    //{
    //    int num = (int)type;  //�v�f�ԍ��w��p�ϐ�

    //    float angleSpace = 160.0f / magicMissileNumber[num];  //���~�̒��ł̒e���m�̊Ԋu
    //    const float baseAngle = 90.0f;  //1�ڂ̒e�̔z�u�p�x�𔼉~�̐^�񒆂ɂ���
    //    float angle = 0.0f;

    //    //���~�̂ǂ��ɔz�u���邩����
    //    if (magicMissileCount == 1)  //1����
    //    {
    //        angle = baseAngle;
    //    }
    //    else if (magicMissileCount % 2 == 0)  //��������
    //    {
    //        evenCount++;
    //        angle = baseAngle - evenCount * angleSpace;  //�G����݂č��ɏ��ɔz�u
    //    }
    //    else  //�����
    //    {
    //        oddCount++;
    //        angle = baseAngle + oddCount * angleSpace;   //�G���猩�ĉE�ɏ��ɔz�u
    //    }

    //    //�G1�̉�]���l�����č��W����
    //    Vector3 magicMissilePos = new Vector3(
    //        magicMissileSpawnPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * halfCircleRadius,
    //        magicMissileSpawnPos.y + Mathf.Sin(angle * Mathf.Deg2Rad) * halfCircleRadius,
    //        magicMissileSpawnPos.z);
    //    magicMissilePos = transform.TransformPoint(magicMissilePos);  //�e�̐�[��O�Ɍ�������Ƃ��͏���

    //    //����
    //    //magicMissileCount��1����n�܂邽��-1
    //    createdMagicMissile[magicMissileCount - 1] =
    //        Instantiate(magicMissile[num], magicMissilePos, Quaternion.identity);


    //    //�G�ƒe��e�q�֌W��
    //    createdMagicMissile[magicMissileCount - 1].transform.SetParent(gameObject.transform);

    //    ////�e�̉�]�l�ݒ�
    //    //Vector3 localEulerAngles = createdMagicMissile[magicMissileCount - 1].transform.localEulerAngles;
    //    //localEulerAngles.y += transform.localEulerAngles.y;
    //    //createdMagicMissile[magicMissileCount - 1].transform.localEulerAngles = localEulerAngles;
    //    //Debug.Log(createdMagicMissile[magicMissileCount - 1].transform.localEulerAngles);

    //    script[magicMissileCount - 1] =
    //        createdMagicMissile[magicMissileCount - 1].GetComponent<CS_Enemy1MagicMissile>();

    //    //�����p
    //    script[magicMissileCount - 1].SetMagicMissileCount = magicMissileCount;
    //    script[magicMissileCount - 1].SetPuddleRenderQueue = defaultPuddleRenderQueue + addPuddleRenderQueue;
    //    script[magicMissileCount - 1].SetPlayerTransform = player.transform;

    //    //�e�ϐ��̍X�V
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
    //    //�����p
    //    addPuddleRenderQueue++;
    //    if(addPuddleRenderQueue >= 15) { addPuddleRenderQueue = 0; }

    //    //�S�Đ��������琶�����~�߁A�U���Ɉڂ�
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
    /// �e�̎�ނ�ݒ肷��
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
    /// ������΂��U���̏���
    /// </summary>
    void ReadyBlowOff()
    {
        isAttack = true;  //�U���J�n
        myAnimator.SetBool("Attack", true);  //���[�V��������

        if (!trailScript.GetSetIsPlay)
        {
            trail.Play();  //�G�t�F�N�g�Đ�
            trailScript.GetSetIsPlay = true;
        }

        ////blowOffCount -= Time.deltaTime;
        ////if(blowOffCount <= 0.0f)
        ////{
        ////    isAttack = true;
        ////    myAnimator.SetBool("Attack", false);  //���[�V������~
        ////}


        //myAnimator.SetBool("Attack", true);  //���[�V��������

        //blowOffCount -= Time.deltaTime;
        //if (blowOffCount <= 0.0f)
        //{
        //    isAttack = true;
        //    myAnimator.SetBool("Attack", false);  //���[�V������~
        //}
    }

    /// <summary>
    /// AnimationEvent�p�̊֐�(��E���U���p)
    /// </summary>
    void AttackEventShoot()
    {
        //������΂��p�ł͂Ȃ��̂ŏ������Ȃ�
        if(attackType == AttackType.BlowOff)
        {
            return;
        }

        //�A�j���[�V�������x��ς���
        if(attackType == AttackType.Weak)
        {
            myAnimator.SetFloat("AnimationSpeed", weakAnimationSpeedRatio);
        }
        if(attackType == AttackType.Strong)
        {
            myAnimator.SetFloat("AnimationSpeed", strongAnimationSpeedRatio);
        }

        //���ˉ\
        canShoot = true;  

        //if(attackType == AttackType.Weak ||
        //    attackType == AttackType.Strong)
        //{
        //    canShoot = true;  //���ˉ\
        //}
    }

    /// <summary>
    /// AnimationEvent�p�̊֐�(������΂��U���p)
    /// </summary>
    void AttackEventBlowOff()
    {
        if(attackType == AttackType.BlowOff)
        {
            canBlowOff = true;//������΂��\
            trail.Stop();  //�G�t�F�N�g��~
            trailScript.GetSetIsPlay = false;
        }
    }

    /// <summary>
    /// AnimationEvent�p�̊֐�(�_�E�����p)
    /// </summary>
    void DownedEvent()
    {
        //���o�ҋ@���������_�E����
        if(isStartReadyStandby && isDowned)
        {
            canFall = false;
            //�~���������̎��ɉ��o�ҋ@�ɂȂ����Ƃ��A�����I��
            if (isFall)
            {
                canFall = true;
            }
            Standby();  //�_�E�����̕ϐ��������g�p

            //�������ł����邩��
            //��������Standby()�̂�
            //canFall = false;
            //Standby();  //�_�E�����̕ϐ��������g�p
        }
    }

    /// <summary>
    /// AnimationEvent�p�̊֐�(�㏸�p)
    /// </summary>
    void RiseEvent()
    {
        //�_�E���łȂ����(�������͍~�����łȂ����)�㏸�A���~���������łȂ��Ƃ�
        if (!isDowned && !isWaitFall)
        {
            isReturningNormalPos = true;
            isWaitRise = false;
        }

        //�ʏ펞���㏸�������G�t�F�N�g�Đ�
        if (!isStartReadyStandby && !isStandby && !isDowned)
        {
            PlayDustCloudEffect();
        }
    }

    /// <summary>
    /// AnimationEvent�p�̊֐�(���S�p)
    /// </summary>
    void DeathEvent()
    {
        //canFallByDeath = true;

        //�d�͂�����FreezePositionY�����������
        myRigidbody.useGravity = true;
        myRigidbody.constraints = RigidbodyConstraints.None;
        myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        //myRigidbody.constraints = RigidbodyConstraints.FreezePositionX;
        //myRigidbody.constraints = RigidbodyConstraints.FreezePositionZ;

    }

    /// <summary>
    /// AnimationEvent�p�̊֐�(���S���S�p)
    /// </summary>
    void DeathEventIsDead()
    {
        isDead = true;
    }

    /// <summary>
    /// �U������
    /// </summary>
    /// <param name="type">�U���̎��</param>
    void Attack(AttackType type)
    {
        switch (type)
        {
            case AttackType.Weak:
            case AttackType.Strong:
                //��苗���ȓ���������e�����
                if (CheckCanShoot())
                {
                    if (canShoot)
                    {
                        //���[�V�����ɍ��킹�Ĕ���
                        Shoot(type);

                        if (trailScript.GetSetIsPlay)
                        {
                            trail.Stop();
                            trailScript.GetSetIsPlay = false;
                        }
                    }
                    else
                    {
                        //�͈͓��ɓ������̂Ń��[�V�������ēx�Đ�
                        myAnimator.SetBool("Attack", true);

                        if (!trailScript.GetSetIsPlay)
                        {
                            trail.Play();
                            trailScript.GetSetIsPlay = true;
                        }
                    }
                }
                //��苗������Ă����甭�˂ƃ��[�V�������~
                else
                {
                    canShoot = false;
                    myAnimator.SetBool("Attack", false);
                    myAnimator.SetFloat("AnimationSpeed", 1);  //���x��߂�

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
                //        isShooting = false;  //���˒��~
                //        myAnimator.SetBool("Attack", false);  //���[�V������~
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
    /// �e�𔭎˂��邱�Ƃ��ł��邩�m�F����
    /// </summary>
    /// <returns>���ˏo���邩�H</returns>
    bool CheckCanShoot()
    {
        //�X�e�[�W�̒��S����v���C���[�܂ł̋���
        Vector2 playerDirection = new Vector2(
            player.transform.position.x,
            player.transform.position.z);
        float distance = playerDirection.sqrMagnitude;

        //��苗���ȏゾ�Ɣ��˕s��
        if(distance > maxCanShootDistance)
        {
            return false;
        }

        return true;  //���ˉ\
    }

    /// <summary>
    /// �e�𔭎˂���
    /// </summary>
    void Shoot(AttackType type)
    {
        int num = (int)type;  //�v�f�ԍ��w��p�ϐ�

        for (int i = 0; i < magicMissileNumber[num]; ++i)
        {
            if (createdMagicMissile[i] == null)
            {
                continue;
            }


            //���ˎ��ԂɂȂ����甭��
            shootInterval[i] -= Time.deltaTime;
            if (shootInterval[i] < 0.0f)
            {
                script[i].GetSetIsCanFire = true;  //����
                //script[i].SetPlayerTransform = player.transform;

                //�����p
                //if(type == AttackType.Weak)
                //{
                //    script[i].SetIsCurve = isCurveWeakMagicMissile;
                //}
                //else
                //{
                //    script[i].SetIsCurve = isCurveStrongMagicMissile;
                //}

                //������
                createdMagicMissile[i] = null;
                script[i] = null;

                //��
                shotAudioSource.PlayOneShot(shotSE);
                //shotAudioSource.PlayOneShot(shotAudioSource.clip);

                //�Ō����O�̒e�������ꂽ��A�j���[�V�������x�����ɖ߂�
                if (i == magicMissileNumber[num] - 2)
                {
                    myAnimator.SetFloat("AnimationSpeed", 1);
                }

                //�Ō�̒e����������U���I��
                if (i == magicMissileNumber[num] - 1)
                {
                    isAttack = false;
                    attackInterval = maxAttackInterval;

                    myAnimator.SetBool("Attack", false);  //���[�V������~
                    //shotCount++;
                }
            }
        }
    }

    /// <summary>
    /// �e������
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
            //���˂���Ă���������Ȃ��i����Ȃ��H�j
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
    /// ������΂��U�����s��
    /// </summary>
    void BlowOff()
    {
        //�G�t�F�N�g����
        Vector3 pos = transform.position + blowOffEffectPosition;
        var effect = Instantiate(blowOffEffect, pos, Quaternion.identity);
        //var effect = Instantiate(blowOffEffect, transform.position, Quaternion.identity);
        effect.SetBlowOffPower = blowOffPower;
        effect.PlayEffect();
        //isBlowingOff = true;

        //blowOffDuration = effect.GetEffectDuration;

        //�ϐ�������
        isAttack = false;
        //isBlowingOff = false;
        attackInterval = maxAttackInterval;

        //�A�j���[�V�����p�ϐ�������
        canBlowOff = false;
        myAnimator.SetBool("Attack", false);  //���[�V������~
    }

    /// <summary>
    /// ���S������
    /// </summary>
    void Death()
    {
        if (!canFight)
        {
            //���S
            canFight = true;
            Debug.Log("���S");

            //�ϐ��̏�����
            isAttack = false;

            //�e������
            DestroyMagicMissile();

            //���S�A�j���[�V�����Đ�
            myAnimator.SetTrigger("Death");
            //myAnimator.SetBool("Fall", true);

            //�r������Đ��H(�A�j���[�V�������Ȃ���Ȃ��j
            //myAnimator.Play("Death", 0, 0.15f);

            //�J�v�Z���R���C�_�[������
            myCollider.enabled = false;

            //�{�[���ɂ��Ă���R���C�_�[�L����
            Collider boneCollider = transform.GetChild(3).GetComponent<Collider>();
            boneCollider.enabled = true;

            //�ڒn���m�I�u�W�F�N�g�L����
            groundDetection.SetActive(true);

            //�G�t�F�N�g��~
            mist.Stop();
            trail.Stop();
            trailScript.GetSetIsPlay = false;

            //�G�t�F�N�g�Đ�
            Vector3 pos = transform.position;
            pos.y += 2.0f;
            Instantiate(releaseMist, pos, Quaternion.identity);

            //�n�ʂɉ��낷(�d�͗����ɂ����̂ŗv��Ȃ�)
            //�ڕW�܂ł̋������犄�����Z�o����timeArriveGround�̐��l��ς���
            // = �~���X�s�[�h���ς��Ȃ�
            //isGround = false;
            //totalFallTime = 0.0f;  //������
            //timeArriveGround = timeFallToGroundByDeath * DistanceRatioToGround();
            //currentPosition = transform.position;

            //myRigidbody.useGravity = true;
        }

        //�A�j���[�V�����������^�C�~���O�ɂȂ����痎��������
        //if (canFallByDeath)
        //{
        //    FallToGround();
        //}
    }

    /// <summary>
    /// �_�E��������
    /// </summary>
    void Downed()
    {
        //Debug.Log("�_�E����");
        if (!isDowned)
        {
            //�㏸���Ƀ_�E��������㏸�Ɋ֌W����ϐ���������
            //�A�j���[�V������������ς���
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

            //�ϐ��̏������i�e�֘A�͐�΁j
            isAttack = false;
            attackInterval = maxAttackInterval;
            magicMissileCount = 1;
            evenCount = oddCount = 0;
            for(int i = 0; i < 2; ++i)
            {
                creationInterval[i] = 0.0f;
            }

            //�e������
            DestroyMagicMissile();

            //�~���A�j���[�V�����Đ�
            myAnimator.SetBool("Fall", true);
            myAnimator.SetBool("Attack", false);
            myAnimator.SetFloat("AnimationSpeed", 1);  //���x��߂�

            //�G�t�F�N�g�̒�~
            mist.Stop();
            trail.Stop();
            trailScript.GetSetIsPlay = false;

            //�n�ʂɉ��낷
            //�ڕW�܂ł̋������犄�����Z�o����timeArriveGround�̐��l��ς���
            // = �~���X�s�[�h���ς��Ȃ�
            isGround = false;
            timeArriveGround = timeFallToGroundByDown * DistanceRatioToDestination(atGroundPosition.y);
            currentPosition = transform.position;

            //myRigidbody.useGravity = true;
            downCount++;

            canFall = false;  //������
            StartCoroutine(WaitFall());
        }

        if (!canFall)
        {
            //�ꎞ��~���ŗ����ł��Ȃ���Ώ������Ȃ�
            return;
        }

        //�n�ʂ܂ō~������
        if (!isGround)
        {
            FallToGround();
            return;  //�~�����̓_�E�����Ԃ��J�E���g���Ȃ�
        }

        //�_�E�����Ԃ��o�߂������ʒu�ɖ߂�
        totalDownedTime += Time.deltaTime;
        if(totalDownedTime > downedTime)
        {
            isDowned = false;
            downedPos = transform.position;
            //myRigidbody.useGravity = false;
            damageAmount = 0.0f;
            totalDownedTime = 0.0f;

            //��ʒu�ɖ߂�A�j���[�V�����Đ�
            myAnimator.SetBool("Rise", true);
            myAnimator.SetBool("Down", false);

            //�G�t�F�N�g�̍Đ�
            mist.Play();

            //��ʒu�ɖ߂�
            //isReturningNormalPos = true;
            isWaitRise = true;
            timeArriveNormalPos = timeReturnNormalPos * DistanceRatioToDestination(normalPos.y);

            //�U���̎�ނ�����
            ChooseAttackType();
            canShoot = false;  //���˕s�i�_�E�������Update()�̓��l�̃R�[�h�ɓ���Ȃ�����)
            canBlowOff = false;//������΂��s��
        }
    }

    /// <summary>
    /// ���b��������҂�
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitFall()
    {
        yield return new WaitForSeconds(timeStopMotion);

        canFall = true;
    }

    /// <summary>
    /// ���݂̈ʒu���ړI�n�܂łǂꂭ�炢�̋��������Ȃ̂��v�Z����
    /// </summary>
    /// <returns>��������</returns>
    float DistanceRatioToDestination(float destinationY)
    {
        float currentY = transform.position.y;
        //�㏸�A�~���ŏ�����ς���
        if (isGround)
        {
            if (currentY >= destinationY)
            {
                return 0.0f;
            }

            //�ō��l���t�ɂȂ邽�ߊ������t
            return 1.0f - (currentY / normalPos.y);  
        }
        else
        {
            if (currentY <= destinationY)
            //if(currentY <= atGroundPosition.y)
            {
                return 0.0f;
            }

            return currentY / normalPos.y;  //����
        }
    }

    /// <summary>
    /// �~������
    /// </summary>
    void FallToGround()
    {
        isFall = true;

        //�ړ�����
        totalFallTime += Time.deltaTime;
        float t = Mathf.Clamp01(totalFallTime / timeArriveGround);
        transform.position = Vector3.Lerp(currentPosition, atGroundPosition, t);

        //���o�ҋ@�������̏ꍇ�A�~�������Ȃ�
        if (isStartReadyStandby)
        {
            t = 1.0f;
        }

        //���n��菭�����߂ɃG�t�F�N�g�Đ�
        if(t >= 0.96f && t < 1)
        {
            if(!isPlayDustCloudEffect && !isStartReadyStandby)
            {
                //�G�t�F�N�g����
                PlayDustCloudEffect();
                isPlayDustCloudEffect = true;
            }
        }
        else if (t >= 1)
        {
            //���o�ҋ@�������ȊO�̎��ɒn�ʂ̍��W������
            if (!isStartReadyStandby)
            {
                transform.position = atGroundPosition;
                isFall = false;
            }
            isGround = true;
            totalFallTime = 0.0f;

            //���S���Ă��Ȃ��Ƃ��_�E�����[�V�����Đ�
            if(hp > 0)
            {
                myAnimator.SetBool("Down", true);
                myAnimator.SetBool("Fall", false);
                myAnimator.SetBool("Rise", false);  //�㏸���Ƀ_�E�������Ƃ��̂���
            }
            //���S���Ă����炷�ׂĂ̏��������Ȃ��悤�ɂ���
            //else
            //{
            //    canFallByDeath = false;
            //}
        }
    }

    /// <summary>
    /// ��ʒu�ɖ߂�
    /// </summary>
    void ReturnNormalPos()
    {
        //�ړ�����
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

            //���o�ҋ@������ԂȂ牉�o�ҋ@���n�߂�
            if (isStartReadyStandby)
            {
                isStartReadyStandby = false;
                isStandby = true;
            }
        }
    }

    /// <summary>
    /// �y���G�t�F�N�g���Đ�����
    /// </summary>
    void PlayDustCloudEffect()
    {
        Vector3 pos = new Vector3(transform.position.y, 0.2f, transform.position.z);
        ParticleSystem p = Instantiate(dustCloud, pos, Quaternion.Euler(-90.0f, 0.0f, 0.0f));

        p.Play();
    }

    /// <summary>
    /// �_���[�W�ʂɉ��Z����
    /// </summary>
    /// <param name="attackPower">�v���C���[�̍U����</param>
    void AddDamageAmount(float attackPower)
    {
        //damageAmount += attackPower;
        //�_�E�����̓_�E���p�̃_���[�W�ʕϐ��ɉ��Z���Ȃ�
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
    /// �G1��HP�����炷
    /// </summary>
    /// <param name="attackPower">�U����</param>
    public void ReduceHp(float attackPower)
    {
        //���S���Ă����珈�����Ȃ�
        if (canFight)
        {
            return;
        }

        //���o�ҋ@���͏������Ȃ�
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
    /// ���o�ҋ@����
    /// </summary>
    public void Standby()
    {
        isStartReadyStandby = true;  //�ҋ@�����J�n

        //�U�����������͍U��������
        if(isAttack || attackInterval <= 0)
        {
            //�ϐ��̏�����
            isAttack = false;
            attackInterval = maxAttackInterval;
            magicMissileCount = 1;
            evenCount = oddCount = 0;
            for (int i = 0; i < 2; ++i)
            {
                creationInterval[i] = 0.0f;
            }

            //�e�̏���
            DestroyMagicMissile();

            //�A�j���[�V�����ݒ�
            myAnimator.SetBool("Attack", false);

            //���o�ҋ@�J�n
            isStartReadyStandby = false;
            isStandby = true;

            Debug.Log("�U���܂��͍U��������");
        }
        else if(isDowned)
        {
            //�~���ҋ@��
            if (!canFall)
            {
                //�ϐ��̏�����
                isDowned = false;
                damageAmount = 0.0f;
                StopCoroutine(waitFall);

                //�A�j���[�V�����ݒ�
                myAnimator.SetBool("Down", false);
                myAnimator.SetBool("Fall", false);

                //���o�ҋ@�J�n
                isStartReadyStandby = false;
                isStandby = true;
                isWaitFall = true;

                Debug.Log("�~���ҋ@��");
            }
            //�~����
            else if (!isGround)
            {
                Debug.Log("�~����");
            }
            //�_�E����
            else
            {
                //�ϐ��̏�����
                isDowned = false;
                downedPos = transform.position;
                damageAmount = 0.0f;
                totalDownedTime = 0.0f;

                //��ʒu�ɖ߂�
                isWaitRise = true;

                //�A�j���[�V�����ݒ�
                myAnimator.SetBool("Rise", true);
                myAnimator.SetBool("Down", false);

                Debug.Log("�_�E����");
            }
        }
        //�󒆑ҋ@��
        else if(!isWaitRise && !isReturningNormalPos)
        {
            //�ϐ��̏�����
            attackInterval = maxAttackInterval;

            //���o�ҋ@�J�n
            isStartReadyStandby = false;
            isStandby = true;

            Debug.Log("�󒆑ҋ@��");
        }

        //�G�t�F�N�g��~
        mist.Stop();
        trail.Stop();
        trailScript.GetSetIsPlay = false;

    }

    /// <summary>
    /// ���o�ҋ@����������
    /// </summary>
    public void CancelStandby()
    {
        isStandby = false;

        //����ȊO�͍U����ނ�����
        if (isStartGame)
        {
            ChooseAttackType();
        }
        else
        {
            isStartGame = true;
        }

        //�G�t�F�N�g�Đ�
        mist.Play();

        damageAmount = 0.0f;
        canShoot = false;  //���˕s��
        canBlowOff = false;//������΂��s��
        isFall = false;  //�~�����łȂ�
        isWaitFall = false;  //�~���������łȂ�
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    //�X�e�[�W�ɓ���������_�E�����[�V�����Đ�
    //    if(collision.gameObject.tag == "Stage" && hp > 0)
    //    {
    //        myAnimator.SetBool("Down", true);
    //        myAnimator.SetBool("Fall", false);
    //    }
    //}
}