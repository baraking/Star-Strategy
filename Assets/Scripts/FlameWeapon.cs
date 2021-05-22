using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameWeapon : MonoBehaviour
{

    public GameObject firePrefab;
    public GameObject existingFire;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire()
    {
        existingFire = Instantiate(firePrefab);
        existingFire.transform.localPosition = transform.position;
        existingFire.transform.rotation = transform.rotation;
        existingFire.GetComponentInChildren<ParticleSystem>().Play();
    }

    public void StopFiring()
    {
        if (existingFire != null)
        {
            existingFire.GetComponentInChildren<ParticleSystem>().Stop();
        }
    }
}
