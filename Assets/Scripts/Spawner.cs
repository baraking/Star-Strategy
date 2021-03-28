using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//shouold add spawn point
//fix the whole spawning command. wrote a basic one for testing only
//should change the Instantiate into photonInstantiate...
public class Spawner : Unit
{
    [SerializeField] public GameObject[] availableUnits;
    public bool isBuilding;

    void Start()
    {
        base.InitUnit();
        isBuilding = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartSpawningUnit(0);
        }
    }

    public void StartSpawningUnit(int unitIndex)
    {
        if (isSelected && !isBuilding)
        {
            StartCoroutine(SpawnUnit(unitIndex));
        }
    }

    public IEnumerator SpawnUnit(int unitIndex)
    {
        isBuilding = true;
        Debug.Log("Started building a " + availableUnits[unitIndex].GetComponent<Unit>().unitDetails.name);
        yield return new WaitForSeconds(availableUnits[unitIndex].GetComponent<Unit>().unitDetails.buildTime);
        GameObject newUnit = Instantiate(availableUnits[unitIndex]);
        newUnit.GetComponent<Unit>().myPlayerNumber = myPlayerNumber;
        newUnit.GetComponent<Unit>().myPlayer = myPlayer;
        newUnit.GetComponent<Unit>().InitUnit();
        newUnit.transform.SetParent(GameManager.Instance.Units.transform);

        newUnit.GetComponent<Unit>().healthBar = newUnit.GetComponentInChildren<HealthBar>();
        Debug.Log("Finished building a " + availableUnits[unitIndex].GetComponent<Unit>().unitDetails.name);
        isBuilding = false;
    }
}
