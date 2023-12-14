using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player武器スクリプト
public class CS_PlayerWeapon : MonoBehaviour
{
    [SerializeField, Header("攻撃空振りSE")]
    private AudioClip SE_PlayerAttackMis;
    [SerializeField, Header("攻撃１ヒットSE")]
    private AudioClip SE_PlayerAttack1Hit;
    [SerializeField, Header("攻撃2ヒットSE")]
    private AudioClip SE_PlayerAttack2Hit;


    private CS_Player cs_Player;

    private float attack1Power;
    private float attack2Power;


    private void Start()
    {
        // コンポーネント取得
        cs_Player = GetComponentInParent<CS_Player>();
        attack1Power = cs_Player.Attack1Power;
        attack2Power = cs_Player.Attack2Power;
    }

    private void OnTriggerEnter(Collider other)
    {
        float damage = cs_Player.GetDamage;

        // 弱点に衝突したら弱点ダメージを与える
        if (other.gameObject.tag == "EnemyWeakness")
        {
            if (other.GetComponentInParent<CS_Titan>() != null)
            {
                cs_Player.IsAttack = true;
                other.GetComponentInParent<CS_Titan>().ReceiveDamageOnWeakPoint(damage);
            }
            else
            {
                Debug.Log("タイタンのコンポーネントがないよ");
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
                Debug.Log("タイタンのコンポーネントがないよ");
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
