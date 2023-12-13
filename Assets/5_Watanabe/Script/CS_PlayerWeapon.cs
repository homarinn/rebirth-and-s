using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player����X�N���v�g
public class CS_PlayerWeapon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        float damage = 0;
        if (other.gameObject.tag == "Enemy")
        {
            Debug.Log("�U��������" + damage);
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
            Debug.Log("��_�ɍU��");
            if (other.GetComponentInParent<CS_Titan>() != null)
            {
                other.GetComponentInParent<CS_Titan>().ReceiveDamageOnWeakPoint(damage);
            }
            else
            {
                Debug.Log("�^�C�^���̃R���|�[�l���g���Ȃ���");
            }
        }
    }
}
