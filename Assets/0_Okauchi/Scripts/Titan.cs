using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Titan : MonoBehaviour
{
    private const float STOPPING_DISTANCE = 0.5f;

    [SerializeField] private int hp = 0;
    [SerializeField] private float walkingSpeed = 0.0f;
    [SerializeField] private float rushingSpeed = 0.0f;
    [SerializeField] private float trackingValue = 0.0f;
    [SerializeField] private Transform targetTransform;   //•ÏX•s‰Â‚ÈŽQÆ‚Á‚Äinspector‚©‚çÝ’è‚Å‚«‚È‚¢‚Ì‚©

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 toTargetVector = targetTransform.position - transform.position;
        toTargetVector -= new Vector3(0.0f, toTargetVector.y, 0.0f);
        if (toTargetVector.magnitude >= STOPPING_DISTANCE)
        {
            TrackRotation(toTargetVector);
            Move(walkingSpeed);
        }
    }

    private void TrackRotation(Vector3 toTargetVector)
    {
        Quaternion lookingRotation = Quaternion.LookRotation(toTargetVector);

        lookingRotation = Quaternion.Slerp(transform.rotation, lookingRotation, Time.deltaTime * trackingValue);
        transform.rotation = lookingRotation;
    }

    private void Move(float speed)
    {
        transform.position += speed * transform.forward * Time.deltaTime;
    }

    public void ReceiveDamage(int damage)
    {
        hp -= damage;
        if(hp <= 0)
        {
            hp = 0;
        }
    }
}
