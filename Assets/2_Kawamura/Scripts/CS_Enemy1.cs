using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CS_Enemy1 : MonoBehaviour
{
    //UŒ‚‚Ìí—Ş
    enum AttackType
    {
        Weak,     //ãUŒ‚
        Strong,   //‹­UŒ‚
        BlowOff,  //‚«”ò‚Î‚µUŒ‚
    }
    AttackType attackType;

    [SerializeField] GameObject player;

    [SerializeField] GameObject weakMagicMissile;
    [SerializeField] GameObject strongMagicMissile;

    [SerializeField] int weakMagicMissileNumber;              //ˆê“x‚É¶¬‚·‚é’e‚Ì”
    [SerializeField] int strongMagicMissileNumber;            //ˆê“x‚É¶¬‚·‚é’e‚Ì”

    [SerializeField] float halfCircleRadius;            //”¼‰~‚Ì”¼Œa
    [SerializeField] Vector3 spawnPos = new Vector3(0.0f, 0.1f, 1.0f);  //“G1‚ğŠî€‚Æ‚µ‚½’e‚Ì¶¬ˆÊ’u

    [SerializeField] float weakAttackProbability;       //ãUŒ‚‚Ì”­¶Šm—¦
    [SerializeField] float strongAttackProbability;     //‹­UŒ‚‚Ì”­¶Šm—¦
    [SerializeField] float blowOffAttackProbability;    //‚«”ò‚Î‚µUŒ‚‚Ì”­¶Šm—¦

    [SerializeField] float weakCreationInterval;  //’e‚ğ¶¬‚·‚éŠÔŠui•bj
    [SerializeField] float strongCreationInterval;  //’e‚ğ¶¬‚·‚éŠÔŠui•bj

    [SerializeField] float weakShootInterval;        //Ÿ‚Ì’e‚ğ”­Ë‚·‚é‚Ü‚Å‚ÌŠÔŠuiãUŒ‚A•bj
    [SerializeField] float strongShootInterval;        //Ÿ‚Ì’e‚ğ”­Ë‚·‚é‚Ü‚Å‚ÌŠÔŠuiãUŒ‚A•bj

    [SerializeField] float maxAttackIntervalSeconds;    //UŒ‚‚ÌƒCƒ“ƒ^[ƒoƒ‹i•bj

    //‚±‚ÌƒR[ƒh“à‚Åg—p‚·‚é‚½‚ß‚Ì•Ï”
    GameObject[] magicMissile = new GameObject[2]; //’e
    int[] magicMissileNumber = new int[2];         //’e‚Ì”
    float[] creationInterval = new float[2];       //’e‚Ì¶¬‘¬“xi•bj
    float[] shootInterval;                         //˜AË‘¬“xi•bj
    GameObject[] createdMagicMissile;              //¶¬‚µ‚½’e
    CS_Enemy1MagicMissile[] script;                //’e‚ÌƒXƒNƒŠƒvƒg

    float attackIntervalSeconds;
    int magicMissileCount = 1;
    int evenCount = 0;  //‹ô””­–Ú‚Ì’e‚ª¶¬‚³‚ê‚½”
    int oddCount = 0;   //1”­–Ú‚ğœ‚­Šï””­–Ú‚Ì’e‚ª¶¬‚³‚ê‚½”
    bool isAttack;      //UŒ‚’†‚©H

    float[] weight = new float[3];  //ŠeUŒ‚‚Ìd‚İi”­¶Šm—¦j
    float totalWeight;  //3í—Ş‚ÌUŒ‚‚Ìd‚İi”­¶Šm—¦j‚Ì‘˜a

    // Start is called before the first frame update
    void Start()
    {
        //•Ï”‚Ì‰Šú‰»
        Initialize();

        //Å‰‚ÌUŒ‚‚ğİ’è
        weight[0] = weakAttackProbability;
        weight[1] = strongAttackProbability;
        weight[2] = blowOffAttackProbability;
        for(int i = 0; i < GetEnumLength<AttackType>(); ++i)
        {
            totalWeight += weight[i];
        }
        ChooseAttackType();


        //ƒGƒ‰[ƒƒbƒZ[ƒW
        for(int i = 0; i < 2; ++i)
        {
            if (magicMissileNumber[i] % 2 == 0)
            {
                AttackType type = (AttackType)i;
                Debug.Log("(" + type + ")‹ô””­‚Ì’e”‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚·BŠï””­‚Ì’e”‚É•ÏX‚µ‚Ä‚­‚¾‚³‚¢B");
            }
        }
    }

    /// <summary>
    /// ‰Šú‰»ˆ—
    /// </summary>
    private void Initialize()
    {
        //ãUŒ‚
        int num = (int)AttackType.Weak;
        magicMissile[num] = weakMagicMissile;
        magicMissileNumber[num] = weakMagicMissileNumber;
        creationInterval[num] = 0.0f;  //1”­–Ú‚Í‘¦À‚É¶¬‚·‚é‚½‚ß0
        //‹­UŒ‚
        num = (int)AttackType.Strong;
        magicMissile[num] = strongMagicMissile;
        magicMissileNumber[num] = strongMagicMissileNumber;
        creationInterval[num] = 0.0f;

        //‚»‚Ì‘¼•Ï”‚Ì‰Šú‰»
        int max = Math.Max(magicMissileNumber[(int)AttackType.Weak],
                           magicMissileNumber[(int)AttackType.Strong]);
        shootInterval = new float[max];
        createdMagicMissile = new GameObject[max];
        script = new CS_Enemy1MagicMissile[max];
        for (int i = 0; i < max; ++i)
        {
            shootInterval[i] = 0.0f;
            createdMagicMissile[i] = null;
            script[i] = null;
        }
        isAttack = false;
        attackIntervalSeconds = maxAttackIntervalSeconds;
    }

    // Update is called once per frame
    void Update()
    {
        //ƒvƒŒƒCƒ„[‚Ì•ûŒü‚ğŒü‚­
        LookAtPlayer();

        //UŒ‚
        if (isAttack)
        {
            Shoot(attackType);  //’e‚ğ”­Ë‚·‚é

            //UŒ‚‚ªI—¹‚µ‚½‚çŸ‚ÌUŒ‚‚Ìí—Ş‚ğŒˆ’è
            if (!isAttack)
            {
                ChooseAttackType();
            }
            return;
        }

        //ƒCƒ“ƒ^[ƒoƒ‹‚ªŒo‰ß‚µ‚½‚çŸ‚ÌUŒ‚‚ÖˆÚ‚é
        attackIntervalSeconds -= Time.deltaTime;
        if(attackIntervalSeconds <= 0.0f)
        {
            //’e‚Ì¶¬
            int num = (int)attackType;  //—v‘f”Ô†w’è—p•Ï”
            creationInterval[num] -= Time.deltaTime;
            if (creationInterval[num] <= 0.0f)
            {
                CreateMagicMissile(attackType);
            }
        }
    }

    /// <summary>
    /// ’e‚ğ”­Ë‚·‚é‚Ü‚Å‚ÌŠÔ‚ğİ’è‚·‚é
    /// </summary>
    /// <param name="type">UŒ‚‚Ìí—ŞiãE‹­j</param>
    /// <param name="iteration">¶¬‚µ‚½’e‚ÌƒCƒeƒŒ[ƒVƒ‡ƒ“</param>
    void SetTimeUntilShoot(AttackType type, int iteration)
    {
        if(type == AttackType.Weak)
        {
            shootInterval[iteration] = weakShootInterval * (iteration + 1);
        }
        else
        {
            shootInterval[iteration] = strongShootInterval * (iteration + 1);
        }
    }

    /// <summary>
    /// ŒJ‚èo‚·UŒ‚‚Ìí—Ş‚ğ‘I‚Ô
    /// </summary>
    void ChooseAttackType()
    {
        //ˆÊ’u‚ğ‘I‚Ô
        float randomPoint = UnityEngine.Random.Range(0, totalWeight);

        //’Š‘I
        float currentWeight = 0.0f;  //Œ»İ‚Ìd‚İ‚ÌˆÊ’u
        for(int i = 0; i < GetEnumLength<AttackType>(); ++i)
        {
            currentWeight += weight[i];

            if(randomPoint < currentWeight)
            {
                SetAttackType(i);
                return;
            }
        }

        //ˆÊ’u‚ªd‚İ‚Ì‘˜aˆÈã‚È‚ç––”ö—v‘f‚Æ‚·‚é
        SetAttackType(weight.Length - 1) ;
    }

    /// <summary>
    /// UŒ‚‚Ìí—Ş‚ğİ’è‚·‚é
    /// </summary>
    /// <param name="iteration">ƒCƒeƒŒ[ƒVƒ‡ƒ“</param>
    void SetAttackType(int iteration)
    {
        if(iteration == 0)
        {
            attackType = AttackType.Weak;
        }
        if(iteration == 1)
        {
            attackType = AttackType.Strong;
        }
        if(iteration == 2)
        {
            attackType = AttackType.BlowOff;
        }
        Debug.Log(attackType);
    }

    /// <summary>
    /// enumŒ^‚Ì—v‘f”‚ğæ“¾‚·‚é
    /// </summary>
    /// <typeparam name="T">enumŒ^</typeparam>
    /// <returns>—v‘f”</returns>
    int GetEnumLength<T>()
    {
        return Enum.GetValues(typeof(T)).Length;
    }

    /// <summary>
    /// ƒvƒŒƒCƒ„[‚Ì•ûŒü‚ğŒü‚­
    /// </summary>
    void LookAtPlayer()
    {
        if(player == null)
        {
            return;
        }

        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0.0f;  //X²‰ñ“]‚ğ”½‰f‚µ‚È‚¢
        transform.forward = direction;
    }

    /// <summary>
    /// ’e‚ğ¶¬‚·‚é
    /// </summary>
    /// <param name="type">UŒ‚‚Ìí—ŞiãE‹­j</param>
    void CreateMagicMissile(AttackType type)
    {
        int num = (int)type;  //—v‘f”Ô†w’è—p•Ï”

        float angleSpace = 180.0f / magicMissileNumber[num];  //’e“¯m‚ÌŠÔŠu
        const float baseAngle = 90.0f;  //1‚Â–Ú‚Ì’e‚Ì”z’uŠp“x
        float angle = 0.0f;

        //”¼‰~‚Ì‚Ç‚±‚É”z’u‚·‚é‚©Œˆ’è
        if (magicMissileCount == 1)  //1”­–Ú
        {
            angle = baseAngle;
        }
        else if (magicMissileCount % 2 == 0)  //‹ô””­–Ú
        {
            evenCount++;
            angle = baseAngle - evenCount * angleSpace;  //“G‚©‚ç‚İ‚Ä¶‚É‡‚É”z’u
        }
        else  //Šï””­–Ú
        {
            oddCount++;
            angle = baseAngle + oddCount * angleSpace;   //“G‚©‚çŒ©‚Ä‰E‚É‡‚É”z’u
        }

        //“G1‚Ì‰ñ“]‚ğl—¶‚µ‚ÄÀ•WŒˆ’è
        Vector3 magicMissilePos = new Vector3(
            spawnPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * halfCircleRadius,
            spawnPos.y + Mathf.Sin(angle * Mathf.Deg2Rad) * halfCircleRadius,
            spawnPos.z);
        magicMissilePos = transform.TransformPoint(magicMissilePos);

        //¶¬
        //magicMissileCount‚Í1‚©‚çn‚Ü‚é‚½‚ß-1
        createdMagicMissile[magicMissileCount - 1] =
            Instantiate(magicMissile[num], magicMissilePos, Quaternion.identity);
        createdMagicMissile[magicMissileCount - 1].transform.SetParent(gameObject.transform);  //“G‚Æ’e‚ğeqŠÖŒW‚É
        script[magicMissileCount - 1] =
            createdMagicMissile[magicMissileCount - 1].GetComponent<CS_Enemy1MagicMissile>();

        //Še•Ï”‚ÌXV
        SetTimeUntilShoot(type, magicMissileCount - 1);
        magicMissileCount++;
        if(type == AttackType.Weak)
        {
            creationInterval[num] = weakCreationInterval;
        }
        else
        {
            creationInterval[num] = strongCreationInterval;
        }

        //‘S‚Ä¶¬‚µ‚½‚ç¶¬‚ğ~‚ßAUŒ‚‚ÉˆÚ‚é
        if (magicMissileCount > magicMissileNumber[num])
        {
            isAttack = true;      //UŒ‚’†
            magicMissileCount = 1;
            evenCount = oddCount = 0;
            creationInterval[num] = 0.0f;  
        }
    }

    /// <summary>
    /// ’e‚ğ”­Ë‚·‚é
    /// </summary>
    void Shoot(AttackType type)
    {
        int num = (int)type;  //—v‘f”Ô†w’è—p•Ï”

        for (int i = 0; i < magicMissileNumber[num]; ++i)
        {
            if (createdMagicMissile[i] == null)
            {
                continue;
            }

            //”­ËŠÔ‚É‚È‚Á‚½‚ç”­Ë
            shootInterval[i] -= Time.deltaTime;
            if (shootInterval[i] < 0.0f)
            {
                script[i].GetSetIsCanFire = true;  //”­Ë

                //‰Šú‰»
                createdMagicMissile[i] = null;
                script[i] = null;

                //ÅŒã‚Ì’e‚ğŒ‚‚Á‚½‚çUŒ‚I—¹
                if (i == magicMissileNumber[num] - 1)
                {
                    isAttack = false;
                    attackIntervalSeconds = maxAttackIntervalSeconds;
                }
            }
        }
    }
}


