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
            if (objectHit.GetComponent<Unit>() && selectedUnits.Count>0)
            {
                if (objectHit.GetComponent<Unit>().myPlayerNumber != myPlayer.playerNumber)
                {
                    foreach (Unit unit in selectedUnits)
                    {
                        unit.Fire(objectHit.GetComponent<Unit>());
                    }
                }
            }
            else if (objectHit.parent.GetComponent<Unit>() && selectedUnits.Count > 0)
            {
                if (objectHit.parent.GetComponent<Unit>().myPlayerNumber != myPlayer.playerNumber)
                {
                    foreach (Unit unit in selectedUnits)
                    {
                        unit.Fire(objectHit.parent.GetComponent<Unit>());
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
