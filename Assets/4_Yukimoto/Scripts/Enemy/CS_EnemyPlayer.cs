using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 敵の管理クラス
/// </summary>
public class CS_EnemyPlayer : MonoBehaviour
{
    // ----------------------------
    // 状態制御用
    // ----------------------------

    /// <summary>
    /// 状態
    /// </summary>
    public enum State
    {
        /// <summary> 待機 </summary>
        Idle,

        /// <summary> 追尾 </summary>
        Chase,

        /// <summary> 行動を選択 </summary>
        SelectAction,

        /// <summary> 攻撃1 </summary>
        Attack1,

        /// <summary> 攻撃2 </summary>
        Attack2,

        /// <summary> 必殺技 </summary>
        Ult,

        /// <summary> 回避 </summary>
        Sliding,

        /// <summary> 防御 </summary>
        Guard,

        /// <summary> 被ダメージ </summary>
        ReceiveDamage,

        /// <summary> 死亡 </summary>
        Dead,
    }

    /// <summary> 現在の状態 </summary>
    [SerializeField] private State currentState;

    /// <summary>
    /// 現在の状態
    /// </summary>
    public State CurrentState { get { return currentState; } }

    // ----------------------
    // プレイヤー
    // ----------------------

    /// <summary> プレイヤー </summary>
    [Header("プレイヤー取得")]
    [SerializeField] private Transform player;

    /// <summary> プレイヤー管理用(与ダメージで使う) </summary>
    private CS_Player playerManager;

    // ---------------------------
    // 動き制御用
    // ---------------------------

    /// <summary> 動きを止める場合はtrueにする </summary>
    private bool isStop = false;

    // ---------------------------
    // HP
    // ---------------------------

    /// <summary> 最大HP </summary>
    [Header("最大HP")]
    [SerializeField] private float maxHp;

    /// <summary> 現在のHP </summary>
    private float hp;

    /// <summary>
    /// 現在のHP
    /// </summary>
    public float Hp { get { return hp; } }

    // ---------------------
    // 速度
    // ---------------------

    /// <summary> 速度関係のパラメータ </summary>
    [System.Serializable]
    private struct SpeedParameter
    {
        /// <summary> 追尾時の最大速度 </summary>
        [Header("追尾時の最大速度")]
        public float chaseMax;

        [Header("追尾時の加速度")]
        public float chaseAcceleration;

        /// <summary> プレイヤーに近づいた後の移動速度 </summary>
        [Header("プレイヤーに近づいた後の移動速度")]
        [NonSerialized] public float nearMove;
    }

    /// <summary> 速度関係のパラメータ </summary>
    [Header("速度関係のパラメータ")]
    [SerializeField] private SpeedParameter speedParameter;

    // --------------------------
    // 攻撃
    // --------------------------

    /// <summary> 攻撃関係のパラメータ </summary>
    [System.Serializable]
    private struct AttackParameter
    {
        /// <summary> 攻撃1の攻撃力 </summary>
        [Header("攻撃1の攻撃力")]
        public float power_attack1;

        /// <summary> 攻撃2の攻撃力 </summary>
        [Header("攻撃2の攻撃力")]
        public float power_attack2;

        /// <summary> 
        /// 攻撃のトリガーとなるプレイヤーとの距離
        /// </summary>
        [Header("攻撃のトリガーとなるプレイヤーとの距離")]
        public float triggerDistance;

        /// <summary> 攻撃待機時間(秒) </summary>
        [Header("攻撃待機時間(秒)")]
        public float interval;

        /// <summary> 攻撃する確率(%) </summary>
        [Header("攻撃した後に再攻撃する確率(%)")]
        public float percent;

        /// <summary> 攻撃時にスーパーアーマーをつけるかどうか </summary>
        [Header("攻撃時にスーパーアーマーをつけるかどうか")]
        public bool hasSuperArmor;

        /// <summary> 攻撃可能ならtrue </summary>
        [NonSerialized] public bool canAttack;

        /// <summary> 攻撃の確率を引き当てたらtrue </summary>
        [NonSerialized] public bool isPercent;
    }

    /// <summary> 攻撃関係のパラメータ </summary>
    [Header("攻撃関係のパラメータ")]
    [SerializeField] private AttackParameter attackParameter;

    // ------------------------------
    // 必殺技
    // ------------------------------

    /// <summary>
    /// 必殺技関係のパラメータ
    /// </summary>
    [System.Serializable]
    private struct UltParameter
    {
        /// <summary> 必殺技の威力 </summary>
        [Header("必殺技の威力")]
        public float power;

        /// <summary> 必殺技のインターバル時間(秒) </summary>
        [Header("必殺技のインターバル時間(秒)")]
        public float interval;

        /// <summary> 必殺技の速度 </summary>
        [Header("必殺技の速度"), Range(1.0f, 3.0f)]
        public float speed;

        /// <summary> 必殺技が使用可能ならtrue </summary>
        [NonSerialized] public bool canUlt;
    }

    /// <summary>
    /// 必殺技関係のパラメータ
    /// </summary>
    [Header("必殺技関係のパラメータ")]
    [SerializeField] private UltParameter ultParameter;

