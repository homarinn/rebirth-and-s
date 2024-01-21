using UnityEngine;

// Playerスクリプト
public class CS_PlayerA : MonoBehaviour
{

    // ============ 移動 =============  //
    [SerializeField, Header("移動速度")]
    private float moveSpeed = 5.0f;

    [SerializeField, Header("プレイヤーの旋回速度")]
    private float rotationSpeed = 0.5f;

    // ============ 回避 ============= //
    [SerializeField, Header("回避速度")]
    private float slidingSpeed = 10.0f;
    [SerializeField, Header("回避インターバル")]
    private float slidingInterval = 1;
    private float slidingTimer = 0;

    // スライディング中/
    private bool slidingNow = false;

    // ============ 攻撃 ============= //
    [SerializeField, Header("Attack1攻撃力")]
    private float attack1Power = 10;
    public float Attack1Power
    {
        get
        {
            return attack1Power;
        }
    }
    [SerializeField, Header("Attack1のインターバル")]
    private float attack1Interval = 0.5f;
    [SerializeField, Header("Attack2攻撃力")]
    private float attack2Power = 20;
    public float Attack2Power
    {
        get
        {
            return attack2Power;
        }
    }
    [SerializeField, Header("Attack2のインターバル")]
    private float attack2Interval = 1.0f;
    private float attackTimer = 0;
    // 攻撃中？
    private bool attackNow = false;
    private bool attackOk = true;
    private bool isAttack = false;
    public bool IsAttack
    {
        get
        {
            return isAttack;
        }
        set
        {
            isAttack = value;
        }
    }

    // ============ 必殺 ============= //
    [SerializeField, Header("必殺の威力")]
    private float ultPower = 30;
    [SerializeField, Header("必殺のインターバル")]
    private float ultInterval = 3;
    private float ultTimer = 0;
    public float UltTimer
    {
        get
        {
            return Mathf.Clamp(ultTimer, 0, 5);
        }
    }
    // 必殺中?
    private bool ultNow = false;

    // ============= 防御 ============ //
    [SerializeField, Header("防御中のダメージカット率")]
    private float defDamgeCut = 0.5f;
    [SerializeField, Header("防御のインターバル")]
    private float gurdInterval = 1;
    private float gurdTimer = 0;

    // ガード中?
    private bool guardNow = false;

    // ========== ステータス ============= //
    [SerializeField, Header("プレイヤーのMaxHP")]
    private int maxHP = 200;
    [SerializeField, Header("プレイヤーのHP")]
    private int hp;
    public int Hp
    {
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
    // ダメージ
    private float damage = 0;
    public float GetDamage
    {
        get
        {
            return damage;
        }
    }

    // カメラの位置
    private Transform cameraTransform = null;

    // 死んでいるか　true=死亡 : false=生きている
    private bool isDead = false;

    // ========== コンポーネント ========= //
    private Rigidbody rb;
    private Animator anim;
    private new AudioSource[] audio;

    // =========== Sound ======== //
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
        audio = GetComponents<AudioSource>();
        // カメラの位置を取得
        cameraTransform = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Transform>();
    }

