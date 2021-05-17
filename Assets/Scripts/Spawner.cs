using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//shouold add spawn point
//fix the whole spawning command. wrote a basic one for testing only
//should change the Instantiate into photonInstantiate...
public class Spawner : Unit
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartSpawningUnit(0);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartSpawningUnit(1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartSpawningUnit(2);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartSpawningUnit(3);
        }
    }
}
