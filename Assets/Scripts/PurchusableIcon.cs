using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PurchusableIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private bool mouseOver = false;
    private float hoverTime = 0;

    void Update()
    {
        if (mouseOver)
        {
            hoverTime += Time.deltaTime;
            Debug.Log(hoverTime);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverTime = 0;
        mouseOver = false;
    }
}
