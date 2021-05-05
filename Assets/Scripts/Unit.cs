using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//show unit datasheet,icon etc
//be able to group units under a number
//change take damage, die etc to be events
//set unitUI height as a paramter on each unit.
//should set the active camera on script
//after taking damage set healthBar as not active after a time threshold
//taking damage should cause particle effect of damage
//add weapons automatically
//have a parameter for a unit for it's main cur action, such as - walking, attacking, building etc. to know its behaivior
//fix auto player pickup
public class Unit : Purchasables, System.IComparable
{
    public Player myPlayer;
    public int myPlayerNumber;
    public bool isSelected;

    [SerializeField]
    public int curHP;

    public UnitDetails unitDetails;
    public List<Weapon> unitWeapons = new List<Weapon>();

    public HealthBar healthBar;
    public static readonly int HEALTH_BAR_LIMITED_TIME_DURATION = 3;

    public PhotonView photonView;

    void Start()
    {
        //photonView.RPC("InitUnit", RpcTarget.All);
        InitUnit();

    }

    [PunRPC]
    public void InitUnit()
    {
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

        gameObject.GetComponentInChildren<Renderer>().material.SetColor("_Color", GameManager.Instance.basicColors1[myPlayerNumber]);

        AddWeapons();

        //SetHealthBarActive(false);
    }

    public void OnMyPlayerJoined()
    {
        myPlayer = GameManager.Instance.playersHolder.getPlayer(myPlayerNumber);

        /*if (!myPlayer.playerUnits.Contains(this))
        {
            myPlayer.playerUnits.Add(this);
        }*/

        AddWeapons();

        gameObject.GetComponentInChildren<Renderer>().material.SetColor("_Color",GameManager.Instance.basicColors1[myPlayerNumber]);

        Debug.Log(gameObject.name + " is ready!");
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

    void AddWeapons()
    {
        Weapon[] tmpWeapons = gameObject.GetComponentsInChildren<Weapon>();
        for(int i = 0; i < tmpWeapons.Length; i++)
        {
            if (!unitWeapons.Contains(tmpWeapons[i]))
            {
                unitWeapons.Add(tmpWeapons[i]);
            }
        }
    }

    void Die()
    {
        print("I am dead :(");
        Destroy(gameObject);
    }

    public int CompareTo(object obj)
    {
        Unit other = obj as Unit;
        return this.name.CompareTo(other.name);
    }
}
