using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//new to remove the new from the functions and design better
//movement is based on all infantry being the same
public class GroupedUnits : Walkable
{

    public List<Unit> groupedUnits = new List<Unit>();
    public List<UnitDetails.UnitType> allowedUnitTypes;
    public int groupUnitSize;
    public int numberOfUnitsAllowed;

    void Start()
    {
        base.InitUnit();
        InitGroupSize();
    }

    void Update()
    {
        AttempotToWalk();
        //transform.rotation = Quaternion.LookRotation(newDirection);
        //transform.position = GetAveragePostition();
    }

    public void InitGroupSize()
    {
        foreach (Unit unit in groupedUnits)
        {
            groupUnitSize += unit.unitDetails.unitSize;
        }
    }

    public Vector3 GetAveragePostition()
    {
        Vector3 ans = new Vector3();
        foreach(Unit unit in groupedUnits)
        {
            ans += unit.transform.position;
        }
        ans /= groupedUnits.Count;
        return ans;
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
            groupUnitSize += unit.unitDetails.unitSize;
        }
    }

    public void deattachUnit(Unit unit)
    {
        if (groupedUnits.Contains(unit))
        {
            groupedUnits.Remove(unit);
            unit.transform.SetParent(GameManager.Instance.Units.transform);
            groupUnitSize -= unit.unitDetails.unitSize;
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

    public new void Fire(Unit targetUnit)
    {
        foreach (Unit unit in groupedUnits)
        {
            unit.Fire(targetUnit);
        }
    }

    public new void SetHasTarget(bool newState)
    {
        foreach (Walkable walkable in groupedUnits)
        {
            walkable.SetHasTarget(newState);
        }
    }

    public new void SetTargetPoint(Vector3 newTargetPoint)
    {
        Vector3[] formation = GroupMovement.ArcDefensiveFormation(groupedUnits, newTargetPoint, GameManager.Instance.playersHolder.allPlayers[gameObject.GetComponentInParent<Unit>().myPlayer.playerNumber].playerCamera.transform.right, .1f);
        int i = 0;
        foreach (Walkable walkable in groupedUnits)
        {
            walkable.GetComponent<Walkable>().SetTargetPoint(new Vector3(formation[i].x, walkable.transform.position.y, formation[i].z));
            i++;
        }
    }
}
