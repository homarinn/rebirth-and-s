using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Enemy1Trail : MonoBehaviour
{
    bool isPlay;  //�Đ������H

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
    /// �G�t�F�N�g�Đ��I�����ɌĂяo�����
    /// </summary>
    //private void OnParticleSystemStopped()
    //{
    //    isPlay = false;
    //}
}
