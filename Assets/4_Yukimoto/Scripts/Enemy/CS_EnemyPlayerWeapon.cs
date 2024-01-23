using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CS_EnemyPlayerWeapon : MonoBehaviour
{
    /// <summary> �v���C���[�̃_���[�W�p </summary>
    [Header("�v���C���[")]
    [SerializeField] private CS_Player player;

    /// <summary> ��Ԋm�F�p </summary>
    private CS_EnemyPlayer enemyPlayer;

    // Start is called before the first frame update
    private void Start()
    {
        enemyPlayer = transform.parent.GetComponent<CS_EnemyPlayer>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // �U�����E�K�E�Z���łȂ��Ȃ牽�����Ȃ�
        if (enemyPlayer.CurrentState != CS_EnemyPlayer.State.Attack &&
            enemyPlayer.CurrentState != CS_EnemyPlayer.State.Ult)
        {
            return;
        }

        // �v���C���[�ɖ���
        if (other.tag == "Player" && enemyPlayer.canWeaponHit)
        {
            // ���i�h�~�Ŗ����ł��Ȃ��悤�ɂ���
            enemyPlayer.canWeaponHit = false;

            // �v���C���[�Ƀ_���[�W
            player.Damage(1);
        }
    }
}
