using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CS_UIControl : MonoBehaviour
{
    [SerializeField,Header("Player��Prefab")]
    GameObject goPlayer;
    //! @brief Player��MaxHP
    float playerHpMax;
    [SerializeField,Header("PlayerHP�o�[")]
    Image cpPlayerHP;
    [SerializeField, Header("�K�E�Z�o�[")]
    Image cpUlt;
    float ultInterval;

    [SerializeField, Header("Enemy��Prefab")]
    GameObject goEnemy;
    [SerializeField, Header("EnemyHP�o�[")]
    Image cpEnemyHP;
    //! @brief �G��MaxHP
    [SerializeField]float enemyHpMax;
    //! @brief �G��HP
    [SerializeField]float enemyHp;

    //! @brief ���l�̃X�N���v�g�i�[
    CS_Titan csTitan = null;
    //! @brief �V���@
    CS_Enemy1 csEnemy01 = null;
    //! @brief Player�~���[
    CS_EnemyPlayer csEnPlayer = null;


    void Start()
    {
        //! �e�L�����AStart�֐��Őݒ肳�ꂽ�l�͂����ł͔��f����Ȃ��B
        //! Awake�֐��ŏ����������Ă��炤���c

        //! Todo:�e�L�����̃X�e�[�^�X�̏�����
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
            enemyHpMax = 100;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(ultInterval);
        SetEnemyHP();

        ControlPlayerHPBar();
        ControlSpecilAttackBar();

        ControlEnemyHPBar();

        //! test
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    cpSpecialAttack.fillAmount = 0.0f;
        //}
    }

    //! @brief Stage�ɍ������G��HP��ݒ�
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
            enemyHp = 100;
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
    //! @brief �K�E�Z�C���^�[�o���o�[
    void ControlSpecilAttackBar()
    {
        var csPlayer = goPlayer.GetComponent<CS_Player>();
        cpUlt.fillAmount = 1.0f - (csPlayer.UltTimer / ultInterval);
        if (cpUlt.fillAmount >= 1.0f)
        {
            cpUlt.fillAmount = 1.0f;
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
