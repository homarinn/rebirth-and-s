using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CS_Enemy1 : MonoBehaviour
{
    enum AttackType
    {
        Weak,     //ãUŒ‚
        Strong,   //‹­UŒ‚
        BlowOff,  //‚«”ò‚Î‚µUŒ‚
    }

    [SerializeField] GameObject player;
    [SerializeField] GameObject magicMissilePrefab;
    [SerializeField] int magicMissileNumber;            //ˆê“x‚É¶¬‚·‚é’e‚Ì”
    [SerializeField] float halfCircleRadius;            //”¼‰~‚Ì”¼Œa
    [SerializeField] Vector3 spawnPos = new Vector3(0.0f, 0.1f, 1.0f);  //“G1‚ğŠî€‚Æ‚µ‚½’e‚Ì¶¬ˆÊ’u

    [SerializeField] float weakAttackProbability;       //ãUŒ‚‚Ì”­¶Šm—¦
    [SerializeField] float strongAttackProbability;     //‹­UŒ‚‚Ì”­¶Šm—¦
    [SerializeField] float blowOffAttackProbability;    //‚«”ò‚Î‚µUŒ‚‚Ì”­¶Šm—¦

    [SerializeField] float maxCreationIntervalSeconds;  //’e‚ğ¶¬‚·‚éŠÔŠui•bj
    [SerializeField] float maxWeakShootInterval;        //Ÿ‚Ì’e‚ğ”­Ë‚·‚é‚Ü‚Å‚ÌŠÔŠuiãUŒ‚A•bj
    [SerializeField] float maxAttackIntervalSeconds;    //UŒ‚‚ÌƒCƒ“ƒ^[ƒoƒ‹i•bj

    GameObject[] createdMagicMissile;                   //¶¬‚µ‚½’e
    CS_Enemy1MagicMissile[] script;

    float creationIntervalSeconds;
    float[] weakShootInterval;    //’e‚ğ”­Ë‚·‚é‚Ü‚Å‚ÌŠÔ
    float attackIntervalSeconds;
    int magicMissileCount = 1;
    int evenCount = 0;  //‹ô””­–Ú‚Ì’e‚ª¶¬‚³‚ê‚½”
    int oddCount = 0;   //1”­–Ú‚ğœ‚­Šï””­–Ú‚Ì’e‚ª¶¬‚³‚ê‚½”
    bool isAttack;      //UŒ‚’†‚©H

    // Start is called before the first frame update
    void Start()
    {
        //•Ï”‚Ì‰Šú‰»
        creationIntervalSeconds = 0.0f;  //1”­–Ú‚Í‘¦À‚É¶¬‚·‚é‚½‚ß0
        isAttack = false;
        attackIntervalSeconds = maxAttackIntervalSeconds;
        createdMagicMissile = new GameObject[magicMissileNumber];
        script = new CS_Enemy1MagicMissile[magicMissileNumber];
        weakShootInterval = new float[magicMissileNumber];
        for(int i = 0; i < magicMissileNumber; ++i)
        {
            createdMagicMissile[i] = null;
            script[i] = null;
            weakShootInterval[i] = 0.0f;
        }

        if (magicMissileNumber % 2 == 0)
        {
            Debug.Log("i“G‚Pj‹ô””­‚Ì’e”‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚·BŠï””­‚Ì’e”‚É•ÏX‚µ‚Ä‚­‚¾‚³‚¢B");
        }
    }

    // Update is called once per frame
    void Update()
    {
        //ƒvƒŒƒCƒ„[‚Ì•ûŒü‚ğŒü‚­
        LookAtPlayer();

        //UŒ‚
        if (isAttack)
        {
            Attack();  //’e‚ğ”­Ë‚·‚é

            //UŒ‚‚ªI—¹‚µ‚½‚çŸ‚ÌUŒ‚‚Ìí—Ş‚ğŒˆ’è
            if (!isAttack)
            {
                
            }
            return;
        }

        //ƒCƒ“ƒ^[ƒoƒ‹‚ªŒo‰ß‚µ‚½‚çŸ‚ÌUŒ‚‚ÖˆÚ‚é
        attackIntervalSeconds -= Time.deltaTime;
        if(attackIntervalSeconds <= 0.0f)
        {
            //’e‚Ì¶¬
            creationIntervalSeconds -= Time.deltaTime;
            if (creationIntervalSeconds <= 0.0f)
            {
                CreateMagicMissile();
            }
        }


        //”­Ë‚µ‚½‚çmagicMissileCount‚Ì‰Šú‰»‚ğ–Y‚ê‚¸‚É
    }

    /// <summary>
    /// ’e‚ğ”­Ë‚·‚é‚Ü‚Å‚ÌŠÔ‚ğİ’è‚·‚é
    /// </summary>
    /// <param name="iteration">¶¬‚µ‚½’e‚ÌƒCƒeƒŒ[ƒVƒ‡ƒ“</param>
    void SetValueOfTimeUntilShoot(int iteration)
    {
        weakShootInterval[iteration] = maxWeakShootInterval * (iteration + 1);
    }

    /// <summary>
    /// ŒJ‚èo‚·UŒ‚‚Ìí—Ş‚ğŒˆ’è‚·‚é
    /// </summary>
    void DecideAttackType()
    {

    }

    /// <summary>
    /// ƒvƒŒƒCƒ„[‚Ì•ûŒü‚ğŒü‚­
    /// </summary>
    void LookAtPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0.0f;  //X²‰ñ“]‚ğ”½‰f‚µ‚È‚¢
        transform.forward = direction;
    }

    /// <summary>
    /// ’e‚ğ¶¬‚·‚é
    /// </summary>
    void CreateMagicMissile()
    {
        float angleSpace = 180.0f / magicMissileNumber;  //’e“¯m‚ÌŠÔŠu
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
            Instantiate(magicMissilePrefab, magicMissilePos, Quaternion.identity);
        createdMagicMissile[magicMissileCount - 1].transform.SetParent(gameObject.transform);  //“G‚Æ’e‚ğeqŠÖŒW‚É
        script[magicMissileCount - 1] =
            createdMagicMissile[magicMissileCount - 1].GetComponent<CS_Enemy1MagicMissile>();


        //Še•Ï”‚ÌXV
        SetValueOfTimeUntilShoot(magicMissileCount - 1);
        magicMissileCount++;
        creationIntervalSeconds = maxCreationIntervalSeconds;

        //‘S‚Ä¶¬‚µ‚½‚ç¶¬‚ğ~‚ßAUŒ‚‚ÉˆÚ‚é
        if (magicMissileCount > magicMissileNumber)
        {
            isAttack = true;      //UŒ‚’†
            magicMissileCount = 1;
            evenCount = oddCount = 0;
            creationIntervalSeconds = 0.0f;  
        }
    }

    /// <summary>
    /// ’e‚ğ”­Ë‚·‚é
    /// </summary>
    void Attack()
    {
        for(int i = 0; i < magicMissileNumber; ++i)
        {
            if (createdMagicMissile[i] == null)
            {
                continue;
            }

            //”­ËŠÔ‚É‚È‚Á‚½‚ç”­Ë
            weakShootInterval[i] -= Time.deltaTime;
            if (weakShootInterval[i] < 0.0f)
            {
                script[i].GetSetIsCanFire = true;

                createdMagicMissile[i] = null;
                script[i] = null;

                //ÅŒã‚Ì’e‚ğŒ‚‚Á‚½‚çUŒ‚I—¹
                if(i == magicMissileNumber - 1)
                {
                    isAttack = false;
                    attackIntervalSeconds = maxAttackIntervalSeconds;
                }
            }
        }
    }
}