    // ------------------------------
    // 回避
    // ------------------------------

    /// <summary> 回避関係のパラメータ </summary>
    [System.Serializable]
    private struct SlidingParameter
    {
        /// <summary> 回避インターバル時間(秒) </summary>
        [Header("回避インターバル時間(秒)")]
        public float interval;

        /// <summary> 回避可能ならtrue </summary> 
        [NonSerialized] public bool canSliding;
    }

    /// <summary> 回避関係のパラメータ </summary>
    [Header("回避関係のパラメータ")]
    [SerializeField] private SlidingParameter slidingParameter;

    // ------------------------------
    // 防御
    // ------------------------------

    /// <summary> 防御関係のパラメータ </summary>
    [System.Serializable]
    private struct GuardParameter
    {
        /// <summary> 防御時のカット率 </summary>
        [Header("防御時のカット率(%)")]
        public float cutRatio;

        /// <summary> 防御インターバル時間(秒) </summary>
        [Header("防御インターバル時間(秒)")]
        public float interval;

        /// <summary> 防御可能ならtrue </summary>
        [NonSerialized] public bool canGuard;

        /// <summary> 防御中ならtrue </summary>
        [NonSerialized] public bool isGuard;
    }

    /// <summary> 防御関係のパラメータ </summary> 
    [Header("防御関係のパラメータ")]
    [SerializeField] private GuardParameter guardParameter;

    // ----------------------------
    // 与ダメージ
    // ----------------------------

    /// <summary> trueのとき、武器が当たるようになる(多段防止) </summary>
    private bool canWeaponHit = false;

    /// <summary>
    /// 武器が当たるかどうか
    /// </summary>
    public bool CanWeaponHit { get { return canWeaponHit; } }

    // ----------------------------
    // 死亡
    // ----------------------------

    /// <summary> 死亡モーションが終了したらtrue </summary>
    private bool isDead = false;

    /// <summary>
    /// 死亡したかどうか
    /// </summary>
    public bool IsDead { get { return isDead; } }

    // ----------------------------
    // 攻撃検知用
    // ----------------------------

    /// <summary> 攻撃などを検知できる時間 </summary>
    [System.Serializable]
    private struct ReceptionTime
    {
        /// <summary> 攻撃を検知できる時間(秒) </summary>
        [Header("攻撃を検知できる時間(秒)")]
        public float attack;

        /// <summary> 必殺技を検知できる時間(秒) </summary>
        [Header("必殺技を検知できる時間(秒)")]
        public float ult;
    }

    /// <summary> 攻撃などを検知できる時間 </summary>
    [Header("攻撃などを検知できる時間")]
    [SerializeField] private ReceptionTime receptionTime;

    // --------------------
    // AI制御用
    // --------------------

    /// <summary> 敵AI制御用 </summary>
    private NavMeshAgent enemyAi;

    // --------------------------------
    // アニメーション検知用
    // --------------------------------

    /// <summary> プレイヤーのアニメーション検知用 </summary>
    [System.Serializable]
    private struct PlayerAnimation
    {
        /// <summary> プレイヤーの必殺技アニメーション </summary>
        [Header("必殺技のアニメーション(プレイヤー)")]
        public AnimationClip ult;

        /// <summary> プレイヤーの攻撃1アニメーション </summary>
        [Header("プレイヤーの攻撃1アニメーション")]
        public AnimationClip attack1;

        /// <summary> プレイヤーの攻撃2アニメーション </summary>
        [Header("プレイヤーの攻撃2アニメーション")]
        public AnimationClip attack2;

        /// <summary> プレイヤーの死亡アニメーション </summary> 
        [Header("プレイヤーの死亡アニメーション")]
        public AnimationClip dead;
    }

    [Header("プレイヤーのアニメーション検知用")]
    [SerializeField] private PlayerAnimation playerAnimation;

    /// <summary> プレイヤーのアニメーション読み取り用 </summary>
    private Animator playerAnimator;

    // ---------------------------------
    // アニメーション制御用
    // ---------------------------------

    /// <summary> 敵アニメーション制御用 </summary>
    private Animator enemyAnimator;

    // ---------------------------------
    // アニメーション遷移用
    // ---------------------------------

    /// <summary> ダッシュモーション遷移用 </summary>
    private readonly string isRun = "IsRun";

    /// <summary> 攻撃アニメーション遷移用 </summary>
    private readonly string isAttack = "IsAttack";

    /// <summary> 必殺技アニメーション遷移用 </summary>
    private readonly string ultTirgger = "UltTrigger";

    /// <summary> 回避アニメーション遷移用 </summary>
    private readonly string slidingTrigger = "SlidingTrigger";

    /// <summary> 防御アニメーション遷移用 </summary>
    private readonly string guardTrigger = "GuardTrigger";

    /// <summary> 被ダメージアニメーション遷移用 </summary>
    private readonly string hitTrigger = "HitTrigger";

    /// <summary> 死亡アニメーション遷移用 </summary>
    private readonly string deadTrigger = "DeadTrigger";

