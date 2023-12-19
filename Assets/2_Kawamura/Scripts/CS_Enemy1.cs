using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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

    [SerializeField] GameObject player;

    [SerializeField] GameObject weakMagicMissile;
    [SerializeField] GameObject strongMagicMissile;

    [SerializeField] int weakMagicMissileNumber;              //��x�ɐ�������e�̐�
    [SerializeField] int strongMagicMissileNumber;            //��x�ɐ�������e�̐�

    [SerializeField] float halfCircleRadius;            //���~�̔��a
    [SerializeField] Vector3 spawnPos = new Vector3(0.0f, 0.1f, 1.0f);  //�G1����Ƃ����e�̐����ʒu

    [SerializeField] float weakAttackProbability;       //��U���̔����m��
    [SerializeField] float strongAttackProbability;     //���U���̔����m��
    [SerializeField] float blowOffAttackProbability;    //������΂��U���̔����m��

    [SerializeField] float weakCreationInterval;  //�e�𐶐�����Ԋu�i�b�j
    [SerializeField] float strongCreationInterval;  //�e�𐶐�����Ԋu�i�b�j

    [SerializeField] float weakShootInterval;        //���̒e�𔭎˂���܂ł̊Ԋu�i��U���A�b�j
    [SerializeField] float strongShootInterval;        //���̒e�𔭎˂���܂ł̊Ԋu�i��U���A�b�j

    [SerializeField] float maxAttackIntervalSeconds;    //�U���̃C���^�[�o���i�b�j

    //���̃R�[�h���Ŏg�p���邽�߂̕ϐ�
    GameObject[] magicMissile = new GameObject[2]; //�e
    int[] magicMissileNumber = new int[2];         //�e�̐�
    float[] creationInterval = new float[2];       //�e�̐������x�i�b�j
    float[] shootInterval;                         //�A�ˑ��x�i�b�j
    GameObject[] createdMagicMissile;              //���������e
    CS_Enemy1MagicMissile[] script;                //�e�̃X�N���v�g

    float attackIntervalSeconds;
    int magicMissileCount = 1;
    int evenCount = 0;  //�������ڂ̒e���������ꂽ��
    int oddCount = 0;   //1���ڂ���������ڂ̒e���������ꂽ��
    bool isAttack;      //�U�������H

    float[] weight = new float[3];  //�e�U���̏d�݁i�����m���j
    float totalWeight;  //3��ނ̍U���̏d�݁i�����m���j�̑��a

    // Start is called before the first frame update
    void Start()
    {
        //�ϐ��̏�����
        Initialize();

        //�ŏ��̍U����ݒ�
        weight[0] = weakAttackProbability;
        weight[1] = strongAttackProbability;
        weight[2] = blowOffAttackProbability;
        for(int i = 0; i < GetEnumLength<AttackType>(); ++i)
        {
            totalWeight += weight[i];
        }
        ChooseAttackType();


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
        isAttack = false;
        attackIntervalSeconds = maxAttackIntervalSeconds;
    }

    // Update is called once per frame
    void Update()
    {
        //�v���C���[�̕���������
        LookAtPlayer();

        //�U��
        if (isAttack)
        {
            Shoot(attackType);  //�e�𔭎˂���

            //�U�����I�������玟�̍U���̎�ނ�����
            if (!isAttack)
            {
                ChooseAttackType();
            }
            return;
        }

        //�C���^�[�o�����o�߂����玟�̍U���ֈڂ�
        attackIntervalSeconds -= Time.deltaTime;
        if(attackIntervalSeconds <= 0.0f)
        {
            //�e�̐���
            int num = (int)attackType;  //�v�f�ԍ��w��p�ϐ�
            creationInterval[num] -= Time.deltaTime;
            if (creationInterval[num] <= 0.0f)
            {
                CreateMagicMissile(attackType);
            }
        }
    }

    /// <summary>
    /// �e�𔭎˂���܂ł̎��Ԃ�ݒ肷��
    /// </summary>
    /// <param name="type">�U���̎�ށi��E���j</param>
    /// <param name="iteration">���������e�̃C�e���[�V����</param>
    void SetTimeUntilShoot(AttackType type, int iteration)
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
        float randomPoint = UnityEngine.Random.Range(0, totalWeight);

        //���I
        float currentWeight = 0.0f;  //���݂̏d�݂̈ʒu
        for(int i = 0; i < GetEnumLength<AttackType>(); ++i)
        {
            currentWeight += weight[i];

            if(randomPoint < currentWeight)
            {
                SetAttackType(i);
                return;
            }
        }

        //�ʒu���d�݂̑��a�ȏ�Ȃ疖���v�f�Ƃ���
        SetAttackType(weight.Length - 1) ;
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

        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0.0f;  //X����]�𔽉f���Ȃ�
        transform.forward = direction;
    }

    /// <summary>
    /// �e�𐶐�����
    /// </summary>
    /// <param name="type">�U���̎�ށi��E���j</param>
    void CreateMagicMissile(AttackType type)
    {
        int num = (int)type;  //�v�f�ԍ��w��p�ϐ�

        float angleSpace = 180.0f / magicMissileNumber[num];  //�e���m�̊Ԋu
        const float baseAngle = 90.0f;  //1�ڂ̒e�̔z�u�p�x
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
            spawnPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * halfCircleRadius,
            spawnPos.y + Mathf.Sin(angle * Mathf.Deg2Rad) * halfCircleRadius,
            spawnPos.z);
        magicMissilePos = transform.TransformPoint(magicMissilePos);

        //����
        //magicMissileCount��1����n�܂邽��-1
        createdMagicMissile[magicMissileCount - 1] =
            Instantiate(magicMissile[num], magicMissilePos, Quaternion.identity);
        createdMagicMissile[magicMissileCount - 1].transform.SetParent(gameObject.transform);  //�G�ƒe��e�q�֌W��
        script[magicMissileCount - 1] =
            createdMagicMissile[magicMissileCount - 1].GetComponent<CS_Enemy1MagicMissile>();

        //�e�ϐ��̍X�V
        SetTimeUntilShoot(type, magicMissileCount - 1);
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
            isAttack = true;      //�U����
            magicMissileCount = 1;
            evenCount = oddCount = 0;
            creationInterval[num] = 0.0f;  
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

                //������
                createdMagicMissile[i] = null;
                script[i] = null;

                //�Ō�̒e����������U���I��
                if (i == magicMissileNumber[num] - 1)
                {
                    isAttack = false;
                    attackIntervalSeconds = maxAttackIntervalSeconds;
                }
            }
        }
    }
}


