using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class CS_Enemy1BlowOffEffect : MonoBehaviour
{
    [Header("風エフェクト")]
    [SerializeField] ParticleSystem effect;

    [Header("風エフェクトのコライダー")]
    [SerializeField] SphereCollider effectCollider;

    //[Header("吹き飛ばす力の強さ")]
    //[SerializeField] float blowOffPower;

    [Header("吹き飛ばしSE")]
    [SerializeField] AudioClip blowOffSE;

    float radiusMax;    //風の最大半径
    float radiusMaxTime;//風の半径が最大になる秒数
    float effectDuration;  //エフェクトの再生時間
    float blowOffPower;
    float startRadius;
    float elapsed;
    bool isMoveCollider;
    //bool isPlayEffect;

    AudioSource audioSource;

    //エフェクトの再生時間を取得する
    public float GetEffectDuration
    {
        get { return radiusMaxTime; }
    }

    public float SetBlowOffPower
    {
        set { blowOffPower = value; }
    }

    private void Awake()
    {
        effect.Stop();

        radiusMax = effectCollider.radius;
        effectCollider.radius = 0.0f;

        //孫のShockwaveのlifeTimeを取得
        ParticleSystem shockwave =
            effect.GetComponent<Transform>().transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        radiusMaxTime = shockwave.main.startLifetime.constant;
        effectDuration = effect.main.duration;

        //radiusMaxTime = effect.main.duration;
        //Debug.Log("radiusMaxTime = " + radiusMaxTime);
        startRadius = 0.0f;
        elapsed = 0.0f;
        isMoveCollider = false;

        //AudioSourceの取得
        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (isMoveCollider && effectCollider.radius <= radiusMax)
        {
            //パーティクルの広がりに合わせてコライダーの範囲も大きくする
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / radiusMaxTime);
            effectCollider.radius = Mathf.Lerp(startRadius, radiusMax, t);

            //最大値を超えたら最大値にしてエフェクト再生終了
            if (effectCollider.radius > radiusMax)
            {
                effectCollider.radius = radiusMax;
                isMoveCollider = false;
            }
        }
    }

    /// <summary>
    /// 爆破する
    /// </summary>
    public void PlayEffect()
    {
        //エフェクト含めてもろもろを消すコルーチン
        StartCoroutine(StopCoroutine());

        //エフェクトと効果音再生
        effect.Play();
        audioSource.PlayOneShot(blowOffSE);

        isMoveCollider = true;  //エフェクト再生
    }

    private IEnumerator StopCoroutine()
    {
        //時間経過後に消す
        yield return new WaitForSeconds(effectDuration);
        //yield return new WaitForSeconds(radiusMaxTime);
        effect.Stop();
        effectCollider.enabled = false;
        Debug.Log("エフェクト終了");

        Destroy(gameObject);
    }

    /// <summary>
    /// 吹き飛ばし処理
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        //Player以外は吹き飛ばさない
        if (other.tag != "Player") return;

        //Rigidbodyがついていないオブジェクトは吹き飛ばさない
        var rigidBody = other.GetComponentInParent<Rigidbody>();
        if (rigidBody == null) return;

        //風によって中央から吹き飛ぶ方向のベクトルを作る
        //var direction = (other.transform.position - transform.position).normalized;

        //中央からの方向
        //Vector3 direction = new Vector3(
        //    other.transform.position.x - transform.position.x,
        //    0.0f,
        //    other.transform.position.z - transform.position.z);

        ////距離が近すぎればプレイヤーの後ろ方向に飛ばす
        //const float rangeNormalDirection = 0.6f * 0.6f;  //中央から吹き飛ぶようにする判定範囲
        //if (direction.sqrMagnitude <= rangeNormalDirection)
        //{
        //    //後ろ方向に吹き飛ばす
        //    direction.x = -other.transform.forward.x;
        //    direction.z = -other.transform.forward.z;

        //    Debug.Log("吹き飛ばす方向 = " + direction);
        //}
        //else
        //{
        //    //中央から外側に吹き飛ばす
        //    direction = direction.normalized;
        //    Debug.Log("吹き飛ばす方向(通常) = " + direction);
        //}

        //吹き飛ばす
        //ForceModeを変えると挙動が変わる（今回は質量無視）
        //rigidBody.AddForce(direction * blowOffPower, ForceMode.Impulse);

        var script = other.gameObject.GetComponent<CS_Player>();
        //script.ReceiveDamage(0);

        script.BlowOff(Vector3.zero, 0);

        Debug.Log("吹き飛ばした");

        //Colliderを無効化して複数回Playerを吹き飛ばさないようにする
        effectCollider.enabled = false;
    }
}