//using System;
//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;

//public class CS_Enemy1 : MonoBehaviour
//{
//    //UŒ‚‚Ìí—Ş
//    enum AttackType
//    {
//        Weak,     //ãUŒ‚
//        Strong,   //‹­UŒ‚
//        BlowOff,  //‚«”ò‚Î‚µUŒ‚
//    }
//    AttackType type;

//    //’e‚ğŒ‚‚ÂUŒ‚
//    struct ShootAttack
//    {
//        GameObject magicMissilePrefab;  //’e‚ÌƒvƒŒƒnƒu
//        int magicMissileNumber;         //’e‚Ì”
//        //float Probability;              //UŒ‚‚Ì”­¶Šm—¦
//        float creationInterval;         //’e‚Ì¶¬‘¬“xi•bj
//        float shootInterval;            //˜AË‘¬“xi•bj
//    }
//    ShootAttack[] shootAttack = new ShootAttack[2];

//    [SerializeField] GameObject player;
//    [SerializeField] GameObject magicMissilePrefab;
//    [SerializeField] int magicMissileNumber;            //ˆê“x‚É¶¬‚·‚é’e‚Ì”
//    [SerializeField] float halfCircleRadius;            //”¼‰~‚Ì”¼Œa
//    [SerializeField] Vector3 spawnPos = new Vector3(0.0f, 0.1f, 1.0f);  //“G1‚ğŠî€‚Æ‚µ‚½’e‚Ì¶¬ˆÊ’u

