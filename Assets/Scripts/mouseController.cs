using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//be able to drag box for a multi selection
//be able to select only units matching your player's number
//give a world position a fast travel number
//2 on originalMousePositionOnClick should be a var
//should write a different ui for units if more than 1 is seleted, and they share a purchasable
public class mouseController : MonoBehaviour
{
    public Player myPlayer;

    public Camera playerCamera;
    public Vector3 curPosition;

    public bool isSelecting;
    public bool isClicking;

    public float curDistance;
    public Vector3 curRightMousePoint;

    public Vector3 originalMousePositionOnClick;
    public List<GameObject> showingFormaitionLocation = new List<GameObject>();

    public GameObject signalObject;

    public Unit displayingUnit;
    public bool isUnitUIDisplaying;
    public float timeToFinishUpgrade;

    public List<Unit> selectedUnits = new List<Unit>();

    public delegate Vector3[] SelectedGroupMovement(List<Unit> selectedUnits, Vector3 targetLocation, Vector3 rightDirection, float radius);
    public SelectedGroupMovement selectedGroupMovement;
    public SelectedGroupMovement previouslySelectedGroupMovement;
    public bool isActionPointMovementByDefault;

    void Start()
    {
        myPlayer = gameObject.GetComponent<Player>();
        playerCamera = myPlayer.playerCamera;
        isUnitUIDisplaying = false;
        selectedGroupMovement = GroupMovement.PointFormation;
        isActionPointMovementByDefault = false;
    }

    public bool IsMouseHoverOnUIElement()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    public void DisplayUnitPurchasables(Unit selectedUnit)
    {
        isUnitUIDisplaying = true;
        displayingUnit = selectedUnit;

        foreach (Purchasables curPurchasable in selectedUnit.GetPurchasables())
        {
            //print("purchasables: " + curPurchasable);
            GameObject newPurchasableUI = Instantiate(GameManager.Instance.purchaseablePrefab);
            newPurchasableUI.GetComponentInChildren<Button>().image.sprite = curPurchasable.GetIcon();

            newPurchasableUI.transform.SetParent(GameManager.Instance.UnitCanvas.GetComponent<UnitUICanvas>().upgradesCanvas.transform,false);

            if (curPurchasable.GetPrerequisites().Length > 0)
            {
                foreach(int i in curPurchasable.GetPrerequisites())
                {
                    if (!myPlayer.PlayerRaceData.landmarks[i])
                    {
                        newPurchasableUI.GetComponentInChildren<Button>().interactable = false;
                    }
                }
            }

            newPurchasableUI.GetComponentInChildren<Button>().onClick.AddListener(delegate () { curPurchasable.Purchase(selectedUnit.gameObject); });
        } 
    }

