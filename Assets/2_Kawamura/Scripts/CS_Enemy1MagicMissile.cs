using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CS_Enemy1MagicMissile : MonoBehaviour
{
    [Header("������")]
    [SerializeField] CS_Enemy1Puddle puddle;

    [Header("�ړ����x�i�����O���j")]
    [SerializeField] float moveSpeed;

    [Header("�U����")]
    [SerializeField] float attackPower;

    [Header("�X�e�[�W�ɐڐG���ď��ł���܂ł̑����i�b�j")]
    [SerializeField] float disappearTime;

    [Header("���e����܂ł̎��ԁi�b�A�Ȑ��O���p�j")]
    [SerializeField] float period;


    CS_Enemy1Puddle puddleObject;    //������
    Transform playerTransform;
    Rigidbody myRigidbody;
    Vector3 direction;          //�v���C���[�̕���
    float elapsedTime;
    bool isCanFire;             //�ˏo�\���H
    bool isCollisionStage;      //�X�e�[�W�ɓ����������H

    //�e�Ɛ�����̃X�P�[���ω��Ɏg�p
    Vector3 startScale;                    
    Vector3 targetScale;                   
    Vector3 puddleStartScale;
    Vector3 puddleTargetScale;

    //�����p
    Vector3 velocity;
    Vector3 targetPosition;
    bool isMove;
    bool isCurve;
    bool isHitBack;  //���˕Ԃ��ꂽ���H

    //�����p2
    int magicMissileCount;
    Vector3[] curveDirection = new Vector3[3];
    float rotateSpeed;

    //�����p3
    int puddleRenderQueue;

    float boudaryCircleRadius;

    float addAngle;

    float scaleRatioBasedOnY;
    

    //�Q�b�^�[�Z�b�^�[
    public bool GetSetIsCanFire
    {
        get { return isCanFire; }
        set { isCanFire = value; }
    }
    public bool SetIsCurve
    {
        set { isCurve = value; }
    }
    public bool SetIsHitBack
    {
        set { isHitBack = value; }
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
    public float SetBoundaryCircleRadius
    {
        set { boudaryCircleRadius = value; }
    }
    public float SetScaleRatioBasedOnY
    {
        set { scaleRatioBasedOnY = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        //playerTransform = GameObject.Find("Player").transform;
        isCanFire = false;
        isCollisionStage = false;
        elapsedTime = 0.0f;
        startScale = new Vector3(0, 0, 0);
        targetScale = new Vector3(0, 0, 0);
        puddleStartScale = new Vector3(0, 0, 0);
        puddleTargetScale = new Vector3(0, 0, 0);

        //�����p
        isMove = false;
        isHitBack = false;
        rotateSpeed = 10.0f;

        curveDirection[0] = new Vector3(0, 15.0f, 0);
        curveDirection[1] = new Vector3(0.0f, 5.0f, -20.0f);
        curveDirection[2] = new Vector3(0.0f, 5.0f, 20.0f);

        //float randomX = Random.Range(-20.0f, 20.0f);
        //float randomY = Random.Range(-10.0f, 10.0f);
        //float randomZ = Random.Range(-20.0f, 20.0f);
        //velocity = new Vector3(randomX, randomY, randomZ);

        //�e�̃X�P�[���𔽉f���Ȃ�
        Vector3 parentLossyScale = transform.parent.lossyScale;
        float scaleY = transform.localScale.y / parentLossyScale.y;
        float newScaleXZ = scaleY * (scaleRatioBasedOnY * 2.0f);  //2.0 = ����
        transform.localScale = new Vector3(
            newScaleXZ,
            scaleY,
            newScaleXZ);

        //�e�̐�[��O�ɂ���
        Transform parent = transform.parent;
        addAngle = 360.0f - transform.parent.transform.localEulerAngles.y;
        transform.rotation = Quaternion.Euler(90f, 0f, addAngle);


        //transform.rotation = Quaternion.Euler(0f, 0f, 90.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //�v���C���[�Ɍ����Ēe�𔭎˂���
        if (isCanFire)
        {
            //�e�q�֌W���������Ȃ��Ɣ��ˌ���e�̉�]�l���O���ɉe����^���Ă��܂�
            //transform.rotation = Quaternion.Euler(-90f, 0f, -addAngle);
            if(transform.parent != null)
            {
                transform.parent = null;
            }

            //�v���C���[�Ɍ����Ĕ��ˁi�Ȑ��O���j
            if (isCurve)
            {
                targetPosition = playerTransform.position;
                InitializeVelocity();

                isCanFire = false;
                isMove = true;
            }
            //�v���C���[�Ɍ����Ĕ��ˁi�����O���j
            else
            {
                direction = playerTransform.position - transform.position;
                direction.Normalize();
                velocity = direction * moveSpeed;
                myRigidbody.velocity = velocity;

                isCanFire = false;
                isMove = true;
            }
        }

        //�v���C���[�Ɍ����Ĉړ�����
        if (isMove)
        {
            //�Ȑ��O��
            //�����O���͔��ˎ��ɗ͂�^���邾���Ȃ̂ŏ������Ȃ�
            if (isCurve)
            {
                Vector3 acceleration = Vector3.zero;

                direction = targetPosition - transform.position;
                acceleration += (direction - velocity * period) * 2.0f / (period * period);

                period -= Time.deltaTime;
                if (period >= 0)
                {
                    velocity += acceleration * Time.deltaTime;
                    transform.position += velocity * Time.deltaTime;
                }
                else
                {
                    myRigidbody.velocity = velocity;
                    isMove = false;
                }
            }

            //�i�s�����������Ă��Ȃ��Ƃ������i�s��������������
            if(transform.up != velocity)
            //if(transform.forward != velocity)
            {
                transform.up =
                    Vector3.Slerp(transform.up, velocity, Time.deltaTime * rotateSpeed);
                //transform.forward =
                //    Vector3.Slerp(transform.forward, velocity, Time.deltaTime * rotateSpeed);
            }
        }


        //�X�e�[�W�ɐڐG�����珙�X�ɒe�����������A���������������傫������
        if (isCollisionStage)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / disappearTime);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            //puddleObject.transform.localScale = Vector3.Lerp(puddleStartScale, puddleTargetScale, t);

            //�e�̏���
            if(t == 1)
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
    /// ������𐶐�����
    /// </summary>
    void CreatePuddle()
    {
        startScale = transform.localScale;
        Vector3 position = new Vector3(transform.position.x, 0.34f, transform.position.z);
        puddleObject = Instantiate(puddle, position, Quaternion.identity);
        //puddleTargetScale = puddleObject.transform.localScale;
        //puddleObject.transform.localScale = new Vector3(0, 0, 0);

        puddleObject.SetRenderQueue = puddleRenderQueue;
        puddleObject.SetBoundaryCircleRadius = boudaryCircleRadius;
        //Debug.Log(puddleRenderQueue);
    }

    void InitializeVelocity()
    {
        if (magicMissileCount == 1)
        {
            velocity = curveDirection[0];
        }
        else if (magicMissileCount % 2 == 0)
        {
            velocity = curveDirection[1];
        }
        else
        {
            velocity = curveDirection[2];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }

        if(other.gameObject.tag == "Puddle")
        {
            Destroy(gameObject);
        }

        //�G�ɐڐG������G��HP�����炷�i�v���C���[�����˕Ԃ����Ƃ��̂݁j
        if(isHitBack && other.gameObject.tag == "Enemy")
        {
            var script = other.gameObject.GetComponent<CS_Enemy1>();
            script.ReduceHp(attackPower);

            Destroy(gameObject);
        }

        //�X�e�[�W�ɐڐG�����琅����𐶐�
        if (!isCollisionStage && other.gameObject.tag == "Stage")
        {
            if (puddleObject == null)
            {
                isCollisionStage = true;
                CreatePuddle();
            }
            //isCollisionStage = true;
            //startScale = transform.localScale;
            //Vector3 position = new Vector3(transform.position.x, 0.2f, transform.position.z);
            //puddleObject = Instantiate(puddle, position, Quaternion.identity);
            //puddleTargetScale = puddleObject.transform.localScale;
            //puddleObject.transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
