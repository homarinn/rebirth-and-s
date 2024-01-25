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
    [SerializeField, Header("���t���N�gSE")]
    private AudioClip SE_Reflect;
    [SerializeField, Header("���˕Ԃ���1")]
    private GameObject reflctBullet;
    [SerializeField, Header("���˕Ԃ���2")]
    private GameObject reflctBullet2;


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
                other.GetComponent<CS_Enemy1>().ReduceHp(attackDamage);
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
        else if(other.gameObject.tag == "MagicMissile")
        {
            Debug.Log("�q�b�g");
            if(other.GetComponent<CS_Enemy1MagicMissile>() != null)
            {
                var type = other.GetComponent<CS_Enemy1MagicMissile>().GetMagicMissileType;
                if(type == "Weak")
                {
                    Instantiate(reflctBullet, transform);
                }
                if(type == "Strong")
                {
                    Instantiate(reflctBullet2, transform);
                }
                Destroy(other.gameObject);
            }

            // �e��AudoSouce���擾
            var audio = cs_Player.GetComponentInParent<AudioSource>();
            if (audio == null)
            {
                Debug.Log("AudioSouce�Ȃ���");
                return;        // AudioSouce���Ȃ�
            }
            audio.PlayOneShot(SE_Reflect);
        }
        else
        {
            return;
        }

    }
}
