using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Notice the collider is a sphere and not a cone or something.
//find other resources in view in case the cur one was moved / destroyed
public class Ant : Gatherer
{
    GameObject detectionRange;

    private void Update()
    {
        if (targetResource != null)
        {
            if (Vector3.Distance(this.transform.position, targetResource.transform.position) < 0.1f)
            {
                Gather(targetResource);
            }
            else
            {
                WalkTowards();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (targetResource == null && other.GetComponent<Resource>())
        {
            targetResource = other.GetComponent<Resource>();
            targetPoint = other.transform.position;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (targetResource != null)
        {
            if (other.GetComponent<Resource>() == targetResource)
            {
                //find other resources in view
                targetResource = null;
                hasTarget = false;
            }
        }
    }

    void WalkTowards()
    {
        targetPoint = targetResource.transform.position;
        Vector3 targetDirection = targetPoint - transform.position;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, unitDetails.rotation_speed * Time.deltaTime, 0.0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, unitDetails.speed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPoint) < 0.1f)
        {
            hasTarget = false;
        }
    }
}
