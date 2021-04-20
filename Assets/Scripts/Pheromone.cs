using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//change the colors system

public class Pheromone : MonoBehaviour
{
    public enum PheromoneType { ToHome, ToFood};

    public PheromoneType myPheromoneType;
    public float timeToReduceIntensity;
    public float intensity;
    bool isShrinking;

    public Vector3 layoutDirection;

    // Start is called before the first frame update
    void Start()
    {
        isShrinking = false;
        layoutDirection = transform.forward;
    }

    public void SetPheromoneType(PheromoneType pheromoneType)
    {
        myPheromoneType = pheromoneType;
        if (myPheromoneType == PheromoneType.ToHome)
        {
            gameObject.GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.white);
        }
        else if (myPheromoneType == PheromoneType.ToFood)
        {
            gameObject.GetComponentInChildren<Renderer>().material.SetColor("_Color", Color.green);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (intensity <= .1f)
        {
            Destroy(gameObject);
        }
        if (!isShrinking)
        {
            StartCoroutine("UpdatePheromeneScale", timeToReduceIntensity);
        }
        
    }

    IEnumerator UpdatePheromeneScale(float time)
    {
        isShrinking = true;
        yield return new WaitForSeconds(time);

        intensity -= .1f;
        transform.localScale = new Vector3(transform.localScale.x * intensity, transform.localScale.y * intensity, transform.localScale.z * intensity);
        yield return new WaitForSeconds(time);
        isShrinking = false;
    }
}
