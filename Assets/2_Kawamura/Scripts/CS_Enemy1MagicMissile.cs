using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Enemy1MagicMissile : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float attackPower;
    [SerializeField] GameObject puddle;
    Transform playerTransform;
    Rigidbody rd;
    Vector3 direction;
    bool isCanFire;  //�ˏo�\���H
    bool isCollisionStage;  //�X�e�[�W�ɓ����������H
    [SerializeField] float disapeearTime;  //������܂ł̎���
    float elapsedTime;
    Vector3 startScale;
    Vector3 targetScale;
    Vector3 puddleStartScale;
    Vector3 puddleTargetScale;
    GameObject puddleObject;

    //�Q�b�^�[�Z�b�^�[
    public bool GetSetIsCanFire
    {
        get { return isCanFire; }
        set { isCanFire = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        rd = GetComponent<Rigidbody>();
        playerTransform = GameObject.Find("Player").transform;
        isCanFire = false;
        isCollisionStage = false;
        elapsedTime = 0.0f;
        startScale = new Vector3(0, 0, 0);
        targetScale = new Vector3(0, 0, 0);
        puddleStartScale = new Vector3(0, 0, 0);
        puddleTargetScale = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        //�v���C���[�Ɍ����Ēe�𔭎˂���
        if (isCanFire)
        {
            //�e�q�֌W���������Ȃ��Ɣ��ˌ���e�̉�]�l���O���ɉe����^���Ă��܂�
            transform.parent = null;

            direction = playerTransform.position - transform.position;
            direction.Normalize();
            rd.velocity = direction * moveSpeed;

            isCanFire = false;
        }

        if (isCollisionStage)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / disapeearTime);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            puddleObject.transform.localScale = Vector3.Lerp(puddleStartScale, puddleTargetScale, t);
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

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }
        if(!isCollisionStage && collision.gameObject.tag == "Stage")
        {
            isCollisionStage = true;
            startScale = transform.localScale;
            Vector3 position = new Vector3(transform.position.x, 0.2f, transform.position.z);
            puddleObject = Instantiate(puddle, position, Quaternion.identity);
            puddleTargetScale = puddleObject.transform.localScale;
            puddleObject.transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
