using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Enemy1MagicMissile : MonoBehaviour
{
    [Header("������")]
    [SerializeField] GameObject puddle;

    [Header("�ړ����x�i�����O���j")]
    [SerializeField] float moveSpeed;

    [Header("�U����")]
    [SerializeField] float attackPower;

    [Header("�X�e�[�W�ɐڐG���ď��ł���܂ł̑����i�b�j")]
    [SerializeField] float disappearTime;

    [Header("���e����܂ł̎��ԁi�b�A�Ȑ��O���p�j")]
    [SerializeField] float period;


    GameObject puddleObject;    //������
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
        float randomX = Random.Range(-20.0f, 20.0f);
        float randomY = Random.Range(-10.0f, 10.0f);
        float randomZ = Random.Range(-20.0f, 20.0f);
        velocity = new Vector3(randomX, randomY, randomZ);
    }

    // Update is called once per frame
    void Update()
    {
        //�v���C���[�Ɍ����Ēe�𔭎˂���
        if (isCanFire)
        {
            //�e�q�֌W���������Ȃ��Ɣ��ˌ���e�̉�]�l���O���ɉe����^���Ă��܂�
            transform.parent = null;

            //�v���C���[�Ɍ����Ĕ��ˁi�Ȑ��O���j
            if(isCurve)
            //if(period != 0.0f)
            {
                targetPosition = playerTransform.position;

                isCanFire = false;
                isMove = true;
            }
            //�v���C���[�Ɍ����Ĕ��ˁi�����O���j
            else
            {
                direction = playerTransform.position - transform.position;
                direction.Normalize();
                myRigidbody.velocity = direction * moveSpeed;

                isCanFire = false;
            }
        }

        //�v���C���[�Ɍ����Ĉړ�����i�Ȑ��O���̎��̂݁j
        if (isMove)
        //if (isMove && period >= 0.0f)
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

        //�X�e�[�W�ɐڐG�����珙�X�ɒe�����������A���������������傫������
        if (isCollisionStage)
        //if (isCollisionStage || period < 0)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / disappearTime);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            puddleObject.transform.localScale = Vector3.Lerp(puddleStartScale, puddleTargetScale, t);

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
        Vector3 position = new Vector3(transform.position.x, 0.2f, transform.position.z);
        puddleObject = Instantiate(puddle, position, Quaternion.identity);
        puddleTargetScale = puddleObject.transform.localScale;
        puddleObject.transform.localScale = new Vector3(0, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
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
