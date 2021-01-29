using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walkable : Unit
{
    public Vector3 targetPoint;
    public GameObject targetObject;

    void Update()
    {
        if (targetPoint != Vector3.zero)
        {
            WalkTowards();
        }
    }

    void WalkTowards()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, unitDetails.speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            targetPoint = Vector3.zero;
        }
        
    }
}
