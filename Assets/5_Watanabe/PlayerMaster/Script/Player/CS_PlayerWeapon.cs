using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player����X�N���v�g
public class CS_PlayerWeapon : MonoBehaviour
{

    [SerializeField, Header("�U���P�q�b�gSE")]
    private AudioClip SE_Attack1Hit;
    [SerializeField, Header("�U��2�q�b�gSE")]
    private AudioClip SE_Attack2Hit;

    private void OnTriggerEnter(Collider other)
    {
        var cs_Player = GetComponentInParent<CS_Player>();
        float attackDamage = cs_Player.AttackDamage;
        //Debug.Log(attackDamage);
        if (attackDamage == 0)
        {
            return;
        }

        // ��_�ɏՓ˂������_�_���[�W��^����
        if (other.gameObject.tag == "EnemyWeakness")
        {
            if (other.GetComponentInParent<CS_Titan>() != null)
            {
                other.GetComponentInParent<CS_Titan>().ReceiveDamageOnWeakPoint();
            } 
        }
        else if (other.gameObject.tag == "Enemy")
        {
            // ��
            if(other.GetComponent<CS_Enemy1>() != null)
            {
            
            }
            // ��
            else if (other.GetComponent<CS_Titan>() != null)
            {
                other.GetComponent<CS_Titan>().ReceiveDamage(attackDamage);
            }
            // �ʂ̎���
            else if(other.GetComponent<CS_EnemyPlayer>() != null)
            {
                other.GetComponent<CS_EnemyPlayer>().ReceiveDamage(attackDamage);
            }
            else
            {
                Debug.Log("�G�ɃR���|�[�l���g���ĂȂ���");
            }
        }
        else
        {
            return;
        }

        // �e��AudoSouce���擾
        var audio = cs_Player.GetComponentInParent<AudioSource>();
        if (audio == null)
        {
            Debug.Log("AudioSouce�Ȃ���");
            return;        // AudioSouce���Ȃ�
        }

        // SE
        if (attackDamage == cs_Player.Attack1Power)
        {
            audio.PlayOneShot(SE_Attack1Hit);
        }
        else if (attackDamage == cs_Player.Attack2Power)
        {
            audio.PlayOneShot(SE_Attack2Hit);
        }
        else if (attackDamage == cs_Player.UltPower)
        {
        }

    }
}
