using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Enemy1MagicMissile : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float attackPower;
    Transform playerTransform;
    Rigidbody rd;
    Vector3 direction;
    bool isCanFire;  //�ˏo�\���H

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
            Debug.Log("isCanFire = " + isCanFire);
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
    }
}
