using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CS_TestCamera : MonoBehaviour
{
    private GameObject playerObject;
    public float rotateSpeed = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        playerObject = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3‚ÅX,Y•ûŒü‚Ì‰ñ“]‚Ì“x‡‚¢‚ğ’è‹`
        Vector3 angle = new Vector3(Input.GetAxis("Mouse X") * rotateSpeed, 
            Input.GetAxis("Mouse Y") * rotateSpeed, 0);

        transform.RotateAround(playerObject.transform.position, Vector3.up, angle.x);
        transform.RotateAround(playerObject.transform.position, transform.right, angle.y);
    }
}