    // -----------------------
    // サウンド
    // -----------------------

    /// <summary> サウンド </summary>
    [System.Serializable]
    private struct Sound
    {
        /// <summary> 攻撃時SE </summary>
        [Header("攻撃時SE")]
        public AudioClip attack;

        /// <summary> 攻撃1のヒット時SE </summary>
        [Header("攻撃1のヒット時SE")]
        public AudioClip hit_attack1;

        /// <summary> 攻撃2のヒット時SE </summary>
        [Header("攻撃2のヒット時SE")]
        public AudioClip hit_attack2;

        /// <summary> 必殺技のジャンプ時SE </summary>
        [Header("必殺技のジャンプ時SE")]
        public AudioClip ult_jump;

        /// <summary> 必殺技SE </summary>
        [Header("必殺技SE")]
        public AudioClip ult;

        /// <summary> 回避SE </summary>
        [Header("回避SE")]
        public AudioClip sliding;

        /// <summary> 防御開始SE </summary>
        [Header("防御開始SE")]
        public AudioClip guardStart;

        /// <summary> 防御SE </summary>
        [Header("防御SE")]
        public AudioClip guard;

        /// <summary> 被ダメージSE </summary>
        [Header("被ダメージSE")]
        public AudioClip receiveDamage;

        /// <summary> 死亡SE </summary>
        [Header("死亡SE")]
        public AudioClip dead;
    }

    /// <summary> サウンド </summary>
    [Header("サウンド")]
    [SerializeField] private Sound sound;

    /// <summary> サウンド用コンポーネント </summary>
    private AudioSource audioSource;

    // ------------------------------
    // エフェクト
    // ------------------------------

    [System.Serializable]
    private struct Effect
    {
        /// <summary> 攻撃1エフェクト </summary>
        [Header("攻撃1エフェクト")]
        public GameObject attack1;

        /// <summary> 攻撃2エフェクト </summary>
        [Header("攻撃2エフェクト")]
        public GameObject attack2;

        /// <summary> 防御エフェクト </summary>
        [Header("防御エフェクト")]
        public GameObject guard;

        /// <summary> 防御エフェクトを付与するオブジェクト </summary>
        [Header("防御エフェクトを付与するオブジェクト")]
        public Transform guardParent;
    }

    /// <summary> エフェクト </summary>
    [Header("エフェクト")]
    [SerializeField] private Effect effect;

    // -----------------------
    // タイマー
    // -----------------------

    /// <summary> 攻撃用タイマー </summary>
    private float attackTimer = 0;

    /// <summary> 必殺技用タイマー </summary>
    private float ultTimer = 0;

    /// <summary> 回避用タイマー </summary>
    private float slidingTimer = 0;

    /// <summary> 防御用タイマー </summary>
    private float guardTimer = 0;

    // ----------------------------
    // 当たり判定
    // ----------------------------

    /// <summary> 当たり判定制御用 </summary>
    private CapsuleCollider capsuleCollider;

    private void Awake()
    {
        // HPを最大にする
        hp = maxHp;
    }

    // Start is called before the first frame update
    private void Start()
    {
        // AI制御用コンポーネントを取得
        enemyAi = gameObject.GetComponent<NavMeshAgent>();

        // アニメーション制御用コンポーネントを取得
        enemyAnimator = gameObject.GetComponent<Animator>();

        // プレイヤー管理用コンポーネントを取得
        playerManager = player.GetComponent<CS_Player>();

        // プレイヤーの
        // アニメーション制御用コンポーネントを取得
        playerAnimator = player.GetComponent<Animator>();

        // 当たり判定制御用コンポーネントを取得
        capsuleCollider = gameObject.GetComponent<CapsuleCollider>();

        // サウンド用コンポーネントを取得
        audioSource = gameObject.GetComponent<AudioSource>();

        // 動きを止めた状態から始める
        Standby();
    }

    // Update is called once per frame
    private void Update()
    {
        // -------------------------------
        // コンポーネント確認
        // -------------------------------

        if (!CheckComponent())
        {
            return;
        }

        // -------------------------------
        // タイマー
        // -------------------------------

        TimerCount();

        // ------------------------------
        // 死亡確認
        // ------------------------------

        // HPが0になっているのに
        // 死亡状態になってないなら移行する
        if (hp <= 0 &&
            currentState != State.Dead)
        {
            ChangeState(State.Dead);
            return;
        }

        // -----------------------------
        // プレイヤーの死亡確認
        // -----------------------------

        // プレイヤーが死亡したので待機
        if (CheckPlayerDead())
        {
            Standby();
        }

        // -------------------------------
        // 状態に合わせて行動
        // -------------------------------

        switch (currentState)
        {
            // ------------------------
            // 待機
            // ------------------------
            case State.Idle:

                Idle();

                break;

            // ---------------------------
            // プレイヤーを追尾する
            // ---------------------------
            case State.Chase:

                Chase();

                break;

            // --------------------------------------------
            // プレイヤーの行動などから、敵の行動を選択
            // (攻撃・回避・防御・移動)
            // --------------------------------------------
            case State.SelectAction:

                // 行動選択
                SelectAction();

                break;

            // ------------------------
            // 攻撃1
            // ------------------------
            case State.Attack1:

                break;

            // ------------------------
            // 攻撃2
            // ------------------------
            case State.Attack2:

                break;

            // ------------------------
            // 必殺技
            // ------------------------
            case State.Ult:

                break;

            // ------------------------
            // 回避
            // ------------------------
            case State.Sliding:

                break;

            // ------------------------
            // 防御
            // ------------------------
            case State.Guard:

                break;

            // ------------------------
            // 被ダメージ
            // ------------------------
            case State.ReceiveDamage:

                break;

            // ------------------------
            // 死亡
            // ------------------------
            case State.Dead:

                break;
        }
    }

