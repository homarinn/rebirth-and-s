using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CS_Enemy1MagicMissile : MonoBehaviour
{
    [Header("水溜り")]
    [SerializeField] CS_Enemy1Puddle puddle;

    [Header("移動速度（直線軌道）")]
    [SerializeField] float moveSpeed;

    [Header("攻撃力")]
    [SerializeField] float attackPower;

    [Header("ステージに接触して消滅するまでの速さ（秒）")]
    [SerializeField] float disappearTime;

    [Header("着弾するまでの時間（秒、曲線軌道用）")]
    [SerializeField] float period;


    CS_Enemy1Puddle puddleObject;    //水溜り
    Transform playerTransform;
    Rigidbody myRigidbody;
    Vector3 direction;          //プレイヤーの方向
    float elapsedTime;
    bool isCanFire;             //射出可能か？
    bool isCollisionStage;      //ステージに当たったか？

    //弾と水溜りのスケール変化に使用
    Vector3 startScale;                    
    Vector3 targetScale;                   
    Vector3 puddleStartScale;
    Vector3 puddleTargetScale;

    //実験用
    Vector3 velocity;
    Vector3 targetPosition;
    bool isMove;
    bool isCurve;
    bool isHitBack;  //跳ね返されたか？

    //実験用2
    int magicMissileCount;
    Vector3[] curveDirection = new Vector3[3];
    float rotateSpeed;

    //実験用3
    int puddleRenderQueue;

    float boudaryCircleRadius;

    float addAngle;

    float scaleRatioBasedOnY;
    

    //ゲッターセッター
    public bool GetSetIsCanFire
    {
        get { return isCanFire; }
        set { isCanFire = value; }
    }
    public bool SetIsCurve
    {
        set { isCurve = value; }
    }
    public bool SetIsHitBack
    {
        set { isHitBack = value; }
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
    public float SetBoundaryCircleRadius
    {
        set { boudaryCircleRadius = value; }
    }
    public float SetScaleRatioBasedOnY
    {
        set { scaleRatioBasedOnY = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        //playerTransform = GameObject.Find("Player").transform;
        isCanFire = false;
        isCollisionStage = false;
        elapsedTime = 0.0f;
        startScale = new Vector3(0, 0, 0);
        targetScale = new Vector3(0, 0, 0);
        puddleStartScale = new Vector3(0, 0, 0);
        puddleTargetScale = new Vector3(0, 0, 0);

        //実験用
        isMove = false;
        isHitBack = false;
        rotateSpeed = 10.0f;

        curveDirection[0] = new Vector3(0, 15.0f, 0);
        curveDirection[1] = new Vector3(0.0f, 5.0f, -20.0f);
        curveDirection[2] = new Vector3(0.0f, 5.0f, 20.0f);

        //float randomX = Random.Range(-20.0f, 20.0f);
        //float randomY = Random.Range(-10.0f, 10.0f);
        //float randomZ = Random.Range(-20.0f, 20.0f);
        //velocity = new Vector3(randomX, randomY, randomZ);

        //親のスケールを反映しない
        Vector3 parentLossyScale = transform.parent.lossyScale;
        float scaleY = transform.localScale.y / parentLossyScale.y;
        float newScaleXZ = scaleY * (scaleRatioBasedOnY * 2.0f);  //2.0 = 調整
        transform.localScale = new Vector3(
            newScaleXZ,
            scaleY,
            newScaleXZ);

        //弾の先端を前にする
        Transform parent = transform.parent;
        addAngle = 360.0f - transform.parent.transform.localEulerAngles.y;
        transform.rotation = Quaternion.Euler(90f, 0f, addAngle);


        //transform.rotation = Quaternion.Euler(0f, 0f, 90.0f);
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーに向けて弾を発射する
        if (isCanFire)
        {
            //親子関係を解除しないと発射後も親の回転値が軌道に影響を与えてしまう
            //transform.rotation = Quaternion.Euler(-90f, 0f, -addAngle);
            if(transform.parent != null)
            {
                transform.parent = null;
            }

            //プレイヤーに向けて発射（曲線軌道）
            if (isCurve)
            {
                targetPosition = playerTransform.position;
                InitializeVelocity();

                isCanFire = false;
                isMove = true;
            }
            //プレイヤーに向けて発射（直線軌道）
            else
            {
                direction = playerTransform.position - transform.position;
                direction.Normalize();
                velocity = direction * moveSpeed;
                myRigidbody.velocity = velocity;

                isCanFire = false;
                isMove = true;
            }
        }

        //プレイヤーに向けて移動する
        if (isMove)
        {
            //曲線軌道
            //直線軌道は発射時に力を与えるだけなので処理しない
            if (isCurve)
            {
                Vector3 acceleration = Vector3.zero;

                direction = targetPosition - transform.position;
                acceleration += (direction - velocity * period) * 2.0f / (period * period);

                period -= Time.deltaTime;
                if (period >= 0)
                {
                    velocity += acceleration * Time.deltaTime;
                    transform.position += velocity * Time.deltaTime;
                }
                else
                {
                    myRigidbody.velocity = velocity;
                    isMove = false;
                }
            }

            //進行方向を向いていないときだけ進行方向を向かせる
            if(transform.up != velocity)
            //if(transform.forward != velocity)
            {
                transform.up =
                    Vector3.Slerp(transform.up, velocity, Time.deltaTime * rotateSpeed);
                //transform.forward =
                //    Vector3.Slerp(transform.forward, velocity, Time.deltaTime * rotateSpeed);
            }
        }


        //ステージに接触したら徐々に弾を小さくし、生成した水溜りを大きくする
        if (isCollisionStage)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / disappearTime);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            //puddleObject.transform.localScale = Vector3.Lerp(puddleStartScale, puddleTargetScale, t);

            //弾の消去
            if(t == 1)
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
    /// 水溜りを生成する
    /// </summary>
    void CreatePuddle()
    {
        startScale = transform.localScale;
        Vector3 position = new Vector3(transform.position.x, 0.34f, transform.position.z);
        puddleObject = Instantiate(puddle, position, Quaternion.identity);
        //puddleTargetScale = puddleObject.transform.localScale;
        //puddleObject.transform.localScale = new Vector3(0, 0, 0);

        puddleObject.SetRenderQueue = puddleRenderQueue;
        puddleObject.SetBoundaryCircleRadius = boudaryCircleRadius;
        //Debug.Log(puddleRenderQueue);
    }

    void InitializeVelocity()
    {
        if (magicMissileCount == 1)
        {
            velocity = curveDirection[0];
        }
        else if (magicMissileCount % 2 == 0)
        {
            velocity = curveDirection[1];
        }
        else
        {
            velocity = curveDirection[2];
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Destroy(gameObject);
        }

        if(other.gameObject.tag == "Puddle")
        {
            Destroy(gameObject);
        }

        //敵に接触したら敵のHPを減らす（プレイヤーが跳ね返したときのみ）
        if(isHitBack && other.gameObject.tag == "Enemy")
        {
            var script = other.gameObject.GetComponent<CS_Enemy1>();
            script.ReduceHp(attackPower);

            Destroy(gameObject);
        }

        //ステージに接触したら水溜りを生成
        if (!isCollisionStage && other.gameObject.tag == "Stage")
        {
            if (puddleObject == null)
            {
                isCollisionStage = true;
                CreatePuddle();
            }
            //isCollisionStage = true;
            //startScale = transform.localScale;
            //Vector3 position = new Vector3(transform.position.x, 0.2f, transform.position.z);
            //puddleObject = Instantiate(puddle, position, Quaternion.identity);
            //puddleTargetScale = puddleObject.transform.localScale;
            //puddleObject.transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
