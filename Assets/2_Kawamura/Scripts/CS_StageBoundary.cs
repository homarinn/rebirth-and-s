using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CS_StageBoundary : MonoBehaviour
{
    private Transform center;  //�X�e�[�W���E�~�̒��S
    [SerializeField] float boundaryCircleRadius;  //�X�e�[�W���E�~�̔��a
    [SerializeField] GameObject[] character = new GameObject[characterNumber];  //��������Ώ�
    [SerializeField] bool isCheckRange;     //�����͈͂��m�F���邩�H

    private const int characterNumber = 2;  //��������Ώۂ̐�

    // Start is called before the first frame update
    void Start()
    {
        center = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < characterNumber; ++i)
        {
            if (!character[i])
            {
                return;
            }

            //X, Z�������̂ݔ͈͂𐧌�����
            Vector2 targetPosition = new Vector2(character[i].transform.position.x,
                                                 character[i].transform.position.z);
            Vector2 centerPosition = new Vector2(center.position.x, center.position.z);
            float distance = (targetPosition - centerPosition).sqrMagnitude;
          
            //�͈͊O�̏ꍇ�A�ʒu��␳
            if (distance > boundaryCircleRadius * boundaryCircleRadius)
            {
                //���S���W�ɔ��a�̒����̃v���C���[�ւ̃x�N�g�������Z���Đ���
                Vector2 direction = (targetPosition - centerPosition).normalized;
                Vector3 newPosition = new Vector3(
                    centerPosition.x + direction.x * boundaryCircleRadius,
                    character[i].transform.position.y,
                    centerPosition.y + direction.y * boundaryCircleRadius);

                character[i].transform.position = newPosition;
            }
        }
    }

    /// <summary>
    /// �����͈͂�\������
    /// </summary>
    private void OnDrawGizmos()
    {
        //�\���A��\���̓C���X�y�N�^�[�Őݒ肵�Ă��������B
        //�X�t�B�A�ŕ\������܂����AY�������̐����͍s��Ȃ��̂�
        //X,Z�������݂̂��Q�l�ɔ͈͂��m�F���Ă��������B
        if (isCheckRange)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(gameObject.transform.position, boundaryCircleRadius);
        }
    }
}
