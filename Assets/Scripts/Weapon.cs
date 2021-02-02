using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//have a pointer to the parent unit
//fix isEligableToFire
//have another rangeCalculationPoint
//add shouldTurnToFire option 
//finish the function Fire(Vector3 targetPosition)
//fix targetUnit.transform.position into actuall targeting a point on the enemy
public class Weapon : MonoBehaviour
{
    public WeaponDetails weaponDetails;
    public bool isInCooldown;
    public Vector3 rangeCalculationPoint;

    // Start is called before the first frame update
    void Start()
    {
        isInCooldown = false;
        //rangeCalculationPoint = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire(Vector3 targetPosition)
    {
        if (!isInCooldown && Vector3.Distance(rangeCalculationPoint, targetPosition) <= weaponDetails.range)
        {

        }
    }

    public void Fire(Unit targetUnit)
    {
        print(Vector3.Distance(rangeCalculationPoint, targetUnit.transform.position));
        print(weaponDetails.range);
        //if (!isInCooldown && Vector3.Distance(rangeCalculationPoint, targetUnit.transform.position) <= weaponDetails.range)
        if (!isInCooldown && Vector3.Distance(transform.position, targetUnit.transform.position) <= weaponDetails.range)
        {
            print(weaponDetails.name + "is Firing!");
            targetUnit.TakeDamage(weaponDetails.damage);
            isInCooldown = true;
            StartCoroutine(AfterFire());
        }
    }

    public IEnumerator AfterFire()
    {
        yield return new WaitForSeconds(weaponDetails.fireRate);
        isInCooldown = false;
    }

    public bool IsEligableToFire(Unit other)
    {
        return true;
    }
}
