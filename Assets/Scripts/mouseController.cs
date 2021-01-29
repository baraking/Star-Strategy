using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//get the coordinates (and unit) which I left click upon
//be able to drag box for a multi selection
//be able to select only units matching your player's number

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
                foreach (Unit unit in selectedUnits)
                {
                    unit.isSelected = false;
                }
                selectedUnits.Clear();

                Transform objectHit = hit.transform;
                if (objectHit.GetComponent<Unit>())
                {
                    selectedUnits.Add(objectHit.GetComponent<Unit>());
                    objectHit.GetComponent<Unit>().isSelected = true;
                }
            }

            //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
            //print(hit.point);
            curPosition = hit.point;
        }
        
        ///*
        if (Input.GetKeyDown(PlayerButtons.RIGHT_CLICK))
        {
            print("clicked!");
            Instantiate(signalObject, hit.point, new Quaternion());
        }
        //*/
    }
}
