using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//should work for the active camera regardless of the player.
public class Billboard : MonoBehaviour
{
    public Transform mainCamera;

    void LateUpdate()
    {
        transform.LookAt(transform.position + mainCamera.forward);
    }
}
