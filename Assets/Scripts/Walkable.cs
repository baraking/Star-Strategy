using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//unitDetails.speed in rotation could/should be rotation speed
//imporve the rotation of the walkable
public class Walkable : Unit
{
    [SerializeField]
    private bool hasTarget = false;
    [SerializeField]
    private Vector3 targetPoint;
    public GameObject targetObject;

    private void Start()
    {
        base.InitUnit();
    }

    void Update()
    {
        AttempotToWalk();
        if (Input.GetKey(PlayerButtons.DISEMBARK))
        {
            foreach(Unit unit in carriedUnits)
            {
                Disembark(unit);
            }
        }
    }

    public void SetHasTarget(bool newState)
    {
        hasTarget = newState;
    }

    public void SetTargetPoint(Vector3 newTargetPoint)
    {
        targetPoint = newTargetPoint;
    }

    public Vector3 GetTargetPoint()
    {
        return targetPoint;
    }

    public void AttempotToWalk()
    {
        if (hasTarget)
        {
            WalkTowards();
        }
    }

    void WalkTowards()
    {   
        Vector3 targetDirection = targetPoint - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, unitDetails.speed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, unitDetails.speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            hasTarget = false;
        }
        
    }
}
