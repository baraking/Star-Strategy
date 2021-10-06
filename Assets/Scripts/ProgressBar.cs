using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public Image image;
    public Slider slider;

    void Awake()
    {
        SetImageToState(false);
    }

    public void SetImageToState(bool state)
    {
        image.gameObject.SetActive(state);
    }
}
