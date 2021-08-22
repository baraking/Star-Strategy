using Photon.Pun;
using System;
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
    public static readonly bool SET_TO_IS_COMPLETE=true;

    public UnitDetails unitDetails;

    public PlayerController myPlayer;
    public int myPlayerNumber;
    [SerializeField]
    private bool isSelected;
    public float buildTime;
    public bool isWaiting;

    [SerializeField]
    public float curHP;

    public List<Weapon> unitWeapons = new List<Weapon>();

    public bool isComplete;
    public float buildProgress;

    public HealthBar healthBar;
    public static readonly int HEALTH_BAR_LIMITED_TIME_DURATION = 3;
    public static readonly Vector3 DEFAULT_SPAWN_LOCATION = new Vector3(0, 0, 0.55f);
    public static float INITIAL_HP_FOR_BUILDINGS = 0.2f;

    public List<Unit> carriedUnits = new List<Unit>();
    public int carriedAmount;

    [SerializeField]
    public List<Purchasables> creationQueue = new List<Purchasables>();
    public float curCreationProgress;
    public float curCreatonTarget;

    public PhotonView photonView;
    public int photonID;

    public delegate void UnitAction(Unit actingUnit, GameObject target, Quaternion endQuaternion, List<Vector3> targetsLocation);
    [SerializeField]
    public UnitAction unitAction;

    public List<Vector3> targetsLocation;
    public Quaternion endQuaternion;
    public GameObject actionTarget;

    public SphereCollider sphereCollider;

    private void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView.InstantiationData != null)
        {
            myPlayerNumber = (int)photonView.InstantiationData[0];

            if (myPlayer == null)
            {
                foreach (PlayerController playerController in GameManager.Instance.playersHolder.allPlayers)
                {
                    if (playerController.playerNumber == myPlayerNumber)
                    {
                        myPlayer = playerController;
                    }
                }
            }

            //print(photonView.ViewID);
            photonID = photonView.ViewID;

            //print(photonView.InstantiationData.Length);
            if (photonView.InstantiationData.Length > 1)
            {
                isComplete = (bool)photonView.InstantiationData[1];
            }
        }
        this.transform.SetParent(GameManager.Instance.Units.transform);
    }

    void Start()
    {
        //photonView.RPC("InitUnit", RpcTarget.All);
        InitUnit();
    }

    private void Update()
    {
        if (isComplete)
        {
            if (isSelected)
            {
                //print(unitAction.Method);
                if (Input.GetKey(KeyCode.P))
                {
                    //Die();
                }
            }
            unitAction(this, actionTarget, endQuaternion, targetsLocation);
        }
    }

    public object[] SendCurrentAction()
    {
        object[] ans = new object[5];
        ans[0] = photonID;
        if (actionTarget != null)
        {
            if (actionTarget.GetComponent<Unit>())
            {
                ans[1] = actionTarget.GetComponent<Unit>().photonID;
            }
            else if (actionTarget.GetComponent<Resource>())
            {
                ans[1] = actionTarget.GetComponent<Resource>().photonID;
            }
            else
            {
                ans[1] = -1;
            }
        }
        else
        {
            ans[1] = -1;
        }
        ans[2] = UnitActions.GetNumberFromUnitAction(unitAction);

        ans[3] = new float[] { endQuaternion.x, endQuaternion.y, endQuaternion.z, endQuaternion.w };
        float[] locations = new float[targetsLocation.Count * 3];
        for (int i = 0; i < targetsLocation.Count; i += 3)
        {
            locations[i] = targetsLocation[i].x;
            locations[i + 1] = targetsLocation[i].y;
            locations[i + 2] = targetsLocation[i].z;
        }
        ans[4] = locations;
        return ans;
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
        if (unitDetails.unitType == UnitDetails.UnitType.Building)
        {
            healthBar.SetMaxConstruction(unitDetails.buildTime);
        }

        curHP = unitDetails.max_hp;
        healthBar.setHealth(curHP);

        /*if (gameObject.GetComponent<PhotonTransformView>() == null)
        {
            gameObject.AddComponent<PhotonTransformView>();
        }*/

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
        InitWeapons();

        isBuilding = false;
        //SetHealthBarActive(false);

        unitAction = UnitActions.Idle;
        isWaiting = false;

        if (sphereCollider == null || !GetComponent<SphereCollider>())
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = .01f;
        }

        GetComponent<Collider>().enabled = false;
        GetComponent<Collider>().enabled = true;

        if (!GetComponent<Rigidbody>())
        {
            gameObject.AddComponent<Rigidbody>();
            //GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    public void AttemptToSpawnUnit(int unitIndex)
    {
        if (isSelected)
        {
            actionTarget = unitDetails.purchasables[unitIndex].gameObject;

            if (!isBuilding)
            {
                //StartCoroutine(SpawnUnit(unitIndex));

                //print(unitDetails.unitType == UnitDetails.UnitType.Building);

                print(actionTarget);
                if (unitDetails.unitType == UnitDetails.UnitType.Building)
                {
                    //print("Will create " + unitDetails.purchasables[unitIndex].GetComponent<Unit>().unitDetails.name);
                    targetsLocation = new List<Vector3>() { transform.position + Unit.DEFAULT_SPAWN_LOCATION };
                    creationQueue.Add(actionTarget.GetComponent<Purchasables>());
                    unitAction = UnitActions.Spawn;
                }
                else
                {
                    //print("Will create " + unitDetails.purchasables[unitIndex].GetComponent<Unit>().unitDetails.name);
                    myPlayer.GetComponent<mouseController>().playerIsTryingToBuild = true;
                }

                //unitAction = UnitActions.Build;
            }
            else if (isBuilding)
            {
                creationQueue.Add(actionTarget.GetComponent<Purchasables>());
            }
        }
        myPlayer.DisplayPurchasableQueue(this);
    }

    public void RemoveFromCreationQueue(int i)
    {
        creationQueue.RemoveAt(i);
        myPlayer.DisplayPurchasableQueue(this);
    }

    public void StartSpawningUnit()
    {
        if (!isBuilding)
        {
            StartCoroutine(SpawnUnit(targetsLocation[0], actionTarget));
        }
    }

    public IEnumerator SpawnUnit(Vector3 location, GameObject purchasable)
    {
        isBuilding = true;
        buildTime = purchasable.GetComponent<Unit>().unitDetails.buildTime;
        Debug.Log("Started building a " + purchasable.GetComponent<Unit>().unitDetails.name);

        yield return new WaitForSeconds(purchasable.GetComponent<Unit>().unitDetails.buildTime);

        //GameObject newUnit = Instantiate(purchasable);
        object[] instantiationData = new object[] { myPlayerNumber, SET_TO_IS_COMPLETE };
        GameObject newUnit = PhotonNetwork.Instantiate(purchasable.name, location, Quaternion.identity, 0, instantiationData);

        newUnit.GetComponent<Unit>().myPlayerNumber = myPlayerNumber;
        newUnit.GetComponent<Unit>().myPlayer = myPlayer;
        //newUnit.transform.position = location + DEFAULT_SPAWN_LOCATION;
        newUnit.transform.position = location;
        newUnit.transform.SetParent(GameManager.Instance.Units.transform);
        newUnit.GetComponent<Unit>().healthBar = newUnit.GetComponentInChildren<HealthBar>();
        newUnit.GetComponent<Unit>().isComplete = true;

        actionTarget.GetComponent<Unit>().healthBar.DisableConstructionBar();
        newUnit.GetComponent<Unit>().InitUnit();
        //newUnit.GetComponent<Unit>().isComplete = false;

        //yield return new WaitForSeconds(purchasable.GetComponent<Unit>().unitDetails.buildTime);

        OnUnitSpawnEnd(purchasable);

        creationQueue.Remove(creationQueue[0]);
        myPlayer.DisplayPurchasableQueue(this);

        Debug.Log("Finished building a " + purchasable.GetComponent<Unit>().unitDetails.name);

        isBuilding = false;
        if (creationQueue.Count < 1)
        {
            unitAction = UnitActions.Idle;
        }
        else
        {
            actionTarget = creationQueue[0].gameObject;
            unitAction = UnitActions.Spawn;
        }
    }

    public void StartSpawningBuilding()
    {
        if (!isBuilding)
        {
            StartCoroutine(SpawnBuilding(targetsLocation[0], actionTarget));
        }
    }

    public IEnumerator SpawnBuilding(Vector3 location, GameObject purchasable)
    {
        isBuilding = true;
        buildTime = purchasable.GetComponent<Unit>().unitDetails.buildTime;
        Debug.Log("Started building a " + purchasable.GetComponent<Unit>().unitDetails.name);

        //yield return new WaitForSeconds(purchasable.GetComponent<Unit>().unitDetails.buildTime);

        //GameObject newUnit = Instantiate(purchasable);
        object[] instantiationData = new object[] { myPlayerNumber };
        GameObject newUnit = PhotonNetwork.Instantiate(purchasable.name, location, Quaternion.identity, 0, instantiationData);

        newUnit.GetComponent<Unit>().myPlayerNumber = myPlayerNumber;
        newUnit.GetComponent<Unit>().myPlayer = myPlayer;
        //newUnit.transform.position = location + DEFAULT_SPAWN_LOCATION;
        newUnit.transform.position = location;
        newUnit.transform.SetParent(GameManager.Instance.Units.transform);
        newUnit.GetComponent<Unit>().healthBar = newUnit.GetComponentInChildren<HealthBar>();
        newUnit.GetComponent<Unit>().InitUnit();
        newUnit.GetComponent<Unit>().isComplete = false;

        actionTarget = newUnit;

        //is this needed?
        //yield return new WaitForSeconds(purchasable.GetComponent<Unit>().unitDetails.buildTime);
        yield return new WaitForSeconds(0);

        //OnUnitSpawnEnd(purchasable);
        //newUnit.GetComponent<Unit>().isComplete = true;
        //Debug.Log("Finished building a " + purchasable.GetComponent<Unit>().unitDetails.name);
        isBuilding = false;

        newUnit.GetComponent<Unit>().curHP = newUnit.GetComponent<Unit>().unitDetails.max_hp * INITIAL_HP_FOR_BUILDINGS;
        newUnit.GetComponent<Unit>().healthBar.setHealth(newUnit.GetComponent<Unit>().unitDetails.max_hp * INITIAL_HP_FOR_BUILDINGS);

        //unitAction = UnitActions.Idle;
    }

    public void Build()
    {
        //print(actionTarget.GetComponent<Unit>().buildProgress);
        actionTarget.GetComponent<Unit>().buildProgress += Time.deltaTime;
        //print("Add Amount: " + ((Time.deltaTime * actionTarget.GetComponent<Unit>().unitDetails.max_hp) / actionTarget.GetComponent<Unit>().unitDetails.buildTime) +"/"+ actionTarget.GetComponent<Unit>().unitDetails.max_hp);
        if (actionTarget.GetComponent<Unit>().curHP < actionTarget.GetComponent<Unit>().unitDetails.max_hp)
        {
            actionTarget.GetComponent<Unit>().curHP += ((Time.deltaTime * actionTarget.GetComponent<Unit>().unitDetails.max_hp )/ actionTarget.GetComponent<Unit>().unitDetails.buildTime);
            actionTarget.GetComponent<Unit>().healthBar.setHealth(actionTarget.GetComponent<Unit>().curHP);
            actionTarget.GetComponent<Unit>().healthBar.SetConstruction(actionTarget.GetComponent<Unit>().buildProgress);
        }
        if(actionTarget.GetComponent<Unit>().curHP > actionTarget.GetComponent<Unit>().unitDetails.max_hp)
        {
            actionTarget.GetComponent<Unit>().curHP = actionTarget.GetComponent<Unit>().unitDetails.max_hp;
            actionTarget.GetComponent<Unit>().healthBar.setHealth(actionTarget.GetComponent<Unit>().curHP);
        }

        print(actionTarget.GetComponent<Unit>().unitDetails.name + ": " + actionTarget.GetComponent<Unit>().buildProgress + "/" + actionTarget.GetComponent<Unit>().unitDetails.buildTime);

        if(actionTarget.GetComponent<Unit>().buildProgress>= actionTarget.GetComponent<Unit>().unitDetails.buildTime)
        {
            //actionTarget.GetComponent<Unit>().buildProgress = actionTarget.GetComponent<Unit>().unitDetails.buildTime;
            OnUnitSpawnEnd(actionTarget);
            actionTarget.GetComponent<Unit>().isComplete = true;

            if (actionTarget.GetComponent<Unit>().unitDetails.unitType == UnitDetails.UnitType.Building)
            {
                actionTarget.GetComponent<Unit>().healthBar.DisableConstructionBar();
            }
            unitAction = UnitActions.Idle;
        }
    }

    public void OnUnitSpawnEnd(GameObject purchasable)
    {
        for(int i=0;i< purchasable.GetComponent<Unit>().unitDetails.requirements.Length; i++)
        {
            myPlayer.PlayerRaceData.landmarks[purchasable.GetComponent<Unit>().unitDetails.requirements[i]] = true;
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
        InitWeapons();

        gameObject.GetComponentInChildren<Renderer>().material.SetColor("_Color",GameManager.Instance.basicColors1[myPlayerNumber]);

        UpdateLandmarksOnSelfSpawn();

        isComplete = true;

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
        /*if (targetUnit.GetComponent<GroupedUnits>())
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
        }*/

        if (targetUnit.GetComponent<GroupedUnits>())
        {
            targetUnit = targetUnit.GetComponent<GroupedUnits>().groupedUnits[0];
        }
        foreach (Weapon weapon in unitWeapons)
        {
            if (weapon.IsEligableToFire(targetUnit))
            {
                print(unitDetails.name + " is ordered to Fire!");
                weapon.targetUnit = targetUnit;
                weapon.weaponAction = WeaponActions.Fire;
            }
        }
    }

    public void Fire(Unit targetUnit, Weapon weapon)
    {
        /*if (targetUnit.GetComponent<GroupedUnits>())
        {
            targetUnit = targetUnit.GetComponent<GroupedUnits>().groupedUnits[0];
        }
        if (weapon.IsEligableToFire(targetUnit))
        {
            print(unitDetails.name + " is ordered to Fire!");
            weapon.Fire(targetUnit);
        }*/

        if (targetUnit.GetComponent<GroupedUnits>())
        {
            targetUnit = targetUnit.GetComponent<GroupedUnits>().groupedUnits[0];
        }
        if (weapon.IsEligableToFire(targetUnit))
        {
            //print(unitDetails.name + " is ordered to Fire!");
            weapon.targetUnit = targetUnit;
            //print(unitDetails.name + " has a new Target!");
            weapon.weaponAction = WeaponActions.Fire;
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

    public void InitWeapons()
    {
        foreach (Weapon weapon in unitWeapons)
        {
            weapon.enabled = true;
            //print(gameObject.name);
            weapon.InitWeapon();
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
        //disembarkingUnit.GetComponent<Walkable>().SetHasTarget(false);
        //disembarkingUnit.GetComponent<Walkable>().SetTargetPoint(transform.position);
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

    [PunRPC]
    public void NotifyUnitDeath(int unit, int player)
    {
        Unit unitToDestroy = PhotonView.Find(unit).GetComponent<Unit>();
        /*if (unitToDestroy.GetIsSelected())
        {
            unitToDestroy.SetIsSelected(false);
        }*/

        foreach (PlayerController playerController in GameManager.Instance.playersHolder.allPlayers)
        {
            if (playerController.playerNumber == player)
            {
                if (playerController.GetComponent<mouseController>().selectedUnits.Contains(unitToDestroy))
                {
                    playerController.GetComponent<mouseController>().selectedUnits.Remove(unitToDestroy);
                }
                playerController.playerUnits.Remove(unitToDestroy);
                playerController.CheckForDefeat(myPlayer);
                PhotonNetwork.Destroy(gameObject);
            }
        }
    }

    void Die()
    {
        //print("I am dead :(");w
        //myPlayer.playerUnits.Remove(this);

        photonView.RPC("NotifyUnitDeath", RpcTarget.All, photonID,myPlayer.playerNumber);

        //myPlayer.CheckForDefeat(myPlayer);
        //UpdateLandmarksOnSelfDeath();
        //PhotonNetwork.Destroy(gameObject);
    }

    public void Wait(UnitAction previousAction, float time)
    {
        if (!isWaiting)
        {
            StartCoroutine(UnitWaitOnIdle(previousAction, time));
        }
    }

    public IEnumerator UnitWaitOnIdle(UnitAction previousAction, float time)
    {
        isWaiting = true;
        unitAction = UnitActions.Idle;
        yield return new WaitForSeconds(time);
        isWaiting = false;
        unitAction = previousAction;
    }

    public int CompareTo(object obj)
    {
        Unit other = obj as Unit;
        return this.name.CompareTo(other.name);
    }
}
