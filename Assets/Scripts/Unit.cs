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
public class Unit : MonoBehaviour
{
    public Player myPlayer;
    public int playerNumber;
    public bool isSelected;

    public int curHP;

    public UnitDetails unitDetails;
    public List<Weapon> unitWeapons = new List<Weapon>();

    public HealthBar healthBar;

    //get my playerNumber
    void Start()
    {
        healthBar.SetMaxHealth(unitDetails.max_hp);
        curHP = unitDetails.max_hp;
        SetHealthBarActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetHealthBarActive(bool setTo)
    {
        healthBar.gameObject.SetActive(setTo);
    }

    public void TakeDamage(int damage)
    {
        if (damage >= 0)
        {
            curHP -= damage;
            SetHealthBarActive(true);
            healthBar.setHealth(curHP);
        }
        if (curHP <= 0)
        {
            Die();
        }
    }

    public void Fire(Unit targetUnit)
    {
        foreach (Weapon weapon in unitWeapons)
        {
            if (weapon.IsEligableToFire(targetUnit))
            {
                print(unitDetails.name + " is Firing!");
                weapon.Fire(targetUnit);
            }
        }
    }

    void Die()
    {
        print("I am dead :(");
    }
}
