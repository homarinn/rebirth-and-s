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
    [Header("必殺技アイコン")]
    Image specialMove;
    [SerializeField]
    [Header("必殺技マスクアイコン")]
    Image spMask;

    //[SerializeField]
    //CS_Spirit csSpirit;
    [SerializeField]
    [Header("精霊回復バー")]
    Image spilit;

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
        spilit.fillAmount = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        ControlPlayerHP();
        ControlSpilit();
        ControlSpecilMove();

        ControlEnemyHP();

        //! test
        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentTime = cooldownTime;
        }
    }


    //! @brief PlayerHPバー操作
    void ControlPlayerHP()
    {
        //! Todo:PlayerScriptからHP取得(割合に変換して代入)
        //playerHP.fillAmount -= 0.001f;
        if (playerHP.fillAmount <= 0.0f)
        {
            playerHP.fillAmount = 0.0f;
        }
    }
    //! @brief Player必殺技アイコン操作
    void ControlSpecilMove()
    {
        if (currentTime <= 0.0f)
        {
            spMask.fillAmount = 0.0f;
        }
        else
        {
            currentTime -= Time.deltaTime;
            float val = Mathf.Clamp01(currentTime / cooldownTime);
            spMask.fillAmount = val;
        }
    }
    //! @brief 精霊回復バー操作
    void ControlSpilit()
    {
        //! Todo:SpilitScriptからゲージ取得(割合に変換して代入)
        spilit.fillAmount += 0.001f;
        if (spilit.fillAmount >= 1.0f)
        {
            spilit.fillAmount = 1.0f;
        }
    }

    //! @brief EnemyHPバー操作
    void ControlEnemyHP()
    {
        //! Todo:EnemyScriptからHP取得(割合に変換して代入)
        //enemyHP.fillAmount -= 0.0001f;
        if (enemyHP.fillAmount <= 0.0f)
        {
            enemyHP.fillAmount = 0.0f;
        }
    }

}