    #region 状態別の処理

    /// <summary>
    /// 待機状態の処理
    /// </summary>
    private void Idle()
    {
        // プレイヤー確認
        if (!player)
        {
            Debug.Log("プレイヤーがいません");
            return;
        }

        // 追尾速度が0なら待機
        if (speedParameter.chaseMax <= 0)
        {
            Debug.Log("追尾速度が0以下に設定されています");
            return;
        }

        // 動きを止めているので待機
        if (isStop)
        {
            return;
        }

        // 追尾状態に移行
        ChangeState(State.Chase);
    }

    /// <summary>
    /// 追尾状態の処理
    /// </summary>
    private void Chase()
    {
        // プレイヤーに向かって移動
        enemyAi.SetDestination(player.position);

        // プレイヤーの方向を向く
        LookTarget(player);

        // プレイヤーとの距離が一定以内
        if (IsNear(player))
        {
            enemyAi.speed = 0;

            // 行動選択状態に移行
            ChangeState(State.SelectAction);
            return;
        }
    }

    /// <summary>
    /// 行動選択
    /// </summary>
    private void SelectAction()
    {
        // -----------------------------
        // 攻撃・必殺技が可能か
        // -----------------------------

        // 必殺技が使用可能
        if (ultParameter.canUlt)
        {
            ChangeState(State.Ult);
            return;
        }
        // 必殺技が使用不可で攻撃可能
        else if (attackParameter.canAttack)
        {
            // タイマーリセット
            attackTimer = 0;

            // 攻撃状態に移行
            ChangeState(State.Attack1);
            return;
        }

        // -------------------------------------
        // プレイヤーの必殺技を検知したか 
        // -------------------------------------

        // プレイヤーの必殺技を検知し
        // 防御可能なら防御する
        if (CheckPlayerUlt() && guardParameter.canGuard)
        {
            // 防御状態に移行
            ChangeState(State.Guard);
            return;
        }

        // -------------------------------------
        // プレイヤーの攻撃を検知したか
        // -------------------------------------

        // プレイヤーの攻撃を検知し
        // 回避可能なら回避する
        if (CheckPlayerAttack() && slidingParameter.canSliding)
        {
            ChangeState(State.Sliding);
            return;
        }
        // 回避不可能で
        // 防御可能なら防御する
        else if (CheckPlayerAttack() && guardParameter.canGuard)
        {
            ChangeState(State.Guard);
            return;
        }

        // ----------------------------------
        // 攻撃の確率を引き当てたか
        // ----------------------------------

        // 攻撃の確率を引き当てたら攻撃
        if (attackParameter.isPercent)
        {
            ChangeState(State.Attack1);
            return;
        }

        // ---------------------------------
        // プレイヤーとの距離を確認
        // ---------------------------------

        // プレイヤーとの距離が一定以上なら
        // 追尾状態に移行
        if (!IsNear(player))
        {
            ChangeState(State.Chase);
            return;
        }

        // ----------------------------------
        // プレイヤーを中心に回転移動
        // ----------------------------------

        // 今は追尾させる
        Chase();
    }

    /// <summary>
    /// プレイヤーを中心に回転移動
    /// </summary>
    /// <param name="moveLeft"> 
    /// <para> true : 右移動 </para>
    /// <para> false : 左移動 </para>
    /// </param>
    private void MoveAround(bool moveRight = true)
    {
        // 回転移動の速度
        float rotateSpeed = speedParameter.nearMove * 10.0f;

        // 右移動
        if (moveRight)
        {
            // 移動方向を逆にする
            rotateSpeed *= -1;
        }

        // プレイヤーを中心に回転移動
        transform.RotateAround(player.position, Vector3.up,
            rotateSpeed * Time.deltaTime);

        return;
    }

    #endregion

    #region アニメーションイベント

    #region 攻撃イベント

    /// <summary>
    /// 攻撃1の開始イベント
    /// </summary>
    private void AnimAttack1()
    {
        // 武器が当たるようにする
        canWeaponHit = true;

        // プレイヤーの方を向く
        LookTarget(player);

        // 攻撃1SE
        PlayOneSound(sound.attack);

        // 攻撃1エフェクト
        CreateEffect(effect.attack1, transform);
    }

    private void AnimAttackOk()
    {
        return;
    }

