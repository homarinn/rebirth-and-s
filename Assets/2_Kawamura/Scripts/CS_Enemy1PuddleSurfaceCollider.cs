using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CS_Enemy1PuddleSurfaceCollider : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        //弾が水面に当たったら水溜まりを生成できなくする
        if(other.gameObject.tag == "MagicMissile")
        {
            var script = other.gameObject.GetComponent<CS_Enemy1MagicMissile>();
            script.SetCanCreatePuddle = false;
        }
    }
}