    public void ResetDisplayedUnitPurchasableUnits()
    {
        isUnitUIDisplaying = false;
        foreach (Transform child in GameManager.Instance.UnitCanvas.GetComponent<UnitUICanvas>().upgradesCanvas.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    void Update()
    {
        if (myPlayer.photonView.IsMine && !IsMouseHoverOnUIElement())
        {
            //print("selectedUnits.Count: " + selectedUnits.Count);

            if (selectedUnits.Count == 0)
            {
                ResetDisplayedUnitPurchasableUnits();
                GameManager.Instance.SetUnitCanvasDeactive();
            }
            if (selectedUnits.Count > 0)
            {
                GameManager.Instance.SetUnitCanvasActive();
            }

            /*if (selectedUnits.Count == 1)
            {
                timeToFinishUpgrade = Time.time / (selectedUnits[0].timeStartedUpgrading + selectedUnits[0].buildTime);
                print(timeToFinishUpgrade);
            }*/

            if (selectedUnits.Count == 1 && (!isUnitUIDisplaying || (isUnitUIDisplaying && (displayingUnit==null || displayingUnit.name!= selectedUnits[0].name))))
            {
                ResetDisplayedUnitPurchasableUnits();
                DisplayUnitPurchasables(selectedUnits[0]);
            }
            else if(selectedUnits.Count>1)
            {
                ResetDisplayedUnitPurchasableUnits();
            }

            if (!isSelecting && Input.GetKeyDown(PlayerButtons.LEFT_CLICK))
            {
                isSelecting = true;
                originalMousePositionOnClick = Input.mousePosition;
            }
            else if (!isSelecting && !isClicking && Input.GetKeyDown(PlayerButtons.RIGHT_CLICK))
            {
                isClicking = true;
                RaycastHit rightClickHit;
                Ray rightClickRay = playerCamera.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(rightClickRay, out rightClickHit))//could add collider
                {
                    curRightMousePoint = rightClickHit.point;
                }
                    
            
            }

            RaycastHit hit;
            Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))//could add collider
            {
                if (isSelecting)
                {
                    if (Vector3.Distance(originalMousePositionOnClick, Input.mousePosition) < 2)
                    {
                        if (!Input.GetKey(PlayerButtons.MULTI_SELECTION))
                        {
                            foreach (Unit unit in selectedUnits)
                            {
                                if (unit.GetComponent<GroupedUnits>())
                                {
                                    unit.GetComponent<GroupedUnits>().SetIsSelected(false);
                                    unit.GetComponent<GroupedUnits>().SetHealthBarActive(false);
                                }
                                else
                                {
                                    unit.SetIsSelected(false);
                                    unit.SetHealthBarActive(false);
                                }   
                            }
                            selectedUnits.Clear();
                        }

                        Transform objectHit = hit.transform;
                        //Debug.Log(objectHit.name);
                        if (objectHit.GetComponentInParent<Unit>())
                        {
                            if (myPlayer.IsUnitSelectable(objectHit.GetComponentInParent<Unit>()))
                            {
                                bool isSelectedUnitsAmountNotOne = selectedUnits.Count == 1;
                                if (!selectedUnits.Contains(objectHit.GetComponentInParent<Unit>()) && objectHit.gameObject.active)
                                {
                                    if (objectHit.GetComponentInParent<GroupedUnits>())
                                    {
                                        selectedUnits.Add(objectHit.GetComponentInParent<GroupedUnits>());
                                        objectHit.GetComponentInParent<GroupedUnits>().SetIsSelected(true);
                                        objectHit.GetComponentInParent<GroupedUnits>().SetHealthBarActive(true);
                                    }
                                    else
                                    {
                                        selectedUnits.Add(objectHit.GetComponentInParent<Unit>());
                                        objectHit.GetComponentInParent<Unit>().SetIsSelected(true);
                                        objectHit.GetComponentInParent<Unit>().SetHealthBarActive(true);
                                    }
                                }
                                if (selectedUnits.Count == 1 && isSelectedUnitsAmountNotOne)
                                {
                                    displayingUnit = objectHit.GetComponentInParent<Unit>();
                                }
                                else
                                {
                                    displayingUnit = null;
                                }
                            }
                        }
                    }
                    else
                    {
                        bool isSelectedUnitsAmountNotOne = selectedUnits.Count == 1;
                        foreach (Unit hooveredUnit in myPlayer.playerUnits)
                        {
                            print(hooveredUnit);
                            if (IsWithinSelectionBounds(hooveredUnit.gameObject))
                            {
                                if (!selectedUnits.Contains(hooveredUnit) && hooveredUnit.gameObject.active)
                                {
                                    if (hooveredUnit.GetComponent<GroupedUnits>()|| hooveredUnit.GetComponentInParent<GroupedUnits>())
                                    {
                                        GroupedUnits grouped;
                                        if (hooveredUnit.GetComponent<GroupedUnits>())
                                        {
                                            grouped = hooveredUnit.GetComponent<GroupedUnits>();
                                        }
                                        else
                                        {
                                            grouped = hooveredUnit.GetComponentInParent<GroupedUnits>();
                                        }
                                        foreach(Unit unit in grouped.groupedUnits)
                                        {
                                            if (selectedUnits.Contains(unit))
                                            {
                                                selectedUnits.Remove(unit);
                                            }
                                        }
                                        if (!selectedUnits.Contains(grouped))
                                        {
                                            selectedUnits.Add(grouped);
                                            grouped.SetIsSelected(true);
                                            grouped.SetHealthBarActive(true);
                                        }
                                    }
                                    else
                                    {
                                        selectedUnits.Add(hooveredUnit);
                                        hooveredUnit.SetIsSelected(true);
                                        hooveredUnit.SetHealthBarActive(true);
                                    }
                                }
                                if (selectedUnits.Count == 1 && isSelectedUnitsAmountNotOne)
                                {
                                    displayingUnit = hooveredUnit;
                                }
                                else
                                {
                                    displayingUnit = null;
                                }
                            }
                        }
                    }

                }

                //Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
                //print(hit.point);
                curPosition = hit.point;
            }
            ///*
            ///

            Vector3[] formation;
            if (Input.GetKeyDown(PlayerButtons.GROUP))
            {
                List<GroupedUnits> listOfGroupedUnits = new List<GroupedUnits>();
                List<Unit> listOfUnGroupedUnits = new List<Unit>();
                foreach (Unit unit in selectedUnits)
                {
                    if (unit.GetComponent<GroupedUnits>())
                    {
                        if (unit.GetComponent<GroupedUnits>().groupUnitSize < unit.GetComponent<GroupedUnits>().numberOfUnitsAllowed)
                        {
                            listOfGroupedUnits.Add(unit.GetComponent<GroupedUnits>());
                        }
                    }
                    else
                    {
                        if(unit.unitDetails.unitType == UnitDetails.UnitType.Infantry)
                        {
                            listOfUnGroupedUnits.Add(unit);
                        }
                    }
                }
                foreach(Unit unit in listOfUnGroupedUnits)
                {
                    if (listOfGroupedUnits.Count > 0)
                    {
                        listOfGroupedUnits[0].AttachUnit(unit);
                        listOfUnGroupedUnits.Remove(unit);
                        selectedUnits.Remove(unit);
                        if (listOfGroupedUnits[0].carriedAmount == listOfGroupedUnits[0].numberOfUnitsAllowed)
                        {
                            listOfGroupedUnits.Remove(listOfGroupedUnits[0]);
                        }
                    }
                }
                for(int i=0;i< listOfGroupedUnits.Count-1; i++)
                {
                    for(int j = listOfGroupedUnits.Count - 1; j > i; j--)
                    {
                        if (listOfGroupedUnits[j] != null)
                        {
                            if(listOfGroupedUnits[i].carriedAmount + listOfGroupedUnits[j].carriedAmount<= listOfGroupedUnits[i].numberOfUnitsAllowed)
                            {
                                listOfGroupedUnits[i].AttachAllUnits(listOfGroupedUnits[j]);
                            }
                        }
                    }
                }
                if (listOfUnGroupedUnits.Count > 1)
                {
                    int numberOfUnitsAllowed = GameManager.Instance.groupedUnitsPrefab.GetComponent<GroupedUnits>().numberOfUnitsAllowed;
                    print(listOfUnGroupedUnits.Count);
                    print(numberOfUnitsAllowed);
                    for (int i=0;i< listOfUnGroupedUnits.Count / numberOfUnitsAllowed + 1; i++)
                    {
                        GameObject newUnit = Instantiate(GameManager.Instance.groupedUnitsPrefab);
                        newUnit.GetComponent<Unit>().myPlayerNumber = listOfUnGroupedUnits[0].myPlayerNumber;
                        newUnit.GetComponent<Unit>().myPlayer = myPlayer;
                        //newUnit.transform.position = transform.position;
                        newUnit.GetComponent<GroupedUnits>().InitUnit();
                        newUnit.transform.SetParent(GameManager.Instance.Units.transform);

                        //newUnit.GetComponent<Unit>().healthBar = newUnit.GetComponentInChildren<HealthBar>();
                        int max = (listOfUnGroupedUnits.Count - numberOfUnitsAllowed * i > numberOfUnitsAllowed) ? numberOfUnitsAllowed : listOfUnGroupedUnits.Count - numberOfUnitsAllowed * i;
                        for (int j=0;j<max; j++)
                        {
                            newUnit.GetComponent<GroupedUnits>().AttachUnit(listOfUnGroupedUnits[j + numberOfUnitsAllowed * i]);
                            selectedUnits.Remove(listOfUnGroupedUnits[j + numberOfUnitsAllowed * i]);
                        }
                        if (newUnit.GetComponent<GroupedUnits>().groupUnitSize == 0)
                        {
                            Destroy(newUnit);
                        }
                        selectedUnits.Add(newUnit.GetComponent<Unit>());
                    }
                }
            }
            if (Input.GetKeyDown(PlayerButtons.DEGROUP))
            {
                foreach(Unit unit in selectedUnits)
                {
                    if (unit.GetComponent<GroupedUnits>())
                    {
                        selectedUnits.Remove(unit);
                        foreach (Unit groupedUnit in unit.GetComponent<GroupedUnits>().groupedUnits)
                        {
                            selectedUnits.Add(groupedUnit);
                            groupedUnit.SetIsSelected(true);
                            groupedUnit.SetHealthBarActive(true);
                        }
                        unit.GetComponent<GroupedUnits>().DeattachAllUnits();
                        Destroy(unit);
                    }
                }
            }
            if (Input.GetKey(PlayerButtons.RIGHT_CLICK))
            {
                if (Vector3.Distance(curRightMousePoint, hit.point) > .1f)
                {
                    Vector3 newdir = (hit.point - curRightMousePoint);
                    if (isActionPointMovementByDefault)
                    {
                        formation = previouslySelectedGroupMovement(selectedUnits, hit.point, Quaternion.AngleAxis(90, Vector3.up) * newdir, Vector3.Distance(hit.point, curRightMousePoint));
                    }
                    else
                    {
                        formation = selectedGroupMovement(selectedUnits, hit.point, Quaternion.AngleAxis(90, Vector3.up) * newdir, Vector3.Distance(hit.point, curRightMousePoint));
                    }

                    foreach (GameObject tmpObject in showingFormaitionLocation)
                    {
                        Destroy(tmpObject);
                    }

                    foreach (Vector3 position in formation)
                    {
                        showingFormaitionLocation.Add(Instantiate(signalObject, position, new Quaternion()));
                    }
                }
            }
            if (Input.GetKeyUp(PlayerButtons.RIGHT_CLICK))
            {
                Transform objectHit = hit.transform;
                if (objectHit.GetComponentInParent<Unit>() && selectedUnits.Count > 0)
                {
                    if (objectHit.GetComponentInParent<Unit>().myPlayerNumber != myPlayer.playerNumber)
                    {
                        foreach (Unit unit in selectedUnits)
                        {
                            //Debug.Log(unit.name + " is firing on " + objectHit.GetComponentInParent<Unit>().name);
                            if (unit.GetComponent<GroupedUnits>())
                            {
                                unit.GetComponent<GroupedUnits>().Fire(objectHit.GetComponentInParent<Unit>());
                            }
                            else
                            {
                                unit.Fire(objectHit.GetComponentInParent<Unit>());
                            }
                        }

                        if (!isActionPointMovementByDefault)
                        {
                            isActionPointMovementByDefault = true;
                            previouslySelectedGroupMovement = selectedGroupMovement;
                            selectedGroupMovement = GroupMovement.PointFormation;
                        }

                    }
                    else if(objectHit.GetComponentInParent<Unit>().myPlayerNumber == myPlayer.playerNumber)
                    {
                        foreach (Unit unit in selectedUnits)
                        {
                            if(unit.unitDetails.unitType==UnitDetails.UnitType.Infantry && objectHit.GetComponentInParent<Unit>().unitDetails.carryingCapacity- objectHit.GetComponentInParent<Unit>().carriedAmount >= unit.unitDetails.unitSize)
                            {
                                if (unit.GetComponent<GroupedUnits>())
                                {
                                    if (objectHit.GetComponentInParent<Unit>().unitDetails.carryingCapacity - objectHit.GetComponentInParent<Unit>().carriedAmount >= unit.GetComponent<GroupedUnits>().groupUnitSize)
                                    {
                                        unit.GetComponent<GroupedUnits>().SetHasTarget(true);
                                        unit.GetComponent<GroupedUnits>().SetTargetPoint(new Vector3(hit.point.x, unit.transform.position.y, hit.point.z));
                                        objectHit.GetComponentInParent<Unit>().Embark(unit);
                                    }
                                }
                                else
                                {
                                    unit.GetComponent<Walkable>().SetHasTarget(true);
                                    unit.GetComponent<Walkable>().SetTargetPoint(new Vector3(hit.point.x, unit.transform.position.y, hit.point.z));
                                    objectHit.GetComponentInParent<Unit>().Embark(unit);
                                }
                            }
                        }

                        if (!isActionPointMovementByDefault)
                        {
                            isActionPointMovementByDefault = true;
                            previouslySelectedGroupMovement = selectedGroupMovement;
                            selectedGroupMovement = GroupMovement.PointFormation;
                        }
                    }
                }
                else if (objectHit.GetComponentInParent<Resource>() && selectedUnits.Count > 0)
                {
                    foreach (Walkable unit in selectedUnits)
                    {
                        unit.GetComponent<Walkable>().SetHasTarget(true);
                        unit.GetComponent<Walkable>().SetTargetPoint(new Vector3(hit.point.x, unit.transform.position.y, hit.point.z));
                        if (unit.GetComponent<Gatherer>())
                        {
                            unit.GetComponent<Gatherer>().targetResource = objectHit.GetComponentInParent<Resource>();
                        }
                    }

                    if (!isActionPointMovementByDefault)
                    {
                        isActionPointMovementByDefault = true;
                        previouslySelectedGroupMovement = selectedGroupMovement;
                        selectedGroupMovement = GroupMovement.PointFormation;
                    }

                }
                else if (objectHit.GetComponentInParent<ResourceSilo>() && selectedUnits.Count > 0)
                {
                    if (objectHit.GetComponentInParent<Unit>().myPlayerNumber != myPlayer.playerNumber)
                    {
                        foreach (Walkable unit in selectedUnits)
                        {
                            unit.GetComponent<Walkable>().SetHasTarget(true);
                            unit.GetComponent<Walkable>().SetTargetPoint(new Vector3(hit.point.x, unit.transform.position.y, hit.point.z));
                            if (unit.GetComponent<Gatherer>())
                            {
                                unit.GetComponent<Gatherer>().targetResourceSilo = objectHit.GetComponentInParent<ResourceSilo>();
                            }
                        }

                        if (!isActionPointMovementByDefault)
                        {
                            isActionPointMovementByDefault = true;
                            previouslySelectedGroupMovement = selectedGroupMovement;
                            selectedGroupMovement = GroupMovement.PointFormation;
                        }

                    }

                }
                else
                {
                    if (isActionPointMovementByDefault)
                    {
                        isActionPointMovementByDefault = false;
                        selectedGroupMovement = previouslySelectedGroupMovement;
                    }
                }           

                foreach (GameObject tmpObject in showingFormaitionLocation)
                {
                    Destroy(tmpObject);
                }

                if (Vector3.Distance(curRightMousePoint, hit.point) < .1f)
                {
                    formation = selectedGroupMovement(selectedUnits, hit.point, playerCamera.transform.right, 1f);
                }
                else
                {
                    Vector3 newdir = (hit.point - curRightMousePoint);
                    formation = selectedGroupMovement(selectedUnits, hit.point, Quaternion.AngleAxis(90, Vector3.up) * newdir, Vector3.Distance(hit.point, curRightMousePoint));
                }

                int i = 0;
                //if target point is walkable
                foreach (Unit unit in selectedUnits)
                {
                    if (unit.GetComponent<Walkable>())
                    {
                        if (unit.GetComponent<GroupedUnits>())
                        {
                            unit.GetComponent<GroupedUnits>().SetHasTarget(true);
                            unit.GetComponent<GroupedUnits>().SetTargetPoint(new Vector3(formation[i].x, unit.transform.position.y, formation[i].z));
                        }
                        else
                        {
                            unit.GetComponent<Walkable>().SetHasTarget(true);
                            unit.GetComponent<Walkable>().SetTargetPoint(new Vector3(formation[i].x, unit.transform.position.y, formation[i].z));
                        }
                    }
                    i++;
                }
            }
        }
        else//if not viewer hovering over UI Element
        {
            isSelecting = false;
        }
        if (Input.GetKeyUp(PlayerButtons.LEFT_CLICK) || !Input.GetKey(PlayerButtons.LEFT_CLICK))
        {
            isSelecting = false;
        }
        if (Input.GetKeyUp(PlayerButtons.RIGHT_CLICK) || !Input.GetKey(PlayerButtons.RIGHT_CLICK))
        {
            isClicking = false;
        }
        //*/
    }

    private void OnGUI()
    {
        if (isSelecting)
        {
            var rect = MouseGUI.GetScreenRect(originalMousePositionOnClick, Input.mousePosition);
            MouseGUI.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            MouseGUI.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    public bool IsWithinSelectionBounds(GameObject gameObject)
    {
        if (!isSelecting)
        {
            return false;
        }

        //var camera = Camera.main;
        var viewportBounds = GetViewPortBounds(playerCamera, originalMousePositionOnClick, Input.mousePosition);

        return viewportBounds.Contains(playerCamera.WorldToViewportPoint(gameObject.transform.position));
    }

    public static Bounds GetViewPortBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
    {
        var v1 = camera.ScreenToViewportPoint(screenPosition1);
        var v2 = camera.ScreenToViewportPoint(screenPosition2);
        var min = Vector3.Min(v1, v2);
        var max = Vector3.Max(v1, v2);
        min.z = camera.nearClipPlane;
        max.z = camera.farClipPlane;

        var bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }

}
