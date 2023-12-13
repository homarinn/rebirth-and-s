using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// �ǔ���Ԃ̊Ǘ��N���X
/// </summary>
public class CS_EnemyChaseState : CS_IEnemyState
{
    /// <summary> 
    /// �U���̃g���K�[�ƂȂ�v���C���[�Ƃ̋���
    /// </summary>
    [Header("�U���̃g���K�[�ƂȂ�v���C���[�Ƃ̋���")]
    [SerializeField] private float triggerDistance;

    /// <summary> �ǔ����x </summary>
    [Header("�ǔ����x")]
    [SerializeField] private float chaseSpeed;

    [Header("�U������m��(%)")]
    [SerializeField] private float probability;

    /// <summary> �GAI����p </summary>
    private NavMeshAgent enemyAi;

    /// <summary> �l�q����Ԃ̊Ǘ��N���X </summary>
    private CS_EnemyKeepState keepState;

    private void Awake()
    {
        // �GAI����p�R���|�[�l���g���擾
        enemyAi = gameObject.GetComponent<NavMeshAgent>();

        // �l�q����Ԃ̊Ǘ��N���X���擾
        keepState = gameObject.GetComponent<CS_EnemyKeepState>();
    }

    /// <summary>
    /// �ǔ���Ԃ̏�������
    /// </summary>
    public override void StartState()
    {
        Debug.Log("�ǔ����");

        // �ǔ����x��ݒ�
        enemyAi.speed = chaseSpeed;
    }

    /// <summary>
    /// �ǔ���Ԃ̏��������s����
    /// </summary>
    /// <returns> ���̏�Ԃ̊Ǘ��N���X </returns>
    public override CS_IEnemyState RunState(Transform player)
    {
        // �GAI�̑��삪�ł��Ȃ��ꍇ
        if (!enemyAi)
        {
            Debug.Log("NavMeshAgent������܂���");

            // null��Ԃ��ҋ@��Ԃɂ���
            return null;
        }

        // �v���C���[�Ɍ������Ĉړ�
        enemyAi.SetDestination(player.position);

        // �v���C���[�Ƃ̋������m�F
        if (CheckDistance(player))
        {
            // �v���C���[�ɋ߂Â����̂�
            // �l�q����ԂɈڍs
            return keepState;
        }

        // �v���C���[���痣��Ă���̂�
        // �ǔ���Ԃ��ێ�
        return this;
    }

    /// <summary>
    /// �^�[�Q�b�g�Ƃ̋����𒲂ׂ�
    /// </summary>
    /// <param name="target"> �Ώ� </param>
    /// <returns>
    /// <para> true : �^�[�Q�b�g�Ƃ̋��������ȓ� </para>
    /// <para> false : �^�[�Q�b�g�Ƃ̋��������ȏ� </para>
    /// </returns>
    public bool CheckDistance(Transform target)
    {
        // �^�[�Q�b�g�Ƃ̋���
        float distance = Vector3.Distance(
            target.position, transform.position);

        // ���ȓ��Ƀ^�[�Q�b�g������̂�
        return distance < triggerDistance;
    }
}
