using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player����X�N���v�g
public class CS_PlayerWeapon : MonoBehaviour
{
    private float damage;
    private float Damage
    {
        set
        {
            damage = value;
        }
    }

    private void Start()
    {
    }

    private void OnTriggerEnter(Collider other)
    {

        //    // ��_�ɏՓ˂������_�_���[�W��^����
        //    if (other.gameObject.tag == "EnemyWeakness")
        //    {
        //        if (other.GetComponentInParent<CS_Titan>() != null)
        //        {
        //            other.GetComponentInParent<CS_Titan>().ReceiveDamageOnWeakPoint();
        //        }
        //        else
        //        {
        //            Debug.Log("�^�C�^���̃R���|�[�l���g���Ȃ���");
        //        }
        //        if (cs_Player.GetComponent<AudioSource>() != null)
        //        {
        //            if (damage == attack1Power)
        //            {
        //                cs_Player.GetComponent<AudioSource>().PlayOneShot(SE_PlayerAttack1Hit);
        //            }
        //            else if (damage == attack2Power)
        //            {
        //                cs_Player.GetComponent<AudioSource>().PlayOneShot(SE_PlayerAttack2Hit);
        //            }
        //        }

        //    }
        //    if (cs_Player.IsAttack)
        //    {
        //        return;
        //    }

        //    if (other.gameObject.tag == "Enemy")
        //    {
        //        if (other.GetComponent<CS_Titan>() != null)
        //        {
        //            cs_Player.IsAttack = true;
        //            other.GetComponent<CS_Titan>().ReceiveDamage(damage);
        //        }
        //        else
        //        {
        //            Debug.Log("�^�C�^���̃R���|�[�l���g���Ȃ���");
        //        }

        //        if (cs_Player.GetComponent<AudioSource>() != null)
        //        {
        //            if (damage == attack1Power)
        //            {
        //                cs_Player.GetComponent<AudioSource>().PlayOneShot(SE_PlayerAttack1Hit);
        //            }
        //            else if (damage == attack2Power)
        //            {
        //                cs_Player.GetComponent<AudioSource>().PlayOneShot(SE_PlayerAttack2Hit);
        //            }
        //        }
        //    }
    }
}
