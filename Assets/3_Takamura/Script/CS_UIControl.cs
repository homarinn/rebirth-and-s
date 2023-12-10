using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CS_UIControl : MonoBehaviour
{
    [SerializeField,Header("PlayerのPrefab")]
    GameObject goPlayer;
    //! @brief PlayerのMaxHP
   public float playerHpMax;
    [SerializeField,Header("PlayerHPバー")]
    Image cpPlayerHP;
    [SerializeField, Header("必殺技バー")]
    Image cpSpecialAttack;

    [SerializeField, Header("EnemyのPrefab")]
    GameObject goEnemy;
    [SerializeField, Header("EnemyHPバー")]
    Image cpEnemyHP;
    //! @brief 敵のMaxHP
    float enemyHpMax;
    //! @brief 敵のHP
    float enemyHp;

    //! @brief 巨人のスクリプト格納
    CS_Titan csTitan = null;
    //! @brief シヴァ
    //! @brief Playerミラー


    void Start()
    {
        //! 各キャラ、Start関数で設定された値はここでは反映されない。
        //! Awake関数で初期化をしてもらうか…

        //! Todo:各キャラのステータスの初期化
        playerHpMax = (float)goPlayer.GetComponent<CS_Player>().Hp;

        

        csTitan = goEnemy.GetComponent<CS_Titan>();
        if(csTitan != null)
        {
            enemyHpMax = csTitan.Hp;
        }       
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(goPlayer.GetComponent<CS_Player>().Hp);
        SetEnemyHP();

        ControlPlayerHPBar();
        ControlSpecilAttackBar();

        ControlEnemyHPBar();

        //! test
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cpSpecialAttack.fillAmount = 0.0f;
        }
    }

    //! @brief Stageに合った敵のHPを設定
    void SetEnemyHP()
    {
        if (csTitan != null)
        {
            enemyHp = csTitan.Hp;
        }
    }
    //! @brief PlayerHPバー操作
    void ControlPlayerHPBar()
    {
        var csPlayer = goPlayer.GetComponent<CS_Player>();
        cpPlayerHP.fillAmount = (float)csPlayer.Hp / playerHpMax;

        if (cpPlayerHP.fillAmount <= 0.0f)
        {
            cpPlayerHP.fillAmount = 0.0f;
        }
    }
    //! @brief 精霊回復バー操作
    void ControlSpecilAttackBar()
    {
        var csPlayer = goPlayer.GetComponent<CS_Player>();
        //specialAttack.fillAmount = csPlayer.;
        if (cpSpecialAttack.fillAmount >= 1.0f)
        {
            cpSpecialAttack.fillAmount = 1.0f;
        }
    }

    //! @brief EnemyHPバー操作
    void ControlEnemyHPBar()
    {
        cpEnemyHP.fillAmount = enemyHp / enemyHpMax;

        if (cpEnemyHP.fillAmount <= 0.0f)
        {
            cpEnemyHP.fillAmount = 0.0f;
        }
    }

}
