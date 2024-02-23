using System;
using UnityEngine;

/// <summary>
/// Playerメイン
/// </summary>
public partial class CS_Player : MonoBehaviour
{
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

    [Serializable]
    private struct Parameter
    {
        [Header("体力")]
        public float hp;
    }
    [SerializeField, Header("基礎パラメータ")]
    private Parameter parameter;


    private float hp;       // 現在のHP
    public float MaxHP
    {
        get
        {
            return parameter.hp;
        }
    }
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


    [Serializable]
    private struct MoveParameter
    {
        [Header("移動速度")]
        public float moveSpeed;

        [Header("旋回速度")]
        public float rotationSpeed;

        [Header("水たまり上の移動速度低下(0～1)")]
        public float waterOnTheMoveSpeedCut;
    }
    [SerializeField, Header("移動パラメータ")]
    private MoveParameter moveParameter;

    private bool isWaterOnThe = false;  // 水たまりの上か判定
    private bool moveOK = true;         // 移動許可

    [Serializable]
    private struct SlidingParameter
    {
        [Header("速度")]
        public float speed;

        [Header("インターバル")]
        public float interval;
    }
    [SerializeField, Header("スライディングパラメータ")]
    private SlidingParameter slidingParameter;
    private float slidingTimer = 0;

    // ======== 攻撃関連 ======== //

    [Serializable]
    private struct Attack01Parameter
    {
        [Header("威力")]
        public float damage;

        [Header("インターバル")]
        public float interval;
    }
    [SerializeField, Header("攻撃1のパラメータ")]
    private Attack01Parameter attack01Parameter;

    // 攻撃01の威力
    public float Attack01Damage
    {
        get
        {
            return attack01Parameter.damage;
        }
    }

    [Serializable]
    private struct Attack02Parameter
    {
        [Header("威力")]
        public float damage;

        [Header("インターバル")]
        public float interval;
    }
    [SerializeField, Header("攻撃2のパラメータ")]
    private Attack02Parameter attack02Parameter;

    // 攻撃02の威力
    public float Attack02Damage
    {
        get
        {
            return attack02Parameter.damage;
        }
    }

    [SerializeField, Header("プレイヤー武器コライダー")]
    private Collider colPlayerWeapon;

    private float attackDamage = 0;
    public float AttackDamage
    {
        get
        {
            return attackDamage;
        }
    }
    bool attackOk = true;
    public bool AttackOk
    {
        get
        {
            return attackOk;
        }
        set
        {
            attackOk = value;
        }
    }
    private float attackTimer = 0;
    private bool attack2Ok = false;

    [Serializable]
    private struct UltParameter
    {
        [Header("威力")]
        public float damage;

        [Header("インターバル")]
        public float interval;
    }
    [SerializeField, Header("必殺パラメータ")]
    private UltParameter ultParameter;
    private float ultTimer = 0;

    public float UltDamage
    {
        get
        {
            return ultParameter.damage;
        }
    }
    public float UltTimer
    {
        get
        {
            return ultTimer;
        }
    }
    public float UltInterval
    {
        get
        {
            return ultParameter.interval;
        }
    }
    private struct DifenceParameter
    {
        [Header("ダメージカット%")]
        public float cut;

        [Header("インターバル")]
        public float interval;
    }
    [SerializeField, Header("防御パラメータ")]
    private DifenceParameter difenceParameter;

    [SerializeField, Header("ダメージアニメーションを再生する攻撃")]
    private float damageAtackOkAttack = 0;
    private float difenceTimer = 0;
    private bool isDifence = false;

    private bool isInvisible = false;
    private bool isDeath = false;
    public bool IsDeath
    {
        get
        {
            return isDeath;
        }
    }
    private bool action = true;
    public bool Action
    {
        get
        {
            return action;
        }
        set
        {
            action = value;
        }
    }


