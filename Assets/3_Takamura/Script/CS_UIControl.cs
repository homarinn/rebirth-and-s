using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CS_UIControl : MonoBehaviour
{
    //[SerializeField]
    //CS_Player csPlayer;
    [SerializeField]
    [Header("PlayerHP�o�[")]
    Image playerHP;
    [SerializeField]
    [Header("�K�E�Z�o�[")]
    Image specialMove;

    //[SerializeField]
    //CS_Enemy csEnemy;
    [SerializeField]
    [Header("EnemyHP�o�[")]
    Image enemyHP;

    //! test
    float cooldownTime = 5.0f;
    float currentTime = 0.0f;


    void Start()
    {
        //! Todo:�e�L�����̃X�e�[�^�X�̏�����
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


    //! @brief PlayerHP�o�[����
    void ControlPlayerHPBar()
    {
        //! Todo:PlayerScript����HP�擾(�����ɕϊ����đ��)
        //playerHP.fillAmount -= 0.001f;
        if (playerHP.fillAmount <= 0.0f)
        {
            playerHP.fillAmount = 0.0f;
        }
    }
    //! @brief ����񕜃o�[����
    void ControlSpecilMoveBar()
    {
        //! Todo:SpilitScript����Q�[�W�擾(�����ɕϊ����đ��)
        //specialMove.fillAmount += 0.001f;
        if (specialMove.fillAmount >= 1.0f)
        {
            specialMove.fillAmount = 1.0f;
        }
    }

    //! @brief EnemyHP�o�[����
    void ControlEnemyHPBar()
    {
        //! Todo:EnemyScript����HP�擾(�����ɕϊ����đ��)
        //enemyHP.fillAmount -= 0.0001f;
        if (enemyHP.fillAmount <= 0.0f)
        {
            enemyHP.fillAmount = 0.0f;
        }
    }

}
