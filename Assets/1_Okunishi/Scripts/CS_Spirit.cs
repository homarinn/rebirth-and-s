using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Spirit : MonoBehaviour
{
    [SerializeField, Header("精霊の定位置のTransformを格納する")]
    Transform spiritPositionTransform;

    [SerializeField, Header("プレイヤーのTransformを格納する")]
    Transform playerTransform;

    [SerializeField, Header("プレイヤーを取得する")]
    private CS_Player player;

    //===精霊の移動===
    [SerializeField, Header("精霊の移動速度")]
    private float speed = 5.0f;

    [SerializeField, Header("回復時の精霊のY位置")]
    private float healSpiritPositionY = 0.5f;

    private float startChaseDistance = 0.5f;   //移動を開始する距離
    private bool canMove = true;               //精霊が動く準備ができたかどうかのフラグ

    //===精霊の回転===
    [SerializeField, Header("精霊の回転速度")]
    private float rotationSpeed = 360.0f;

    private float startAngle = 0f;      //精霊回転開始時の角度
    private float currentAngle = 0f;    //精霊の現在の角度

    //===回復===
    [SerializeField, Header("回復クールタイム(秒)")]
    private float healCoolTime = 10.0f;

    [SerializeField, Header("回復量(Playerの最大HPのn%分加算)")]
    private float healPercentage = 50.0f;

    [SerializeField, Header("HPがPlayerの最大HPの何%まで減ったら回復するか")]
    private float healTrrigerPercentage = 50.0f;


    //===音声===
    [SerializeField, Header("回復音声")]
    AudioClip healing_SE;

    AudioSource spiritAudio;    //自身の音源

    //===その他変数===
    private float healAmount = 0.0f;                //回復量
    private float currentCoolTime = 0.0f;           //現在のクールタイム
    private bool healFlag = false;                  //回復フラグ
    private bool log = true;                        //デバッグ表示用

    void Start()
    {
        spiritAudio = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (spiritPositionTransform != null)
        {
            //====================
            //=====精霊の移動=====
            //====================
            {
                //プレイヤーと精霊の距離を計算
                float distanceToSpiritPosition = Vector3.Distance(
                    transform.position, spiritPositionTransform.position);

                //上下にふわふわさせる
                FloatEffectWhileIdle();

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
                    transform.position = Vector3.MoveTowards(transform.position,
                        spiritPositionTransform.position, speed * Time.deltaTime);
                }
            }

            //==============================
            //=====プレイヤーのHPを回復=====
            //==============================
            {
                //プレイヤーのHPが指定した割合を下回ったら回復
                if (player.Hp <= player.MaxHP * (healTrrigerPercentage / 100.0f) && currentCoolTime <= 0.0f)
                {
                    //回復するHPの計算
                    healAmount = player.MaxHP * (healPercentage / 100.0f);

                    if (player.Hp != 0)
                    {
                        healFlag = true;
                    }
                }

                //回復の際、プレイヤーの周りを一回転する
                if (healFlag)
                {
                    RotateAroundPlayer();
                }

                //クールタイム減少
                if (currentCoolTime >= 0.0f)
                {
                    currentCoolTime -= 1.0f * Time.deltaTime;
                }
            }

            //==============================
            //=====デバッグ用===============
            //=====実装時に必ず消すこと=====
            //==============================
            if (Input.GetKey(KeyCode.H) && healCoolTime <= 0)
            {
                healFlag = true;
            }
        }
        else if (log)
        {
            Debug.Log("SpiritPositionが設定されていません。\n inspectorよりSpiritPositionをアタッチしてください。");
            log = false;
        }
    }

    //回復の際、プレイヤーの周りを回転する関数
    void RotateAroundPlayer()
    {
        //回転速度に基づいて角度を更新
        currentAngle += rotationSpeed * Time.deltaTime;

        //プレイヤーの周りを4周したら回転を終了
        if (currentAngle - startAngle >= 1320.0f)
        {
            spiritAudio.PlayOneShot(healing_SE);  //回復音声を鳴らす
            player.Hp += healAmount;              //HP回復
            healFlag = false;                     //回復終わりましたー
            currentCoolTime = healCoolTime;       //回復クールタイムを設定
            currentAngle = 0.0f;                  //現在の角度をリセット

            //HPが最大値を超えないように制限
            player.Hp = Mathf.Min(player.Hp, player.MaxHP);
            return;
        }

        //===回転処理===
        Vector3 rotation = new Vector3(0.0f, currentAngle, 0.0f);
        transform.eulerAngles = rotation;
        transform.position = playerTransform.position +
            Quaternion.Euler(0f, currentAngle, 0f) *
            new Vector3(0f, healSpiritPositionY, -2f);
    }

    //精霊を上下にふわふわさせる処理
    void FloatEffectWhileIdle()
    {
        //ふわふわさせる幅と速さを調整
        float yOffset = Mathf.Sin(Time.time * 3f) * 0.2f;
        transform.position = new Vector3(transform.position.x,
            spiritPositionTransform.position.y + yOffset, transform.position.z);
    }

    //回復するときに外部から呼び出す用
    public void StartHeal()
    {
        healFlag = true;
    }
}
