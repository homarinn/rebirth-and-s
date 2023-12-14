using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player����X�N���v�g
public class CS_PlayerWeapon : MonoBehaviour
{
    [SerializeField, Header("�U����U��SE")]
    private AudioClip SE_PlayerAttackMis;
    [SerializeField, Header("�U���P�q�b�gSE")]
    private AudioClip SE_PlayerAttack1Hit;
    [SerializeField, Header("�U��2�q�b�gSE")]
    private AudioClip SE_PlayerAttack2Hit;


    private CS_Player cs_Player;

    private float attack1Power;
    private float attack2Power;


    private void Start()
    {
        // �R���|�[�l���g�擾
        cs_Player = GetComponentInParent<CS_Player>();
        attack1Power = cs_Player.Attack1Power;
        attack2Power = cs_Player.Attack2Power;
    }

    private void OnTriggerEnter(Collider other)
    {
        float damage = cs_Player.GetDamage;

        // ��_�ɏՓ˂������_�_���[�W��^����
        if (other.gameObject.tag == "EnemyWeakness")
        {
            if (other.GetComponentInParent<CS_Titan>() != null)
            {
                cs_Player.IsAttack = true;
                other.GetComponentInParent<CS_Titan>().ReceiveDamageOnWeakPoint(damage);
            }
            else
            {
                Debug.Log("�^�C�^���̃R���|�[�l���g���Ȃ���");
            }
            if (cs_Player.GetComponent<AudioSource>() != null)
            {
                if (damage == attack1Power)
                {
                    cs_Player.GetComponent<AudioSource>().PlayOneShot(SE_PlayerAttack1Hit);
                }
                else if (damage == attack2Power)
                {
                    cs_Player.GetComponent<AudioSource>().PlayOneShot(SE_PlayerAttack2Hit);
                }
            }

        }
        if (cs_Player.IsAttack)
        {
            return;
        }

        if (other.gameObject.tag == "Enemy")
        {
            if (other.GetComponent<CS_Titan>() != null)
            {
                cs_Player.IsAttack = true;
                other.GetComponent<CS_Titan>().ReceiveDamage(damage);
            }
            else
            {
                Debug.Log("�^�C�^���̃R���|�[�l���g���Ȃ���");
            }

            if (cs_Player.GetComponent<AudioSource>() != null)
            {
                if (damage == attack1Power)
                {
                    cs_Player.GetComponent<AudioSource>().PlayOneShot(SE_PlayerAttack1Hit);
                }
                else if (damage == attack2Power)
                {
                    cs_Player.GetComponent<AudioSource>().PlayOneShot(SE_PlayerAttack2Hit);
                }
            }
        }
    }
}
