using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserWeapon : MonoBehaviour
{

    public GameObject laserPrefab;
    public float laserWidth;
    public Color laserColor;

    public void Start()
    {
        //GetComponent<LineRenderer>().gameObject.SetActive(false);
        GetComponent<LineRenderer>().SetPosition(0, this.transform.position);
        GetComponent<LineRenderer>().SetPosition(1, this.transform.position);
        GetComponent<LineRenderer>().startWidth = laserWidth;
        GetComponent<LineRenderer>().endWidth = laserWidth;
        GetComponent<LineRenderer>().startColor = GameManager.Instance.basicColors1[0];
        GetComponent<LineRenderer>().endColor = GameManager.Instance.basicColors1[0];
    }

    public void FireLaser(Vector3 target)
    {
        //laserPrefab.GetComponent<LineRenderer>().gameObject.SetActive(true);
        GetComponent<LineRenderer>().SetPosition(0, this.transform.position);
        GetComponent<LineRenderer>().SetPosition(1, target);
    }

    public void StopFiringLaser()
    {
        //GetComponent<LineRenderer>().gameObject.SetActive(false);
        GetComponent<LineRenderer>().SetPosition(1, this.transform.position);
        GetComponent<LineRenderer>().SetPosition(0, this.transform.position);
    }
}
