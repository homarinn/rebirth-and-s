using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Spirit : MonoBehaviour
{
    //[SerializeField, Header("精霊の定位置のTransformを格納する変数")]
    [SerializeField] Transform spiritPositionTransform;         

    [SerializeField] Transform playerTransform;   //プレイヤーのTransformを格納する変数
    
    //===精霊の移動===
    [SerializeField] private float speed = 5.0f;                //精霊の速度
    [SerializeField] private float startChaseDistance = 0.5f;   //移動を開始する距離
    private bool canMove = true;                                //精霊が動く準備ができたかどうかのフラグ

    //===精霊の回転===
    [SerializeField] private float rotationSpeed = 180f; //精霊の回転速度
    private float startAngle = 0f;      //精霊回転開始時の角度
    private float currentAngle = 0f;    //精霊の現在の角度

    //===回復===
    [SerializeField] private float healCoolTime = 0.0f;  //回復のクールタイム
    [SerializeField] private float healAmount = 50.0f;   //回復量
    
    //===その他変数===
    private float distanceToSpiritPosition = 0.0f;  //精霊の定位置と現在地の差
    private bool healFlag = false;                  //回復フラグ


    void Start()
    {
    }

    void Update()
    {
        if (spiritPositionTransform != null)
        {
            
            //プレイヤーと精霊の距離を計算
            distanceToSpiritPosition = Vector3.Distance(transform.position, spiritPositionTransform.position);

            //一定距離を超えたら移動を開始
            if (distanceToSpiritPosition >= startChaseDistance)
            {
                canMove = true;
            }
            //定位置に戻ってきたら移動停止
            if (distanceToSpiritPosition < 0.01f)
            {
                canMove = false;
            }
            //精霊の定位置に向かって移動する
            if (canMove && !healFlag)
            {        
                transform.position = Vector3.MoveTowards(transform.position, spiritPositionTransform.position, speed * Time.deltaTime);
            }


            //回復の際、プレイヤーの周りを一回転する
            if (healFlag)
            {
                RotateAroundPlayer();
            }

            //クールタイム減少
            if (healCoolTime >= 0.0f)
            {
                healCoolTime -= 0.1f * Time.deltaTime;
            }

            //===デバッグ用===
            if (Input.GetKey(KeyCode.H) && healCoolTime <= 0)
            {
                healFlag = true;
            }
        }
    }

    //回復の際、プレイヤーの周りを回転する関数
    void RotateAroundPlayer()
    {
        //回転速度に基づいて角度を更新
        currentAngle += rotationSpeed * Time.deltaTime;

        //プレイヤーの周りを1周したら回転を終了
        if (currentAngle - startAngle >= 360f)
        {
            healFlag = false;       //回復終わりましたー
            healCoolTime = 1.0f;    //回復クールタイムを設定
            currentAngle = 0;       //現在の角度をリセット
            return;
        }

        //回転処理
        Vector3 rotation = new Vector3(0f, currentAngle, 0f);
        transform.eulerAngles = rotation;
        transform.position = playerTransform.position + Quaternion.Euler(0f, currentAngle, 0f) * new Vector3(0f, 0.5f, -2f);  //プレイヤーの周りを半径2の円周上に配置
    }

    //回復するときに外部から呼び出す用
    public void StartHeal()
    {
        healFlag = true;
    }
}
