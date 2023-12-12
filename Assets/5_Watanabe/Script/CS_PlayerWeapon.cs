using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_PlayerWeapon : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        float damage = (int)(GetComponentInParent<CS_Player>().AttackPower);
        if (other.gameObject.tag == "Enemy")
        {
            Debug.Log("�U��������������" + damage);
            if (other.GetComponent<CS_Titan>() != null)
            {
                other.GetComponent<CS_Titan>().ReceiveDamage(damage);
            }
            else
            {
                Debug.Log("�^�C�^���̃R���|�[�l���g���Ȃ���");
            }
        }
        // ��_�ɏՓ˂������_�_���[�W��^����
        if(other.gameObject.tag == "EnemyWeakness")
        {
            Debug.Log("��_�ɂ�������");
            if (other.GetComponent<CS_Titan>() != null)
            {
                other.GetComponent<CS_Titan>().ReceiveDamageOnWeakPoint(damage);
            }
            else
            {
                Debug.Log("�^�C�^���̃R���|�[�l���g���Ȃ���");
            }
        }
    }
}
