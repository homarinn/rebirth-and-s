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

    private CS_Player cs_Player = null;
    private GameObject effReflect = null;
    private Transform trsReflectEffect = null;

    /// <summary>
    /// �X�^�[�g�C�x���g
    /// </summary>
    private void Start()
    {
        // Player�̃X�N���v�g���擾
        cs_Player = GetComponentInParent<CS_Player>();
        effReflect = cs_Player.EffReflct;
        trsReflectEffect = cs_Player.TrsReflectEffect;
    }

    /// <summary>
    /// �Փˏ���
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        float attackDamage = cs_Player.AttackDamage;
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
            if(!cs_Player.AttackOk)
            {
                return;
            }
            cs_Player.AttackOk = false;
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
            // �G�t�F�N�g�쐬
            var eff = Instantiate(effReflect, trsReflectEffect);
            Destroy(eff);

            if (other.GetComponent<CS_Enemy1MagicMissile>() != null)
            {
                // �e�̎�ނ��擾
                var type = other.GetComponent<CS_Enemy1MagicMissile>().GetMagicMissileType;
                // �e�̎�ނɉ����Ē��˕Ԃ��̒e���쐬
                if(type == "Weak")
                {
                    Instantiate(reflctBullet, transform);
                }
                if(type == "Strong")
                {
                    Instantiate(reflctBullet2, transform);
                }

                // ���ł����e���폜
                Destroy(other.gameObject);
            }

            // �e��AudoSouce���擾
            var audio = cs_Player.GetComponentInParent<AudioSource>();
            if (audio == null)
            {
                Debug.Log("AudioSouce�Ȃ���");
                return;
            }
            audio.PlayOneShot(SE_Reflect);
        }
        else
        {
            return;
        }

    }
}
