using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//new to remove the new from the functions and design better
public class GroupedUnits : Walkable
{

    public List<Unit> groupedUnits = new List<Unit>();
    public List<UnitDetails.UnitType> allowedUnitTypes;
    public int numberOfUnitsAllowed;

    void Start()
    {
        base.InitUnit();
    }

    void Update()
    {
        AttempotToWalk();
    }

    public new void AttempotToWalk()
    {
        foreach(Walkable walkable in groupedUnits)
        {
            walkable.AttempotToWalk();
        }
    }

    public void attachUnit(Unit unit)
    {
        if(groupedUnits.Count<numberOfUnitsAllowed && allowedUnitTypes.Contains(unit.unitDetails.unitType) && myPlayerNumber == unit.myPlayerNumber && !groupedUnits.Contains(unit))
        {
            groupedUnits.Add(unit);
            unit.transform.SetParent(gameObject.transform);
        }
    }

    public void deattachUnit(Unit unit)
    {
        if (groupedUnits.Contains(unit))
        {
            groupedUnits.Remove(unit);
            unit.transform.SetParent(GameManager.Instance.Units.transform);
        }
    }

    public new void SetHealthBarActive(bool setTo)
    {
        foreach (Unit unit in groupedUnits)
        {
            unit.SetHealthBarActive(setTo);
        }
    }

    public new void SetIsSelected(bool newState)
    {
        print(gameObject + " attempting to set selection to " + newState);
        foreach (Unit unit in groupedUnits)
        {
            unit.SetIsSelected(newState);
        }
    }
}
