using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//show hp
//show unit datasheet,icon etc
//be able to group units under a number
//change take damage, die etc to be events
public class Unit : MonoBehaviour
{
    Player myPlayer;
    int playerNumber;
    public bool isSelected;

    public UnitDetails unitDetails;

    void Start()
    {
        //get playerNumber
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
