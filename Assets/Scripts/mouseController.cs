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
                        unit.setHealthBarActive(false);
                    }
                    selectedUnits.Clear();
                }

                Transform objectHit = hit.transform;
                if (objectHit.GetComponent<Unit>())
                {
                    if (myPlayer.IsUnitSelectable(objectHit.GetComponent<Unit>()))
                    {
                        selectedUnits.Add(objectHit.GetComponent<Unit>());
                        objectHit.GetComponent<Unit>().isSelected = true;
                        objectHit.GetComponent<Unit>().setHealthBarActive(true);
                    }
                }
                else
                {
                    if (objectHit.parent.GetComponent<Unit>())
                    {
                        if (myPlayer.IsUnitSelectable(objectHit.parent.GetComponent<Unit>()))
                        {
                            selectedUnits.Add(objectHit.parent.GetComponent<Unit>());
                            objectHit.parent.GetComponent<Unit>().isSelected = true;
                            objectHit.parent.GetComponent<Unit>().setHealthBarActive(true);
                        }
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
            //print("clicked!");
            //Instantiate(signalObject, hit.point, new Quaternion());
            foreach (Unit unit in selectedUnits)
            {
                if (unit.GetComponent<Walkable>())
                {
                    unit.GetComponent<Walkable>().targetPoint = hit.point;
                }
            }
        }
        //*/
    }
}