//    [SerializeField] float weakAttackProbability;       //ãUŒ‚‚Ì”­¶Šm—¦
//    [SerializeField] float strongAttackProbability;     //‹­UŒ‚‚Ì”­¶Šm—¦
//    [SerializeField] float blowOffAttackProbability;    //‚«”ò‚Î‚µUŒ‚‚Ì”­¶Šm—¦

//    [SerializeField] float maxCreationIntervalSeconds;  //’e‚ğ¶¬‚·‚éŠÔŠui•bj
//    [SerializeField] float maxWeakShootInterval;        //Ÿ‚Ì’e‚ğ”­Ë‚·‚é‚Ü‚Å‚ÌŠÔŠuiãUŒ‚A•bj
//    [SerializeField] float maxAttackIntervalSeconds;    //UŒ‚‚ÌƒCƒ“ƒ^[ƒoƒ‹i•bj

//    GameObject[] createdMagicMissile;                   //¶¬‚µ‚½’e
//    CS_Enemy1MagicMissile[] script;

//    float creationIntervalSeconds;
//    float[] weakShootInterval;    //’e‚ğ”­Ë‚·‚é‚Ü‚Å‚ÌŠÔ
//    float attackIntervalSeconds;
//    int magicMissileCount = 1;
//    int evenCount = 0;  //‹ô””­–Ú‚Ì’e‚ª¶¬‚³‚ê‚½”
//    int oddCount = 0;   //1”­–Ú‚ğœ‚­Šï””­–Ú‚Ì’e‚ª¶¬‚³‚ê‚½”
//    bool isAttack;      //UŒ‚’†‚©H

