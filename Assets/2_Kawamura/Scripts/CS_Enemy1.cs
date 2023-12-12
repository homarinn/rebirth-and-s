using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CS_Enemy1 : MonoBehaviour
{
    enum AttackType
    {
        Weak,     //��U��
        Strong,   //���U��
        BlowOff,  //������΂��U��
    }

    [SerializeField] GameObject player;
    [SerializeField] GameObject magicMissilePrefab;
    [SerializeField] int magicMissileNumber;            //��x�ɐ�������e�̐�
    [SerializeField] float halfCircleRadius;            //���~�̔��a
    [SerializeField] Vector3 spawnPos = new Vector3(0.0f, 0.1f, 1.0f);  //�G1����Ƃ����e�̐����ʒu

    [SerializeField] float weakAttackProbability;       //��U���̔����m��
    [SerializeField] float strongAttackProbability;     //���U���̔����m��
    [SerializeField] float blowOffAttackProbability;    //������΂��U���̔����m��

    [SerializeField] float maxCreationIntervalSeconds;  //�e�𐶐�����Ԋu�i�b�j
    [SerializeField] float maxWeakShootInterval;        //���̒e�𔭎˂���܂ł̊Ԋu�i��U���A�b�j
    [SerializeField] float maxAttackIntervalSeconds;    //�U���̃C���^�[�o���i�b�j

    GameObject[] createdMagicMissile;                   //���������e
    CS_Enemy1MagicMissile[] script;

    float creationIntervalSeconds;
    float[] weakShootInterval;    //�e�𔭎˂���܂ł̎���
    float attackIntervalSeconds;
    int magicMissileCount = 1;
    int evenCount = 0;  //�������ڂ̒e���������ꂽ��
    int oddCount = 0;   //1���ڂ���������ڂ̒e���������ꂽ��
    bool isAttack;      //�U�������H

    // Start is called before the first frame update
    void Start()
    {
        //�ϐ��̏�����
        creationIntervalSeconds = 0.0f;  //1���ڂ͑����ɐ������邽��0
        isAttack = false;
        attackIntervalSeconds = maxAttackIntervalSeconds;
        createdMagicMissile = new GameObject[magicMissileNumber];
        script = new CS_Enemy1MagicMissile[magicMissileNumber];
        weakShootInterval = new float[magicMissileNumber];
        for(int i = 0; i < magicMissileNumber; ++i)
        {
            createdMagicMissile[i] = null;
            script[i] = null;
            weakShootInterval[i] = 0.0f;
        }

        if (magicMissileNumber % 2 == 0)
        {
            Debug.Log("�i�G�P�j�������̒e�����ݒ肳��Ă��܂��B����̒e���ɕύX���Ă��������B");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //�v���C���[�̕���������
        LookAtPlayer();

        //�U��
        if (isAttack)
        {
            Attack();  //�e�𔭎˂���

            //�U�����I�������玟�̍U���̎�ނ�����
            if (!isAttack)
            {
                
            }
            return;
        }

        //�C���^�[�o�����o�߂����玟�̍U���ֈڂ�
        attackIntervalSeconds -= Time.deltaTime;
        if(attackIntervalSeconds <= 0.0f)
        {
            //�e�̐���
            creationIntervalSeconds -= Time.deltaTime;
            if (creationIntervalSeconds <= 0.0f)
            {
                CreateMagicMissile();
            }
        }


        //���˂�����magicMissileCount�̏�������Y�ꂸ��
    }

    /// <summary>
    /// �e�𔭎˂���܂ł̎��Ԃ�ݒ肷��
    /// </summary>
    /// <param name="iteration">���������e�̃C�e���[�V����</param>
    void SetValueOfTimeUntilShoot(int iteration)
    {
        weakShootInterval[iteration] = maxWeakShootInterval * (iteration + 1);
    }

    /// <summary>
    /// �J��o���U���̎�ނ����肷��
    /// </summary>
    void DecideAttackType()
    {

    }

    /// <summary>
    /// �v���C���[�̕���������
    /// </summary>
    void LookAtPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0.0f;  //X����]�𔽉f���Ȃ�
        transform.forward = direction;
    }

    /// <summary>
    /// �e�𐶐�����
    /// </summary>
    void CreateMagicMissile()
    {
        float angleSpace = 180.0f / magicMissileNumber;  //�e���m�̊Ԋu
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
            Instantiate(magicMissilePrefab, magicMissilePos, Quaternion.identity);
        createdMagicMissile[magicMissileCount - 1].transform.SetParent(gameObject.transform);  //�G�ƒe��e�q�֌W��
        script[magicMissileCount - 1] =
            createdMagicMissile[magicMissileCount - 1].GetComponent<CS_Enemy1MagicMissile>();


        //�e�ϐ��̍X�V
        SetValueOfTimeUntilShoot(magicMissileCount - 1);
        magicMissileCount++;
        creationIntervalSeconds = maxCreationIntervalSeconds;

        //�S�Đ��������琶�����~�߁A�U���Ɉڂ�
        if (magicMissileCount > magicMissileNumber)
        {
            isAttack = true;      //�U����
            magicMissileCount = 1;
            evenCount = oddCount = 0;
            creationIntervalSeconds = 0.0f;  
        }
    }

    /// <summary>
    /// �e�𔭎˂���
    /// </summary>
    void Attack()
    {
        for(int i = 0; i < magicMissileNumber; ++i)
        {
            if (createdMagicMissile[i] == null)
            {
                continue;
            }

            //���ˎ��ԂɂȂ����甭��
            weakShootInterval[i] -= Time.deltaTime;
            if (weakShootInterval[i] < 0.0f)
            {
                script[i].GetSetIsCanFire = true;

                createdMagicMissile[i] = null;
                script[i] = null;

                //�Ō�̒e����������U���I��
                if(i == magicMissileNumber - 1)
                {
                    isAttack = false;
                    attackIntervalSeconds = maxAttackIntervalSeconds;
                }
            }
        }
    }
}