//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//public class CS_Enemy1 : MonoBehaviour
//{
//    //�U���̎��
//    enum AttackType
//    {
//        Weak,     //��U��
//        Strong,   //���U��
//        BlowOff,  //������΂��U��
//    }
//    AttackType type;

//    //�e�����U��
//    struct ShootAttack
//    {
//        GameObject magicMissilePrefab;  //�e�̃v���n�u
//        int magicMissileNumber;         //�e�̐�
//        //float Probability;              //�U���̔����m��
//        float creationInterval;         //�e�̐������x�i�b�j
//        float shootInterval;            //�A�ˑ��x�i�b�j
//    }
//    ShootAttack[] shootAttack = new ShootAttack[2];

//    [SerializeField] GameObject player;
//    [SerializeField] GameObject magicMissilePrefab;
//    [SerializeField] int magicMissileNumber;            //��x�ɐ�������e�̐�
//    [SerializeField] float halfCircleRadius;            //���~�̔��a
//    [SerializeField] Vector3 spawnPos = new Vector3(0.0f, 0.1f, 1.0f);  //�G1����Ƃ����e�̐����ʒu

//    [SerializeField] float weakAttackProbability;       //��U���̔����m��
//    [SerializeField] float strongAttackProbability;     //���U���̔����m��
//    [SerializeField] float blowOffAttackProbability;    //������΂��U���̔����m��

//    [SerializeField] float maxCreationIntervalSeconds;  //�e�𐶐�����Ԋu�i�b�j
//    [SerializeField] float maxWeakShootInterval;        //���̒e�𔭎˂���܂ł̊Ԋu�i��U���A�b�j
//    [SerializeField] float maxAttackIntervalSeconds;    //�U���̃C���^�[�o���i�b�j

