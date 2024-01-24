using UnityEngine;

/// <summary>
/// Playerメイン
/// </summary>
public partial class CS_Player : MonoBehaviour
{
    // =======================
    //
    // 変数
    //
    // =======================

    enum State
    {
        Normal,
        Sliding,
        Attack,
        Difence,
        Damage,
        Ult,
        Death
    }
    State state;

    // 移動
    [SerializeField, Header("移動速度")]
    private float moveSpeed = 0;
    [SerializeField, Header("プレイヤーの旋回速度")]
    private float rotationSpeed = 0;

    [SerializeField, Header("水たまり上の移動速度低下(0～1)")]
    private float waterOnTheMoveSpeedCut = 0;
    private bool isWaterOnThe = false;

    private bool moveOK = true;    // 移動許可

    // スライディング
    [SerializeField, Header("スライディング速度")]
    private float slidingSpeed = 0;
    [SerializeField, Header("スライディングインターバル")]
    private float slidingInterval = 0;
    private float slidingTimer = 0;

    // 攻撃威力
    [SerializeField, Header("攻撃１の威力")]
    private float attack1Power = 0;
    public float Attack1Power
    {
        get
        {
            return attack1Power;
        }
    }
    [SerializeField, Header("攻撃2の威力")]
    private float attack2Power = 0;
    public float Attack2Power
    { 
        get
        {
            return attack2Power;
        }
    }    
    private float attackDamage = 0;
    public float AttackDamage
    {
        get
        {
            return attackDamage;
        }
    }

    [SerializeField, Header("攻撃1インターバル")]
    private float attack1Interval = 0;
    [SerializeField, Header("攻撃2インターバル")]
    private float attack2Interval = 0;
    private float attackTimer = 0;

    // 攻撃２が発動可能か
    private bool attack2Ok = false;

    // 必殺
    [SerializeField, Header("必殺技の威力")]
    private float ultPower = 0;
    public float UltPower
    {
        get
        {
            return ultPower;
        }
    }
    [SerializeField, Header("必殺技のインターバル")]
    private float ultInterval = 0;
    private float ultTimer = 0;

    // 防御
    [SerializeField, Header("防御インターバル")]
    private float difenceInterval = 0;
    private float difenceTimer = 0;
    [SerializeField, Header("防御中のダメージカット%")]
    private float difenceDamageCut = 0;
    [SerializeField]
    private bool isDifence = false;


    [SerializeField, Header("HPの最大値")]
    private float maxHP = 0;    // 最大HP
    private float hp;           // 現在のHP
    private bool isInvisible = false;
    private bool isDeath = false;
    public bool IsDeath
    {
        get
        {
            return isDeath;
        }
    }
    
    private Transform cameraTransform = null;       // カメラの位置

    // コンポーネント
    private Rigidbody rb;
    private Animator anim;
    private AudioSource audio;

    // SE
    [SerializeField, Header("移動SE")]
    private AudioClip SE_Move;
    [SerializeField, Header("攻撃SE")]
    private AudioClip SE_Attack;
    [SerializeField, Header("スライディングSE")]
    private AudioClip SE_Sliding;
    [SerializeField, Header("防御開始SE")]
    private AudioClip SE_DifenceStart;
    [SerializeField, Header("防御SE")]
    private AudioClip SE_Difence;
    [SerializeField, Header("ダメージSE")]
    private AudioClip SE_Damage;
    [SerializeField, Header("必殺SE")]
    private AudioClip SE_Ult;
    [SerializeField, Header("必殺ジャンプSE")]
    private AudioClip SE_Jump;

    // Effect
    [SerializeField, Header("防御エフェクト")]
    private GameObject Eff_Difence;

    // =======================
    //
    // ゲッター・セッター
    //
    // =======================

    // HPの最大値
    public float MaxHP
    {
        get
        {
            return maxHP;
        }
    }

    // HP
    public float Hp
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

    // 必殺技タイマー
    public float UltTimer
    {
        get
        {
            return Mathf.Clamp(ultTimer, 0, 5);
        }
    }

    // =======================
    //
    // 関数
    //
    // =======================


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
        // インターバルを更新
        IntervalUpdate();

