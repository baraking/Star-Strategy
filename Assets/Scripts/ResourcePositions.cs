using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePositions : MonoBehaviour
{

    public List<ResourceSpawner> resourcePositionsList;
    public GameObject resourcePrefab;

    void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            resourcePositionsList.Add(transform.GetChild(i).GetComponent<ResourceSpawner>());
        }
    }

    public void SpawnResourceInChildren()
    {
        foreach(ResourceSpawner spawner in resourcePositionsList)
        {
            spawner.SpawnResources(resourcePrefab);
        }
    }
}
