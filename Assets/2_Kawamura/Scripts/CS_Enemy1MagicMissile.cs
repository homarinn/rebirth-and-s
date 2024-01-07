using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Enemy1MagicMissile : MonoBehaviour
{
    [Header("水溜り")]
    [SerializeField] GameObject puddle;

    [Header("移動速度（直線軌道）")]
    [SerializeField] float moveSpeed;

    [Header("攻撃力")]
    [SerializeField] float attackPower;

    [Header("ステージに接触して消滅するまでの速さ（秒）")]
    [SerializeField] float disappearTime;

    [Header("着弾するまでの時間（秒、曲線軌道用）")]
    [SerializeField] float period;


    GameObject puddleObject;    //水溜り
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
        float randomX = Random.Range(-20.0f, 20.0f);
        float randomY = Random.Range(-10.0f, 10.0f);
        float randomZ = Random.Range(-20.0f, 20.0f);
        velocity = new Vector3(randomX, randomY, randomZ);
    }

    // Update is called once per frame
    void Update()
    {
        //プレイヤーに向けて弾を発射する
        if (isCanFire)
        {
            //親子関係を解除しないと発射後も親の回転値が軌道に影響を与えてしまう
            transform.parent = null;

            //プレイヤーに向けて発射（曲線軌道）
            if(isCurve)
            //if(period != 0.0f)
            {
                targetPosition = playerTransform.position;

                isCanFire = false;
                isMove = true;
            }
            //プレイヤーに向けて発射（直線軌道）
            else
            {
                direction = playerTransform.position - transform.position;
                direction.Normalize();
                myRigidbody.velocity = direction * moveSpeed;

                isCanFire = false;
            }
        }

        //プレイヤーに向けて移動する（曲線軌道の時のみ）
        if (isMove)
        //if (isMove && period >= 0.0f)
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

        //ステージに接触したら徐々に弾を小さくし、生成した水溜りを大きくする
        if (isCollisionStage)
        //if (isCollisionStage || period < 0)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / disappearTime);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            puddleObject.transform.localScale = Vector3.Lerp(puddleStartScale, puddleTargetScale, t);

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
        Vector3 position = new Vector3(transform.position.x, 0.2f, transform.position.z);
        puddleObject = Instantiate(puddle, position, Quaternion.identity);
        puddleTargetScale = puddleObject.transform.localScale;
        puddleObject.transform.localScale = new Vector3(0, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
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
