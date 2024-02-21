using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_EffectDestroyer : MonoBehaviour
{
    private float destroyTimeCount;
    public float destroyTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        destroyTimeCount += Time.deltaTime;
        if(destroyTimeCount >= destroyTime)
        {
            Destroy(this.gameObject);
        }
    }
}
