using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveWeapon : MonoBehaviour
{

    public GameObject explosion;
    public Vector3 location;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Explode()
    {
        GameObject newExplosion = Instantiate(explosion);
        newExplosion.transform.localPosition = location;
        newExplosion.GetComponentInChildren<ParticleSystem>().Play();
    }
}
