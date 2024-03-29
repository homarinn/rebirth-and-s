using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CS_Enemy1MagicMissile : MonoBehaviour
{
    [Header("水溜り")]
    [SerializeField] CS_Enemy1Puddle puddle;

    [Header("ヒットエフェクト")]
    [SerializeField] GameObject hitEffect;

    [Header("ステージに接触して消滅するまでの速さ（秒）")]
    [SerializeField] float disappearTime;


    CS_Enemy1Puddle puddleObject;    //水溜り
    Transform playerTransform;
    Rigidbody myRigidbody;
    Vector3 direction;          //プレイヤーの方向
    float elapsedTime;
    bool isCanFire;             //射出可能か？
    bool isCollisionStage;      //ステージに当たったか？

    float moveSpeed;
    float attackPower;

    //弾と水溜りのスケール変化に使用
    Vector3 startScale;                    
    Vector3 targetScale;                   

    //実験用
    Vector3 velocity;
    bool isMove;

    //実験用2
    int magicMissileCount;
    Vector3[] curveDirection = new Vector3[3];

    //実験用3
    int puddleRenderQueue;

    float addAngle;

    //Vector3 scaleRatioBasedOnY;

    bool canCreatePuddle;  //水溜まりを生成できるか？

    //スケール大きくする用
    Vector3 targetScaleForCreate;
    float elapsedTimeForScaleUp;
    float timeScaleUpCompletely;
    bool isFinishScaleUp;

    bool isCollisionPlayer;

    string magicMissileType;  //弾の種類

    const float adjustPositionY = 0.3f;

    float towardsSpeed;

    bool isCollisionWeapon;

    //const float rotateSpeed = 518.0f;

    //float time = 0.0f;
    //bool isTowards = false;

    //ゲッターセッター
    public float SetMoveSpeed
    {
        set { moveSpeed = value; }
    }
    public float SetAttackPower
    {
        set { attackPower = value; }
    }

    public bool GetSetIsCanFire
    {
        get { return isCanFire; }
        set { isCanFire = value; }
    }
    public Transform SetPlayerTransform
    {
        set { playerTransform = value; }
    }
    public int SetMagicMissileCount
    {
        set { magicMissileCount = value; }
    }

    public int SetPuddleRenderQueue
    {
        set { puddleRenderQueue = value; }
    }
    //public Vector3 SetScaleRatioBasedOnY
    //{
    //    set { scaleRatioBasedOnY = value; }
    //}

    public bool SetCanCreatePuddle
    {
        set { canCreatePuddle = value; }
    }
    public Vector3 SetTargetScaleForCreate
    {
        set { targetScaleForCreate = value; }
    }
    public float SetTimeScaleUpCompletely
    {
        set { timeScaleUpCompletely = value; }
    }
    public string SetMagicMissileType
    {
        set
        {
            if(magicMissileType == null)
            {
                magicMissileType = value;
            }
        }
    }
    public string GetMagicMissileType
    {
        get { return magicMissileType; }
    }

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        isCanFire = false;
        isCollisionStage = false;
        elapsedTime = 0.0f;
        startScale = new Vector3(0, 0, 0);
        targetScale = new Vector3(0, 0, 0);

        //実験用
        isMove = false;
        towardsSpeed = 12.5f;

        curveDirection[0] = new Vector3(0, 15.0f, 0);
        curveDirection[1] = new Vector3(0.0f, 5.0f, -20.0f);
        curveDirection[2] = new Vector3(0.0f, 5.0f, 20.0f);

        ////親のスケールを反映しないYを計算
        //Vector3 parentLossyScale = transform.parent.lossyScale;
        //float scaleY = transform.localScale.y / parentLossyScale.y;
        ////XZが同じ比率になるようにYを基準とした比率をYにかけて代入
        //float newScaleX = scaleY * (scaleRatioBasedOnY.x);  //2.0 = 調整
        //float newScaleZ = scaleY * (scaleRatioBasedOnY.z);  //2.0 = 調整
        //transform.localScale = new Vector3(
        //    newScaleX,
        //    scaleY,
        //    newScaleZ);


        //Vector3 parentLossyScale = transform.parent.lossyScale;
        //float scaleY = transform.localScale.y / parentLossyScale.y;
        ////XZが同じ比率になるようにYを基準とした比率をYにかけて代入
        //float newScaleXZ = scaleY * (scaleRatioBasedOnY * 2.0f);  //2.0 = 調整
        //transform.localScale = new Vector3(
        //    newScaleXZ,
        //    scaleY,
        //    newScaleXZ);

        //弾の先端を前にする
        Transform parent = transform.parent;
        addAngle = 360.0f - transform.parent.transform.localEulerAngles.y;
        transform.rotation = Quaternion.Euler(90f, -90.0f, addAngle);
        //transform.rotation = Quaternion.Euler(90f, 0f, addAngle);  //元のMagicMissile用



        canCreatePuddle = true;
        //transform.rotation = Quaternion.Euler(0f, 0f, 90.0f);

        elapsedTimeForScaleUp = 0.0f;
        isFinishScaleUp = false;

        isCollisionPlayer = false;

        isCollisionWeapon = false;

        Vector3 tar = new Vector3(
            playerTransform.position.x,
            playerTransform.position.y + adjustPositionY,
            playerTransform.position.z);

        direction = tar - transform.position;
        direction.Normalize();
        transform.right = direction;
    }

    // Update is called once per frame
    void Update()
    {
        //回転させながらスケールを徐々に大きくする
        if (!isFinishScaleUp)
        {
            ScaleUp();

            //回転の時は不必要
            Vector3 tar = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y + adjustPositionY,
                playerTransform.position.z);

            direction = tar - transform.position;
            direction.Normalize();
            transform.right = direction;

            ////回転
            //Vector3 t = transform.right;
            ////Vector3 t = transform.parent.transform.forward;
            //if (magicMissileCount % 2 == 0)
            //{
            //    t *= -1.0f;
            //}
            //transform.RotateAround(transform.position, t, rotateSpeed * Time.deltaTime);

        }

        //プレイヤーに向けて弾を発射する
        if (isCanFire)
        {
            //親子関係を解除しないと発射後も親の回転値が軌道に影響を与えてしまう
            if(transform.parent != null)
            {
                transform.parent = null;
            }

            //プレイヤーに向けて発射
            Vector3 target = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y + adjustPositionY,
                playerTransform.position.z);
            direction = target - transform.position;
            direction.Normalize();
            velocity = direction * moveSpeed;
            myRigidbody.velocity = velocity;

            isCanFire = false;
            isMove = true;
        }

        //プレイヤーに向けて移動する
        if (isMove)
        {
            //進行方向を向いていないときだけ進行方向を向かせる
            if (transform.right != myRigidbody.velocity)
            {
                transform.right =
                    Vector3.Slerp(transform.right, myRigidbody.velocity, Time.deltaTime * towardsSpeed);
            }
        }
        //生成が終わって発射前の時
        else if(isFinishScaleUp)
        {
            //プレイヤーのほうに向かせる
            Vector3 tar = new Vector3(
                playerTransform.position.x,
                playerTransform.position.y + adjustPositionY,
                playerTransform.position.z);

            direction = tar - transform.position;
            direction.Normalize();
            transform.right = direction;

            ////回転の時必要
            //Vector3 tar = new Vector3(
            //    playerTransform.position.x,
            //    playerTransform.position.y + adjustPositionY,
            //    playerTransform.position.z);

            //direction = tar - transform.position;
            //direction.Normalize();
            //if (transform.right != direction)
            //{
            //    transform.right =
            //        Vector3.Slerp(transform.right, direction, Time.deltaTime * 1.5f);
            //}
        }


        //ステージに接触したら徐々に弾を小さくし、生成した水溜りを大きくする
        if (isCollisionStage)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / disappearTime);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            //弾の消去
            if (t == 1)
            {
                Destroy(gameObject);
            }
        }

        //弾の消去
        if (transform.position.y < -5.0f)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 弾を徐々に大きくする
    /// </summary>
    void ScaleUp()
    {
        elapsedTimeForScaleUp += Time.deltaTime;
        float t = Mathf.Clamp01(elapsedTimeForScaleUp / timeScaleUpCompletely);
        transform.localScale = Vector3.Lerp(Vector3.zero, targetScaleForCreate, t);

        if(t >= 1)
        {
            transform.localScale = targetScaleForCreate;
            isFinishScaleUp = true;
        }
    }

    /// <summary>
    /// 水溜りを生成する
    /// </summary>
    void CreatePuddle()
    {
        Vector3 position = new Vector3(
            transform.position.x, 0.0f, transform.position.z);
        puddleObject = Instantiate(puddle, position, Quaternion.identity);

        puddleObject.SetRenderQueue = puddleRenderQueue;
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (isCollisionStage) { return; }

        //発射するまで当たり判定しない
        if (!isMove)
        {
            return;
        }

        //プレイヤーに当たっていたら判定しない(2回当たるバグ対策)
        if (isCollisionPlayer)
        {
            return;
        }

        //武器に当たったら消す
        if(!isCollisionStage && other.gameObject.tag == "PlayerWeapon")
        {
            isCollisionWeapon = true;
            Destroy(gameObject);
        }

        //武器に当たっていたら判定しない
        if (isCollisionWeapon)
        {
            return;
        }

        //水溜まり生成可能ならプレイヤーとの当たり判定しない
        //プレイヤーに当たったらHPを減らして弾を消す
        if (!isCollisionStage && other.gameObject.tag == "Player")
        {
            //HP減らす
            var script = other.gameObject.GetComponent<CS_Player>();
            script.ReceiveDamage(attackPower);
            isCollisionPlayer = true;

            //エフェクト出す
            Vector3 instancePosition = other.transform.position;
            instancePosition.y += adjustPositionY * 2.5f;
            Instantiate(hitEffect, instancePosition, Quaternion.identity);

            //消す
            Destroy(gameObject);
        }

        ////敵に接触したら敵のHPを減らす（プレイヤーが跳ね返したときのみ）
        //if (isHitBack && other.gameObject.tag == "Enemy")
        //{
        //    var script = other.gameObject.GetComponent<CS_Enemy1>();
        //    script.ReduceHp(attackPower);

        //    Destroy(gameObject);
        //}

        //水溜まり外でステージに接触したら水溜りを生成
        if (!isCollisionStage && canCreatePuddle && 
            other.gameObject.tag == "Stage")
        {
            if (puddleObject == null)
            {
                isCollisionStage = true;
                startScale = transform.localScale;
                CreatePuddle();
            }
        }

        //水溜まり内でステージに接触したら水溜まりは生成しない
        if (!isCollisionStage && !canCreatePuddle &&
            other.gameObject.tag == "Stage")
        {
            isCollisionStage = true;
            startScale = transform.localScale;
        }
    }
}
