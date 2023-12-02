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
    [Header("�K�E�Z�A�C�R��")]
    Image specialMove;
    [SerializeField]
    [Header("�K�E�Z�}�X�N�A�C�R��")]
    Image spMask;

    //[SerializeField]
    //CS_Spirit csSpirit;
    [SerializeField]
    [Header("����񕜃o�[")]
    Image spilit;

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


    //! @brief PlayerHP�o�[����
    void ControlPlayerHP()
    {
        //! Todo:PlayerScript����HP�擾(�����ɕϊ����đ��)
        //playerHP.fillAmount -= 0.001f;
        if (playerHP.fillAmount <= 0.0f)
        {
            playerHP.fillAmount = 0.0f;
        }
    }
    //! @brief Player�K�E�Z�A�C�R������
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
    //! @brief ����񕜃o�[����
    void ControlSpilit()
    {
        //! Todo:SpilitScript����Q�[�W�擾(�����ɕϊ����đ��)
        spilit.fillAmount += 0.001f;
        if (spilit.fillAmount >= 1.0f)
        {
            spilit.fillAmount = 1.0f;
        }
    }

    //! @brief EnemyHP�o�[����
    void ControlEnemyHP()
    {
        //! Todo:EnemyScript����HP�擾(�����ɕϊ����đ��)
        //enemyHP.fillAmount -= 0.0001f;
        if (enemyHP.fillAmount <= 0.0f)
        {
            enemyHP.fillAmount = 0.0f;
        }
    }

}
