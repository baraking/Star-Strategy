using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Notice the collider is a sphere and not a cone or something.
public class Ant : Gatherer
{
    GameObject detectionRange;

    private void Update()
    {
        if (targetResource != null)
        {
            WalkTowards();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other);
        if (targetResource == null && other.GetComponent<Resource>())
        {
            targetResource = other.GetComponent<Resource>();
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