//    float[] weight = new float[3];  //ŠeUŒ‚‚Ìd‚İi”­¶Šm—¦j
//    float totalWeight;  //3í—Ş‚ÌUŒ‚‚Ìd‚İi”­¶Šm—¦j‚Ì‘˜a

//    // Start is called before the first frame update
//    void Start()
//    {
//        //Å‰‚ÌUŒ‚‚ğİ’è
//        weight[0] = weakAttackProbability;
//        weight[1] = strongAttackProbability;
//        weight[2] = blowOffAttackProbability;
//        for(int i = 0; i < GetEnumLength<AttackType>(); ++i)
//        {
//            totalWeight += weight[i];
//        }

//        ChooseAttackType();

//        //UŒ‚‚Ìí—Ş‚É‡‚í‚¹‚½’l‚ğ•Ï”‚ÉŠi”[‚·‚é



//        //•Ï”‚Ì‰Šú‰»
//        creationIntervalSeconds = 0.0f;  //1”­–Ú‚Í‘¦À‚É¶¬‚·‚é‚½‚ß0
//        isAttack = false;
//        attackIntervalSeconds = maxAttackIntervalSeconds;
//        createdMagicMissile = new GameObject[magicMissileNumber];
//        script = new CS_Enemy1MagicMissile[magicMissileNumber];
//        weakShootInterval = new float[magicMissileNumber];
//        for(int i = 0; i < magicMissileNumber; ++i)
//        {
//            createdMagicMissile[i] = null;
//            script[i] = null;
//            weakShootInterval[i] = 0.0f;
//        }

