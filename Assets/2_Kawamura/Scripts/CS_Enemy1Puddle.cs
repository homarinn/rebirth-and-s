using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Enemy1Puddle : MonoBehaviour
{
    [Header("���݂ł��鎞�ԁi�b�j")]
    [SerializeField] float existTime;

    [Header("���ł���X�s�[�h�i�b�j")]
    [SerializeField] float disappearTime;

    [Header("�����萶������SE")]
    [SerializeField] AudioClip puddleSE;

    float elapsed;              //�o�ߎ��ԁi�c�葱���鎞�ԗp�j

    //�X�P�[���ω��Ɏg�p
    float elapsedForDisappear;  //�o�ߎ��ԁi���ŗp�j
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
        //���݂ł��鎞�Ԃ��o�߂����珙�X�ɏk������
        elapsed += Time.deltaTime;
        if(elapsed > existTime)
        {
            ReduceScale();
        }
    }

    /// <summary>
    /// �X�P�[�����k������
    /// </summary>
    void ReduceScale()
    {
        if (!isDisappearing)
        {
            isDisappearing = true;
            startScale = transform.localScale;
        }

        //���X�ɏk��
        elapsedForDisappear += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedForDisappear / disappearTime);
        transform.localScale = Vector3.Lerp(startScale, targetScale, t);

        //������̏���
        if(t == 1)
        {
            Destroy(gameObject);
        }
    }
}
