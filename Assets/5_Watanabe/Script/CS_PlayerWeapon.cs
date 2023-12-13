using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Player武器スクリプト
public class CS_PlayerWeapon : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        float damage = GetComponentInParent<CS_Player>().GetDamage;
        if (other.gameObject.tag == "Enemy")
        {
            if (other.GetComponent<CS_Titan>() != null)
            {
                other.GetComponent<CS_Titan>().ReceiveDamage(damage);
            }
            else
            {
                Debug.Log("タイタンのコンポーネントがないよ");
            }
        }
        // 弱点に衝突したら弱点ダメージを与える
        if(other.gameObject.tag == "EnemyWeakness")
        {
            if (other.GetComponentInParent<CS_Titan>() != null)
            {
                other.GetComponentInParent<CS_Titan>().ReceiveDamageOnWeakPoint(damage);
            }
            else
            {
                Debug.Log("タイタンのコンポーネントがないよ");
            }
        }
    }
}
