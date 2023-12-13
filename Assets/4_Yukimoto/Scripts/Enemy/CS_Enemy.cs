using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// �G�̊Ǘ��N���X
/// </summary>
public class CS_Enemy : MonoBehaviour
{
    /// <summary> �v���C���[ </summary>
    [Header("�v���C���[�擾")]
    [SerializeField] private Transform player;

    /// <summary> �A�j���[�V��������p </summary>
    private Animator anim;

    /// <summary> �����p </summary>
    private Rigidbody rd;

    /// <summary> ���݂̏�� </summary>
    private CS_IEnemyState currentState;

    /// <summary> ���̏�� </summary>
    private CS_IEnemyState nextState;

    /// <summary> �ҋ@��Ԃ̊Ǘ��N���X </summary>
    private CS_EnemyIdleState idleState;

    // Start is called before the first frame update
    void Start()
    {
        // �A�j���[�V��������p�R���|�[�l���g���擾
        anim = gameObject.GetComponent<Animator>();

        // �����p�R���|�[�l���g���擾
        rd = gameObject.GetComponent<Rigidbody>();

        // �ǔ���Ԃ̊Ǘ��N���X���擾
        currentState = gameObject.GetComponent<CS_EnemyChaseState>();

        // �ҋ@��Ԃ̊Ǘ��N���X���擾
        idleState = gameObject.GetComponent<CS_EnemyIdleState>();

        // ��Ԃ̏�������
        currentState.StartState();
    }

    // Update is called once per frame
    void Update()
    {
        // �v���C���[���ݒ肳��Ă��Ȃ��̂ŁA
        // ���������s���Ȃ��悤�ɂ���
        if (!player)
        {
            Debug.Log("�v���C���[���ݒ肳��Ă��܂���");
            return;
        }

        // �R���|�[�l���g�m�F
        if (!CheckComponent())
        {
            return;
        }

        // ��Ԃ��ݒ肳��Ă��邩�m�F
        if (!currentState)
        {
            Debug.Log("��Ԃ��ݒ肳��Ă��܂���");
        }

        // ��Ԃ̏��������s���A
        // ���̏�Ԃ��擾����
        nextState = currentState.RunState(player);

        anim.SetFloat("Speed", rd.velocity.magnitude);

        // ���̏�Ԃ��w�肳��ĂȂ��ꍇ
        if (!nextState)
        {
            Debug.Log("���̏�Ԃ����݂��܂���");

            // �ҋ@��Ԃɂ���
            currentState = idleState;
        }

        // ���݂ƈႤ��Ԃ��w�肳�ꂽ�ꍇ
        if (nextState != currentState)
        {
            // ���̏�Ԃֈڍs����
            currentState = nextState;

            // ��������
            currentState.StartState();
        }
    }

    /// <summary>
    /// �R���|�[�l���g������̂��m�F����
    /// </summary>
    /// <returns>
    /// <para> true : �R���|�[�l���g������ </para>
    /// <para> false : �R���|�[�l���g���Ȃ� </para>
    /// </returns>
    private bool CheckComponent()
    {
        // �A�j���[�V��������p�R���|�[�l���g���Ȃ�
        if (!anim)
        {
            Debug.Log("Animator������܂���");
            return false;
        }

        // �����p�R���|�[�l���g���Ȃ�
        if (!rd)
        {
            Debug.Log("Rigidbody������܂���");
            return false;
        }

        return true;
    }
}
