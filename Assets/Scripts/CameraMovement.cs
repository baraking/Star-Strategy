using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//zoom in and out with an xDeg angle correction
//limit movement out of map borders
//focus on selected unit/landmark
public class CameraMovement : MonoBehaviour
{
    float cameraMovementSpeed = 1;
    float fasterSpeed = 3;
    float cameraRotationSpeed = 60;//angular speed

    public float xDeg = 30;
    public float yDeg = 0;
    public float zDeg = 0;

    private Quaternion desiredRotation;
    private Quaternion currentRotation;
    private Quaternion rotation;

    public float zoomDampening = 5.0f;

    public GameObject cameraHolder;

    void Start()
    {
        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
    }

    void Update()
    {
        //print(xDeg);

        Vector3 plannedMovement = new Vector3();

        if (Input.GetKey(PlayerButtons.FORWARD))
        {
            plannedMovement += new Vector3(Mathf.Sin(Mathf.Deg2Rad * xDeg), 0f, Mathf.Cos(Mathf.Deg2Rad * xDeg));
        }
        if (Input.GetKey(PlayerButtons.BACKWARDS))
        {
            plannedMovement -= new Vector3(Mathf.Sin(Mathf.Deg2Rad * xDeg), 0f, Mathf.Cos(Mathf.Deg2Rad * xDeg));
        }
        if (Input.GetKey(PlayerButtons.LEFT))
        {
            plannedMovement -= new Vector3(Mathf.Sin(Mathf.Deg2Rad * (xDeg + 90)), 0f, Mathf.Cos(Mathf.Deg2Rad * (xDeg + 90)));
        }
        if (Input.GetKey(PlayerButtons.RIGHT))
        {
            plannedMovement += new Vector3(Mathf.Sin(Mathf.Deg2Rad * (xDeg + 90)), 0f, Mathf.Cos(Mathf.Deg2Rad * (xDeg + 90)));
        }

        if (Input.GetKey(PlayerButtons.FASTER_MOVEMENT))
        {
            transform.position += plannedMovement.normalized * Time.deltaTime * cameraMovementSpeed * fasterSpeed;
        }
        else
        {
            transform.position += plannedMovement.normalized * Time.deltaTime * cameraMovementSpeed;
        }
        

        if (Input.GetKey(PlayerButtons.ROTATE_LEFT))
        {
            xDeg -= cameraRotationSpeed * Time.deltaTime;
            if (xDeg < 0)
            {
                xDeg += 360;
            }
        }
        if (Input.GetKey(PlayerButtons.ROTATE_RIGHT))
        {
            xDeg += cameraRotationSpeed * Time.deltaTime;
            if (xDeg > 360)
            {
                xDeg -= 360;
            }
        }

        desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
        currentRotation = transform.rotation;

        rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
        transform.rotation = rotation;
    }
}
