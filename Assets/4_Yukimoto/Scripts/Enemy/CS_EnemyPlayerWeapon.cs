using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CS_EnemyPlayerWeapon : MonoBehaviour
{
    /// <summary> ��Ԋm�F�p </summary>
    private CS_EnemyPlayer enemyPlayer;

    // Start is called before the first frame update
    private void Start()
    {
        enemyPlayer = transform.parent.parent.GetComponent<CS_EnemyPlayer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // �����傪�擾�ł��Ȃ�
        if (!enemyPlayer)
        {
            Debug.Log("��������擾�ł��Ă��Ȃ�");
            return;
        }

        // �v���C���[�ɓ������ĂȂ��Ȃ牽�����Ȃ�
        if(other.tag != "Player")
        {
            return;
        }

        // �U�����E�K�E�Z���łȂ��Ȃ牽�����Ȃ�
        if (enemyPlayer.CurrentState != CS_EnemyPlayer.State.Attack &&
            enemyPlayer.CurrentState != CS_EnemyPlayer.State.Ult)
        {
            return;
        }

        // �U����������悤�ɂȂ��Ă���̂ŗ^�_���[�W����
        if (enemyPlayer.CanWeaponHit)
        {
            // �v���C���[�Ƀ_���[�W
            enemyPlayer.PlayerDamage();
        }
    }
}
