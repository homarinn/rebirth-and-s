using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CS_UIControl : MonoBehaviour
{
    //[SerializeField]
    //CS_Player csPlayer;
    [SerializeField]
    [Header("PlayerHPバー")]
    Image playerHP;
    [SerializeField]
    [Header("必殺技バー")]
    Image specialMove;

    //[SerializeField]
    //CS_Enemy csEnemy;
    [SerializeField]
    [Header("EnemyHPバー")]
    Image enemyHP;

    //! test
    float cooldownTime = 5.0f;
    float currentTime = 0.0f;


    void Start()
    {
        //! Todo:各キャラのステータスの初期化
        specialMove.fillAmount = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {

        ControlPlayerHPBar();
        ControlSpecilMoveBar();

        ControlEnemyHPBar();

        //! test
        if (Input.GetKeyDown(KeyCode.Space))
        {
            specialMove.fillAmount = 0.0f;
        }
    }


    //! @brief PlayerHPバー操作
    void ControlPlayerHPBar()
    {
        //! Todo:PlayerScriptからHP取得(割合に変換して代入)
        //playerHP.fillAmount -= 0.001f;
        if (playerHP.fillAmount <= 0.0f)
        {
            playerHP.fillAmount = 0.0f;
        }
    }
    //! @brief 精霊回復バー操作
    void ControlSpecilMoveBar()
    {
        //! Todo:SpilitScriptからゲージ取得(割合に変換して代入)
        //specialMove.fillAmount += 0.001f;
        if (specialMove.fillAmount >= 1.0f)
        {
            specialMove.fillAmount = 1.0f;
        }
    }

    //! @brief EnemyHPバー操作
    void ControlEnemyHPBar()
    {
        //! Todo:EnemyScriptからHP取得(割合に変換して代入)
        //enemyHP.fillAmount -= 0.0001f;
        if (enemyHP.fillAmount <= 0.0f)
        {
            enemyHP.fillAmount = 0.0f;
        }
    }

}
