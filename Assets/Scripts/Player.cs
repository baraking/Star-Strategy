using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//fix IsUnitSelectable(Unit other)
public class Player : MonoBehaviour
{
    public int playerNumber;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsUnitSelectable(Unit other)
    {
        return (other.playerNumber==playerNumber);
    }
}