//    GameObject[] createdMagicMissile;                   //���������e
//    CS_Enemy1MagicMissile[] script;

//    float creationIntervalSeconds;
//    float[] weakShootInterval;    //�e�𔭎˂���܂ł̎���
//    float attackIntervalSeconds;
//    int magicMissileCount = 1;
//    int evenCount = 0;  //�������ڂ̒e���������ꂽ��
//    int oddCount = 0;   //1���ڂ���������ڂ̒e���������ꂽ��
//    bool isAttack;      //�U�������H

//    float[] weight = new float[3];  //�e�U���̏d�݁i�����m���j
//    float totalWeight;  //3��ނ̍U���̏d�݁i�����m���j�̑��a

//    // Start is called before the first frame update
//    void Start()
//    {
//        //�ŏ��̍U����ݒ�
//        weight[0] = weakAttackProbability;
//        weight[1] = strongAttackProbability;
//        weight[2] = blowOffAttackProbability;
//        for(int i = 0; i < GetEnumLength<AttackType>(); ++i)
//        {
//            totalWeight += weight[i];
//        }

//        ChooseAttackType();

//        //�U���̎�ނɍ��킹���l��ϐ��Ɋi�[����



//        //�ϐ��̏�����
//        creationIntervalSeconds = 0.0f;  //1���ڂ͑����ɐ������邽��0
//        isAttack = false;
//        attackIntervalSeconds = maxAttackIntervalSeconds;
//        createdMagicMissile = new GameObject[magicMissileNumber];
//        script = new CS_Enemy1MagicMissile[magicMissileNumber];
//        weakShootInterval = new float[magicMissileNumber];
//        for(int i = 0; i < magicMissileNumber; ++i)
//        {
//            createdMagicMissile[i] = null;
//            script[i] = null;
//            weakShootInterval[i] = 0.0f;
//        }

//        if (magicMissileNumber % 2 == 0)
//        {
//            Debug.Log("�i�G�P�j�������̒e�����ݒ肳��Ă��܂��B����̒e���ɕύX���Ă��������B");
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        ChooseAttackType();
//        Debug.Log(type);

//        //�v���C���[�̕���������
//        LookAtPlayer();

//        //�U��
//        if (isAttack)
//        {
//            Shoot();  //�e�𔭎˂���

//            //�U�����I�������玟�̍U���̎�ނ�����
//            if (!isAttack)
//            {
                
//            }
//            return;
//        }

//        //�C���^�[�o�����o�߂����玟�̍U���ֈڂ�
//        attackIntervalSeconds -= Time.deltaTime;
//        if(attackIntervalSeconds <= 0.0f)
//        {
//            //�e�̐���
//            creationIntervalSeconds -= Time.deltaTime;
//            if (creationIntervalSeconds <= 0.0f)
//            {
//                CreateMagicMissile();
//            }
//        }


//        //���˂�����magicMissileCount�̏�������Y�ꂸ��
//    }

//    /// <summary>
//    /// �e�𔭎˂���܂ł̎��Ԃ�ݒ肷��
//    /// </summary>
//    /// <param name="iteration">���������e�̃C�e���[�V����</param>
//    void SetValueOfTimeUntilShoot(int iteration)
//    {
//        weakShootInterval[iteration] = maxWeakShootInterval * (iteration + 1);
//    }

//    /// <summary>
//    /// �J��o���U���̎�ނ�I��
//    /// </summary>
//    void ChooseAttackType()
//    {
//        //�ʒu��I��
//        float randomPoint = UnityEngine.Random.Range(0, totalWeight);

//        //���I
//        float currentWeight = 0.0f;  //���݂̏d�݂̈ʒu
//        for(int i = 0; i < GetEnumLength<AttackType>(); ++i)
//        {
//            currentWeight += weight[i];

//            if(randomPoint < currentWeight)
//            {
//                SetAttackType(i);
//                return;
//            }
//        }

//        //�ʒu���d�݂̑��a�ȏ�Ȃ疖���v�f�Ƃ���
//        SetAttackType(weight.Length - 1) ;
//    }

