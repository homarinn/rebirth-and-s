using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Player : MonoBehaviour
{
    [SerializeField, Header("カメラのTransformを取得")]
    private Transform cameraTransform = null;

    // =========== 移動 =============== //
    [SerializeField, Header("移動加速度")]
    private float moveAcceleration = 3.0f;

    [SerializeField, Header("移動減速度")]
    private float moveDecelerate = 5.0f;

    [SerializeField, Header("最高加速度")]
    private float moveMaxSpeed = 5.0f;

    [SerializeField, Header("水たまり上の最高速度")]
    private float moveOnTheWaterMaxSpeed = 0.5f;

    [SerializeField, Header("プレイヤーの旋回速度")]
    private float rotationSpeed = 0.5f;

    // 移動速度
    [SerializeField]
    private float moveSpeed = 0f;
    // 水たまりの上かどうか
    private bool isOnTheWater = false;

    // ============　攻撃 ============= //
    [SerializeField, Header("攻撃１の威力")]
    private int attack1Damage = 1;

    [SerializeField, Header("攻撃2の威力")]
    private int attack2Damage = 2;

    [SerializeField, Header("攻撃アクション後の、他の行動へのインターバル")]
    private float attackToActionoInterval = 0;

    // コンボの回数
    private int attackCount = 0;

    // =========== 必殺 ============== // 
    [SerializeField, Header("必殺技の威力")]
    private int specalAttackDamage = 10;

    [SerializeField, Header("必殺技発動へのインターバル")]
    private int specalAttackInterval = 0;

    // =========== 防御 ============= //
    [SerializeField, Header("防御中ダメージカット率")]
    private float damageCut = 0;
    public float DamageCut
    { 
        get
        {
            return damageCut;
        } 
    }

    [SerializeField, Header("防御後の、他への行動のインターバル")]
    private float deffecToActionInterval = 0.0f;


    // ============= 回避 ============ //
    [SerializeField, Header("回避アクションの時間")]
    private float avoidTime = 1.0f;

    [SerializeField, Header("回避アクションの速度")]
    private float avoidSpeed = 1.0f;

    [SerializeField, Header("回避アクション後の、他への行動のインターバル")]
    private float avoidToActionInterval = 0.0f;    


    // ========== ステータス ============= //
    [SerializeField, Header("プレイヤーのMaxHP")]
    private int maxHP = 200;
    [SerializeField, Header("プレイヤーのHP")]
    private int hp;
    public int Hp {
        get
        {
            return hp;
        }
    }

    // ========== コンポーネント ========= //
    private Rigidbody rb;
    private Animator anim;

    /// <summary>
    /// スタートイベント
    /// </summary>
    private void Start()
    {
        // コンポーネントを取得
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        // ステータスの初期化
        hp = maxHP;
    }

    /// <summary>
    /// 更新イベント
    /// </summary>
    private void Update()
    {
        // コンポーネントが取得できていない場合Logを出す
        if(rb == null || anim == null)
        {
            Debug.Log("コンポーネントが取得できていない");
            return;
        }
        // 移動入力を取得
        Vector2 moveAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // 移動処理
        Move(moveAxis.x, moveAxis.y);

        // 攻撃処理
        Attack();

        // 防御処理
        if(Input.GetMouseButton(1))
        {
            Defence();
        }

        Avoid();

        // 必殺処理
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SpecialAttack();
        }
    }

    /// <summary>
    /// 移動関数
    /// </summary>
    private void Move(float horizontal, float vertical)
    {
        // カメラとRigidoBody取得できない場合何もしない
        if(cameraTransform == null)
        {
            Debug.Log("カメラの位置が取得できていない");
            return;
        }

        // カメラの方向からX-Z単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveForward = cameraForward * vertical + cameraTransform.right * horizontal;

        // 進行方向に回転
        if (moveForward != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveForward), rotationSpeed);
        }
        if(moveForward != Vector3.zero)
        {
            // 移動入力されているときは加速する
            moveSpeed += moveAcceleration * Time.deltaTime;
        }
        else if (vertical == 0 && horizontal == 0)
        {
            // 何もしていないときは減速する
            moveSpeed -= moveDecelerate * Time.deltaTime;
        }
        // 速度を補正
        if (!isOnTheWater)
        {
            moveSpeed = Mathf.Clamp(moveSpeed, 0, moveMaxSpeed);
        }
        else
        {
            moveSpeed = Mathf.Clamp(moveSpeed, 0, moveOnTheWaterMaxSpeed);
        }
        if(anim != null)
        {
            anim.SetFloat("Speed", moveSpeed);
        }
        // Playerの向いている方向に進む
        rb.velocity = transform.forward * moveSpeed;
    }

    /// <summary>
    /// 回避関数
    /// </summary>
    private void Avoid()
    {
        // 回避処理
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            
            rb.AddForce(transform.forward * 50, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// 防御関数
    /// </summary>
    private void Defence()
    {

    }

    /// <summary>
    /// 攻撃関数
    /// </summary>
    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {

            switch (attackCount)
            {
                case 0:
                    break;
                case 1:
                    break;
            }
        }
    }

    /// <summary>
    /// 必殺技関数
    /// </summary>
    private void SpecialAttack()
    {

    }

    /// <summary>
    /// ダメージ関数
    /// </summary>
    public void Damage()
    {
        // HPをへらす
        hp--;
        if(anim != null)
        {
            anim.SetTrigger("Hit");
        }
    }
}
