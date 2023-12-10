using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CS_UIControl : MonoBehaviour
{
    [SerializeField,Header("Player��Prefab")]
    GameObject goPlayer;
    //! @brief Player��MaxHP
   public float playerHpMax;
    [SerializeField,Header("PlayerHP�o�[")]
    Image cpPlayerHP;
    [SerializeField, Header("�K�E�Z�o�[")]
    Image cpSpecialAttack;

    [SerializeField, Header("Enemy��Prefab")]
    GameObject goEnemy;
    [SerializeField, Header("EnemyHP�o�[")]
    Image cpEnemyHP;
    //! @brief �G��MaxHP
    float enemyHpMax;
    //! @brief �G��HP
    float enemyHp;

    //! @brief ���l�̃X�N���v�g�i�[
    CS_Titan csTitan = null;
    //! @brief �V���@
    //! @brief Player�~���[


    void Start()
    {
        //! �e�L�����AStart�֐��Őݒ肳�ꂽ�l�͂����ł͔��f����Ȃ��B
        //! Awake�֐��ŏ����������Ă��炤���c

        //! Todo:�e�L�����̃X�e�[�^�X�̏�����
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

    //! @brief Stage�ɍ������G��HP��ݒ�
    void SetEnemyHP()
    {
        if (csTitan != null)
        {
            enemyHp = csTitan.Hp;
        }
    }
    //! @brief PlayerHP�o�[����
    void ControlPlayerHPBar()
    {
        var csPlayer = goPlayer.GetComponent<CS_Player>();
        cpPlayerHP.fillAmount = (float)csPlayer.Hp / playerHpMax;

        if (cpPlayerHP.fillAmount <= 0.0f)
        {
            cpPlayerHP.fillAmount = 0.0f;
        }
    }
    //! @brief ����񕜃o�[����
    void ControlSpecilAttackBar()
    {
        var csPlayer = goPlayer.GetComponent<CS_Player>();
        //specialAttack.fillAmount = csPlayer.;
        if (cpSpecialAttack.fillAmount >= 1.0f)
        {
            cpSpecialAttack.fillAmount = 1.0f;
        }
    }

    //! @brief EnemyHP�o�[����
    void ControlEnemyHPBar()
    {
        cpEnemyHP.fillAmount = enemyHp / enemyHpMax;

        if (cpEnemyHP.fillAmount <= 0.0f)
        {
            cpEnemyHP.fillAmount = 0.0f;
        }
    }

}