//    /// <summary>
//    /// AttackType�^�ϐ��ɒl��ݒ肷��
//    /// </summary>
//    /// <param name="iteration">�C�e���[�V����</param>
//    void SetAttackType(int iteration)
//    {
//        if(iteration == 0)
//        {
//            type = AttackType.Weak;
//        }
//        if(iteration == 1)
//        {
//            type = AttackType.Strong;
//        }
//        if(iteration == 2)
//        {
//            type = AttackType.BlowOff;
//        }
//    }

//    /// <summary>
//    /// enum�^�̗v�f�����擾����
//    /// </summary>
//    /// <typeparam name="T">enum�^</typeparam>
//    /// <returns>�v�f��</returns>
//    int GetEnumLength<T>()
//    {
//        return Enum.GetValues(typeof(T)).Length;
//    }

//    /// <summary>
//    /// �v���C���[�̕���������
//    /// </summary>
//    void LookAtPlayer()
//    {
//        Vector3 direction = player.transform.position - transform.position;
//        direction.y = 0.0f;  //X����]�𔽉f���Ȃ�
//        transform.forward = direction;
//    }

//    /// <summary>
//    /// �e�𐶐�����
//    /// </summary>
//    void CreateMagicMissile()
//    {
//        float angleSpace = 180.0f / magicMissileNumber;  //�e���m�̊Ԋu
//        const float baseAngle = 90.0f;  //1�ڂ̒e�̔z�u�p�x
//        float angle = 0.0f;

//        //���~�̂ǂ��ɔz�u���邩����
//        if (magicMissileCount == 1)  //1����
//        {
//            angle = baseAngle;
//        }
//        else if (magicMissileCount % 2 == 0)  //��������
//        {
//            evenCount++;
//            angle = baseAngle - evenCount * angleSpace;  //�G����݂č��ɏ��ɔz�u
//        }
//        else  //�����
//        {
//            oddCount++;
//            angle = baseAngle + oddCount * angleSpace;   //�G���猩�ĉE�ɏ��ɔz�u
//        }

//        //�G1�̉�]���l�����č��W����
//        Vector3 magicMissilePos = new Vector3(
//            spawnPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * halfCircleRadius,
//            spawnPos.y + Mathf.Sin(angle * Mathf.Deg2Rad) * halfCircleRadius,
//            spawnPos.z);
//        magicMissilePos = transform.TransformPoint(magicMissilePos);

//        //����
//        //magicMissileCount��1����n�܂邽��-1
//        createdMagicMissile[magicMissileCount - 1] = 
//            Instantiate(magicMissilePrefab, magicMissilePos, Quaternion.identity);
//        createdMagicMissile[magicMissileCount - 1].transform.SetParent(gameObject.transform);  //�G�ƒe��e�q�֌W��
//        script[magicMissileCount - 1] =
//            createdMagicMissile[magicMissileCount - 1].GetComponent<CS_Enemy1MagicMissile>();


//        //�e�ϐ��̍X�V
//        SetValueOfTimeUntilShoot(magicMissileCount - 1);
//        magicMissileCount++;
//        creationIntervalSeconds = maxCreationIntervalSeconds;

//        //�S�Đ��������琶�����~�߁A�U���Ɉڂ�
//        if (magicMissileCount > magicMissileNumber)
//        {
//            isAttack = true;      //�U����
//            magicMissileCount = 1;
//            evenCount = oddCount = 0;
//            creationIntervalSeconds = 0.0f;  
//        }
//    }

//    /// <summary>
//    /// �e�𔭎˂���
//    /// </summary>
//    void Shoot()
//    {
//        for(int i = 0; i < magicMissileNumber; ++i)
//        {
//            if (createdMagicMissile[i] == null)
//            {
//                continue;
//            }

//            //���ˎ��ԂɂȂ����甭��
//            weakShootInterval[i] -= Time.deltaTime;
//            if (weakShootInterval[i] < 0.0f)
//            {
//                script[i].GetSetIsCanFire = true;

//                createdMagicMissile[i] = null;
//                script[i] = null;

//                //�Ō�̒e����������U���I��
//                if(i == magicMissileNumber - 1)
//                {
//                    isAttack = false;
//                    attackIntervalSeconds = maxAttackIntervalSeconds;
//                }
//            }
//        }
//    }
//}
