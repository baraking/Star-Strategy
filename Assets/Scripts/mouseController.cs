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

    public Vector3 originalMousePositionOnClick;

    public GameObject signalObject;

    public Unit displayingUnit;
    public bool isUnitUIDisplaying;

    public List<Unit> selectedUnits = new List<Unit>();

    void Start()
    {
        myPlayer = gameObject.GetComponent<Player>();
        playerCamera = myPlayer.playerCamera;
        isUnitUIDisplaying = false;
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
            print("selectedUnits.Count: " + selectedUnits.Count);

            if (selectedUnits.Count == 0)
            {
                ResetDisplayedUnitPurchasableUnits();
                GameManager.Instance.SetUnitCanvasDeactive();
            }
            if (selectedUnits.Count > 0)
            {
                GameManager.Instance.SetUnitCanvasActive();
            }

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
        }
        if (Input.GetKeyUp(PlayerButtons.LEFT_CLICK) || !Input.GetKey(PlayerButtons.LEFT_CLICK))
        {
            isSelecting = false;
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