    /// <summary>
    /// 攻撃1の終了イベント
    /// </summary>
    private void AnimAttack1Faild()
    {
        // プレイヤーがまだ近くにいる場合
        if (IsNear(player))
        {
            // 攻撃2に移行
            ChangeState(State.Attack2);
            return;
        }

        // 武器が当たらないようにする
        canWeaponHit = false;

        // 攻撃終了
        enemyAnimator.SetBool(isAttack, false);

        // この後また攻撃するか抽選する
        attackParameter.isPercent = CheckProbability(attackParameter.percent);

        // 待機状態に移行
        ChangeState(State.Idle);
    }

    /// <summary>
    /// 攻撃2の開始イベント
    /// </summary>
    private void AnimAttack2()
    {
        // 武器が当たるようにする
        canWeaponHit = true;

        // プレイヤーの方を向く
        LookTarget(player);

        // 攻撃2SE
        PlayOneSound(sound.attack);

        // 攻撃2エフェクト
        CreateEffect(effect.attack2, transform);
    }

    /// <summary>
    /// 攻撃2の終了イベント
    /// </summary>
    private void AnimAttack2Faild()
    {
        // 武器が当たらないようにする
        canWeaponHit = false;

        // 攻撃終了
        enemyAnimator.SetBool(isAttack, false);

        // タイマーリセット
        attackTimer = 0;

        // この後で攻撃するか抽選する
        attackParameter.isPercent = CheckProbability(attackParameter.percent);

        // 待機状態に移行
        ChangeState(State.Idle);

        return;
    }

    #endregion

    #region 必殺技イベント

    /// <summary>
    /// 必殺技の開始イベント
    /// </summary>
    private void AnimUlt()
    {
        // 武器が当たるようにする
        canWeaponHit = true;

        // プレイヤーの方を向く
        LookTarget(player);

        // プレイヤーが下を通れるように
        // 衝突判定を無しにする
        capsuleCollider.isTrigger = true;

        // 必殺技サウンド
        PlayOneSound(sound.ult);
    }

    /// <summary>
    /// 必殺技の判定終了イベント
    /// </summary>
    private void UltFinish()
    {
        // 武器が当たらないようにする
        canWeaponHit = false;

        // 敵を貫通しないように
        // 衝突判定を戻す
        capsuleCollider.isTrigger = false;
    }

    /// <summary>
    /// 必殺技の終了イベント
    /// </summary>
    private void AnimUltFailed()
    {
        // タイマーリセット
        ultTimer = 0;

        // アニメーション速度を初期化
        enemyAnimator.speed = 1;

        // 待機状態に移行
        ChangeState(State.Idle);
    }

    #endregion

    #region 回避イベント

    /// <summary>
    /// 回避終了イベント
    /// </summary>
    private void AnimSlidingFiled()
    {
        // 回避用タイマーをリセット
        slidingTimer = 0;

        // 待機状態に移行
        ChangeState(State.Idle);
    }

    #endregion

    #region 防御イベント

    /// <summary>
    /// 防御判定の終了イベント
    /// </summary>
    private void GuardFinish()
    {
        // 防御終了
        guardParameter.isGuard = false;
    }

    /// <summary>
    /// 防御アニメーションの終了イベント
    /// </summary>
    private void AnimGuardFailed()
    {
        // 防御用タイマーをリセット
        guardTimer = 0;

        // 待機状態に移行
        ChangeState(State.Idle);
    }

    #endregion

    #region 被ダメージイベント

    private void AnimDamgeFailed()
    {
        // 待機状態に移行
        ChangeState(State.Idle);
    }

    #endregion

    #region 死亡イベント

    private void DeadSound()
    {
        PlayOneSound(sound.dead);
    }

    private void AnimDeadFailed()
    {
        isDead = true;
    }

    #endregion

    #endregion

    #region 被ダメージ

    /// <summary>
    /// ダメージを受ける
    /// </summary>
    /// <param name="damage"> ダメージ量 </param>
    public void ReceiveDamage(float damage)
    {
        // 既に死亡したなら何もしない
        if (currentState == State.Dead)
        {
            return;
        }

        // 必殺技中はダメージを受けない
        if (currentState == State.Ult)
        {
            return;
        }

        // 回避中はダメージを受けない
        if (currentState == State.Sliding)
        {
            return;
        }

        // 防御中
        if (guardParameter.isGuard)
        {
            // ダメージの軽減値
            float cut = damage * (guardParameter.cutRatio / 100);

            // ダメージを軽減したうえで
            // HPを減らす
            damage -= cut;
            hp -= damage;

            // 防御SE
            PlayOneSound(sound.guard);

            // 防御エフェクト
            CreateEffect(effect.guard, effect.guardParent);
        }
        else
        {
            // ダメージ分HPを減らす
            hp -= damage;

            // 被ダメージSE
            PlayOneSound(sound.receiveDamage);
        }

        // HPが無くなったら死亡
        if (hp <= 0)
        {
            ChangeState(State.Dead);
            return;
        }

        // スーパーアーマーなら攻撃中に
        // 被ダメージモーションはしない
        if (currentState == State.Attack1 ||
            currentState == State.Attack2)
        {
            if (attackParameter.hasSuperArmor)
                return;
        }

        // 被ダメージモーション中に追加でモーションを起こさない
        if (currentState == State.ReceiveDamage)
        {
            return;
        }

        // 防御中なら被ダメージモーションはしない
        if (guardParameter.isGuard)
        {
            return;
        }

        // HPが残っているので被ダメージ状態に移行
        ChangeState(State.ReceiveDamage);
    }

