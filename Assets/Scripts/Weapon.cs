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
    public int curMagazineAmmo;
    public bool isReloading;

    public Unit targetUnit;
    public List<Unit> enemiesAtRange;
    public SphereCollider sphereCollider;

    public delegate void WeaponAction(Weapon actingWeapon, GameObject target);
    [SerializeField]
    public WeaponAction weaponAction;

    // Start is called before the first frame update
    void Start()
    {
        purchasableDetails = weaponDetails;
        isInCooldown = false;
        //rangeCalculationPoint = transform.position;
        weaponParent = gameObject.GetComponentInParent<Unit>();
        curMagazineAmmo = weaponDetails.magazineSize;
        isReloading = false;
        enemiesAtRange = new List<Unit>();

        if (sphereCollider == null || !GetComponent<SphereCollider>())
        {
            sphereCollider = gameObject.AddComponent<SphereCollider>();
            sphereCollider.isTrigger = true;
            sphereCollider.radius = weaponDetails.range;
        }

        weaponAction = WeaponActions.Idle;
    }

    private void OnTriggerEnter(Collider other)
    {
        print(other);
        if (other.GetComponent<Unit>().myPlayerNumber != weaponParent.myPlayerNumber && !enemiesAtRange.Contains(other.GetComponent<Unit>()))
        {
            enemiesAtRange.Add(other.GetComponent<Unit>());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Unit>().myPlayerNumber != weaponParent.myPlayerNumber && enemiesAtRange.Contains(other.GetComponent<Unit>()))
        {
            enemiesAtRange.Remove(other.GetComponent<Unit>());
        }
    }

    public List<Purchasables> GetPurchasables()
    {
        return weaponDetails.purchasables;
    }

    public override Sprite GetIcon()
    {
        return weaponDetails.icon;
    }

    public override int[] GetPrerequisites()
    {
        return weaponDetails.prerequisites;
    }

    public override int[] GetRequirements()
    {
        return weaponDetails.requirements;
    }

    // Update is called once per frame
    void Update()
    {
        if (targetUnit != null)
        {
            Fire(targetUnit);
            Vector3 targetDirection = targetUnit.transform.position - transform.position;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 1f * Time.deltaTime, 0.0f);
            if (!weaponDetails.shouldTurnToFire)
            {
                transform.rotation = Quaternion.LookRotation(newDirection);
            }
            else
            {
                weaponParent.transform.rotation = Quaternion.LookRotation(newDirection);
            }
        }
        else
        {
            Vector3 targetDirection = gameObject.GetComponentInParent<Unit>().transform.forward;
            Vector3 newDirection = Vector3.RotateTowards(transform.forward, targetDirection, 1f * Time.deltaTime, 0.0f);
            if (!weaponDetails.shouldTurnToFire)
            {
                transform.rotation = Quaternion.LookRotation(newDirection);
            }
            else
            {
                weaponParent.transform.rotation = Quaternion.LookRotation(newDirection);
            }
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
                /*if (weaponParent.GetComponent<Walkable>().GetTargetPoint() == Vector3.zero)
                {
                    print("start moving!");
                    weaponParent.GetComponent<Walkable>().SetTargetPoint(transform.position - (targetUnit.transform.position.normalized * distanceToTarget));
                }*/
            }
        }
    }

    public IEnumerator Reload()
    {
        isReloading = true;
        yield return new WaitForSeconds(weaponDetails.timeToShoot + weaponDetails.timeToReload);
        curMagazineAmmo = weaponDetails.magazineSize;
        isReloading = false;
    }

    public IEnumerator AfterFire()
    {
        if (isReloading)
        {
            yield return new WaitForSeconds(0);
        }
        curMagazineAmmo--;
        if (GetComponent<LaserWeapon>())
        {
            GetComponent<LaserWeapon>().FireLaser(targetUnit.transform.position);
        }
        if (GetComponent<ExplosiveWeapon>())
        {
            GetComponent<ExplosiveWeapon>().location = targetUnit.transform.position;
            GetComponent<ExplosiveWeapon>().Explode();
        }
        if (GetComponent<FlameWeapon>())
        {
            GetComponent<FlameWeapon>().Fire();
        }
        if (curMagazineAmmo > 0)
        {
            yield return new WaitForSeconds(weaponDetails.timeToShoot);
        }
        //reload
        else if (curMagazineAmmo == 0)
        {
            yield return new WaitForSeconds(weaponDetails.timeToShoot + weaponDetails.timeToReload);
            curMagazineAmmo = weaponDetails.magazineSize;
        }
        if (GetComponent<LaserWeapon>())
        {
            GetComponent<LaserWeapon>().StopFiringLaser();
        }
        if (GetComponent<FlameWeapon>())
        {
            GetComponent<FlameWeapon>().StopFiring();
        }
        isInCooldown = false;
    }

    public bool IsEligableToFire(Unit other)
    {
        return true;
    }
}
