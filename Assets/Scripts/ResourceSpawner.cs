using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    public static readonly int BASE_RESOURCES_NUMBER = 5;

    public void SpawnResources(GameObject resource)
    {
        int radius = GetComponent<DrawGizmo>().value;

        for (int i = 0; i < radius * BASE_RESOURCES_NUMBER; i++)
        {

        }
        //int x = Random in -radius,radius
        //int y = Random in -sqrt(radius^2-x^2),sqrt(radius^2-x^2)

        //random rotation

        //instantiate at (x,0,y) with random rotation
    }

    public void OnResourceDepleted()
    {

    }
}