    #endregion

    #region 与ダメージ

    /// <summary>
    /// プレイヤーにダメージを与える
    /// </summary>
    public void PlayerDamage()
    {
        // 多段防止で攻撃が当たらないようにする
        canWeaponHit = false;

        // 攻撃1
        if (currentState == State.Attack1)
        {
            // 攻撃1の攻撃力を参照
            playerManager.ReceiveDamage(attackParameter.power_attack1);

            // 攻撃1のサウンド
            PlayOneSound(sound.hit_attack1);
        }

        // 攻撃2
        if (currentState == State.Attack2)
        {
            // 攻撃2の攻撃力を参照
            playerManager.ReceiveDamage(attackParameter.power_attack2);

            // 攻撃2サウンド
            PlayOneSound(sound.hit_attack2);
        }

        // 必殺技
        if (currentState == State.Ult)
        {
            // 必殺技の威力を参照
            playerManager.ReceiveDamage(ultParameter.power);
        }
    }

    #endregion

    #region 動き制御用

    /// <summary>
    /// 動きを止める
    /// </summary>
    public void Standby()
    {
        ChangeState(State.Idle);
        isStop = true;
    }

    /// <summary>
    /// 停止状態を解除する
    /// </summary>
    public void CancelStandby()
    {
        ChangeState(State.Idle);
        isStop = false;
    }

    #endregion

    #region 判定用

    /// <summary>
    /// コンポーネントがあるのか確認する
    /// </summary>
    /// <returns>
    /// <para> true : コンポーネントがある </para>
    /// <para> false : コンポーネントがない </para>
    /// </returns>
    private bool CheckComponent()
    {
        // 敵操作用コンポーネントがない
        if (!enemyAi)
        {
            Debug.Log("NavMeshAgentがありません");
            return false;
        }

        // アニメーション制御用コンポーネントがない
        if (!enemyAnimator)
        {
            Debug.Log("敵のAnimatorがありません");
            return false;
        }

        // コライダーを確認
        if (!capsuleCollider)
        {
            Debug.Log("コライダーがありません");
            return false;
        }

        return true;
    }

    /// <summary>
    /// ターゲットが近くにいるか
    /// </summary>
    /// <param name="target"> 対象 </param>
    /// <returns>
    /// <para> true : ターゲットとの距離が一定以内 </para>
    /// <para> false : ターゲットとの距離が一定以上 </para>
    /// </returns>
    private bool IsNear(Transform target)
    {
        // ターゲットとの距離
        float distance = Vector3.Distance(
            target.position, transform.position);

        // 一定以内にターゲットがいるのか
        return distance < attackParameter.triggerDistance;
    }

    /// <summary>
    /// 確率判定
    /// </summary>
    /// <param name="percent"> trueになる確率(%) </param>
    /// <returns>
    /// 指定した確率でtrueになる
    /// </returns>
    private bool CheckProbability(float percent)
    {
        // 乱数(0 ～ 100.0)
        float randomValue = UnityEngine.Random.value * 100.0f;

        // 確率より値が小さい場合は、
        // 確率を引いたと判定する
        return randomValue < percent;
    }

    #endregion

    #region プレイヤーの行動検知用

    /// <summary>
    /// プレイヤーの必殺技を検知したか確認する
    /// </summary>
    /// <returns> 
    /// <para> true : 必殺技を検知した </para>
    /// <para> false : 必殺技を検知しなかった </para>
    /// </returns>
    private bool CheckPlayerUlt()
    {
        // プレイヤーのアニメーターがない
        if (!playerAnimator)
        {
            Debug.Log("プレイヤーのアニメーターがありません");
            return false;
        }

        // 必殺技のアニメーションが設定されていない
        if (!playerAnimation.ult)
        {
            Debug.Log("プレイヤーの必殺技アニメーションが設定されていません");
            return false;
        }

        // プレイヤーが必殺技をしていない
        if (!IsPlayingAnim(playerAnimator, playerAnimation.ult))
        {
            return false;
        }

        // プレイヤーの必殺技の判定開始時間
        float ultStartTime = playerAnimation.ult.events[0].time;

        // アニメーションの経過時間
        float elapsedTime = GetAnimElapsedTime(playerAnimator);

        // プレイヤーの必殺技のアニメーションが
        // ある程度再生されたら検知
        return elapsedTime >= ultStartTime &&
            elapsedTime < ultStartTime + receptionTime.ult;
    }

