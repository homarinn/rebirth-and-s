using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Enemy1DetectGround : MonoBehaviour
{
    [Header("�y���G�t�F�N�g")]
    [SerializeField] ParticleSystem dustCloud;

    [Header("���S���̌��ʉ�")]
    [SerializeField] AudioClip deathSE;

    AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Stage")
        {
            Vector3 pos = transform.position;
            pos.y = 0.1f;
            ParticleSystem p =
                Instantiate(dustCloud, pos, Quaternion.Euler(-90.0f, 0.0f, 0.0f));
            p.Play();
            audioSource.PlayOneShot(deathSE);
            Debug.Log("�X�e�[�W�ɐڐG");
        }
    }
}
