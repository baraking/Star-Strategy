using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//show unit datasheet,icon etc
//be able to group units under a number
//change take damage, die etc to be events
//set unitUI height as a paramter on each unit.
//taking damage should cause particle effect of damage
//have a parameter for a unit for it's main cur action, such as - walking, attacking, building etc. to know its behaivior
//have a delegateprobably for unit's action, such as embark, move, attack, harvest...
//fix auto player pickup
public class Unit : Purchasables, System.IComparable
{
    public UnitDetails unitDetails;

    public Player myPlayer;
    public int myPlayerNumber;
    [SerializeField]
    private bool isSelected;
    public float buildTime;

    [SerializeField]
    public int curHP;

    public List<Weapon> unitWeapons = new List<Weapon>();

    public HealthBar healthBar;
    public static readonly int HEALTH_BAR_LIMITED_TIME_DURATION = 3;
    public static readonly Vector3 DEFAULT_SPAWN_LOCATION = new Vector3(0, 0, 0.55f);

    public List<Unit> carriedUnits = new List<Unit>();
    public int carriedAmount;

    public PhotonView photonView;

    public delegate void UnitAction(Unit actingUnit, List<Vector3> targetsLocation, Quaternion endQuaternion);
    [SerializeField]
    public UnitAction unitAction;

    public List<Vector3> targetsLocation;
    public Quaternion endQuaternion;

    void Start()
    {
        //photonView.RPC("InitUnit", RpcTarget.All);
        InitUnit();

    }

    public void SetIsSelected(bool newState)
    {
        isSelected = newState;
    }

    public bool GetIsSelected()
    {
        return isSelected;
    }

    public List<Purchasables> GetPurchasables()
    {
        return unitDetails.purchasables;
    }

    public override int[] GetPrerequisites()
    {
        return unitDetails.prerequisites;
    }

    public override int[] GetRequirements()
    {
        return unitDetails.requirements;
    }

    public override Sprite GetIcon()
    {
        return unitDetails.icon;
    }

    [PunRPC]
    public void InitUnit()
    {
        purchasableDetails = unitDetails;
        if (healthBar == null)
        {
            healthBar = gameObject.GetComponentInChildren<HealthBar>();
        }
        SetHealthBarActive(true);
        healthBar.SetMaxHealth(unitDetails.max_hp);
        curHP = unitDetails.max_hp;

        if (gameObject.GetComponent<PhotonTransformView>() == null)
        {
            gameObject.AddComponent<PhotonTransformView>();
        }

        if (myPlayer != null)
        {
            myPlayerNumber = myPlayer.playerNumber;
            myPlayer.playerUnits.Add(this);
            myPlayer.SortUnits();
        }

        //gameObject.GetComponentInChildren<Renderer>().material.SetColor("_Color", GameManager.Instance.basicColors1[myPlayerNumber]);
        foreach(Renderer curRenderer in gameObject.GetComponentsInChildren<Renderer>())
        {
            curRenderer.material.SetColor("_Color", GameManager.Instance.basicColors1[myPlayerNumber]);
        }
        
        AddWeapons();

        isBuilding = false;
        //SetHealthBarActive(false);

        unitAction = UnitActions.Idle;
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
        buildTime = unitDetails.purchasables[unitIndex].GetComponent<Unit>().unitDetails.buildTime;
        Debug.Log("Started building a " + unitDetails.purchasables[unitIndex].GetComponent<Unit>().unitDetails.name);
        yield return new WaitForSeconds(unitDetails.purchasables[unitIndex].GetComponent<Unit>().unitDetails.buildTime);
        GameObject newUnit = Instantiate(unitDetails.purchasables[unitIndex].gameObject);
        newUnit.GetComponent<Unit>().myPlayerNumber = myPlayerNumber;
        newUnit.GetComponent<Unit>().myPlayer = myPlayer;
        newUnit.transform.position = transform.position + DEFAULT_SPAWN_LOCATION;
        newUnit.GetComponent<Unit>().InitUnit();
        newUnit.transform.SetParent(GameManager.Instance.Units.transform);

        newUnit.GetComponent<Unit>().healthBar = newUnit.GetComponentInChildren<HealthBar>();
        OnUnitSpawnEnd(unitIndex);
        Debug.Log("Finished building a " + unitDetails.purchasables[unitIndex].GetComponent<Unit>().unitDetails.name);

        isBuilding = false;
    }

