using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class CS_Enemy1BlowOffEffect : MonoBehaviour
{
    [Header("���G�t�F�N�g")]
    [SerializeField] ParticleSystem effect;

    [Header("���G�t�F�N�g�̃R���C�_�[")]
    [SerializeField] SphereCollider effectCollider;

    //[Header("������΂��͂̋���")]
    //[SerializeField] float blowOffPower;

    [Header("������΂�SE")]
    [SerializeField] AudioClip blowOffSE;

    float radiusMax;    //���̍ő唼�a
    float radiusMaxTime;//���̔��a���ő�ɂȂ�b��
    float effectDuration;  //�G�t�F�N�g�̍Đ�����
    float blowOffPower;
    float startRadius;
    float elapsed;
    bool isMoveCollider;
    //bool isPlayEffect;

    AudioSource audioSource;

    //�G�t�F�N�g�̍Đ����Ԃ��擾����
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

        //����Shockwave��lifeTime���擾
        ParticleSystem shockwave =
            effect.GetComponent<Transform>().transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        radiusMaxTime = shockwave.main.startLifetime.constant;
        effectDuration = effect.main.duration;

        //radiusMaxTime = effect.main.duration;
        //Debug.Log("radiusMaxTime = " + radiusMaxTime);
        startRadius = 0.0f;
        elapsed = 0.0f;
        isMoveCollider = false;

        //AudioSource�̎擾
        audioSource = GetComponent<AudioSource>();
    }

    public void Update()
    {
        if (isMoveCollider && effectCollider.radius <= radiusMax)
        {
            //�p�[�e�B�N���̍L����ɍ��킹�ăR���C�_�[�͈̔͂��傫������
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / radiusMaxTime);
            effectCollider.radius = Mathf.Lerp(startRadius, radiusMax, t);

            //�ő�l�𒴂�����ő�l�ɂ��ăG�t�F�N�g�Đ��I��
            if (effectCollider.radius > radiusMax)
            {
                effectCollider.radius = radiusMax;
                isMoveCollider = false;
            }
        }
    }

    /// <summary>
    /// ���j����
    /// </summary>
    public void PlayEffect()
    {
        //�G�t�F�N�g�܂߂Ă������������R���[�`��
        StartCoroutine(StopCoroutine());

        //�G�t�F�N�g�ƌ��ʉ��Đ�
        effect.Play();
        audioSource.PlayOneShot(blowOffSE);

        isMoveCollider = true;  //�G�t�F�N�g�Đ�
    }

    private IEnumerator StopCoroutine()
    {
        //���Ԍo�ߌ�ɏ���
        yield return new WaitForSeconds(effectDuration);
        //yield return new WaitForSeconds(radiusMaxTime);
        effect.Stop();
        effectCollider.enabled = false;
        Debug.Log("�G�t�F�N�g�I��");

        Destroy(gameObject);
    }

    /// <summary>
    /// ������΂�����
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        //Player�ȊO�͐�����΂��Ȃ�
        if (other.tag != "Player") return;

        //Rigidbody�����Ă��Ȃ��I�u�W�F�N�g�͐�����΂��Ȃ�
        var rigidBody = other.GetComponentInParent<Rigidbody>();
        if (rigidBody == null) return;

        //���ɂ���Ē������琁����ԕ����̃x�N�g�������
        //var direction = (other.transform.position - transform.position).normalized;

        //��������̕���
        //Vector3 direction = new Vector3(
        //    other.transform.position.x - transform.position.x,
        //    0.0f,
        //    other.transform.position.z - transform.position.z);

        ////�������߂�����΃v���C���[�̌������ɔ�΂�
        //const float rangeNormalDirection = 0.6f * 0.6f;  //�������琁����Ԃ悤�ɂ��锻��͈�
        //if (direction.sqrMagnitude <= rangeNormalDirection)
        //{
        //    //�������ɐ�����΂�
        //    direction.x = -other.transform.forward.x;
        //    direction.z = -other.transform.forward.z;

        //    Debug.Log("������΂����� = " + direction);
        //}
        //else
        //{
        //    //��������O���ɐ�����΂�
        //    direction = direction.normalized;
        //    Debug.Log("������΂�����(�ʏ�) = " + direction);
        //}

        //������΂�
        //ForceMode��ς���Ƌ������ς��i����͎��ʖ����j
        //rigidBody.AddForce(direction * blowOffPower, ForceMode.Impulse);

        var script = other.gameObject.GetComponent<CS_Player>();
        //script.ReceiveDamage(0);

        script.BlowOff(Vector3.zero, 0);

        Debug.Log("������΂���");

        //Collider�𖳌������ĕ�����Player�𐁂���΂��Ȃ��悤�ɂ���
        effectCollider.enabled = false;
    }
}