    /// <summary>
    /// 更新イベント
    /// </summary>
    private void Update()
    {
        // HPが0以下なら死んでいる
        if (hp <= 0)
        {
            if (!isDead)
            {
                isDead = true;
                if (anim != null)
                {
                    anim.SetTrigger("DeadTrigger");
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }
        // 無敵時間になったら減らす
        if (invincibleTimer > 0)
        {
            invincibleTimer -= Time.deltaTime;
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
    }

    /// <summary>
    /// 移動関数
    /// </summary>
    private void Move()
    {
        if (slidingNow)
        {
            return;
        }
        if (attackNow || guardNow || ultNow)
        {
            rb.velocity = Vector3.zero;
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
        rb.velocity = moveForward * moveSpeed + new Vector3(0, rb.velocity.y, 0);
    }

    #region 回避

    /// <summary>
    /// 回避関数
    /// </summary>
    private void Sliding()
    {
        // スライディングインターバルがあるとき減らす
        if (slidingTimer > 0)
        {
            slidingTimer -= Time.deltaTime;
        }

        // 他の行動中は何もしない
        if (attackNow || guardNow || ultNow)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && !slidingNow && slidingTimer <= 0)
        {
            // スライディング中
            slidingNow = true;
            if (anim != null)
            {
                // スライディング再生
                anim.SetTrigger("SlidingTrigger");
            }
            if (audio != null)
            {
                // スライディング音声
                audio[0].PlayOneShot(SE_PlayerEscape);
            }
        }

        // スライディング中
        if (slidingNow)
        {
            rb.velocity = transform.forward * slidingSpeed;
        }
    }

    /// <summary>
    /// スライディングアニメーションの終了処理
    /// </summary>
    private void AnimSlidingFiled()
    {
        // インターバル
        slidingTimer = slidingInterval;
        // スライディングを終了
        slidingNow = false;
    }

    #endregion

    #region 攻撃

    /// <summary>
    /// 攻撃処理
    /// </summary>
    private void Attack()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        // 他の行動中ならなにもしない
        if (slidingNow || guardNow || ultNow)
        {
            return;
        }
        if (Input.GetMouseButtonDown(0) && attackOk && attackTimer <= 0)
        {
            attackOk = false;
            attackNow = true;
            if (anim != null)
            {
                // 攻撃トリガー
                anim.SetTrigger("AttackTrigger");
            }
        }

    }

    /// <summary>
    /// 攻撃アニメーション1
    /// </summary>
    private void AnimAttack1()
    {
        isAttack = false;
        damage = attack1Power;
    }

    /// <summary>
    /// 攻撃アニメーションOk
    /// </summary>
    private void AnimAttackOk()
    {
        attackOk = true;
    }

    /// <summary>
    /// 攻撃アニメーションの終了処理
    /// </summary>
    private void AnimAttack1Faild()
    {
        damage = 0;
        attackTimer = attack1Interval;
        attackNow = false;
        isAttack = false;
    }

    /// <summary>
    /// 攻撃アニメーション２
    /// </summary>
    private void AnimAttack2()
    {
        isAttack = false;
        damage = attack2Power;
    }

    /// <summary>
    /// 攻撃2アニメーションの終了処理
    /// </summary>
    private void AnimAttack2Faild()
    {
        damage = 0;
        attackTimer = attack2Interval;
        attackNow = false;
        attackOk = true;
        isAttack = false;
    }

    #endregion

    #region 防御

    /// <summary>
    ///  防御処理
    /// </summary>
    private void Guard()
    {
        // ガードインターバルを減らす
        if (gurdTimer > 0)
        {
            gurdTimer -= Time.deltaTime;
        }
        // 他の行動してたら何もしない
        if (attackNow || slidingNow || ultNow)
        {
            return;
        }
        if (Input.GetMouseButtonDown(1) && !guardNow && gurdTimer <= 0)
        {
            // ガード中
            guardNow = true;
            if (anim != null)
            {
                // ガードアニメーション再生
                anim.SetTrigger("GuardTrigger");
            }
        }
    }

    /// <summary>
    /// ガードアニメーション終了処理
    /// </summary>
    private void AnimGuardFailed()
    {
        guardNow = false;
        gurdTimer = gurdInterval;
    }
    #endregion

    #region 必殺

    /// <summary>
    /// 必殺処理
    /// </summary>
    private void Ult()
    {
        // インターバルがあった場合減らす
        if (ultTimer > 0 && !ultNow)
        {
            ultTimer -= Time.deltaTime;
        }

        // 他の行動していた場合何もしない
        if (slidingNow || attackNow || guardNow)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !ultNow && ultTimer <= 0)
        {
            // 必殺中
            ultNow = true;
            ultTimer = ultInterval;
            if (anim != null)
            {
                anim.SetTrigger("UltTrigger");
            }
        }
    }

    /// <summary>
    /// アニメーション必殺処理
    /// </summary>
    private void AnimUlt()
    {
        isAttack = false;
        damage = ultPower;
    }

    /// <summary>
    /// アニメーション必殺終了処理
    /// </summary>
    private void AnimUltFailed()
    {
        damage = 0;
        ultNow = false;
        isAttack = false;
    }
    #endregion

    /// <summary>
    /// ダメージ処理
    /// 攻撃した人に読んでもらう
    /// </summary>
    /// <param name="damage">与えるダメージ</param>
    public void Damage(float _damage)
    {
        damage = 0;
        // 無敵状態の場合無効
        if (invincibleTimer > 0)
        {
            return;
        }
        if (!guardNow)
        {
            if (audio != null || SE_PlayerReceiveDamage != null)
            {
                audio[0].PlayOneShot(SE_PlayerReceiveDamage);
            }

            // damage分Hpを減らす
            hp -= (int)(_damage);
        }
        else
        {
            if (audio != null || SE_PlayerGuard != null)
            {
                audio[0].PlayOneShot(SE_PlayerGuard);
            }
            hp -= (int)(_damage * defDamgeCut);
        }
        // 無敵時間を入れる
        invincibleTimer = invincibleTime;

        if (guardNow || ultNow)
        {
            return;
        }

        if (anim != null)
        {
            // アニメーションを再生
            anim.SetTrigger("HitTrigger");
        }
    }

    private void AnimDamgeFailed()
    {
        slidingNow = false;
        attackNow = false;
        guardNow = false;
        ultNow = false;
        attackOk = true;
    }
}