    public void OnUnitSpawnEnd(int unitIndex)
    {
        for(int i=0;i< unitDetails.purchasables[unitIndex].GetComponent<Unit>().unitDetails.requirements.Length; i++)
        {
            myPlayer.PlayerRaceData.landmarks[unitDetails.purchasables[unitIndex].GetComponent<Unit>().unitDetails.requirements[i]] = true;
        }
        UpdateSelectedUnitsGUI();
    }

    public void UpdateSelectedUnitsGUI()
    {
        if (myPlayer.GetComponent<mouseController>().isUnitUIDisplaying)
        {
            myPlayer.GetComponent<mouseController>().ResetDisplayedUnitPurchasableUnits();
            myPlayer.GetComponent<mouseController>().DisplayUnitPurchasables(myPlayer.GetComponent<mouseController>().selectedUnits[0]);
        }
    }

    public void UpdateLandmarksOnSelfSpawn()
    {
        //if another exists
        /*foreach (Unit unit in myPlayer.playerUnits)
        {
            if (unit.name == this.name && unit != this)
            {
                return;
            }
        }*/
        for (int i = 0; i < unitDetails.requirements.Length; i++)
        {
            myPlayer.PlayerRaceData.landmarks[GetRequirements()[i]] = true;
        }
        //UpdateSelectedUnitsGUI();
    }

    public void UpdateLandmarksOnSelfDeath()
    {
        //if another exists
        if (myPlayer != null)
        {
            foreach (Unit unit in myPlayer.playerUnits)
            {
                if (unit.name == this.name && unit != this)
                {
                    print(unit.name);
                    return;
                }
            }
            for (int i = 0; i < unitDetails.requirements.Length; i++)
            {
                myPlayer.PlayerRaceData.landmarks[GetPrerequisites()[i]] = false;
            }
            UpdateSelectedUnitsGUI();
        }
    }

    public void OnMyPlayerJoined()
    {
        myPlayer = GameManager.Instance.playersHolder.getPlayer(myPlayerNumber);

        /*if (!myPlayer.playerUnits.Contains(this))
        {
            myPlayer.playerUnits.Add(this);
        }*/

        if (GetComponent<GroupedUnits>())
        {
            return;
        }
        AddWeapons();

        gameObject.GetComponentInChildren<Renderer>().material.SetColor("_Color",GameManager.Instance.basicColors1[myPlayerNumber]);

        UpdateLandmarksOnSelfSpawn();

        //Debug.Log(gameObject.name + " is ready!");
    }

    public void SetHealthBarActive(bool setTo)
    {
        healthBar.gameObject.SetActive(setTo);
    }

    public void DisplayeHealthForLimitedTime()
    {
        StartCoroutine(SetHealthBarActiveForLimitedTime());
    }

    public IEnumerator SetHealthBarActiveForLimitedTime()
    {
        SetHealthBarActive(true);
        yield return new WaitForSeconds(HEALTH_BAR_LIMITED_TIME_DURATION);
        SetHealthBarActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (damage >= 0)
        {
            curHP -= damage;
            DisplayeHealthForLimitedTime();
         healthBar.setHealth(curHP);
        }
        if (curHP <= 0)
        {
            Die();
        }
    }

    [PunRPC]
    public void Fire(Unit targetUnit)
    {
        if (targetUnit.GetComponent<GroupedUnits>())
        {
            targetUnit = targetUnit.GetComponent<GroupedUnits>().groupedUnits[0];
        }
        foreach (Weapon weapon in unitWeapons)
        {
            if (weapon.IsEligableToFire(targetUnit))
            {
                print(unitDetails.name + " is ordered to Fire!");
                weapon.Fire(targetUnit);
            }
        }
    }

