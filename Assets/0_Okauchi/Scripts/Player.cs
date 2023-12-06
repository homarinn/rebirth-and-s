using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float speed = 0.0f;
    private float upHeight = 2.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //�ړ��i�e�X�g�p�j
        Vector3 direction = new Vector3(0.0f, 0.0f, 0.0f);
        if (Input.GetKey(KeyCode.W)) direction += transform.forward;
        if (Input.GetKey(KeyCode.S)) direction -= transform.forward;
        if (Input.GetKey(KeyCode.D)) direction += transform.right;
        if (Input.GetKey(KeyCode.A)) direction -= transform.right;

        transform.position += speed * direction.normalized * Time.deltaTime;

        //�㉺�i�e�X�g�p�j
        if (Input.GetKeyDown(KeyCode.Space)) transform.position += new Vector3(0.0f, upHeight, 0.0f);
        if (Input.GetKeyUp(KeyCode.Space))   transform.position -= new Vector3(0.0f, upHeight, 0.0f);
    }
}
