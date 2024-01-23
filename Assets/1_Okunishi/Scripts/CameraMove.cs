using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform player;  //�v���C���[��Transform

    public float rotationSpeed = 5.0f;
    public Vector3 offset;    //�J�����̑��ΓI�Ȉʒu�I�t�Z�b�g

    void Update()
    {
        if (player == null) return;

        //�v���C���[�̉�]�ɍ��킹�ăJ��������]
        float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;

        //�v���C���[���ړ����Ă���ꍇ�̓v���C���[����]
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            player.Rotate(0, horizontal, 0);
        }

        //�J�����̈ʒu���X�V
        float desiredAngle = player.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);
        transform.position = player.position - (rotation * offset);
        transform.LookAt(player.position);
    }
}