    /// <summary>
    /// プレイヤーの攻撃1・攻撃2を検知したか確認する
    /// </summary>
    /// <returns>
    /// <para> true : 検知した </para>
    /// <para> false : 検知しなかった </para>
    /// </returns>
    private bool CheckPlayerAttack()
    {
        // プレイヤーのアニメーターがない
        if (!playerAnimator)
        {
            Debug.Log("プレイヤーのアニメーターがありません");
            return false;
        }

        // プレイヤーの攻撃が設定されていない
        if (!CheckPlayerAttakAnim())
        {
            return false;
        }

        // プレイヤーが攻撃していない
        if (!IsPlayingAnim(playerAnimator, playerAnimation.attack1) &&
            !IsPlayingAnim(playerAnimator, playerAnimation.attack2))
        {
            return false;
        }

        // 攻撃の経過時間
        float elapsedTime = GetAnimElapsedTime(playerAnimator);

        // 攻撃検知可能な時間を経過しているので
        // 攻撃を検知できない
        if (elapsedTime > receptionTime.attack)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// プレイヤーの死亡を検知したか確認する
    /// </summary>
    /// <returns>
    /// <para> true : 検知した </para>
    /// <para> false : 検知しなかった </para>
    /// </returns>
    private bool CheckPlayerDead()
    {
        // プレイヤーのアニメーターがない
        if (!playerAnimator)
        {
            Debug.Log("プレイヤーのアニメーターがありません");
            return false;
        }

        // 死亡アニメーションが設定されていない
        if (!playerAnimation.dead)
        {
            Debug.Log("プレイヤーの死亡アニメーションが設定されていません");
            return false;
        }

        // プレイヤーが死亡していない
        if (!IsPlayingAnim(playerAnimator, playerAnimation.dead))
        {
            return false;
        }

        return true;
    }

    #endregion

    #region アニメーション

    /// <summary>
    /// アニメーションの経過時間を取得する
    /// </summary>
    /// <param name="animator"> アニメーター </param>
    /// <returns> アニメーションの経過時間 </returns>
    private float GetAnimElapsedTime(Animator animator)
    {
        // 現在の経過割合(0 ～ 1.0)
        float currentTimeRatio = GetCurrentAnim(animator).normalizedTime;

        // アニメーションの再生時間に掛けて
        // 経過時間を求める
        return playerAnimation.ult.length * currentTimeRatio;
    }

    /// <summary>
    /// 再生中のアニメーションを取得する
    /// </summary>
    /// <param name="animator"> アニメーター </param>
    /// <returns> 再生中のアニメーション </returns>
    private AnimatorStateInfo GetCurrentAnim(
        Animator animator, int layerIndex = 0)
    {
        // 再生中のアニメーション
        return animator.GetCurrentAnimatorStateInfo(layerIndex);
    }

    /// <summary>
    /// 指定したアニメーションが再生中か確認する
    /// </summary>
    /// <param name="animator"> アニメーター </param>
    /// <param name="anim"> 確認するアニメーション </param>
    /// <returns>
    /// <para> true : 再生中 </para>
    /// <para> false : 再生していない </para>
    /// </returns>
    private bool IsPlayingAnim(
        Animator animator, AnimationClip anim)
    {
        // 再生中のアニメーション
        var currentAnim = GetCurrentAnim(animator);

        // 再生中のアニメーションの名前が
        // 指定した名前と一致するならtrue
        return currentAnim.IsName(anim.name);
    }

    /// <summary>
    /// プレイヤーの攻撃アニメーションが設定されているか確認する
    /// </summary>
    /// <returns>
    /// <para> true : 設定されている </para>
    /// <para> false : 設定されていない </para>
    /// </returns>
    private bool CheckPlayerAttakAnim()
    {
        // プレイヤーの攻撃1が設定されていない
        if (!playerAnimation.attack1)
        {
            Debug.Log("プレイヤーの攻撃1アニメーションが設定されていません");
            return false;
        }

        // プレイヤーの攻撃2が設定されていない
        if (!playerAnimation.attack1)
        {
            Debug.Log("プレイヤーの攻撃2アニメーションが設定されていません");
            return false;
        }

        return true;
    }

    #endregion

    #region サウンド

    /// <summary>
    /// サウンドが設定されているか確認する
    /// </summary>
    /// <param name="clip"> 確認するサウンド </param>
    /// <returns>
    /// <para> true : サウンドがある </para>
    /// <para> true : サウンドまたは、サウンド用コンポーネントがない </para>
    /// </returns>
    bool CheckSound(AudioClip clip)
    {
        // サウンド用コンポーネント
        // またはサウンドクリップが設定されていない
        if (!audioSource || !clip)
        {
            Debug.Log("SEまたは、AudioSorceコンポーネントがありません");
            return false;
        }

        return true;
    }

    /// <summary>
    /// 1回だけサウンドを鳴らす
    /// </summary>
    /// <param name="clip"> 鳴らしたいサウンド </param>
    void PlayOneSound(AudioClip clip)
    {
        if (CheckSound(clip))
        {
            audioSource.PlayOneShot(clip);
        }
    }

    #endregion

    #region エフェクト

    /// <summary>
    /// エフェクトを生成する
    /// </summary>
    /// <param name="e"> 生成するエフェクト </param>
    /// <param name="effectParent"> エフェクトを付与するオブジェクト </param>
    private void CreateEffect(GameObject e, Transform effectParent)
    {
        var effectObject = Instantiate(e, effectParent);
        Destroy(effectObject, 1.0f);
    }

    #endregion

    /// <summary>
    /// 状態を移行する
    /// </summary>
    /// <param name="state"> 移行先の状態 </param>
    private void ChangeState(State state)
    {
        // 指定された状態に移行する
        switch (state)
        {
            // -------------------------
            // 待機状態に移行
            // -------------------------
            case State.Idle:

                // ダッシュモーション終了
                enemyAnimator.SetBool(isRun, false);

                enemyAi.speed = 0;

                // 待機状態に移行
                currentState = State.Idle;

                break;

            // ----------------------
            // 追尾状態に移行
            // ----------------------
            case State.Chase:

                // 最大速度を設定
                enemyAi.speed = speedParameter.chaseMax;

                // 加速度を設定
                enemyAi.acceleration = speedParameter.chaseAcceleration;

                // ダッシュモーション開始
                enemyAnimator.SetBool(isRun, true);

                // 追尾状態に移行
                currentState = State.Chase;

                break;

            // --------------------------
            // 行動選択状態に移行
            // --------------------------

            case State.SelectAction:

                // 行動選択状態に移行
                currentState = State.SelectAction;

                break;

            // -----------------------
            // 攻撃1状態に移行
            // -----------------------
            case State.Attack1:

                // 攻撃1アニメーション開始
                enemyAnimator.SetBool(isAttack, true);

                // 攻撃1状態に移行
                currentState = State.Attack1;

                break;

            // -----------------------
            // 攻撃2状態に移行
            // -----------------------
            case State.Attack2:

                // 攻撃2状態に移行
                currentState = State.Attack2;

                break;

            // -----------------------
            // 必殺技状態に移行
            // -----------------------
            case State.Ult:

                // 必殺技の速度を設定
                enemyAnimator.speed = ultParameter.speed;

                // 必殺技アニメーション開始
                enemyAnimator.SetTrigger(ultTirgger);

                // 必殺技のジャンプ時SE
                PlayOneSound(sound.ult_jump);

                // 必殺技状態に移行
                currentState = State.Ult;

                break;

            // -----------------------
            // 回避状態に移行
            // -----------------------
            case State.Sliding:

                // 回避アニメーション開始
                enemyAnimator.SetTrigger(slidingTrigger);

                // 回避SE
                PlayOneSound(sound.sliding);

                // 回避状態に移行
                currentState = State.Sliding;

                break;

            // ------------------------
            // 防御状態に移行
            // ------------------------
            case State.Guard:

                // 防御開始
                guardParameter.isGuard = true;

                // 防御アニメーション開始
                enemyAnimator.SetTrigger(guardTrigger);

                // 防御SE
                PlayOneSound(sound.guardStart);

                // 防御状態に移行
                currentState = State.Guard;

                break;

            // ---------------------------
            // 被ダメージ状態に移行
            // ---------------------------
            case State.ReceiveDamage:

                // 移動しないようにする
                enemyAi.speed = 0;

                // 被ダメージモーション開始
                enemyAnimator.SetTrigger(hitTrigger);

                // 被ダメージ状態に移行
                currentState = State.ReceiveDamage;

                break;

            // ------------------------
            // 死亡状態に移行
            // ------------------------
            case State.Dead:

                // 移動しないようにする
                enemyAi.speed = 0;

                // 死亡アニメーション開始
                enemyAnimator.SetTrigger(deadTrigger);

                // 死亡状態に移行
                currentState = State.Dead;

                break;
        }
    }

    /// <summary>
    /// ターゲットの方を向く(Y軸回転のみ)
    /// </summary>
    /// <param name="target"> 対象 </param>
    private void LookTarget(Transform target)
    {
        // ターゲットの座標
        Vector3 targetPos = target.position;

        // 上下の回転をしないように
        // Y座標を同じにする
        targetPos.y = transform.position.y;

        // プレイヤーの方を向くように回転
        transform.LookAt(targetPos);
    }

    /// <summary>
    /// 各種タイマーを進める
    /// </summary>
    private void TimerCount()
    {
        // ----------------------------
        // タイマーを進める
        // ----------------------------

        // 攻撃してからの時間を計測
        if (!attackParameter.canAttack) attackTimer += Time.deltaTime;

        // 必殺技を使用してからの時間を計測
        if (!ultParameter.canUlt) ultTimer += Time.deltaTime;

        // 回避してからの時間を計測
        if (!slidingParameter.canSliding) slidingTimer += Time.deltaTime;

        // 防御してからの時間を計測
        if (!guardParameter.canGuard) guardTimer += Time.deltaTime;

        // ----------------------------------
        // 一定時間経ったか確認
        // ----------------------------------

        attackParameter.canAttack = attackTimer >= attackParameter.interval;
        ultParameter.canUlt = ultTimer >= ultParameter.interval;
        slidingParameter.canSliding = slidingTimer >= slidingParameter.interval;
        guardParameter.canGuard = guardTimer >= guardParameter.interval;
    }
}
