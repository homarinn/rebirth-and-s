using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_TitanWeaknessCollision : MonoBehaviour
{
    [SerializeField, Header("���l�̃X�N���v�g")]
    private CS_Titan titan;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //��_�ɓ���������_�E�����X�^�[�g������
        titan.StartDown();
    }
}
