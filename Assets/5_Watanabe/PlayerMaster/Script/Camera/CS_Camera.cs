using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Camera : MonoBehaviour
{
    [SerializeField, Header("�J�����̑��x")]
    private float cameraSpeed = 1;
    [SerializeField, Header("�v���C���[�Ƃ�OffsetPosition")]
    private Vector3 offsetPos;

    // �v���C���[��Transform
    private Transform playerTransform = null;

    /// <summary>
    /// �X�^�[�g�C�x���g
    /// </summary>
    private void Start()
    {
        // �J�[�\�����\���ɂ���
        Cursor.visible = false;
        // �J�[�\����^�񒆂ɌŒ�
        Cursor.lockState = CursorLockMode.Locked;

        // �v���C���[�̈ʒu���擾
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    /// <summary>
    /// �X�V�C�x���g
    /// </summary>
    private void Update()
    {
        // �}�E�X�̈ړ��ʂ��擾
        Vector2 mouseMove = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // �J�����̓���
        CameraMove(mouseMove.x, mouseMove.y);

        // �f�o�b�O�p
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// �J�����̓���
    /// </summary>
    /// <param name="mouseX">�}�E�XX�ړ���</param>
    /// <param name="mouseY">�}�E�XY�ړ���</param>
    private void CameraMove(float mouseX, float mouseY)
    {
        // �v���C���[���擾�ł��ĂȂ��ꍇ�������Ȃ�
        if(playerTransform == null)
        {
            Debug.Log("�v���C���[�擾�ł��Ȃ�");
            return;
        }

        transform.position = playerTransform.position +  transform.rotation * offsetPos;
        // X�����Ɉ��ʈړ����Ă���Ή���]
        if (Mathf.Abs(mouseX) > 0.001f)
        {
            // ��]���̓��[���h���W��Y��
            transform.RotateAround(playerTransform.position, Vector3.up, mouseX * cameraSpeed);
        }

    }

}
