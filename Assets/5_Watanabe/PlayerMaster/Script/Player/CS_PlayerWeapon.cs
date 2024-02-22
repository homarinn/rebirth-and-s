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

    private CS_Player cs_Player = null;
    private GameObject effReflect = null;
    private Transform trsReflectEffect = null;

    /// <summary>
    /// スタートイベント
    /// </summary>
    private void Start()
    {
        // Playerのスクリプトを取得
        cs_Player = GetComponentInParent<CS_Player>();
        effReflect = cs_Player.EffReflct;
        trsReflectEffect = cs_Player.TrsReflectEffect;
    }

    /// <summary>
    /// 衝突処理
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        float attackDamage = cs_Player.AttackDamage;
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
            if(!cs_Player.AttackOk)
            {
                return;
            }
            cs_Player.AttackOk = false;
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
            // エフェクト作成
            var eff = Instantiate(effReflect, trsReflectEffect);
            Destroy(eff);

            if (other.GetComponent<CS_Enemy1MagicMissile>() != null)
            {
                // 弾の種類を取得
                var type = other.GetComponent<CS_Enemy1MagicMissile>().GetMagicMissileType;
                // 弾の種類に応じて跳ね返しの弾を作成
                if(type == "Weak")
                {
                    Instantiate(reflctBullet, transform);
                }
                if(type == "Strong")
                {
                    Instantiate(reflctBullet2, transform);
                }

                // 飛んできた弾を削除
                Destroy(other.gameObject);
            }

            // 親のAudoSouceを取得
            var audio = cs_Player.GetComponentInParent<AudioSource>();
            if (audio == null)
            {
                Debug.Log("AudioSouceないよ");
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
