using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CS_Enemy1Puddle : MonoBehaviour
{
    [Header("�����܂�\�ʂ̃R���C�_�[")]
    [SerializeField] BoxCollider surfaceCollider;

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

    //�����p2
    Material material;
    int renderQueue;
    float boundaryCircleRadius;

    //�Z�b�^�[
    public int SetRenderQueue
    {
        set { renderQueue = value; }
    }
    public float SetBoundaryCircleRadius
    {
        set { boundaryCircleRadius = value; }
    }

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

        material = GetComponent<MeshRenderer>().material;

        //�����p2
        Vector2 direction = new Vector2(
            transform.position.x,
            transform.position.z);
        float distance = direction.sqrMagnitude;
        Debug.Log(distance);

        Vector3 newPos = new Vector3(transform.position.x, 0.0f, transform.position.z);
        float radiusDistance = boundaryCircleRadius * boundaryCircleRadius;
        Debug.Log("RadiusDistance = " + radiusDistance);
        if (distance > radiusDistance * 0.65f)
        {
            newPos.y = 0.35f;
        }
        else if(distance > radiusDistance * 0.3f)
        {
            newPos.y = 0.25f;
        }
        else if(distance > radiusDistance * 0.15f)
        {
            newPos.y = 0.22f;
        }
        //else if(distance > radiusDistance * 0.4f)
        //{
        //    newPos.y = 0.2f;
        //}
        //else if(distance > radiusDistance * 0.3f)
        //{
        //    newPos.y = 0.2f;
        //}
        //else if(distance > radiusDistance * 0.2f)
        //{
        //    newPos.y = 0.15f;
        //}
        else
        {
            newPos.y = 0.12f;
            //newPos.y = 0.08f;
        }
        transform.position = newPos;
        //if (distance > 480.0f)
        //{
        //    newPos.y = 0.35f;
        //    //Vector3 newPos = new Vector3(transform.position.x, 0.35f, transform.position.z);
        //}
        //else if(distance > 300.0f)
        //{
        //    newPos.y = 0.27f;
        //}
        //transform.position = newPos;
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

        //�������I�u�W�F�N�g�̂�����𖳂������߂�RenderQueue���ʂɐݒ�
        if (renderQueue != 0 && material.renderQueue != renderQueue)
        {
            material.renderQueue = renderQueue;
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

    private void OnDestroy()
    {
        if(material != null)
        {
            Destroy(material);
            material = null;
        }
    }

    public void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("�v���C���[�N��");
        }
    }

    //private void OnTriggerStay(Collider other)
    //{
    //    if(other.gameObject.tag == "Player")
    //    {
    //        Debug.Log("�v���C���[�N��");
    //    }
    //    //�g�咆�ɑ��̐����܂�Ɠ���������g��I��
    //    //if(!isFinishExpansion && other.gameObject.tag == "Puddle")
    //    //{
    //    //    Debug.Log("�����܂�ڐG");
    //    //    isFinishExpansion = true;
    //    //}
    //}
}
