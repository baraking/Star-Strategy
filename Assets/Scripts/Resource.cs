using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{

    public int curValue;

    public int GiveResources(int amountTaken)
    {
        int tmpValue = curValue;
        if (amountTaken >= 0)
        {
            curValue -= amountTaken;
        }
        if (curValue < 0)
        {
            OnDepleted();
            return tmpValue;
        }
        return amountTaken;
    }

    public void OnDepleted()
    {
        if (curValue <= 0)
        {
            print("Resource depleted :(");
            Destroy(gameObject);
        }
    }
}
