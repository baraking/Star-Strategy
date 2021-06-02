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
                                bool isSelectedUnitsAmountNotOne = selectedUnits.Count == 1;
                                if (!selectedUnits.Contains(objectHit.GetComponentInParent<Unit>()))
                                {
                                    selectedUnits.Add(objectHit.GetComponentInParent<Unit>());
                                    objectHit.GetComponentInParent<Unit>().isSelected = true;
                                    objectHit.GetComponentInParent<Unit>().SetHealthBarActive(true);
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
                                if (!selectedUnits.Contains(hooveredUnit))
                                {
                                    selectedUnits.Add(hooveredUnit);
                                    hooveredUnit.isSelected = true;
                                    hooveredUnit.SetHealthBarActive(true);
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
                            unit.Fire(objectHit.GetComponentInParent<Unit>());
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
                            if(objectHit.GetComponentInParent<Unit>().unitDetails.carryingCapacity- objectHit.GetComponentInParent<Unit>().carriedAmount >= unit.unitDetails.unitSize)
                            {
                                objectHit.GetComponentInParent<Unit>().Embark(unit);
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
                        unit.GetComponent<Walkable>().hasTarget = true;
                        unit.GetComponent<Walkable>().targetPoint = new Vector3(hit.point.x, unit.transform.position.y, hit.point.z);
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
                            unit.GetComponent<Walkable>().hasTarget = true;
                            unit.GetComponent<Walkable>().targetPoint = new Vector3(hit.point.x, unit.transform.position.y, hit.point.z);
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
                        unit.GetComponent<Walkable>().hasTarget = true;
                        unit.GetComponent<Walkable>().targetPoint = new Vector3(formation[i].x, unit.transform.position.y, formation[i].z);
                    }
                    i++;
                }
            }
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
