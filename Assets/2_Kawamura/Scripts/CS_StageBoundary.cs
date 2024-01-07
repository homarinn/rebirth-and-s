using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class CS_StageBoundary : MonoBehaviour
{
    [Header("ステージ境界円の半径")]
    [SerializeField] float boundaryCircleRadius;  //ステージ境界円の半径

    [Header("範囲制限する対象キャラクター")]
    [SerializeField] GameObject[] character = new GameObject[characterNumber];  //制限する対象

    [Header("ステージ境界円の範囲確認(XZ軸方向のみ確認、チェックで範囲を表示)")]
    [SerializeField] bool isCheckRange;  //制限範囲を確認するか？

    private Transform center;  //ステージ境界円の中心
    private CapsuleCollider[] capsuleCollider = new CapsuleCollider[characterNumber];  //コライダー
    private bool[] isUseNavMesh = new bool[characterNumber];  //ナビメッシュを使用しているか？  

    private const int characterNumber = 2;  //制限する対象の数

    // Start is called before the first frame update
    void Start()
    {
        center = gameObject.transform;

        for(int i = 0; i < characterNumber; ++i)
        {
            isUseNavMesh[i] = false;
            if (!character[i])
            {
                continue;
            }

            //ナビメッシュを使用しているか調べる
            if (character[i].GetComponent<NavMeshAgent>())
            {
                isUseNavMesh[i] = true;  //使用している
            }

            //コライダーの取得
            if (character[i].GetComponent<CapsuleCollider>())
            {
                capsuleCollider[i] = character[i].GetComponent<CapsuleCollider>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //範囲制限を行う(X,Z軸方向のみ)
        for(int i = 0; i < characterNumber; ++i)
        {
            if (!character[i])
            {
                continue;
            }

            if (isUseNavMesh[i])
            {
                continue;
            }

            //キャラクターの半径を考慮した境界円半径を算出
            //半径が大きいほうを使用
            float effectiveRadius = 0.0f;  //有効な境界円半径
            float maxScale = Mathf.Max(
                character[i].transform.localScale.x, character[i].transform.localScale.z);
            if (capsuleCollider[i])
            {
                float characterRadius = 0.0f;  //キャラクターの半径
                characterRadius = capsuleCollider[i].radius * maxScale;
                effectiveRadius = boundaryCircleRadius - characterRadius;
            }
            else
            {
                effectiveRadius = boundaryCircleRadius - (maxScale * 0.5f);  //maxScaleの半分
            }

            //X, Z軸方向のみを考慮したステージの中心からの距離を算出
            Vector2 targetPosition = new Vector2(character[i].transform.position.x,
                                                 character[i].transform.position.z);
            Vector2 centerPosition = new Vector2(center.position.x, center.position.z);
            float distance = (targetPosition - centerPosition).sqrMagnitude;

            //範囲外の場合、位置を補正
            if (distance > effectiveRadius * effectiveRadius)
            {
                //中心座標に有効な境界円半径の長さのプレイヤーへのベクトルを加算して制限
                Vector2 direction = (targetPosition - centerPosition).normalized;
                Vector3 newPosition = new Vector3(
                    centerPosition.x + direction.x * effectiveRadius,
                    character[i].transform.position.y,
                    centerPosition.y + direction.y * effectiveRadius);

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
