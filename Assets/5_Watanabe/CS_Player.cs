using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Player : MonoBehaviour
{
    [SerializeField, Header("カメラのTransformを取得")]
    private Transform cameraTransform = null;

    // =========== 移動 =============== //

    [SerializeField, Header("移動速度")]
    private float moveSpeed = 5.0f;

    [SerializeField, Header("プレイヤーの旋回速度")]
    private float rotationSpeed = 0.5f;

    // ============　攻撃 ============= //

    // =========== 必殺 ============== // 

    // =========== 防御 ============= //


    // ============= 回避 ============ //B

    // ========= ダメージ ============= //
    [SerializeField, Header("ダメージを受けたときの無敵時間")]
    private float mutekiTime = 1;
    private float mutekiTimer = 0;


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
        set
        {
            hp = value;
        }
    }

    // ========== コンポーネント ========= //
    private Rigidbody rb;
    private Animator anim;

    /// <summary>
    /// 実体化したときに呼び出される
    /// </summary>
    private void Awake()
    {
        hp = maxHP;
    }

    /// <summary>
    /// スタートイベント
    /// </summary>
    private void Start()
    {
        // コンポーネントを取得
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
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

        // 移動処理
        Move();

        // 回避処理
        Avoid();

        // 無敵時間だった場合タイマーを減らす
        if(mutekiTimer > 0)
        {
            mutekiTimer -= Time.deltaTime;
        }

    }

    /// <summary>
    /// 移動関数
    /// </summary>
    private void Move()
    {
        // カメラとRigidoBody取得できない場合何もしない
        if(cameraTransform == null)
        {
            Debug.Log("カメラの位置が取得できていない");
            return;
        }

        // 移動入力を取得
        Vector2 inputAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // カメラの方向からX-Z単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(cameraTransform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveForward = cameraForward * inputAxis.y + cameraTransform.right * inputAxis.x;

        // 進行方向に回転
        if (moveForward != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveForward), rotationSpeed);
        }
        if(anim != null)
        {
            anim.SetFloat("Speed", rb.velocity.magnitude);
        }
        // Playerの向いている方向に進む
        rb.velocity = moveForward * moveSpeed + new Vector3(0,rb.velocity.y,0); 
    }

    /// <summary>
    /// 回避関数
    /// </summary>
    private void Avoid()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            anim.SetTrigger("SlidingTrigger");
        }
    }

    /// <summary>
    /// ダメージ関数
    /// </summary>
    public void Damage(int i)
    {
        // 無敵時間だったらダメージを受けない
        if (mutekiTimer <= 0)
        {
            return;
        }
        // HPをへらす
        hp--;
        mutekiTimer = mutekiTime;
        if (anim != null)
        {
            anim.SetTrigger("HitTrigger");
        }
    }

}
