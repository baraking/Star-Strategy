using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//show hp
//show unit datasheet,icon etc
//be able to group units under a number
//change take damage, die etc to be events
//set unitUI height as a paramter on each unit.
//should set the active camera on script
//after taking damage set healthBar as not active after a time threshold
public class Unit : MonoBehaviour
{
    public Player myPlayer;
    public int playerNumber;
    public bool isSelected;

    public UnitDetails unitDetails;

    public HealthBar healthBar;

    void Start()
    {
        //get playerNumber
        healthBar.SetMaxHealth(unitDetails.max_hp);
        setHealthBarActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setHealthBarActive(bool setTo)
    {
        healthBar.gameObject.SetActive(setTo);
    }

    void TakeDamage(int damage)
    {
        if (damage >= 0)
        {
            unitDetails.cur_hp -= damage;
            healthBar.gameObject.SetActive(true);
            healthBar.setHealth(unitDetails.cur_hp);
        }
        if (unitDetails.cur_hp <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        print("I am dead :(");
    }
}
