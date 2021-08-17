using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//consider the usage of a gradiant for different color as the hp gets lower
//The positioning is much easier if you use this: [Reference to UI health bar gameobject].transform.position = Camera.main.WorldToScreenPoint([Reference to target].transform.position + posOffset); posOffset is public Vector3 posOffset; this method will also remove the issue with render scale, because you are not renderring the canvas as an actual object in the scene which is away from the camera.
public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider constructionSlider;

    public static readonly float SET_UP_TIME = 0.5f;

    void Start()
    {
        StartCoroutine(LateStart(SET_UP_TIME));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        this.gameObject.SetActive(false);
    }

    public void SetMaxHealth(float maxHealth)
    {
        healthSlider.maxValue = maxHealth;
        healthSlider.value = maxHealth;
    }

    public void SetMaxConstruction(float maxConstruction)
    {
        constructionSlider.maxValue = maxConstruction;
    }

    public bool IsHealthVisible()
    {
        return gameObject.active;
    }

    public void setHealth(float health)
    {
        healthSlider.value = health;
    }

    public void setConstruction(float health)
    {
        constructionSlider.value = health;
    }

    public void DisableConstructionBar()
    {
        constructionSlider.gameObject.SetActive(false);
    }
}
