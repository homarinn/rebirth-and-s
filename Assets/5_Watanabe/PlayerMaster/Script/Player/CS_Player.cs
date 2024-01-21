using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Player : MonoBehaviour
{
    // ステート
    enum State
    {
        None,
        Attack,
        Ult,
        Difence,
        Sliding,
        Hit,
        Death
    }
    State state;

    // カメラの位置
    private Transform cameraTransform = null;

    // HP
    [SerializeField, Header("HPの最大値")]
    private int maxHP = 200;
    private int hp;
    public int Hp{ get{ return hp; }}

    // Move
    [SerializeField, Header("移動速度")]
    private float moveSpeed = 0;
    [SerializeField, Header("旋回速度")]
    private float rotationSpeed = 0;

    private float ultTimer = 0; // 必殺技タイマー
    public float UltTimer{ get{ return Mathf.Clamp(ultTimer, 0, 5); }}

    // Component
    private Rigidbody rb;
    private Animator anim;
    private new AudioSource[] audio;

    // SE
    [SerializeField, Header("必殺SE")]
    private AudioClip SE_PlayerSpecalAttack;
    [SerializeField, Header("ダメージSE")]
    private AudioClip SE_PlayerReceiveDamage;
    [SerializeField, Header("移動SE")]
    private AudioClip SE_PlayerMove;
    [SerializeField, Header("スライディング")]
    private AudioClip SE_PlayerEscape;
    [SerializeField, Header("ガードSE")]
    private AudioClip SE_PlayerGuard;


    void Start()
    {
        // コンポーネントを取得
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        audio = GetComponents<AudioSource>();
        // カメラの位置を取得
        cameraTransform = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Transform>();

        // 初期化
        Initialize();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Initialize()
    {
        hp = maxHP;
        ultTimer = 0;
        state = State.None;
    }

    void FixedUpdate()
    {
        Move();
    }

# region 移動

    /// <summary>
    /// 移動処理
    /// </summary>
    private void Move()
    {
        if(state != State.None)
        {
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
        float speed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
        if (anim != null)
        {
            // アニメーションを再生
            anim.SetFloat("Speed", speed);
        }
        // Playerの向いている方向に進む
        rb.velocity = moveForward.normalized * moveSpeed;

    }

    #endregion

    #region 攻撃
    #endregion

    #region 必殺
    #endregion

    #region スライディング

    #endregion

    #region 防御
    #endregion

    /// <summary>
    /// ダメージ関数
    /// </summary>
    /// <param name="damage">与える量</param>
    public void Damage(int damage)
    {

    }
}
