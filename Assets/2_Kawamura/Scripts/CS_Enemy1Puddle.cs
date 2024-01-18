using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Enemy1Puddle : MonoBehaviour
{
    [Header("存在できる時間（秒）")]
    [SerializeField] float existTime;

    [Header("水たまりが広がり終えるまでの時間（秒）")]
    [SerializeField] float finishExpansionTime;

    [Header("消滅するスピード（秒）")]
    [SerializeField] float disappearTime;

    [Header("水溜り生成時のSE")]
    [SerializeField] AudioClip puddleSE;

    float elapsed;              //経過時間（残り続ける時間用）

    //スケール変化に使用
    float elapsedForDisappear;  //経過時間（消滅用）
    bool isDisappearing;
    Vector3 startScale;
    Vector3 targetScale;

    AudioSource audioSource;

    //実験用
    bool isFinishExpansion;
    float elapsedForExpansion;

    // Start is called before the first frame update
    void Start()
    {
        elapsed = 0.0f;
        elapsedForDisappear = 0.0f;
        isDisappearing = false;
        startScale = new Vector3(0, 0, 0);

        targetScale = transform.localScale;
        transform.localScale = new Vector3(0, 0, 0);
        //targetScale = new Vector3(0, 0, 0);

        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(puddleSE);

        //実験用
        isFinishExpansion = false;
        elapsedForExpansion = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //拡大する
        if (!isFinishExpansion)
        {
            ExpansionScale();
        }

        //存在できる時間が経過したら徐々に縮小する
        elapsed += Time.deltaTime;
        if(isFinishExpansion && elapsed > existTime)
        {
            ReduceScale();
        }
    }

    /// <summary>
    /// スケールを拡大する
    /// </summary>
    void ExpansionScale()
    {
        //徐々に拡大
        elapsedForExpansion += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedForExpansion / finishExpansionTime);
        transform.localScale = Vector3.Lerp(startScale, targetScale, t);

        //完全に広がったら拡大をやめる
        if (t == 1)
        {
            isFinishExpansion = true;
        }
    }

    /// <summary>
    /// スケールを縮小する
    /// </summary>
    void ReduceScale()
    {
        if (!isDisappearing)
        {
            isDisappearing = true;
            startScale = transform.localScale;

            targetScale = Vector3.zero;
        }

        //徐々に縮小
        elapsedForDisappear += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedForDisappear / disappearTime);
        transform.localScale = Vector3.Lerp(startScale, targetScale, t);

        //水溜りの消去
        if(t == 1)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //拡大中に他の水たまりと当たったら拡大終了
        //if(!isFinishExpansion && other.gameObject.tag == "Puddle")
        //{
        //    Debug.Log("水たまり接触");
        //    isFinishExpansion = true;
        //}
    }
}
