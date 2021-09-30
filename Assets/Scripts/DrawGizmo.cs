using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawGizmo : MonoBehaviour
{
    public static readonly float DIAMETER_CONST = 0.05f;

    public int color;
    public int value;

    void OnDrawGizmos()
    {
        if (color == 0)
        {
            Gizmos.color = Color.black;
        }
        if (color == 1)
        {
            Gizmos.color = Color.blue;
        }
        if (color == 2)
        {
            Gizmos.color = Color.cyan;
        }
        if (color == 3)
        {
            Gizmos.color = Color.gray;
        }
        if (color == 4)
        {
            Gizmos.color = Color.green;
        }
        if (color == 5)
        {
            Gizmos.color = Color.magenta;
        }
        if (color == 6)
        {
            Gizmos.color = Color.red;
        }
        if (color == 7)
        {
            Gizmos.color = Color.white;
        }
        if (color == 8)
        {
            Gizmos.color = Color.yellow;
        }

        Gizmos.DrawSphere(transform.position, value * DIAMETER_CONST);
    }
}
