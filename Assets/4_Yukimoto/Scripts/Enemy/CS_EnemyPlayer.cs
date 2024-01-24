using System;
using System.Collections.Generic;
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

        /// <summary> 攻撃 </summary>
        Attack,

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
    private State currentState;

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

    /// <summary> 追尾時の最大速度 </summary>
    [Header("追尾時の最大速度")]
    [SerializeField] private float chaseSpeed;

    [Header("追尾時の加速度")]
    [SerializeField] private float chaseAcceleration;

    /// <summary> プレイヤーに近づいた後の移動速度 </summary>
    [Header("プレイヤーに近づいた後の移動速度")]
    [SerializeField] private float moveSpeed;

    // --------------------------
    // 攻撃
    // --------------------------

    /// <summary> 攻撃力 </summary>
    [Header("攻撃力")]
    [SerializeField] private float attackPower;

    /// <summary> 
    /// 攻撃のトリガーとなるプレイヤーとの距離
    /// </summary>
    [Header("攻撃のトリガーとなるプレイヤーとの距離")]
    [SerializeField] private float triggerDistance;

    /// <summary> 攻撃待機時間(秒) </summary>
    [Header("攻撃待機時間(秒)")]
    [SerializeField] private float attackInterval;

    /// <summary> 攻撃する確率(%) </summary>
    [Header("攻撃した後に再攻撃する確率(%)")]
    [SerializeField] private float attackPercent;

    [Header("trueなら、攻撃中に被ダメージモーションを起こさない")]
    [SerializeField] private bool hasSuperArmor;

    /// <summary> 攻撃可能ならtrue </summary>
    private bool canAttack = false;

    /// <summary> 攻撃の確率を引き当てたらtrue </summary>
    private bool isAttackPercent = false;

    // ------------------------------
    // 必殺技
    // ------------------------------

    [Header("必殺技の威力")]
    [SerializeField] private float ultPower;

    /// <summary> 必殺技のインターバル時間(秒) </summary>
    [Header("必殺技のインターバル時間(秒)")]
    [SerializeField] private float ultInterval;

    [Header("必殺技の速度")]
    [Range(1.0f, 5.0f)]
    [SerializeField] private float ultSpeed;

    /// <summary> 必殺技が使用可能ならtrue </summary>
    private bool canUlt = false;

    // ------------------------------
    // 回避
    // ------------------------------

    /// <summary> 回避インターバル時間(秒) </summary>
    [Header("回避インターバル時間(秒)")]
    [SerializeField] private float slidingInterval;

    /// <summary> 回避可能ならtrue </summary> 
    private bool canSliding = false;

    // ------------------------------
    // 防御
    // ------------------------------

    /// <summary> 防御時のカット率 </summary>
    [Header("防御時のカット率(%)")]
    [SerializeField] private float damageCutRatio;

    /// <summary> 防御インターバル時間(秒) </summary>
    [Header("防御インターバル時間(秒)")]
    [SerializeField] private float guardInterval;

    /// <summary> 防御可能ならtrue </summary>
    private bool canGuard = false;

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
    // 被ダメージ
    // ----------------------------

    /// <summary> 被ダメージからの無敵時間(秒) </summary>
    [Header("被ダメージからの無敵時間(秒)")]
    [SerializeField] private float invincibleTime;

    /// <summary> 無敵中ならtrue </summary>
    private bool isInvincible = false;

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

    /// <summary> 攻撃を検知できる時間(秒) </summary>
    [Header("攻撃を検知できる時間(秒)")]
    [SerializeField] private float attackReceptionTime;

    /// <summary> 必殺技を検知できる時間(秒) </summary>
    [Header("必殺技を検知できる時間(秒)")]
    [SerializeField] private float ultReceptionTime;

    // --------------------
    // AI制御用
    // --------------------

    /// <summary> 敵AI制御用 </summary>
    private NavMeshAgent enemyAi;

    // --------------------------------
    // アニメーション検知用
    // --------------------------------

    /// <summary> プレイヤーの必殺技アニメーション </summary>
    [Header("必殺技のアニメーション(プレイヤー)")]
    [SerializeField] private AnimationClip playerUltAnim;

    /// <summary> プレイヤーの攻撃1アニメーション </summary>
    [Header("プレイヤーの攻撃1アニメーション")]
    [SerializeField] private AnimationClip playerAttack1Anim;

    /// <summary> プレイヤーの攻撃2アニメーション </summary>
    [Header("プレイヤーの攻撃2アニメーション")]
    [SerializeField] private AnimationClip playerAttack2Anim;

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
    // タイマー
    // -----------------------

    /// <summary> 無敵時間用タイマー </summary>
    private float invincibleTimer = 0;

    /// <summary> 攻撃用タイマー </summary>
    private float attackTimer = 0;

    /// <summary> 必殺技用タイマー </summary>
    private float ultTimer = 0;

    /// <summary> 回避用タイマー </summary>
    private float slidingTimer = 0;

    /// <summary> 防御用タイマー </summary>
    private float guardTimer = 0;

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

        // 待機状態から始める
        ChangeState(State.Idle);
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
            // 攻撃
            // ------------------------
            case State.Attack:

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
        if (canUlt)
        {
            ChangeState(State.Ult);
            return;
        }
        // 必殺技が使用不可で攻撃可能
        else if (canAttack)
        {
            // タイマーリセット
            attackTimer = 0;

            // 攻撃状態に移行
            ChangeState(State.Attack);
            return;
        }

        // -------------------------------------
        // プレイヤーの必殺技を検知したか 
        // -------------------------------------

        // プレイヤーの必殺技を検知し
        // 防御可能なら防御する
        if (CheckPlayerUlt() && canGuard)
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
        if (CheckPlayerAttack() && canSliding)
        {
            ChangeState(State.Sliding);
            return;
        }
        // 回避不可能で
        // 防御可能なら防御する
        else if (CheckPlayerAttack() && canGuard)
        {
            ChangeState(State.Guard);
            return;
        }

        // ----------------------------------
        // 攻撃の確率を引き当てたか
        // ----------------------------------

        // 攻撃の確率を引き当てたら攻撃
        if (isAttackPercent)
        {
            ChangeState(State.Attack);
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
        float rotateSpeed = moveSpeed * 10.0f;

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

    private void AnimAttackOk()
    {
        return;
    }

    /// <summary>
    /// 攻撃1の開始イベント
    /// </summary>
    private void AnimAttack1()
    {
        // 武器が当たるようにする
        canWeaponHit = true;

        LookTarget(player);
    }

    /// <summary>
    /// 攻撃1の終了イベント
    /// </summary>
    private void AnimAttack1Faild()
    {
        // プレイヤーがまだ近くにいる場合
        if (IsNear(player))
        {
            // 連続攻撃するので状態を維持する
            return;
        }

        // 武器が当たらないようにする
        canWeaponHit = false;

        // 攻撃終了
        enemyAnimator.SetBool(isAttack, false);

        // この後また攻撃するか抽選する
        isAttackPercent = CheckProbability(attackPercent);

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
        isAttackPercent = CheckProbability(attackPercent);

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
    }

    /// <summary>
    /// 必殺技の終了イベント
    /// </summary>
    private void AnimUltFailed()
    {
        // 武器が当たらないようにする
        canWeaponHit = false;

        // タイマーリセット
        ultTimer = 0;

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
    /// 防御の終了イベント
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
        ChangeState(State.Idle);
    }

    #endregion

    #region 死亡イベント

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

        // 無敵時間中はダメージを受けない
        if (isInvincible)
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

        // 防御中ならtrue
        bool isGuard = currentState == State.Guard;

        // 防御中
        if (isGuard)
        {
            // ダメージの軽減値
            float cut = damage * (damageCutRatio / 100);
            damage -= cut;
            hp -= damage;
        }
        else
        {
            // ダメージ分HPを減らす
            hp -= damage;
        }

        // HPが無くなったら死亡
        if (hp <= 0)
        {
            ChangeState(State.Dead);
            return;
        }

        // スーパーアーマーなら攻撃中に
        // 被ダメージモーションはしない
        if (currentState == State.Attack &&
            hasSuperArmor)
        {
            return;
        }

        // 防御中なら被ダメージモーションはしない
        if (isGuard)
        {
            return;
        }

        // 被ダメージ後の無敵時間を設定
        invincibleTimer = invincibleTime;

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

        // 通常の攻撃
        if (currentState == State.Attack)
        {
            // 通常の攻撃力を参照
            playerManager.ReceiveDamage(attackPower);
        }

        // 必殺技
        if (currentState == State.Ult)
        {
            // 必殺技の威力を参照
            playerManager.ReceiveDamage(ultPower);
        }
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
        if (!enemyAnimator || !playerAnimator)
        {
            Debug.Log("Animatorがありません");
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
        return distance < triggerDistance;
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
        // 必殺技のアニメーションが設定されていない
        if (!playerUltAnim)
        {
            Debug.Log("プレイヤーの必殺技が設定されていません");
            return false;
        }

        // プレイヤーが必殺技をしていない
        if (!IsPlayingAnim(playerAnimator, playerUltAnim))
        {
            return false;
        }

        // プレイヤーの必殺技の判定開始時間
        float ultStartTime = playerUltAnim.events[0].time;

        // アニメーションの経過時間
        float elapsedTime = GetAnimElapsedTime(playerAnimator);

        // プレイヤーの必殺技のアニメーションが
        // ある程度再生されたら検知
        return elapsedTime >= ultStartTime &&
            elapsedTime < ultStartTime + ultReceptionTime;
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
        // プレイヤーの攻撃が設定されていない
        if (!CheckPlayerAttakAnim())
        {
            return false;
        }

        // プレイヤーが攻撃していない
        if (!IsPlayingAnim(playerAnimator, playerAttack1Anim) &&
            !IsPlayingAnim(playerAnimator, playerAttack2Anim))
        {
            return false;
        }

        // 攻撃の経過時間
        float elapsedTime = GetAnimElapsedTime(playerAnimator);

        // 攻撃検知可能な時間を経過しているので
        // 攻撃を検知できない
        if (elapsedTime > attackReceptionTime)
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
        return playerUltAnim.length * currentTimeRatio;
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
        if (!playerAttack1Anim)
        {
            Debug.Log("プレイヤーの攻撃1が設定されていません");
            return false;
        }

        // プレイヤーの攻撃2が設定されていない
        if (!playerAttack1Anim)
        {
            Debug.Log("プレイヤーの攻撃2が設定されていません");
            return false;
        }

        return true;
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

                // アニメーション速度を初期化
                enemyAnimator.speed = 1;

                // ダッシュモーション終了
                enemyAnimator.SetBool(isRun, false);

                // 待機状態に移行
                currentState = State.Idle;

                break;

            // ----------------------
            // 追尾状態に移行
            // ----------------------
            case State.Chase:

                // 最大速度を設定
                enemyAi.speed = chaseSpeed;

                // 加速度を設定
                enemyAi.acceleration = chaseAcceleration;

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
            // 攻撃状態に移行
            // -----------------------
            case State.Attack:

                // 攻撃アニメーション開始
                enemyAnimator.SetBool(isAttack, true);

                // 攻撃状態に移行
                currentState = State.Attack;

                break;

            case State.Ult:

                // 必殺技の速度を設定
                enemyAnimator.speed = ultSpeed;

                // 必殺技アニメーション開始
                enemyAnimator.SetTrigger(ultTirgger);

                // 必殺技状態に移行
                currentState = State.Ult;

                break;

            // -----------------------
            // 回避状態に移行
            // -----------------------
            case State.Sliding:

                // 回避アニメーション開始
                enemyAnimator.SetTrigger(slidingTrigger);

                // 回避状態に移行
                currentState = State.Sliding;

                break;

            // ------------------------
            // 防御状態に移行
            // ------------------------
            case State.Guard:

                // 防御アニメーション開始
                enemyAnimator.SetTrigger(guardTrigger);

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

        // 無敵時間を減らす
        if (isInvincible) invincibleTimer -= Time.deltaTime;

        // 攻撃してからの時間を計測
        if (!canAttack) attackTimer += Time.deltaTime;

        // 必殺技を使用してからの時間を計測
        if (!canUlt) ultTimer += Time.deltaTime;

        // 回避してからの時間を計測
        if (!canSliding) slidingTimer += Time.deltaTime;

        // 防御してからの時間を計測
        if (!canGuard) guardTimer += Time.deltaTime;

        // ----------------------------------
        // 一定時間経ったか確認
        // ----------------------------------

        isInvincible = invincibleTimer > 0;

        canAttack = attackTimer >= attackInterval;
        canUlt = ultTimer >= ultInterval;
        canSliding = slidingTimer >= slidingInterval;
        canGuard = guardTimer >= guardInterval;
    }
}
