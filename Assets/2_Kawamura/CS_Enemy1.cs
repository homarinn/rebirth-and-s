using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CS_Enemy1 : MonoBehaviour
{
    private GameObject playerObject;
    private GameObject magicMissileSpawnner;
    [SerializeField] GameObject magicMissile;
    [SerializeField] int magicMissileNumber;
    [SerializeField] float radius;

    //�����p
    [SerializeField] float maxAttackInterval;
    private float attackInterval;

    // Start is called before the first frame update
    void Start()
    {
        //�I�u�W�F�N�g�̎擾
        playerObject = GameObject.Find("Player");
        magicMissileSpawnner = transform.Find("MagicMissileSpawnner").gameObject;

        //�ϐ��̏�����
        attackInterval = maxAttackInterval;

        //�e�̐���
        for(int i = 0; i < magicMissileNumber; ++i)
        {
            float angle = i * (360.0f / magicMissileNumber);
            Vector3 pos = new Vector3(
                magicMissileSpawnner.transform.position.x + Mathf.Cos(angle * Mathf.Deg2Rad) * radius,
                magicMissileSpawnner.transform.position.y + Mathf.Sin(angle * Mathf.Deg2Rad) * radius,
                magicMissileSpawnner.transform.position.z);
            GameObject go = Instantiate(magicMissile, pos, magicMissileSpawnner.transform.rotation);
            CS_Enemy1MagicMissile script = go.GetComponent<CS_Enemy1MagicMissile>();
            script.SetBaseAngle = angle;
            script.SetRadius = radius;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //�v���C���[�̕���������
        LookAtPlayer();

        //�e�̐���
        //attackInterval -= Time.deltaTime;
        //if(attackInterval < 0)
        //{
        //    Instantiate(magicMissile, 
        //        magicMissileSpawnner.transform.position, Quaternion.identity);
        //    attackInterval = maxAttackInterval;
        //}
    }

    /// <summary>
    /// �v���C���[�̕���������
    /// </summary>
    void LookAtPlayer()
    {
        Vector3 direction = playerObject.transform.position - transform.position;
        direction.y = 0.0f;  //X����]�𔽉f���Ȃ�
        transform.forward = direction;
    }
}