//        if (magicMissileNumber % 2 == 0)
//        {
//            Debug.Log("i“G‚Pj‹ô””­‚Ì’e”‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚·BŠï””­‚Ì’e”‚É•ÏX‚µ‚Ä‚­‚¾‚³‚¢B");
//        }
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        ChooseAttackType();
//        Debug.Log(type);

//        //ƒvƒŒƒCƒ„[‚Ì•ûŒü‚ğŒü‚­
//        LookAtPlayer();

//        //UŒ‚
//        if (isAttack)
//        {
//            Shoot();  //’e‚ğ”­Ë‚·‚é

//            //UŒ‚‚ªI—¹‚µ‚½‚çŸ‚ÌUŒ‚‚Ìí—Ş‚ğŒˆ’è
//            if (!isAttack)
//            {
                
//            }
//            return;
//        }

//        //ƒCƒ“ƒ^[ƒoƒ‹‚ªŒo‰ß‚µ‚½‚çŸ‚ÌUŒ‚‚ÖˆÚ‚é
//        attackIntervalSeconds -= Time.deltaTime;
//        if(attackIntervalSeconds <= 0.0f)
//        {
//            //’e‚Ì¶¬
//            creationIntervalSeconds -= Time.deltaTime;
//            if (creationIntervalSeconds <= 0.0f)
//            {
//                CreateMagicMissile();
//            }
//        }


//        //”­Ë‚µ‚½‚çmagicMissileCount‚Ì‰Šú‰»‚ğ–Y‚ê‚¸‚É
//    }

//    /// <summary>
//    /// ’e‚ğ”­Ë‚·‚é‚Ü‚Å‚ÌŠÔ‚ğİ’è‚·‚é
//    /// </summary>
//    /// <param name="iteration">¶¬‚µ‚½’e‚ÌƒCƒeƒŒ[ƒVƒ‡ƒ“</param>
//    void SetValueOfTimeUntilShoot(int iteration)
//    {
//        weakShootInterval[iteration] = maxWeakShootInterval * (iteration + 1);
//    }

//    /// <summary>
//    /// ŒJ‚èo‚·UŒ‚‚Ìí—Ş‚ğ‘I‚Ô
//    /// </summary>
//    void ChooseAttackType()
//    {
//        //ˆÊ’u‚ğ‘I‚Ô
//        float randomPoint = UnityEngine.Random.Range(0, totalWeight);

//        //’Š‘I
//        float currentWeight = 0.0f;  //Œ»İ‚Ìd‚İ‚ÌˆÊ’u
//        for(int i = 0; i < GetEnumLength<AttackType>(); ++i)
//        {
//            currentWeight += weight[i];

//            if(randomPoint < currentWeight)
//            {
//                SetAttackType(i);
//                return;
//            }
//        }

//        //ˆÊ’u‚ªd‚İ‚Ì‘˜aˆÈã‚È‚ç––”ö—v‘f‚Æ‚·‚é
//        SetAttackType(weight.Length - 1) ;
//    }

//    /// <summary>
//    /// AttackTypeŒ^•Ï”‚É’l‚ğİ’è‚·‚é
//    /// </summary>
//    /// <param name="iteration">ƒCƒeƒŒ[ƒVƒ‡ƒ“</param>
//    void SetAttackType(int iteration)
//    {
//        if(iteration == 0)
//        {
//            type = AttackType.Weak;
//        }
//        if(iteration == 1)
//        {
//            type = AttackType.Strong;
//        }
//        if(iteration == 2)
//        {
//            type = AttackType.BlowOff;
//        }
//    }