    // コンポーネント
    private Rigidbody rb;
    private Animator anim;
    private AudioSource audio;
    private CS_LookCollision csLookCollision; // 敵検知スクリプト
    private Transform trsCamera;          // カメラのTrs

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
    [SerializeField, Header("エフェクト位置")]
    private Transform effectTrs;
    [SerializeField, Header("防御エフェクト")]
    private GameObject Eff_Difence;
    [SerializeField, Header("通常攻撃01")]
    private GameObject Eff_Attack01;
    [SerializeField, Header("通常攻撃02")]
    private GameObject Eff_Attack02;
    [SerializeField, Header("必殺着地")]
    private GameObject Eff_UltTachi;
    [SerializeField, Header("必殺エフェクト")]
    private GameObject Eff_Ult;
    [SerializeField, Header("水たまりエフェクト")]
    private GameObject puddleEffect;
    [SerializeField, Header("水たまり足場")]
    private Transform lefTrs;



    // =======================
    //
    // 関数
    //
    // =======================

    /// <summary>
    /// 実体化イベント
    /// </summary>
    private void Awake()
    {
        // HPを設定
        hp = parameter.hp;
        // 必殺インターバル設定
        ultTimer = ultParameter.interval; ;
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
        csLookCollision = GetComponentInChildren<CS_LookCollision>();

        // カメラの位置を取得
        trsCamera = GameObject.FindGameObjectWithTag("PlayerCamera").GetComponent<Transform>();
    }

