using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlameWeapon : MonoBehaviour
{

    public GameObject fire;

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
        GameObject newExplosion = Instantiate(fire);
        newExplosion.transform.localPosition = transform.position;
        newExplosion.GetComponentInChildren<ParticleSystem>().Play();
    }
}
