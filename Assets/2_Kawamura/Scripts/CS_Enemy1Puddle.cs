using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Enemy1Puddle : MonoBehaviour
{
    [Header("存在できる時間（秒）")]
    [SerializeField] float existTime;

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

    // Start is called before the first frame update
    void Start()
    {
        elapsed = 0.0f;
        elapsedForDisappear = 0.0f;
        isDisappearing = false;
        startScale = new Vector3(0, 0, 0);
        targetScale = new Vector3(0, 0, 0);

        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(puddleSE);
    }

    // Update is called once per frame
    void Update()
    {
        //存在できる時間が経過したら徐々に縮小する
        elapsed += Time.deltaTime;
        if(elapsed > existTime)
        {
            ReduceScale();
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
}