    /// <summary>
    /// 更新イベント
    /// </summary>
    private void Update()
    {
        if (isDeath || !action)
        {
            return;
        }
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
                    if (csLookCollision.IsHit)
                    {
                        Vector3 pos = Vector3.Scale(csLookCollision.EnemyPos, new Vector3(1, 0, 1));
                        transform.LookAt(pos);
                    }
                    else
                    {
                        Vector3 cameraForward = Vector3.Scale(trsCamera.forward, new Vector3(1, 0, 1)).normalized;
                        // 進行方向に回転
                        if (cameraForward != Vector3.zero)
                        {
                            transform.rotation = Quaternion.LookRotation(cameraForward);
                        }

                    }

                    rb.velocity = Vector3.zero;
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
                if (Input.GetKeyDown(KeyCode.Space) && ultTimer <= 0)
                {
                    Vector3 cameraForward = Vector3.Scale(trsCamera.forward, new Vector3(1, 0, 1)).normalized;
                    // 進行方向に回転
                    if (cameraForward != Vector3.zero)
                    {
                        transform.rotation = Quaternion.LookRotation(cameraForward);
                    }
                    if (csLookCollision.IsHit)
                    {
                        Vector3 pos = Vector3.Scale(csLookCollision.EnemyPos, new Vector3(1, 0, 1));
                        transform.LookAt(pos);
                    }

                    state = State.Ult;
                    rb.velocity = Vector3.zero;
                    anim.SetTrigger("UltTrigger");
                }
                if (hp <= 0)
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
                    if (csLookCollision.IsHit)
                    {
                        Vector3 pos = Vector3.Scale(csLookCollision.EnemyPos, new Vector3(1, 0, 1));
                        transform.LookAt(pos);
                    }
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
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        // スライディングのインターバルタイマーを減らす
        if (slidingParameter.interval > 0)
        {
            slidingTimer -= Time.deltaTime;
        }

        // ディフェンスのインターバルタイマーを減らす
        if (difenceTimer > 0)
        {
            difenceTimer -= Time.deltaTime;
        }

        // 必殺のインターバルタイマーを減らす
        if (ultTimer > 0)
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
        Vector3 cameraForward = Vector3.Scale(trsCamera.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 moveForward = cameraForward * inputAxis.y + trsCamera.right * inputAxis.x;

        // 進行方向に回転
        if (moveForward != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveForward), moveParameter.rotationSpeed);
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
            rb.velocity = moveForward.normalized * (moveParameter.moveSpeed * moveParameter.waterOnTheMoveSpeedCut);
        }
        else
        {
            // Playerの向いている方向に進む
            rb.velocity = moveForward.normalized * moveParameter.moveSpeed;
        }
    }

    /// <summary>
    /// 足音をながす
    /// </summary>
    private void AnimMoveAudio()
    {
        if (isWaterOnThe)
        {
            var eff = Instantiate(puddleEffect, lefTrs);
            Destroy(eff, 1);
        }
        else
        {
            audio.PlayOneShot(SE_Move);
        }
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
            rb.velocity = transform.forward * (slidingParameter.speed * moveParameter.waterOnTheMoveSpeedCut);
        }
        else
        {
            rb.velocity = transform.forward * slidingParameter.speed;
        }
    }

    /// <summary>
    /// スライディングアニメーション終了の時に呼び出す
    /// </summary>
    private void AnimSlidingFailed()
    {
        state = State.Normal;
        slidingTimer = slidingParameter.interval;
    }

    #endregion

    #region 攻撃

    /// <summary>
    /// 攻撃1の威力設定
    /// </summary>
    private void AnimAttackSetPower()
    {
        attackDamage = attackDamage == 0 ? attack01Parameter.damage : 0;
        if (attackDamage == 0)
        {
            attackOk = false;
            colPlayerWeapon.enabled = false;
        }
        else if (attackDamage != 0)
        {
            colPlayerWeapon.enabled = true;
            attackOk = true;
            var eff = Instantiate(Eff_Attack01, transform);
            Destroy(eff, 1);
        }
    }

    /// <summary>
    /// 攻撃アニメーション終了の時に呼び出す
    /// </summary>
    private void AnimAttack1Failied()
    {
        state = State.Normal;
        attackTimer = attack01Parameter.interval;
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
        attackDamage = attackDamage == 0 ? attack02Parameter.damage : 0;
        if (attackDamage == 0)
        {
            colPlayerWeapon.enabled = false;
            attackOk = false;
        }
        else if (attackDamage != 0)
        {
            colPlayerWeapon.enabled = true;
            attackOk = true;
            var eff = Instantiate(Eff_Attack02, transform);
            Destroy(eff, 1);
        }

    }

    /// <summary>
    /// 攻撃2アニメーション終了の時に呼び出す
    /// </summary>
    private void AnimAttack2Failied()
    {
        state = State.Normal;
        attackTimer = attack02Parameter.interval;
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
        difenceTimer = difenceParameter.interval;
        state = State.Normal;
    }

    #endregion

    #region 必殺

    /// <summary>
    /// 必殺の威力設定
    /// </summary>
    private void AnimUltAttackSetPower()
    {
        attackDamage = attackDamage == 0 ? ultParameter.damage : 0;
        if (attackDamage == 0)
        {
            attackOk = false;
            colPlayerWeapon.enabled = false;
        }
        else if (attackDamage != 0)
        {
            attackOk = true;
            colPlayerWeapon.enabled = true;
            var eff = Instantiate(Eff_Ult, effectTrs);
            Destroy(eff, 2);
        }
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
        ultTimer = ultParameter.interval;
    }

    private void AnimUltTachi()
    {
        var eff = Instantiate(Eff_UltTachi, lefTrs);
        Destroy(eff, 2);
    }

    #endregion

    /// <summary>
    /// ダメージ処理
    /// 攻撃した人に読んでもらう
    /// </summary>
    /// <param name="_damage">与えるダメージ</param>
    public void ReceiveDamage(float _damage)
    {
        // 無敵だったら何もしない
        if (isInvisible || hp <= 0 || state == State.Sliding || state == State.Ult)
        {
            return;
        }

        if (isDifence)
        {
            var eff = Instantiate(Eff_Difence, effectTrs);
            Destroy(eff, 1);
            audio.PlayOneShot(SE_Difence);
            // ガード中ダメージ半減
            hp -= _damage * difenceParameter.cut;
        }
        else
        {
            audio.PlayOneShot(SE_Damage);
            isInvisible = true;
            hp -= _damage;
        }

        if (hp <= 0)
        {
            hp = 0;
        }

        if (state == State.Difence || state == State.Ult)
        {
            isInvisible = false;
            return;
        }
        if (_damage >= damageAtackOkAttack)
        {
            attackDamage = 0;
            state = State.Damage;
            anim.SetTrigger("DamageTrigger");
        }
        else
        {
            isInvisible = false;
        }
    }

    /// <summary>
    /// 吹き飛ばす
    /// </summary>
    /// <param name="direcion">飛ばす方向</param>
    /// <param name="power">飛ばす威力</param>
    public void BlowOff(Vector3 direcion, float power)
    {
        if (state == State.Difence || state == State.Ult)
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

    private void AnimDead()
    {
        isDeath = true;
    }


    /// <summary>
    /// コリジョンと接触したとき
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        // 水たまり
        if (other.gameObject.tag == "Puddle")
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
        if (other.gameObject.tag == "Puddle")
        {
            isWaterOnThe = false;
        }
    }
}

