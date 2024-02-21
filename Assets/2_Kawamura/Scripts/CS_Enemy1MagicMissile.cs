using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CS_Enemy1MagicMissile : MonoBehaviour
{
    [Header("������")]
    [SerializeField] CS_Enemy1Puddle puddle;

    [Header("�q�b�g�G�t�F�N�g")]
    [SerializeField] GameObject hitEffect;

    [Header("�X�e�[�W�ɐڐG���ď��ł���܂ł̑����i�b�j")]
    [SerializeField] float disappearTime;


    CS_Enemy1Puddle puddleObject;    //������
    Transform playerTransform;
    Rigidbody myRigidbody;
    Vector3 direction;          //�v���C���[�̕���
    float elapsedTime;
    bool isCanFire;             //�ˏo�\���H
    bool isCollisionStage;      //�X�e�[�W�ɓ����������H

    float moveSpeed;
    float attackPower;

    //�e�Ɛ�����̃X�P�[���ω��Ɏg�p
    Vector3 startScale;                    
    Vector3 targetScale;                   

    //�����p
    Vector3 velocity;
    bool isMove;

    //�����p2
    int magicMissileCount;
    Vector3[] curveDirection = new Vector3[3];

    //�����p3
    int puddleRenderQueue;

    float addAngle;

    //Vector3 scaleRatioBasedOnY;

    bool canCreatePuddle;  //�����܂�𐶐��ł��邩�H

    //�X�P�[���傫������p
    Vector3 targetScaleForCreate;
    float elapsedTimeForScaleUp;
    float timeScaleUpCompletely;
    bool isFinishScaleUp;

    bool isCollisionPlayer;

    string magicMissileType;  //�e�̎��

    const float adjustPositionY = 0.3f;

    float towardsSpeed;

    bool isCollisionWeapon;

    //const float rotateSpeed = 518.0f;

    //float time = 0.0f;
    //bool isTowards = false;

    //�Q�b�^�[�Z�b�^�[
    public float SetMoveSpeed
    {
        set { moveSpeed = value; }
    }
    public float SetAttackPower
    {
        set { attackPower = value; }
    }

    public bool GetSetIsCanFire
    {
        get { return isCanFire; }
        set { isCanFire = value; }
    }
    public Transform SetPlayerTransform
    {
        set { playerTransform = value; }
    }
    public int SetMagicMissileCount
    {
        set { magicMissileCount = value; }
    }

    public int SetPuddleRenderQueue
    {
        set { puddleRenderQueue = value; }
    }
    //public Vector3 SetScaleRatioBasedOnY
    //{
    //    set { scaleRatioBasedOnY = value; }
    //}

    public bool SetCanCreatePuddle
    {
        set { canCreatePuddle = value; }
    }
    public Vector3 SetTargetScaleForCreate
    {
        set { targetScaleForCreate = value; }
    }
    public float SetTimeScaleUpCompletely
    {
        set { timeScaleUpCompletely = value; }
    }
    public string SetMagicMissileType
    {
        set
        {
            if(magicMissileType == null)
            {
                magicMissileType = value;
            }
        }
    }
    public string GetMagicMissileType
    {
        get { return magicMissileType; }
    }

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        isCanFire = false;
        isCollisionStage = false;
        elapsedTime = 0.0f;
        startScale = new Vector3(0, 0, 0);
        targetScale = new Vector3(0, 0, 0);

        //�����p
        isMove = false;
        towardsSpeed = 12.5f;

        curveDirection[0] = new Vector3(0, 15.0f, 0);
        curveDirection[1] = new Vector3(0.0f, 5.0f, -20.0f);
        curveDirection[2] = new Vector3(0.0f, 5.0f, 20.0f);

        ////�e�̃X�P�[���𔽉f���Ȃ�Y���v�Z
        //Vector3 parentLossyScale = transform.parent.lossyScale;
        //float scaleY = transform.localScale.y / parentLossyScale.y;
        ////XZ�������䗦�ɂȂ�悤��Y����Ƃ����䗦��Y�ɂ����đ��
        //float newScaleX = scaleY * (scaleRatioBasedOnY.x);  //2.0 = ����
        //float newScaleZ = scaleY * (scaleRatioBasedOnY.z);  //2.0 = ����
        //transform.localScale = new Vector3(
        //    newScaleX,
        //    scaleY,
        //    newScaleZ);


        //Vector3 parentLossyScale = transform.parent.lossyScale;
        //float scaleY = transform.localScale.y / parentLossyScale.y;
        ////XZ�������䗦�ɂȂ�悤��Y����Ƃ����䗦��Y�ɂ����đ��
        //float newScaleXZ = scaleY * (scaleRatioBasedOnY * 2.0f);  //2.0 = ����
        //transform.localScale = new Vector3(
        //    newScaleXZ,
        //    scaleY,
        //    newScaleXZ);

        //�e�̐�[��O�ɂ���
        Transform parent = transform.parent;
        addAngle = 360.0f - transform.parent.transform.localEulerAngles.y;
        transform.rotation = Quaternion.Euler(90f, -90.0f, addAngle);
        //transform.rotation = Quaternion.Euler(90f, 0f, addAngle);  //����MagicMissile�p



        canCreatePuddle = true;
        //transform.rotation = Quaternion.Euler(0f, 0f, 90.0f);

        elapsedTimeForScaleUp = 0.0f;
        isFinishScaleUp = false;

        isCollisionPlayer = false;

        isCollisionWeapon = false;

        Vector3 tar = new Vector3(
            playerTransform.position.x,
            playerTransform.position.y + adjustPositionY,
            playerTransform.position.z);

        direction = tar - transform.position;
        direction.Normalize();
        transform.right = direction;
    }

    // Update is called once per frame
    void Update()
    {
        //��]�����Ȃ���X�P�[�������X�ɑ傫������
        if (!isFinishScaleUp)
        {
            ScaleUp();

            //��]�̎��͕s�K�v
            Vector3 tar = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y + adjustPositionY,
                playerTransform.position.z);

            direction = tar - transform.position;
            direction.Normalize();
            transform.right = direction;

            ////��]
            //Vector3 t = transform.right;
            ////Vector3 t = transform.parent.transform.forward;
            //if (magicMissileCount % 2 == 0)
            //{
            //    t *= -1.0f;
            //}
            //transform.RotateAround(transform.position, t, rotateSpeed * Time.deltaTime);

        }

        //�v���C���[�Ɍ����Ēe�𔭎˂���
        if (isCanFire)
        {
            //�e�q�֌W���������Ȃ��Ɣ��ˌ���e�̉�]�l���O���ɉe����^���Ă��܂�
            if(transform.parent != null)
            {
                transform.parent = null;
            }

            //�v���C���[�Ɍ����Ĕ���
            Vector3 target = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y + adjustPositionY,
                playerTransform.position.z);
            direction = target - transform.position;
            direction.Normalize();
            velocity = direction * moveSpeed;
            myRigidbody.velocity = velocity;

            isCanFire = false;
            isMove = true;
        }

        //�v���C���[�Ɍ����Ĉړ�����
        if (isMove)
        {
            //�i�s�����������Ă��Ȃ��Ƃ������i�s��������������
            if (transform.right != myRigidbody.velocity)
            {
                transform.right =
                    Vector3.Slerp(transform.right, myRigidbody.velocity, Time.deltaTime * towardsSpeed);
            }
        }
        //�������I����Ĕ��ˑO�̎�
        else if(isFinishScaleUp)
        {
            //�v���C���[�̂ق��Ɍ�������
            Vector3 tar = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y + adjustPositionY,
                playerTransform.position.z);

            direction = tar - transform.position;
            direction.Normalize();
            transform.right = direction;

            ////��]�̎��K�v
            //Vector3 tar = new Vector3(
            //    playerTransform.position.x,
            //    playerTransform.position.y + adjustPositionY,
            //    playerTransform.position.z);

            //direction = tar - transform.position;
            //direction.Normalize();
            //if (transform.right != direction)
            //{
            //    transform.right =
            //        Vector3.Slerp(transform.right, direction, Time.deltaTime * 1.5f);
            //}
        }


        //�X�e�[�W�ɐڐG�����珙�X�ɒe�����������A���������������傫������
        if (isCollisionStage)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / disappearTime);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            //�e�̏���
            if (t == 1)
            {
                Destroy(gameObject);
            }
        }

        //�e�̏���
        if (transform.position.y < -5.0f)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �e�����X�ɑ傫������
    /// </summary>
    void ScaleUp()
    {
        elapsedTimeForScaleUp += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTimeForScaleUp / timeScaleUpCompletely);
        transform.localScale = Vector3.Lerp(Vector3.zero, targetScaleForCreate, t);

        if(t >= 1)
        {
            transform.localScale = targetScaleForCreate;
            isFinishScaleUp = true;
        }
    }

    /// <summary>
    /// ������𐶐�����
    /// </summary>
    void CreatePuddle()
    {
        Vector3 position = new Vector3(
            transform.position.x, 0.0f, transform.position.z);
        puddleObject = Instantiate(puddle, position, Quaternion.identity);

        puddleObject.SetRenderQueue = puddleRenderQueue;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (isCollisionStage) { return; }

        //���˂���܂œ����蔻�肵�Ȃ�
        if (!isMove)
        {
            return;
        }

        //�v���C���[�ɓ������Ă����画�肵�Ȃ�(2�񓖂���o�O�΍�)
        if (isCollisionPlayer)
        {
            return;
        }

        //����ɓ������������
        if(!isCollisionStage && other.gameObject.tag == "PlayerWeapon")
        {
            isCollisionWeapon = true;
            Destroy(gameObject);
        }

        //����ɓ������Ă����画�肵�Ȃ�
        if (isCollisionWeapon)
        {
            return;
        }

        //�����܂萶���\�Ȃ�v���C���[�Ƃ̓����蔻�肵�Ȃ�
        //�v���C���[�ɓ���������HP�����炵�Ēe������
        if (!isCollisionStage && other.gameObject.tag == "Player")
        {
            //HP���炷
            var script = other.gameObject.GetComponent<CS_Player>();
            script.ReceiveDamage(attackPower);
            isCollisionPlayer = true;

            //�G�t�F�N�g�o��
            Vector3 instancePosition = other.transform.position;
            instancePosition.y += adjustPositionY * 2.5f;
            Instantiate(hitEffect, instancePosition, Quaternion.identity);

            //����
            Destroy(gameObject);
        }

        ////�G�ɐڐG������G��HP�����炷�i�v���C���[�����˕Ԃ����Ƃ��̂݁j
        //if (isHitBack && other.gameObject.tag == "Enemy")
        //{
        //    var script = other.gameObject.GetComponent<CS_Enemy1>();
        //    script.ReduceHp(attackPower);

        //    Destroy(gameObject);
        //}

        //�����܂�O�ŃX�e�[�W�ɐڐG�����琅����𐶐�
        if (!isCollisionStage && canCreatePuddle && 
            other.gameObject.tag == "Stage")
        {
            if (puddleObject == null)
            {
                isCollisionStage = true;
                startScale = transform.localScale;
                CreatePuddle();
            }
        }

        //�����܂���ŃX�e�[�W�ɐڐG�����琅���܂�͐������Ȃ�
        if (!isCollisionStage && !canCreatePuddle &&
            other.gameObject.tag == "Stage")
        {
            isCollisionStage = true;
            startScale = transform.localScale;
        }
    }
}
