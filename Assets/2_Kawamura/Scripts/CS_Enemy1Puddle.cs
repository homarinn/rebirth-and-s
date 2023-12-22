using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Enemy1Puddle : MonoBehaviour
{
    [SerializeField] float leftTime;
    [SerializeField] float disappearTime;
    float elapsed;
    float elapsedForDisappear;
    bool isDisappearing;
    Vector3 startScale;
    Vector3 targetScale;

    // Start is called before the first frame update
    void Start()
    {
        elapsed = 0.0f;
        elapsedForDisappear = 0.0f;
        isDisappearing = false;
        startScale = new Vector3(0, 0, 0);
        targetScale = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        elapsed += Time.deltaTime;
        if(elapsed > leftTime)
        {
            ReduceScale();
        }
    }

    void ReduceScale()
    {
        if (!isDisappearing)
        {
            isDisappearing = true;
            startScale = transform.localScale;
        }

        elapsedForDisappear += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedForDisappear / disappearTime);
        transform.localScale = Vector3.Lerp(startScale, targetScale, t);
        if(t == 1)
        {
            Destroy(gameObject);
        }

    }
}
