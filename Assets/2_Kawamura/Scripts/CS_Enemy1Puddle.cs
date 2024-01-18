using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Enemy1Puddle : MonoBehaviour
{
    [Header("���݂ł��鎞�ԁi�b�j")]
    [SerializeField] float existTime;

    [Header("�����܂肪�L����I����܂ł̎��ԁi�b�j")]
    [SerializeField] float finishExpansionTime;

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

    //�����p
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

        //�����p
        isFinishExpansion = false;
        elapsedForExpansion = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //�g�傷��
        if (!isFinishExpansion)
        {
            ExpansionScale();
        }

        //���݂ł��鎞�Ԃ��o�߂����珙�X�ɏk������
        elapsed += Time.deltaTime;
        if(isFinishExpansion && elapsed > existTime)
        {
            ReduceScale();
        }
    }

    /// <summary>
    /// �X�P�[�����g�傷��
    /// </summary>
    void ExpansionScale()
    {
        //���X�Ɋg��
        elapsedForExpansion += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedForExpansion / finishExpansionTime);
        transform.localScale = Vector3.Lerp(startScale, targetScale, t);

        //���S�ɍL��������g�����߂�
        if (t == 1)
        {
            isFinishExpansion = true;
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

            targetScale = Vector3.zero;
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

    private void OnTriggerEnter(Collider other)
    {
        //�g�咆�ɑ��̐����܂�Ɠ���������g��I��
        //if(!isFinishExpansion && other.gameObject.tag == "Puddle")
        //{
        //    Debug.Log("�����܂�ڐG");
        //    isFinishExpansion = true;
        //}
    }
}