//    /// <summary>
//    /// enumŒ^‚Ì—v‘f”‚ğæ“¾‚·‚é
//    /// </summary>
//    /// <typeparam name="T">enumŒ^</typeparam>
//    /// <returns>—v‘f”</returns>
//    int GetEnumLength<T>()
//    {
//        return Enum.GetValues(typeof(T)).Length;
//    }

//    /// <summary>
//    /// ƒvƒŒƒCƒ„[‚Ì•ûŒü‚ğŒü‚­
//    /// </summary>
//    void LookAtPlayer()
//    {
//        Vector3 direction = player.transform.position - transform.position;
//        direction.y = 0.0f;  //X²‰ñ“]‚ğ”½‰f‚µ‚È‚¢
//        transform.forward = direction;
//    }

//    /// <summary>
//    /// ’e‚ğ¶¬‚·‚é
//    /// </summary>
//    void CreateMagicMissile()
//    {
//        float angleSpace = 180.0f / magicMissileNumber;  //’e“¯m‚ÌŠÔŠu
//        const float baseAngle = 90.0f;  //1‚Â–Ú‚Ì’e‚Ì”z’uŠp“x
//        float angle = 0.0f;

//        //”¼‰~‚Ì‚Ç‚±‚É”z’u‚·‚é‚©Œˆ’è
//        if (magicMissileCount == 1)  //1”­–Ú
//        {
//            angle = baseAngle;
//        }
//        else if (magicMissileCount % 2 == 0)  //‹ô””­–Ú
//        {
//            evenCount++;
//            angle = baseAngle - evenCount * angleSpace;  //“G‚©‚ç‚İ‚Ä¶‚É‡‚É”z’u
//        }
//        else  //Šï””­–Ú
//        {
//            oddCount++;
//            angle = baseAngle + oddCount * angleSpace;   //“G‚©‚çŒ©‚Ä‰E‚É‡‚É”z’u
//        }

//        //“G1‚Ì‰ñ“]‚ğl—¶‚µ‚ÄÀ•WŒˆ’è
//        Vector3 magicMissilePos = new Vector3(
//            spawnPos.x + Mathf.Cos(angle * Mathf.Deg2Rad) * halfCircleRadius,
//            spawnPos.y + Mathf.Sin(angle * Mathf.Deg2Rad) * halfCircleRadius,
//            spawnPos.z);
//        magicMissilePos = transform.TransformPoint(magicMissilePos);

//        //¶¬
//        //magicMissileCount‚Í1‚©‚çn‚Ü‚é‚½‚ß-1
//        createdMagicMissile[magicMissileCount - 1] = 
//            Instantiate(magicMissilePrefab, magicMissilePos, Quaternion.identity);
//        createdMagicMissile[magicMissileCount - 1].transform.SetParent(gameObject.transform);  //“G‚Æ’e‚ğeqŠÖŒW‚É
//        script[magicMissileCount - 1] =
//            createdMagicMissile[magicMissileCount - 1].GetComponent<CS_Enemy1MagicMissile>();


//        //Še•Ï”‚ÌXV
//        SetValueOfTimeUntilShoot(magicMissileCount - 1);
//        magicMissileCount++;
//        creationIntervalSeconds = maxCreationIntervalSeconds;

//        //‘S‚Ä¶¬‚µ‚½‚ç¶¬‚ğ~‚ßAUŒ‚‚ÉˆÚ‚é
//        if (magicMissileCount > magicMissileNumber)
//        {
//            isAttack = true;      //UŒ‚’†
//            magicMissileCount = 1;
//            evenCount = oddCount = 0;
//            creationIntervalSeconds = 0.0f;  
//        }
//    }

//    /// <summary>
//    /// ’e‚ğ”­Ë‚·‚é
//    /// </summary>
//    void Shoot()
//    {
//        for(int i = 0; i < magicMissileNumber; ++i)
//        {
//            if (createdMagicMissile[i] == null)
//            {
//                continue;
//            }

//            //”­ËŠÔ‚É‚È‚Á‚½‚ç”­Ë
//            weakShootInterval[i] -= Time.deltaTime;
//            if (weakShootInterval[i] < 0.0f)
//            {
//                script[i].GetSetIsCanFire = true;

//                createdMagicMissile[i] = null;
//                script[i] = null;

//                //ÅŒã‚Ì’e‚ğŒ‚‚Á‚½‚çUŒ‚I—¹
//                if(i == magicMissileNumber - 1)
//                {
//                    isAttack = false;
//                    attackIntervalSeconds = maxAttackIntervalSeconds;
//                }
//            }
//        }
//    }
//}
