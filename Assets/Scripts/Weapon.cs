using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//have a pointer to the parent unit
//fix isEligableToFire
//have another rangeCalculationPoint
//add shouldTurnToFire option 
//finish the function Fire(Vector3 targetPosition)
//fix targetUnit.transform.position into actuall targeting a point on the enemy
//fix the update function!!!!!
//fix print("start moving!"); section, the vector could be opposite
//should replace 1f with weapon rotation parameter
//update rotate weapon should be more efficent
//should fire only after weapon turned on target
public class Weapon : Purchasables
{
    public WeaponDetails weaponDetails;
    public bool isInCooldown;
    public Vector3 rangeCalculationPoint;
    public Unit weaponParent;

    public Unit targetUnit;

    // Start is called before the first frame update
    void Start()
    {
        isInCooldown = false;
        //rangeCalculationPoint = transform.position;
        weaponParent = gameObject.GetComponentInParent<Unit>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targetUnit != null)
        {
            Fire(targetUnit);
            Vector3 targetDirection = targetUnit.transform.position - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 1f * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
        else
        {
            Vector3 targetDirection = gameObject.GetComponentInParent<Unit>().transform.forward;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 1f * Time.deltaTime, 0.0f);
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
    }

    public void Fire(Vector3 targetPosition)
    {
        if (!isInCooldown && Vector3.Distance(rangeCalculationPoint, targetPosition) <= weaponDetails.range)
        {

        }
    }

    public void Fire(Unit targetUnit)
    {
        this.targetUnit = targetUnit;
        //if (!isInCooldown && Vector3.Distance(rangeCalculationPoint, targetUnit.transform.position) <= weaponDetails.range)
        float distanceToTarget = Vector3.Distance(transform.position, targetUnit.transform.position);
        if (distanceToTarget <= weaponDetails.range)
        {
            if (!isInCooldown)
            {
                print(weaponDetails.name + " is Firing!");
                targetUnit.TakeDamage(weaponDetails.damage);
                isInCooldown = true;
                StartCoroutine(AfterFire());
            }
        }
        else
        {
            if (weaponParent.GetComponent<Walkable>())
            {
                if (weaponParent.GetComponent<Walkable>().targetPoint == Vector3.zero)
                {
                    print("start moving!");
                    weaponParent.GetComponent<Walkable>().targetPoint = transform.position - (targetUnit.transform.position.normalized * distanceToTarget);
                }
            }
        }
    }

    public IEnumerator AfterFire()
    {
        if (GetComponent<LaserWeapon>())
        {
            GetComponent<LaserWeapon>().FireLaser(targetUnit.transform.position);
        }
        yield return new WaitForSeconds(weaponDetails.fireRate);
        if (GetComponent<LaserWeapon>())
        {
            GetComponent<LaserWeapon>().StopFiringLaser();
        }
        isInCooldown = false;
    }

    public bool IsEligableToFire(Unit other)
    {
        return true;
    }
}
