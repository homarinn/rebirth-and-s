using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CS_UIControl : MonoBehaviour
{
    [SerializeField,Header("PlayerのPrefab")]
    GameObject goPlayer;
    //! @brief PlayerのMaxHP
    float playerHpMax;
    [SerializeField,Header("PlayerHPバー")]
    Image cpPlayerHP;
    [SerializeField, Header("必殺技バー")]
    Image cpUlt;
    float ultInterval;

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
    CS_Enemy1 csEnemy01 = null;
    //! @brief Playerミラー
    CS_EnemyPlayer csEnPlayer = null;

    [SerializeField, Header("セリフUI")]
    GameObject goDialogue;

    //! @brief 一度だけ処理を行うフラグ
    bool bOnce = false;

    void Start()
    {
        bOnce = false;

        //! 各キャラのステータスの初期化
        playerHpMax = (float)goPlayer.GetComponent<CS_Player>().Hp;
        ultInterval = goPlayer.GetComponent<CS_Player>().UltTimer;
        

        csTitan = goEnemy.GetComponent<CS_Titan>();
        if(csTitan != null)
        {
            enemyHpMax = csTitan.Hp;
        }
        csEnemy01 = goEnemy.GetComponent<CS_Enemy1>();
        if (csEnemy01 != null)
        {
            enemyHpMax = csEnemy01.GetHp;
        }
        csEnPlayer = goEnemy.GetComponent<CS_EnemyPlayer>();
        if(csEnPlayer != null)
        {
            enemyHpMax = csEnPlayer.Hp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        SetEnemyHP();

        ControlPlayerHPBar();
        ControlSpecilAttackBar();

        ControlEnemyHPBar();

        if ((enemyHp <= (enemyHpMax * 0.5f)) && (bOnce == false) && (goDialogue.GetComponent<CS_Dialogue>().Benable == false))
        {
            bOnce = true;
        }
        Debug.Log("once : " + bOnce);
    }

    //! @brief Stageに合った敵のHPを設定
    void SetEnemyHP()
    {
        if (csTitan != null)
        {
            enemyHp = csTitan.Hp;
        }
        if(csEnemy01 != null)
        {
            enemyHp = csEnemy01.GetHp;
        }
        if(csEnPlayer != null)
        {
            enemyHp = csEnPlayer.Hp;
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
    //! @brief 必殺技インターバルバー
    void ControlSpecilAttackBar()
    {
        var csPlayer = goPlayer.GetComponent<CS_Player>();
        cpUlt.fillAmount = 1.0f - (csPlayer.UltTimer / ultInterval);
        if (cpUlt.fillAmount >= 1.0f)
        {
            cpUlt.fillAmount = 1.0f;
        }
    }

    //! @brief EnemyHPバー操作
    void ControlEnemyHPBar()
    {
        cpEnemyHP.fillAmount = enemyHp / enemyHpMax;

        if (cpEnemyHP.fillAmount <= 0.0f)
        {
            cpEnemyHP.fillAmount = 0.0f;
            goDialogue.GetComponent<CS_Dialogue>().Benable = true;
        }
    }

}