        switch (state)
        {
            case State.Normal:

                // 回避入力
                if (Input.GetKeyDown(KeyCode.LeftShift) && slidingTimer <= 0)
                {
                    state = State.Sliding;
                    audio.PlayOneShot(SE_Sliding);      // 効果音を鳴らす
                    anim.SetTrigger("SlidingTrigger");  // アニメーションを再生
                }

                // 攻撃入力
                if (Input.GetMouseButtonDown(0) && attackTimer <= 0)
                {
                    state = State.Attack;
                    rb.velocity = Vector3.zero;
                    anim.SetTrigger("AttackTrigger");  // アニメーションを再生
                }

                // 防御入力
                if (Input.GetMouseButtonDown(1) && difenceTimer <= 0)
                {
                    state = State.Difence;
                    rb.velocity = Vector3.zero;
                    anim.SetTrigger("GuardTrigger");  // アニメーションを再生
                }

                // 必殺入力
                if(Input.GetKeyDown(KeyCode.Space) && ultTimer <= 0)
                {
                    state = State.Ult;
                    rb.velocity = Vector3.zero;
                    anim.SetTrigger("UltTrigger");
                }
                if(hp <= 0)
                {
                    state = State.Death;
                    anim.SetTrigger("DeadTrigger");
                    hp = 0;
                }
                
                // 移動処理
                Move();
                break;
            case State.Sliding:
                // 回避処理
                Sliding();
                break;
            case State.Attack:

                // 攻撃2の入力
                if (Input.GetMouseButtonDown(0) && attack2Ok)
                {
                    attack2Ok = false;
                    anim.SetTrigger("AttackTrigger");  // アニメーションを再生
                }
                break;
        }
    }

    /// <summary>
    /// インターバル処理
    /// </summary>
    private void IntervalUpdate()
    {
        if(attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // スライディングのインターバルタイマーを減らす
        if(slidingInterval > 0)
        {
            slidingTimer -= Time.deltaTime;
        }

        // ディフェンスのインターバルタイマーを減らす
        if(difenceTimer > 0)
        {
            difenceTimer -= Time.deltaTime;
        }

        // 必殺のインターバルタイマーを減らす
        if(ultTimer > 0)
        {
            ultTimer -= Time.deltaTime;
        }
    }

    #region 移動

    /// <summary>
    /// 移動関数
    /// </summary>
    private void Move()
    {
        if (!moveOK && state != State.Normal)
        {
            return; // 移動不可
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
        if (anim != null)
        {
            // 速度を取得
            float speed = new Vector2(rb.velocity.x, rb.velocity.z).magnitude;
            // アニメーションを再生
            anim.SetFloat("Speed", speed);
        }
        if (isWaterOnThe)
        {
            rb.velocity = moveForward.normalized * (moveSpeed * waterOnTheMoveSpeedCut);
        }
        else
        {
            // Playerの向いている方向に進む
            rb.velocity = moveForward.normalized * moveSpeed;
        }
    }

    /// <summary>
    /// 足音をながす
    /// </summary>
    private void AnimMoveAudio()
    {
        audio.PlayOneShot(SE_Move);
    }

    #endregion

    #region スライディング

    /// <summary>
    /// スライディング関数
    /// </summary>
    private void Sliding()
    {
        if (isWaterOnThe)
        {
            rb.velocity = transform.forward * (slidingSpeed * waterOnTheMoveSpeedCut);
        }
        else
        {
            rb.velocity = transform.forward * slidingSpeed;
        }
    }

    /// <summary>
    /// スライディングアニメーション終了の時に呼び出す
    /// </summary>
    private void AnimSlidingFailed()
    {
        state = State.Normal;
        slidingTimer = slidingInterval;
        //rb.velocity = Vector3.zero;
    }

    #endregion

    #region 攻撃

    /// <summary>
    /// 攻撃1の威力設定
    /// </summary>
    private void AnimAttackSetPower()
    {
        attackDamage = attackDamage == 0 ? attack1Power : 0;
    }

    /// <summary>
    /// 攻撃アニメーション終了の時に呼び出す
    /// </summary>
    private void AnimAttack1Failied()
    {
        state = State.Normal;
        attackTimer = attack1Interval;
        attack2Ok = false;
    }

    /// <summary>
    /// 攻撃２を発動可能にする
    /// </summary>
    private void AnimAttack2OK()
    {
        attack2Ok = true;
    }

    /// <summary>
    /// 攻撃サウンドを鳴らす
    /// </summary>
    private void AnimAttackAudio()
    {
        audio.PlayOneShot(SE_Attack);      // 効果音を鳴らす
    }

    /// <summary>
    /// 攻撃2の威力設定
    /// </summary>
    private void AnimAttack2SetPower()
    {
        attackDamage = attackDamage == 0 ? attack2Power : 0;
    }

    /// <summary>
    /// 攻撃2アニメーション終了の時に呼び出す
    /// </summary>
    private void AnimAttack2Failied()
    {
        state = State.Normal;
        attackTimer = attack2Interval;
    }


    #endregion

    #region 防御

    private void AnimDifenceStart()
    {
        audio.PlayOneShot(SE_DifenceStart);
    }

    /// <summary>
    /// 防御中の設定
    /// </summary>
    private void SetDifence()
    {
        isDifence = isDifence == true ? false : true;
    }

    private void AnimDifenceFailed()
    {
        difenceTimer = difenceInterval;
        state = State.Normal;
    }

    #endregion

    #region 必殺

    /// <summary>
    /// 必殺の威力設定
    /// </summary>
    private void AnimUltAttackSetPower()
    {
        attackDamage = attackDamage == 0 ? ultPower : 0;
    }
    
    private void AnimUltAudioJump()
    {
        audio.PlayOneShot(SE_Jump);
    }

    private void AnimUltAudio()
    {
        audio.PlayOneShot(SE_Ult);
    }

    /// <summary>
    /// 必殺アニメーション終了の時に呼び出す
    /// </summary>
    private void AnimUltFailed()
    {
        state = State.Normal;
        ultTimer = ultInterval;
    }

    #endregion

    public void Damage(float damage)
    {

    }

    /// <summary>
    /// ダメージ処理
    /// 攻撃した人に読んでもらう
    /// </summary>
    /// <param name="_damage">与えるダメージ</param>
    public void ReceiveDamage(float _damage)
    {
        // 無敵だったら何もしない
        if(isInvisible)
        {
            return;
        }

        if (isDifence)
        {
            Instantiate(Eff_Difence,transform);
            audio.PlayOneShot(SE_Difence);
            // ガード中ダメージ半減
            hp -= _damage * difenceDamageCut;
        }
        else
        {
            audio.PlayOneShot(SE_Damage);
            isInvisible = true;
            hp -= _damage;
        }

        if(hp <= 0)
        {
            hp = 0;
        }

        if(state == State.Difence || state == State.Ult)
        {
            isInvisible = false;
            return;
        }
        state = State.Damage;
        rb.velocity = Vector3.zero;
        anim.SetTrigger("DamageTrigger");
    }

    /// <summary>
    /// 吹き飛ばす
    /// </summary>
    /// <param name="direcion">飛ばす方向</param>
    /// <param name="power">飛ばす威力</param>
    public void BlowOff(Vector3 direcion, float power)
    {
        rb.AddForce(direcion * power, ForceMode.Impulse);
        if(state == State.Difence)
        {
            return;
        }
        state = State.Damage;
        anim.SetTrigger("BlowOffTrigger");
    }

    /// <summary>
    /// ダメージアニメーション終了の時に呼び出す
    /// </summary>
    private void AnimDamageFailed()
    {
        state = State.Normal;
        isInvisible = false;
        attack2Ok = false;
        attackDamage = 0;
    }


    /// <summary>
    /// コリジョンと接触したとき
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // 水たまり
        if(other.gameObject.tag == "")
        {
            isWaterOnThe = true;
        }
    }

    /// <summary>
    /// コリジョンと離れたとき
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        // 水たまり
        if (other.gameObject.tag == "")
        {
            isWaterOnThe = false;
        }
    }
}

