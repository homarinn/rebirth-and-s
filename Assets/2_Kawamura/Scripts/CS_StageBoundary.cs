using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CS_StageBoundary : MonoBehaviour
{
    private Transform center;  //ステージ境界円の中心
    [SerializeField] float boundaryCircleRadius;  //ステージ境界円の半径
    [SerializeField] GameObject[] character = new GameObject[characterNumber];  //制限する対象
    [SerializeField] bool isCheckRange;     //制限範囲を確認するか？

    private const int characterNumber = 2;  //制限する対象の数

    // Start is called before the first frame update
    void Start()
    {
        center = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < characterNumber; ++i)
        {
            if (!character[i])
            {
                return;
            }

            //X, Z軸方向のみ範囲を制限する
            Vector2 targetPosition = new Vector2(character[i].transform.position.x,
                                                 character[i].transform.position.z);
            Vector2 centerPosition = new Vector2(center.position.x, center.position.z);
            float distance = (targetPosition - centerPosition).sqrMagnitude;
          
            //範囲外の場合、位置を補正
            if (distance > boundaryCircleRadius * boundaryCircleRadius)
            {
                //中心座標に半径の長さのプレイヤーへのベクトルを加算して制限
                Vector2 direction = (targetPosition - centerPosition).normalized;
                Vector3 newPosition = new Vector3(
                    centerPosition.x + direction.x * boundaryCircleRadius,
                    character[i].transform.position.y,
                    centerPosition.y + direction.y * boundaryCircleRadius);

                character[i].transform.position = newPosition;
            }
        }
    }

    /// <summary>
    /// 制限範囲を表示する
    /// </summary>
    private void OnDrawGizmos()
    {
        //表示、非表示はインスペクターで設定してください。
        //スフィアで表示されますが、Y軸方向の制限は行わないので
        //X,Z軸方向のみを参考に範囲を確認してください。
        if (isCheckRange)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(gameObject.transform.position, boundaryCircleRadius);
        }
    }
}
