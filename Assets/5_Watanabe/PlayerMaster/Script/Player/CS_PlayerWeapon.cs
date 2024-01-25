using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player武器スクリプト
public class CS_PlayerWeapon : MonoBehaviour
{

    [SerializeField, Header("攻撃１ヒットSE")]
    private AudioClip SE_Attack1Hit;
    [SerializeField, Header("攻撃2ヒットSE")]
    private AudioClip SE_Attack2Hit;
    [SerializeField, Header("リフレクトSE")]
    private AudioClip SE_Reflect;
    [SerializeField, Header("跳ね返す玉1")]
    private GameObject reflctBullet;
    [SerializeField, Header("跳ね返す玉2")]
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

        // 弱点に衝突したら弱点ダメージを与える
        if (other.gameObject.tag == "EnemyWeakness")
        {
            if (other.GetComponentInParent<CS_Titan>() != null)
            {
                other.GetComponentInParent<CS_Titan>().ReceiveDamageOnWeakPoint();
            } 
        }
        else if (other.gameObject.tag == "Enemy")
        {
            // 母
            if(other.GetComponent<CS_Enemy1>() != null)
            {
                other.GetComponent<CS_Enemy1>().ReduceHp(attackDamage);
            }
            // 父
            else if (other.GetComponent<CS_Titan>() != null)
            {
                other.GetComponent<CS_Titan>().ReceiveDamage(attackDamage);
            }
            // 別の自分
            else if(other.GetComponent<CS_EnemyPlayer>() != null)
            {
                other.GetComponent<CS_EnemyPlayer>().ReceiveDamage(attackDamage);
            }
            else
            {
                Debug.Log("敵にコンポーネントついてないよ");
            }

            // 親のAudoSouceを取得
            var audio = cs_Player.GetComponentInParent<AudioSource>();
            if (audio == null)
            {
                Debug.Log("AudioSouceないよ");
                return;        // AudioSouceがない
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
            Destroy(other.gameObject);
            Debug.Log("ヒット");
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
            }

            // 親のAudoSouceを取得
            var audio = cs_Player.GetComponentInParent<AudioSource>();
            if (audio == null)
            {
                Debug.Log("AudioSouceないよ");
                return;        // AudioSouceがない
            }
            audio.PlayOneShot(SE_Reflect);
        }
        else
        {
            return;
        }

    }
}
