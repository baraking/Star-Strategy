using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//be able to drag box for a multi selection
//be able to select only units matching your player's number
//give a world position a fast travel number
public class mouseController : MonoBehaviour
{
    public Player myPlayer;

    public Camera playerCamera;
    public Vector3 curPosition;

    public GameObject signalObject;

    public List<Unit> selectedUnits = new List<Unit>();

    void Start()
    {
        myPlayer = gameObject.GetComponent<Player>();
        playerCamera = myPlayer.playerCamera;
    }

    void Update()
    {
        if (myPlayer.photonView.IsMine)
        {
            RaycastHit hit;
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))//could add collider
            {
                if (Input.GetKeyDown(PlayerButtons.LEFT_CLICK))
                {
                    if (!Input.GetKey(PlayerButtons.MULTI_SELECTION))
                    {
                        foreach (Unit unit in selectedUnits)
                        {
                            unit.isSelected = false;
                            unit.SetHealthBarActive(false);
                        }
                        selectedUnits.Clear();
                    }

                    Transform objectHit = hit.transform;
                    //Debug.Log(objectHit.name);
                    if (objectHit.GetComponentInParent<Unit>())
                    {
                        if (myPlayer.IsUnitSelectable(objectHit.GetComponentInParent<Unit>()))
                        {
                            selectedUnits.Add(objectHit.GetComponentInParent<Unit>());
                            objectHit.GetComponentInParent<Unit>().isSelected = true;
                            objectHit.GetComponentInParent<Unit>().SetHealthBarActive(true);
                        }
                    }
                }

                //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
                //print(hit.point);
                curPosition = hit.point;
            }

            ///*
            if (Input.GetKeyDown(PlayerButtons.RIGHT_CLICK))
            {
                Transform objectHit = hit.transform;
                if (objectHit.GetComponentInParent<Unit>() && selectedUnits.Count > 0)
                {
                    if (objectHit.GetComponentInParent<Unit>().myPlayerNumber != myPlayer.playerNumber)
                    {
                        foreach (Unit unit in selectedUnits)
                        {
                            //Debug.Log(unit.name + " is firing on " + objectHit.GetComponentInParent<Unit>().name);
                            unit.Fire(objectHit.GetComponentInParent<Unit>());
                        }
                    }
                }
                else if (objectHit.GetComponentInParent<Resource>() && selectedUnits.Count > 0)
                {
                    foreach (Walkable unit in selectedUnits)
                    {
                        unit.GetComponent<Walkable>().hasTarget = true;
                        unit.GetComponent<Walkable>().targetPoint = new Vector3(hit.point.x, unit.transform.position.y, hit.point.z);
                        if (unit.GetComponent<Gatherer>())
                        {
                            unit.GetComponent<Gatherer>().targetResource = objectHit.GetComponentInParent<Resource>();
                        }
                    }
                }
                else if (objectHit.GetComponentInParent<ResourceSilo>() && selectedUnits.Count > 0)
                {
                    if (objectHit.GetComponentInParent<Unit>().myPlayerNumber != myPlayer.playerNumber)
                    {
                        foreach (Walkable unit in selectedUnits)
                        {
                            unit.GetComponent<Walkable>().hasTarget = true;
                            unit.GetComponent<Walkable>().targetPoint = new Vector3(hit.point.x, unit.transform.position.y, hit.point.z);
                            if (unit.GetComponent<Gatherer>())
                            {
                                unit.GetComponent<Gatherer>().targetResourceSilo = objectHit.GetComponentInParent<ResourceSilo>();
                            }
                        }
                    }

                }

                //print("clicked!");
                //Instantiate(signalObject, hit.point, new Quaternion());

                //if target point is walkable
                foreach (Unit unit in selectedUnits)
                {
                    if (unit.GetComponent<Walkable>())
                    {
                        unit.GetComponent<Walkable>().hasTarget = true;
                        unit.GetComponent<Walkable>().targetPoint = new Vector3(hit.point.x, unit.transform.position.y, hit.point.z);
                    }
                }
            }
            //*/
        }
    }

}
