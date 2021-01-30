using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//show hp
//show unit datasheet,icon etc
//be able to group units under a number
//change take damage, die etc to be events
//set unitUI height as a paramter on each unit.
//should set the active camera on script
public class Unit : MonoBehaviour
{
    Player myPlayer;
    int playerNumber;
    public bool isSelected;

    public UnitDetails unitDetails;

    public HealthBar healthBar;

    void Start()
    {
        //get playerNumber
        healthBar.SetMaxHealth(unitDetails.max_hp);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void TakeDamage(int damage)
    {
        if (damage >= 0)
        {
            unitDetails.cur_hp -= damage;
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
