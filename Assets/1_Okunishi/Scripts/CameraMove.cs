using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public Transform player;  //プレイヤーのTransform

    public float rotationSpeed = 5.0f;
    public Vector3 offset;    //カメラの相対的な位置オフセット

    void Update()
    {
        if (player == null) return;

        //プレイヤーの回転に合わせてカメラを回転
        float horizontal = Input.GetAxis("Mouse X") * rotationSpeed;

        //プレイヤーが移動している場合はプレイヤーも回転
        if (Mathf.Abs(horizontal) > 0.1f)
        {
            player.Rotate(0, horizontal, 0);
        }

        //カメラの位置を更新
        float desiredAngle = player.eulerAngles.y;
        Quaternion rotation = Quaternion.Euler(0, desiredAngle, 0);
        transform.position = player.position - (rotation * offset);
        transform.LookAt(player.position);
    }
}
