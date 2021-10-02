using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePositions : MonoBehaviour
{

    public List<ResourceSpawner> resourcePositions;
    public GameObject resourcePrefab;

    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            resourcePositions.Add(transform.GetChild(i).GetComponent<ResourceSpawner>());

            //if is the master
            //resourcePositions[i].SpawnResources(resourcePrefab);
        }
    }

    void Update()
    {
        
    }
}
