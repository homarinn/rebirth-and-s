using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Enemy1Trail : MonoBehaviour
{
    bool isPlay;  //再生中か？

    public bool GetSetIsPlay
    {
        get { return isPlay; }
        set { isPlay = value; }
    }

    private void Start()
    {
        isPlay = false;
    }

    /// <summary>
    /// エフェクト再生終了時に呼び出される
    /// </summary>
    //private void OnParticleSystemStopped()
    //{
    //    isPlay = false;
    //}
}
