using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Camera : MonoBehaviour
{
    [SerializeField, Header("カメラの速度")]
    private float cameraSpeed = 1;
    [SerializeField, Header("プレイヤーとのOffsetPosition")]
    private Vector3 offsetPos;

    // プレイヤーのTransform
    private Transform playerTransform = null;

    /// <summary>
    /// スタートイベント
    /// </summary>
    private void Start()
    {
        // カーソルを非表示にする
        Cursor.visible = false;
        // カーソルを真ん中に固定
        Cursor.lockState = CursorLockMode.Locked;

        // プレイヤーの位置を取得
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
    }

    /// <summary>
    /// 更新イベント
    /// </summary>
    private void Update()
    {
        // マウスの移動量を取得
        Vector2 mouseMove = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // カメラの動き
        CameraMove(mouseMove.x, mouseMove.y);

        // デバッグ用
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    /// <summary>
    /// カメラの動き
    /// </summary>
    /// <param name="mouseX">マウスX移動量</param>
    /// <param name="mouseY">マウスY移動量</param>
    private void CameraMove(float mouseX, float mouseY)
    {
        // プレイヤーが取得できてない場合何もしない
        if(playerTransform == null)
        {
            Debug.Log("プレイヤー取得できない");
            return;
        }

        transform.position = playerTransform.position +  transform.rotation * offsetPos;
        // X方向に一定量移動していれば横回転
        if (Mathf.Abs(mouseX) > 0.001f)
        {
            // 回転軸はワールド座標のY軸
            transform.RotateAround(playerTransform.position, Vector3.up, mouseX * cameraSpeed);
        }

    }

}
