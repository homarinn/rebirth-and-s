using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Playerスクリプト
public class CS_Player : MonoBehaviour
{

    // ============ 移動 =============  //
    [SerializeField, Header("移動速度")]
    private float moveSpeed = 5.0f;

    [SerializeField, Header("プレイヤーの旋回速度")]
    private float rotationSpeed = 0.5f;

    // ============ 回避 ============= //
    [SerializeField, Header("回避速度")]
    private float slidingSpeed = 10.0f;

    // スライディング中
    private bool slidingNow = false;

    // ============ 攻撃 ============= //

    // ============ 必殺 ============= //
    [SerializeField, Header("必殺のインターバル")]
    private float ultInterval = 5;
    private float ultTimer = 0;
    public float UltTimer
    {
        get
        {
            return Mathf.Clamp(ultTimer,0,  5);
        }
    }

    // ============= 防御 ============ //

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

    [SerializeField, Header("無敵時間")]
    private float invincibleTime = 1;
    // 無敵時間タイマー
    private float invincibleTimer = 0;

    // カメラの位置
    private Transform cameraTransform = null;

    // 死んでいるか　true=死亡 : false=生きている
    private bool isDead = false; 

    // ========== コンポーネント ========= //
    private Rigidbody rb;
    private Animator anim;
    private new AudioSource audio;

    // =========== Sound ======== //
    [SerializeField, Header("攻撃空振りSE")]
    private AudioClip SE_PlayerAttackMis;
    [SerializeField, Header("攻撃１ヒットSE")]
    private AudioClip SE_PlayerAttack1Hit;
    [SerializeField, Header("攻撃2ヒットSE")]
    private AudioClip SE_PlayerAttack2Hit;
    [SerializeField, Header("必殺SE")]
    private AudioClip SE_PlayerSpecalAttack;
    [SerializeField, Header("ダメージSE")]
    private AudioClip SE_PlayerReceiveDamage;
    [SerializeField, Header("移動SE")]
    private AudioClip SE_PlayerMove;
    [SerializeField, Header("スライディング")]
    private AudioClip SE_PlayerEscape;

    /// <summary>
    /// 実体化したときに呼び出される
    /// </summary>
    private void Awake()
    {
        // HPを設定
        hp = maxHP;
        // 必殺インターバル設定
        ultTimer = ultInterval;
    }

    /// <summary>
    /// スタートイベント
    /// </summary>
    private void Start()
    {
        // コンポーネントを取得
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();
        // カメラの位置を取得
        cameraTransform = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Transform>();
    }

    /// <summary>
    /// 更新イベント
    /// </summary>
    private void Update()
    {
        // HPが0以下なら死んでいる
        if(hp <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                if (anim != null)
                {
                    anim.SetTrigger("DeadTrigger");
                }
            }
        }

        // 移動処理
        Move();

        // 回避処理
        Sliding();
    }

    /// <summary>
    /// 移動関数
    /// </summary>
    private void Move()
    {
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
            // アニメーションを再生
            float speed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
            anim.SetFloat("Speed", speed);
            if(audio != null)
            {
                // 移動音を再生
                audio.PlayOneShot(SE_PlayerMove);
            }
        }
        // Playerの向いている方向に進む
        rb.velocity = moveForward * moveSpeed + new Vector3(0,rb.velocity.y,0); 
    }

    #region 回避

    /// <summary>
    /// 回避関数
    /// </summary>
    private void Sliding()
    {
        if(Input.GetKeyDown(KeyCode.LeftShift))
        {
            // スライディング中
            slidingNow = true;
            if(anim != null)
            {
                // スライディング再生
                anim.SetTrigger("SlidingTrigger");
            }
            if(audio != null)
            {
                // スライディング音声
                audio.PlayOneShot(SE_PlayerEscape);
            }
        }

        // スライディング中
        if(slidingNow)
        {
            rb.velocity = transform.forward * slidingSpeed;
        }
    }

    /// <summary>
    /// スライディングアニメーションの終了処理
    /// </summary>
    private void AnimSlidingFiled()
    {
        // スライディングを終了
        slidingNow = false;
    }

    #endregion

    /// <summary>
    /// ダメージ処理
    /// 攻撃した人に読んでもらう
    /// </summary>
    /// <param name="damage">与えるダメージ</param>
    public void ReceiveDamage(float _damage)
    {
        // 無敵状態の場合無効
        if (invincibleTimer <= 0)
        {
            return;
        }
        // damage分Hpを減らす
        hp -= (int)(_damage);
        // 無敵時間を入れる
        invincibleTimer = invincibleTime;

        if(anim != null)
        {            
            // アニメーションを再生
            anim.SetTrigger("HitTrigger");
        }
        if(audio != null)
        {
            // ダメージ音を再生
            audio.PlayOneShot(SE_PlayerReceiveDamage);
        }
    }
}
