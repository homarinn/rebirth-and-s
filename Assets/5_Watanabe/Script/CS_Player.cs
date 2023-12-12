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

    // 移動できるか true=可能 : false=不可 
    private bool isMove = true;
    private void IsMoveOk()
    {
        isMove = true;
        if (isSliding)
        {
            isSliding = false;
        }
        if (isGuard)
        {
            isGuard = false;
        }
        if(!isAttack)
        {
            attackTimer = attackInterval;
            attackPower = 0;
            isAttack = true;
        }
        if (isUlt)
        {
            ultTimer = ultInterval;
            attackPower = 0;
            isUlt = false;
        }
    }

    // ============　攻撃 ============= //
    [SerializeField, Header("Attack1の攻撃力")]
    private int attack1Power = 10;
    [SerializeField, Header("Attack2の攻撃力")]
    private int attack2Power = 20;
    // 現在の攻撃力
    private int attackPower = 0; 
    public int AttackPower
    {
        get
        {
            return attackPower;
        }
    }
    [SerializeField, Header("攻撃インターバル")]
    private float attackInterval = 0.5f;
    private float attackTimer = 0;
    private bool isAttack = true; // コンボ可能か
    public void IsAttackOk()
    {
        isAttack = true;
    }

    // =========== 必殺 ============== //
    [SerializeField, Header("必殺の攻撃力")]
    private int ultPower = 50;
    [SerializeField, Header("必殺のインターバル")]
    private float ultInterval = 5;
    private float ultTimer = 0;
    public float UltTimer
    {
        get
        {
            return ultTimer;
        }
    }
    private bool isUlt = false;

    // =========== 防御 ============= //
    [SerializeField, Header("防御力")]
    private float guardPower = 0.5f;
    [SerializeField, Header("スライディングインターバル")]
    private float guardInterval = 1;
    private float guardTimer = 0;
    // 防御しているか true=している : false=していない
    private bool isGuard = false;
    

    // ============= 回避 ============ //
    [SerializeField, Header("スライディングの速度")]
    private float slidingSpeed = 10.0f;
    // スライディングしているか true=している : false=していない
    private bool isSliding = false;

    [SerializeField, Header("スライディングインターバル")]
    private float slidingInterval = 1;
    private float slidingTimer = 0;

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

    // 死んでいるか　true=死亡 : false=生きている
    private bool isDead = false; 

    // ========== コンポーネント ========= //
    private Rigidbody rb;
    private Animator anim;

    // =========== Sound ======== //
    [SerializeField, Header("攻撃空振りSE")]
    private AudioClip SE_PlayerAttackMis;
    [SerializeField, Header("攻撃１ヒットSE")]
    private AudioClip SE_PlayerAttack1Hit;
    [SerializeField, Header("攻撃2ヒットSE")]
    private AudioClip SE_PlayerAttack2Hit;
    [SerializeField, Header("必殺SE")]
    private AudioClip SE_PlayerSpecalAttack;


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
        anim = GetComponent<Animator>();
        ultTimer = ultInterval;
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

        if(hp <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                anim.SetTrigger("DeadTrigger");

            }
        }

        // 移動処理
        Move();

        // 回避処理
        Sliding();

        // 攻撃処理
        Attack();

        // 防御処理
        Guard();

        // 必殺処理
        Ult();

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

        // 移動を許可しない
        if (!isMove)
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
    private void Sliding()
    {
        // 他の行動していたら何もしない
        if (!isAttack && isGuard && isUlt)
        {
            rb.velocity = Vector3.zero;
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && slidingTimer <= 0)
        {
            isMove = false;
            isSliding = true;
            slidingTimer = slidingInterval;
            anim.SetTrigger("SlidingTrigger");
        }

        // 向いている方向に移動する
        if (isSliding && !isUlt && !isGuard && isAttack)
        {
            rb.velocity = transform.forward * slidingSpeed;
        }

        // タイマーが0以上なら減らす
        if(slidingTimer > 0 && !isSliding)
        {
            slidingTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// 必殺
    /// </summary>
    void Ult()
    {
        if(Input.GetKeyDown(KeyCode.Space) && ultTimer <= 0)
        {
            isMove = false; // 移動しない
            anim.SetTrigger("UltTrigger");
            ultTimer = ultInterval;
            isAttack = true;
        }

        // タイマーが0以上なら減らす
        if (ultTimer > 0)
        {
            ultTimer -= Time.deltaTime;
        }
    }

    public void UltDamage()
    {
        attackPower = ultPower;
    }

    /// <summary>
    /// 攻撃関数
    /// </summary>
    private void Attack()
    {
        // 他の行動していたら何もしない
        if (isGuard && isSliding)
        {
            return;
        }
        if (Input.GetMouseButtonDown(1) && attackTimer <= 0 && isAttack)
        {
            isMove = false; // 移動しない
            anim.SetTrigger("AttackTrigger");
            isAttack = false;
        }

        // タイマーが0以上なら減らす
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

    }

    public void Attack1Damage()
    {
        attackPower = attack1Power;
    }
    public void Attack2Damage()
    {
        attackPower = attack2Power;

    }


    /// <summary>
    /// 防御関数
    /// </summary>
    private void Guard()
    {
        // 他の行動していたら何もしない
        if (!isAttack && isSliding)
        {
            return;
        }
        if(Input.GetMouseButtonDown(0) && guardTimer <= 0)
        {
            isMove = false; // 移動しない
            isGuard = true; // 防御している
            guardTimer = guardInterval;
            anim.SetTrigger("GuardTrigger");
        }

        // タイマーが0以上なら減らす
        if (guardTimer > 0 && !isGuard)
        {
            guardTimer -= Time.deltaTime;
        }

    }

    /// <summary>
    /// ダメージ関数
    /// </summary>
    public void Damage(int damage)
    {
        // 無敵時間だったらダメージを受けない
        if (mutekiTimer <= 0)
        {
            return;
        }
        // HPをへらす
        if (isGuard)
        {
            hp -= (int)(damage * guardPower);
        }
        else
        {
            hp -= damage;
        }
        mutekiTimer = mutekiTime;
        if (anim != null)
        {
            anim.SetTrigger("HitTrigger");
        }
    }

}
