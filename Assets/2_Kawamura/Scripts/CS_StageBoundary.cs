using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class CS_StageBoundary : MonoBehaviour
{
    [Header("�X�e�[�W���E�~�̔��a")]
    [SerializeField] float boundaryCircleRadius;  //�X�e�[�W���E�~�̔��a

    [Header("�͈͐�������ΏۃL�����N�^�[")]
    [SerializeField] GameObject[] character = new GameObject[characterNumber];  //��������Ώ�

    [Header("�X�e�[�W���E�~�͈̔͊m�F(XZ�������̂݊m�F�A�`�F�b�N�Ŕ͈͂�\��)")]
    [SerializeField] bool isCheckRange;  //�����͈͂��m�F���邩�H

    private Transform center;  //�X�e�[�W���E�~�̒��S
    private CapsuleCollider[] capsuleCollider = new CapsuleCollider[characterNumber];  //�R���C�_�[
    private bool[] isUseNavMesh = new bool[characterNumber];  //�i�r���b�V�����g�p���Ă��邩�H  

    private const int characterNumber = 2;  //��������Ώۂ̐�

    // Start is called before the first frame update
    void Start()
    {
        center = gameObject.transform;

        for(int i = 0; i < characterNumber; ++i)
        {
            isUseNavMesh[i] = false;
            if (!character[i])
            {
                continue;
            }

            //�i�r���b�V�����g�p���Ă��邩���ׂ�
            if (character[i].GetComponent<NavMeshAgent>())
            {
                isUseNavMesh[i] = true;  //�g�p���Ă���
            }

            //�R���C�_�[�̎擾
            if (character[i].GetComponent<CapsuleCollider>())
            {
                capsuleCollider[i] = character[i].GetComponent<CapsuleCollider>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //�͈͐������s��(X,Z�������̂�)
        for(int i = 0; i < characterNumber; ++i)
        {
            if (!character[i])
            {
                continue;
            }

            if (isUseNavMesh[i])
            {
                continue;
            }

            //�L�����N�^�[�̔��a���l���������E�~���a���Z�o
            //���a���傫���ق����g�p
            float effectiveRadius = 0.0f;  //�L���ȋ��E�~���a
            float maxScale = Mathf.Max(
                character[i].transform.localScale.x, character[i].transform.localScale.z);
            if (capsuleCollider[i])
            {
                float characterRadius = 0.0f;  //�L�����N�^�[�̔��a
                characterRadius = capsuleCollider[i].radius * maxScale;
                effectiveRadius = boundaryCircleRadius - characterRadius;
            }
            else
            {
                effectiveRadius = boundaryCircleRadius - (maxScale * 0.5f);  //maxScale�̔���
            }

            //X, Z�������݂̂��l�������X�e�[�W�̒��S����̋������Z�o
            Vector2 targetPosition = new Vector2(character[i].transform.position.x,
                                                 character[i].transform.position.z);
            Vector2 centerPosition = new Vector2(center.position.x, center.position.z);
            float distance = (targetPosition - centerPosition).sqrMagnitude;

            //�͈͊O�̏ꍇ�A�ʒu��␳
            if (distance > effectiveRadius * effectiveRadius)
            {
                //���S���W�ɗL���ȋ��E�~���a�̒����̃v���C���[�ւ̃x�N�g�������Z���Đ���
                Vector2 direction = (targetPosition - centerPosition).normalized;
                Vector3 newPosition = new Vector3(
                    centerPosition.x + direction.x * effectiveRadius,
                    character[i].transform.position.y,
                    centerPosition.y + direction.y * effectiveRadius);

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
