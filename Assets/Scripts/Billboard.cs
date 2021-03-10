using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour {

    public Transform playerCamera;
    public static readonly float TIME_TO_START_LOOKING_FOR_CAMERA = 0.25f;

    void Start()
    {
        StartCoroutine(LateStart(TIME_TO_START_LOOKING_FOR_CAMERA));
    }

    IEnumerator LateStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        setPlayerCamera();
    }

    public void setPlayerCamera()
    {
        playerCamera = GameManager.Instance.playersHolder.allPlayers[gameObject.GetComponentInParent<Unit>().myPlayer.playerNumber].playerCamera.transform;
    }

    void LateUpdate()
    {
        if (playerCamera == null)
        {
            setPlayerCamera();
        }
        transform.LookAt(transform.position + playerCamera.forward);
    }
}
