using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_Enemy1PuddleSurfaceCollider : MonoBehaviour
{
    Transform parentTransform;
    bool isDecideParentPosition;

    // Start is called before the first frame update
    void Start()
    {
        parentTransform = transform.root.gameObject.transform;
        isDecideParentPosition = false;

        //Vector3 groundNormal = GetGroundNormal();
        //float angle = Vector3.Angle(Vector3.up, groundNormal);
        //Debug.Log(angle);
        //if(angle < 3.0f)
        //{
        //    parentTransform.rotation = Quaternion.FromToRotation(Vector3.up, groundNormal);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        //…‚½‚Ü‚è‚Ì•\–Ê‚ðŒÀ‚è‚È‚­’n–Ê‚É‹ß‚Ã‚¯‚é
        //if (!isDecideParentPosition)
        //{
        //    Vector3 addPosition = new Vector3(0.0f, -0.4f * Time.deltaTime, 0.0f);
        //    parentTransform.position += addPosition;
        //}
    }

    //Vector3 GetGroundNormal()
    //{
    //    RaycastHit hit;
    //    if (Physics.Raycast(parentTransform.transform.position, Vector3.down, out hit))
    //    {
    //        return hit.normal;
    //    }

    //    return Vector3.up;
    //}

    private void OnTriggerEnter(Collider other)
    {
        if(!isDecideParentPosition && other.gameObject.tag == "Stage")
        {
            Debug.Log("ˆÊ’uŒˆ’è");
            isDecideParentPosition = true;
        }
    }
}
