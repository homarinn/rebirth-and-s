using System;
using System.Collections;
using System.Collections.Generic;
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

    [Header("�e�i��U���j")]
    [SerializeField] GameObject weakMagicMissile;

    [Header("�e�i���U���j")]
    [SerializeField] GameObject strongMagicMissile;

    [Header("������΂��G�t�F�N�g")]
    [SerializeField] CS_Enemy1BlowOffEffect blowOffEffect;

    [Header("�ő�HP")]
    [SerializeField] float maxHp;

    [Header("��]���x")]
    [SerializeField] float rotateSpeed;

    [Header("�U���Ԃ̃C���^�[�o���i�b�j")]
    [SerializeField] float maxAttackInterval;

    [Header("��U���̔����m���i%�j")]
    [SerializeField] float weakAttackProbability;   

    [Header("���U���̔����m���i%�j")]
    [SerializeField] float strongAttackProbability; 

    [Header("������΂��U���̔����m���i%�j")]
    [SerializeField] float blowOffAttackProbability;

    [Header("���~��ɒe�𐶐����鎞�̔��~�̔��a")]
    [SerializeField] float halfCircleRadius;

    [Header("�G�̈ʒu����Ƃ����e�̐����ʒu")]
    [SerializeField] Vector3 magicMissileSpawnPos = new Vector3(0.0f, 0.1f, 1.0f);

    [Header("�Ȑ��O���ɂ��邩�H�i��U���j")]
    [SerializeField] bool isCurveWeakMagicMissile;

    [Header("�Ȑ��O���ɂ��邩�H�i���U���j")]
    [SerializeField] bool isCurveStrongMagicMissile;

    [Header("1��̍U���Ő�������e�̐��i��U���j")]
    [SerializeField] int weakMagicMissileNumber;

    [Header("1��̍U���Ő�������e�̐��i���U���j")]
    [SerializeField] int strongMagicMissileNumber;

    [Header("�e�𐶐�����Ԋu�i��U���A�b�j")]
    [SerializeField] float weakCreationInterval;  

    [Header("�e�𐶐�����Ԋu�i���U���A�b�j")]
    [SerializeField] float strongCreationInterval;

    [Header("�e�̔��ˊԊu�i��U���A�b)")]
    [SerializeField] float weakShootInterval;   

    [Header("�e�̔��ˊԊu�i���U���A�b)")]
    [SerializeField] float strongShootInterval;

    [Header("������΂���")]
    [SerializeField] float blowOffPower;

    [Header("�_�E�������邽�߂ɕK�v�ȃ_���[�W��")]
    [SerializeField] float downedDamageAmount;

    [Header("�_�E������")]
    [SerializeField] float downedTime;

    [Header("�_�E���I����A��ʒu�ɖ߂�܂ł̑����i�b�j")]
    [SerializeField] float timeReturnNormalPos;

    [Header("�e������SE")]
    [SerializeField] private AudioClip shotSE;

    [Header("������΂��U����SE")]
    [SerializeField] private AudioClip blowOffSE;


    //���̃R�[�h���Ŏg�p���邽�߂̕ϐ�
    Rigidbody myRigidbody;                         //������Rigidbody
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
    bool isDead;                                   //���S�������H

    AudioSource shotAudioSource;                   //�e�����p��AudioSource
    //AudioSource blowOffAudioSource;              //������΂��U���p��AudioSource

    //�����p
    [Header("������΂��U������܂ł̎��ԁi�����p�j")]
    [SerializeField] float maxBlowOffCount;

    float blowOffCount;
    float blowOffDuration;
    bool isBlowingOff;

    //�Q�b�^�[
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

        //�ŏ��̍U����ݒ�
        probability[0] = weakAttackProbability;
        probability[1] = strongAttackProbability;
        probability[2] = blowOffAttackProbability;
        for (int i = 0; i < GetEnumLength<AttackType>(); ++i)
        {
            totalProbability += probability[i];
        }
        ChooseAttackType();

        //AudioSource�̎擾
        AudioSource[] audioSources = GetComponents<AudioSource>();
        shotAudioSource = audioSources[0];
        //blowOffAudioSource = audioSources[1];


        //�����p�ϐ��̏�����
        blowOffCount = maxBlowOffCount;
        isBlowingOff = false;
        blowOffDuration = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //ReduceHp(Time.deltaTime);
        //Debug.Log("damageAmount = " + damageAmount);

        //���S
        if (hp <= 0.0f)
        {
            Death();
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
            ReturnNormalPos();
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
            shootInterval[iteration] = weakShootInterval * (iteration + 1);
        }
        else
        {
            shootInterval[iteration] = strongShootInterval * (iteration + 1);
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

        float angleSpace = 160.0f / magicMissileNumber[num];  //���~�̒��ł̒e���m�̊Ԋu
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
        magicMissilePos = transform.TransformPoint(magicMissilePos);

        //����
        //magicMissileCount��1����n�܂邽��-1
        createdMagicMissile[magicMissileCount - 1] =
            Instantiate(magicMissile[num], magicMissilePos, Quaternion.identity);
        //�G�ƒe��e�q�֌W��
        createdMagicMissile[magicMissileCount - 1].transform.SetParent(gameObject.transform);

        //�e�̉�]�l�ݒ�
        Vector3 localEulerAngles = createdMagicMissile[magicMissileCount - 1].transform.localEulerAngles;
        localEulerAngles.y += transform.localEulerAngles.y;
        createdMagicMissile[magicMissileCount - 1].transform.localEulerAngles = localEulerAngles;
        //Debug.Log(createdMagicMissile[magicMissileCount - 1].transform.localEulerAngles);

        script[magicMissileCount - 1] =
            createdMagicMissile[magicMissileCount - 1].GetComponent<CS_Enemy1MagicMissile>();

        //�����p
        script[magicMissileCount - 1].SetMagicMissileCount = magicMissileCount;

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

        //�S�Đ��������琶�����~�߁A�U���Ɉڂ�
        if (magicMissileCount > magicMissileNumber[num])
        {
            isAttack = true;
            magicMissileCount = 1;
            evenCount = oddCount = 0;
            creationInterval[num] = 0.0f;  
        }
    }

    /// <summary>
    /// ������΂��U���̏���
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
    /// �U������
    /// </summary>
    /// <param name="type">�U���̎��</param>
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
                script[i].SetPlayerTransform = player.transform;
                //�����p
                if(type == AttackType.Weak)
                {
                    script[i].SetIsCurve = isCurveWeakMagicMissile;
                }
                else
                {
                    script[i].SetIsCurve = isCurveStrongMagicMissile;
                }

                //������
                createdMagicMissile[i] = null;
                script[i] = null;

                //��
                shotAudioSource.PlayOneShot(shotSE);
                //shotAudioSource.PlayOneShot(shotAudioSource.clip);

                //�Ō�̒e����������U���I��
                if (i == magicMissileNumber[num] - 1)
                {
                    isAttack = false;
                    attackInterval = maxAttackInterval;
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
        }
    }

    /// <summary>
    /// ������΂��U�����s��
    /// </summary>
    void BlowOff()
    {
        //�G�t�F�N�g����
        if (!isBlowingOff)
        {
            var effect = Instantiate(blowOffEffect, transform.position, Quaternion.identity);
            effect.SetBlowOffPower = blowOffPower;
            effect.PlayEffect();

            isBlowingOff = true;
            blowOffDuration = effect.GetEffectDuration;
        }

        //�U�����I�������ϐ���������
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
    /// ���S������
    /// </summary>
    void Death()
    {
        if (!isDead)
        {
            //���S
            isDead = true;
            Debug.Log("���S");

            //�ϐ��̏�����
            isAttack = false;

            //�e������
            DestroyMagicMissile();

            //���S�A�j���[�V�����Đ�

            //�n�ʂɉ��낷�i�����������g��Ȃ����@�ł��悢�H�j
            myRigidbody.useGravity = true;
        }
    }

    /// <summary>
    /// �_�E��������
    /// </summary>
    void Downed()
    {
        if (!isDowned)
        {
            isDowned = true;
            myRigidbody.useGravity = true;

            //�ϐ��̏������i�e�֘A�͐�΁j
            isAttack = false;
            attackInterval = maxAttackInterval;
            magicMissileCount = 1;
            evenCount = oddCount = 0;
            for(int i = 0; i < 2; ++i)
            {
                creationInterval[i] = 0.0f;
            }
            //������΂��U��
            blowOffCount = maxBlowOffCount;

            //�e������
            DestroyMagicMissile();

            //�_�E���A�j���[�V�����Đ�

            //�n�ʂɉ��낷�i�����������g��Ȃ����@�ł��悢�H�j
            myRigidbody.useGravity = true;
        }

        //�_�E�����Ԃ��o�߂������ʒu�ɖ߂�
        totalDownedTime += Time.deltaTime;
        if(totalDownedTime > downedTime)
        {
            isDowned = false;
            downedPos = transform.position;
            myRigidbody.useGravity = false;
            damageAmount = 0.0f;
            totalDownedTime = 0.0f;

            //��ʒu�ɖ߂�A�j���[�V�����Đ�

            //��ʒu�ɖ߂�
            isReturningNormalPos = true;

            //�U���̎�ނ�����
            ChooseAttackType();
        }
    }

    /// <summary>
    /// ��ʒu�ɖ߂�
    /// </summary>
    void ReturnNormalPos()
    {
        //�ړ�����
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
    /// �_���[�W�ʂɉ��Z����
    /// </summary>
    /// <param name="attackPower">�v���C���[�̍U����</param>
    void AddDamageAmount(float attackPower)
    {
        if (!isDowned)
        {
            damageAmount += attackPower;
        }
    }

    /// <summary>
    /// �G1��HP�����炷
    /// </summary>
    /// <param name="attackPower">�v���C���[�̍U����</param>
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