    public float GetShortestRangeOfWeapons()
    {
        float shortestRange = float.MaxValue;
        foreach (Weapon weapon in unitWeapons)
        {
            //if weapon is eligable for firing on target
            if (weapon.weaponDetails.range< shortestRange)
            {
                shortestRange = weapon.weaponDetails.range;
            }
        }
        return shortestRange;
    }

    public void AddWeapons()
    {
        Weapon[] tmpWeapons = gameObject.GetComponentsInChildren<Weapon>();
        for(int i = 0; i < tmpWeapons.Length; i++)
        {
            if (!unitWeapons.Contains(tmpWeapons[i]))
            {
                unitWeapons.Add(tmpWeapons[i]);
            }
        }
        foreach(WeaponHolder weaponHolder in GetComponentsInChildren<WeaponHolder>())
        {
            //print(gameObject.name);
            weaponHolder.UpdateIfHasAWeapon();
        }
    }

    public void Embark(Unit embarkingUnit)
    {
        if (embarkingUnit.GetComponent<GroupedUnits>())
        {
            foreach(Unit unit in embarkingUnit.GetComponent<GroupedUnits>().groupedUnits)
            {
                if (carriedUnits.Contains(unit))
                {
                    return;
                }
            }
            if (Vector3.Distance(transform.position, embarkingUnit.GetComponent<GroupedUnits>().GetAveragePostition()) < 0.2f)
            {
                foreach (Unit unit in embarkingUnit.GetComponent<GroupedUnits>().groupedUnits)
                {
                    EmbarkSingleUnit(embarkingUnit);
                }
            }
        }
        else
        {
            if (carriedUnits.Contains(embarkingUnit))
            {
                return;
            }
            if (Vector3.Distance(transform.position, embarkingUnit.transform.position) < 0.2f)
            {
                EmbarkSingleUnit(embarkingUnit);
            }
        }
    }

    private void EmbarkSingleUnit(Unit embarkingUnit)
    {
        print(embarkingUnit + " is embarking " + gameObject.name);
        carriedUnits.Add(embarkingUnit);
        carriedAmount += embarkingUnit.unitDetails.unitSize;
        embarkingUnit.transform.SetParent(gameObject.transform);
        embarkingUnit.gameObject.SetActive(false);
    }

    public void Disembark(Unit disembarkingUnit)
    {
        if (disembarkingUnit.GetComponent<GroupedUnits>())
        {
            foreach (Unit unit in disembarkingUnit.GetComponent<GroupedUnits>().groupedUnits)
            {
                DisembarkGroupedUnit(disembarkingUnit);
            }
        }
        else
        {
            DisembarkSingleUnit(disembarkingUnit);
        }
    }

    public void DisembarkSingleUnit(Unit disembarkingUnit)
    {
        print(disembarkingUnit + " is disembarking " + gameObject.name);
        carriedUnits.Remove(disembarkingUnit);
        carriedAmount -= disembarkingUnit.unitDetails.unitSize;
        disembarkingUnit.transform.SetParent(GameManager.Instance.Units.transform);
        disembarkingUnit.GetComponent<Walkable>().SetHasTarget(false);
        disembarkingUnit.GetComponent<Walkable>().SetTargetPoint(transform.position);
        disembarkingUnit.gameObject.SetActive(true);
    }

    public void DisembarkGroupedUnit(Unit disembarkingUnit)
    {
        disembarkingUnit.gameObject.SetActive(true);
        print(disembarkingUnit + " is disembarking " + gameObject.name);
        carriedUnits.Remove(disembarkingUnit);
        carriedAmount -= disembarkingUnit.unitDetails.unitSize;
        disembarkingUnit.transform.SetParent(GameManager.Instance.Units.transform);
        disembarkingUnit.GetComponent<GroupedUnits>().SetHasTarget(false);
        disembarkingUnit.GetComponent<GroupedUnits>().SetTargetPoint(transform.position);
    }

    void Die()
    {
        print("I am dead :(");
        UpdateLandmarksOnSelfDeath();
        Destroy(gameObject);
    }

    public int CompareTo(object obj)
    {
        Unit other = obj as Unit;
        return this.name.CompareTo(other.name);
    }
}
