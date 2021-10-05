using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceSpawner : MonoBehaviour
{
    public static readonly int BASE_RESOURCES_NUMBER = 5;
    public List<GameObject> myResources = new List<GameObject>();

    public void SpawnResources(GameObject resource)
    {
        float radius = GetComponent<DrawGizmo>().value * 0.1f;

        float r2 = radius * radius;

        for (int i = 0; i < GetComponent<DrawGizmo>().value * BASE_RESOURCES_NUMBER; i++)
        {
            float x = Random.Range(-radius, radius);
            float x2 = x * x;
            float z = Random.Range(-Mathf.Sqrt(r2-x2), Mathf.Sqrt(r2 - x2));

            myResources.Add(Photon.Pun.PhotonNetwork.Instantiate(resource.name, new Vector3(x + transform.position.x,0, z + transform.position.z), Quaternion.Euler(0,Random.Range(0f,360f),0), 0));
        }
    }

    public void OnResourceDepleted()
    {

    }
}
