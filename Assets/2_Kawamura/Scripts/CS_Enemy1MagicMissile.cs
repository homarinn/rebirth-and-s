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

    //[Header("�����܂�𐶐�����Y���W")]
    //[SerializeField] float puddleCreatePositionY;


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
    //float rotateSpeed;

    //�����p3
    int puddleRenderQueue;

    float addAngle;

    Vector3 scaleRatioBasedOnY;

    bool canCreatePuddle;  //�����܂�𐶐��ł��邩�H

    //float hitBackTime = 0.0f;

    Transform enemyTransform;

    //�X�P�[���傫������p
    Vector3 targetScaleForCreate;
    float elapsedTimeForScaleUp;
    float timeScaleUpCompletely;
    bool isFinishScaleUp;

    bool isCollisionPlayer;

    string magicMissileType;  //�e�̎��


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
    public Vector3 SetScaleRatioBasedOnY
    {
        set { scaleRatioBasedOnY = value; }
    }

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
        //rotateSpeed = 10.0f;

        curveDirection[0] = new Vector3(0, 15.0f, 0);
        curveDirection[1] = new Vector3(0.0f, 5.0f, -20.0f);
        curveDirection[2] = new Vector3(0.0f, 5.0f, 20.0f);

        //float randomX = Random.Range(-20.0f, 20.0f);
        //float randomY = Random.Range(-10.0f, 10.0f);
        //float randomZ = Random.Range(-20.0f, 20.0f);
        //velocity = new Vector3(randomX, randomY, randomZ);

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

        enemyTransform = parent;

        elapsedTimeForScaleUp = 0.0f;
        isFinishScaleUp = false;

        isCollisionPlayer = false;

        Debug.Log("type = " + magicMissileType);
    }

    // Update is called once per frame
    void Update()
    {
        //�X�P�[�������X�ɑ傫������
        if (!isFinishScaleUp)
        {
            ScaleUp();
        }

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
                Vector3 target = new Vector3(
                    playerTransform.position.x,
                    playerTransform.position.y + 0.2f,
                    playerTransform.position.z);

                direction = target - transform.position;
                //direction = playerTransform.position - transform.position;
                direction.Normalize();
                velocity = direction * moveSpeed;
                myRigidbody.velocity = velocity;

                transform.right = velocity;

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

            //hitBackTime += Time.deltaTime;
            //if (!isHitBack && hitBackTime > 0.5f)
            //{
            //    Vector3 to = new Vector3(
            //        enemyTransform.position.x,
            //        enemyTransform.position.y + 3.0f,
            //        enemyTransform.position.z);

            //    direction = to - transform.position;
            //    direction.Normalize();
            //    velocity = direction * moveSpeed;
            //    myRigidbody.velocity = velocity;

            //    Debug.Log("���˕Ԃ�");
            //    //myRigidbody.velocity *= -1.0f;
            //    isHitBack = true;
            //    //transform.right = myRigidbody.velocity;
            //}

            //�i�s�����������Ă��Ȃ��Ƃ������i�s��������������
            //if(transform.up != velocity)
            if (transform.right != myRigidbody.velocity)
            {
                transform.right = myRigidbody.velocity;

                //transform.right =
                //    Vector3.Slerp(transform.right, myRigidbody.velocity, Time.deltaTime * rotateSpeed);
            }
            //if (transform.right != velocity)
            //{
            //    //transform.up =
            //    //    Vector3.Slerp(transform.up, velocity, Time.deltaTime * rotateSpeed);
            //    transform.right =
            //        Vector3.Slerp(transform.right, velocity, Time.deltaTime * rotateSpeed);
            //}


            ////�i�s�����������Ă��Ȃ��Ƃ������i�s��������������
            //if(transform.up != velocity)
            ////if(transform.forward != velocity)
            //{
            //    transform.up =
            //        Vector3.Slerp(transform.up, velocity, Time.deltaTime * rotateSpeed);
            //    //transform.forward =
            //    //    Vector3.Slerp(transform.forward, velocity, Time.deltaTime * rotateSpeed);
            //}
        }


        //�X�e�[�W�ɐڐG�����珙�X�ɒe�����������A���������������傫������
        if (isCollisionStage)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / disappearTime);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            //puddleObject.transform.localScale = Vector3.Lerp(puddleStartScale, puddleTargetScale, t);

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
        //startScale = transform.localScale;
        Vector3 position = new Vector3(
            transform.position.x, 0.0f, transform.position.z);
        puddleObject = Instantiate(puddle, position, Quaternion.identity);
        //puddleTargetScale = puddleObject.transform.localScale;
        //puddleObject.transform.localScale = new Vector3(0, 0, 0);

        puddleObject.SetRenderQueue = puddleRenderQueue;
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

        //�����܂萶���\�Ȃ�v���C���[�Ƃ̓����蔻�肵�Ȃ�
        //�v���C���[�ɓ���������HP�����炵�Ēe������
        if (!isCollisionStage && other.gameObject.tag == "Player")
        {
            //HP���炷
            //var script = other.gameObject.GetComponent<CS_Player>();
            //script.ReceiveDamage(attackPower);
            Debug.Log("�v���C���[�ւ̃_���[�W");
            isCollisionPlayer = true;

            //�G�t�F�N�g�o���H


            //����
            Destroy(gameObject);
        }

        //�G�ɐڐG������G��HP�����炷�i�v���C���[�����˕Ԃ����Ƃ��̂݁j
        if (isHitBack && other.gameObject.tag == "Enemy")
        {
            var script = other.gameObject.GetComponent<CS_Enemy1>();
            script.ReduceHp(attackPower);

            Destroy(gameObject);
        }

        //�����܂�O�ŃX�e�[�W�ɐڐG�����琅����𐶐�
        if (!isCollisionStage && canCreatePuddle && 
            other.gameObject.tag == "Stage")
        {
            if (puddleObject == null)
            {
                isCollisionStage = true;
                startScale = transform.localScale;
                CreatePuddle();
                Debug.Log("�X�e�[�W�ɓ�������");
            }
        }

        //�����܂���ŃX�e�[�W�ɐڐG�����琅���܂�͐������Ȃ�
        if (!isCollisionStage && !canCreatePuddle &&
            other.gameObject.tag == "Stage")
        {
            isCollisionStage = true;
            startScale = transform.localScale;
            Debug.Log("�����܂�ɓ�������");
        }

    }

    //private void OnTriggerExit(Collider other)
    //{
    //    if(other.gameObject.tag == "Puddle")
    //    {
    //        Debug.Log("�����܂���o��");
    //        isExitPuddleRange = true;
    //    }
    //}





    //private void OnTriggerEnter(Collider other)
    //{
    //    //if (isCollisionStage) { return; }

    //    if (other.gameObject.tag == "Player")
    //    {
    //        Destroy(gameObject);
    //    }

    //    if (isExitPuddleRange && other.gameObject.tag == "Puddle")
    //    {
    //        isExitPuddleRange = false;
    //        //Destroy(gameObject);
    //    }

    //    //�G�ɐڐG������G��HP�����炷�i�v���C���[�����˕Ԃ����Ƃ��̂݁j
    //    if (isHitBack && other.gameObject.tag == "Enemy")
    //    {
    //        var script = other.gameObject.GetComponent<CS_Enemy1>();
    //        script.ReduceHp(attackPower);

    //        Destroy(gameObject);
    //    }

    //    //�����܂�O�ŃX�e�[�W�ɐڐG�����琅����𐶐�
    //    if (!isCollisionStage && isExitPuddleRange && 
    //        other.gameObject.tag == "Stage")
    //    {
    //        if (puddleObject == null)
    //        {
    //            isCollisionStage = true;
    //            startScale = transform.localScale;
    //            CreatePuddle();
    //        }
    //    }

    //    if (!isCollisionStage && other.gameObject.tag == "Stage")
    //    {
    //        Debug.Log("�����܂�͈͓��œ�������");
    //        isCollisionStage = true;
    //        startScale = transform.localScale;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if(other.gameObject.tag == "Puddle")
    //    {
    //        Debug.Log("�����܂���o��");
    //        isExitPuddleRange = true;
    //    }
    //}
}
