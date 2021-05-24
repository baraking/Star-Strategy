using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GroupMovement : MonoBehaviour
{
    public Image smallChosenIcon;

    public Sprite smallArcDefensiveFormation;
    public Sprite smallArcOffensiveFormation;
    public Sprite smallCircleFormation;
    public Sprite smallRowFormation;
    public Sprite smallPointFormation;

    public static float GetRotationDegree(Vector3 rightDirection, float additionalRotation)
    {
        float rotationDeg = Vector3.Angle(Vector3.right, rightDirection);
        Vector3 dirVector = Vector3.Cross(Vector3.right, rightDirection);
        if (dirVector.y < 0)
        {
            rotationDeg = -rotationDeg;
        }
        rotationDeg -= additionalRotation;
        return rotationDeg;
    }

    public void closeMovementCanvas()
    {
        GameManager.Instance.SetMovementCanvasDeactive();
    }

    public void openMovementCanvas()
    {
        GameManager.Instance.SetMovementCanvasActive();
    }

    public static Vector3[] ArcFormation(List<Unit> selectedUnits, Vector3 targetLocation, Vector3 rightDirection, float radius, float additionalRotation)
    {
        Vector3[] ans = new Vector3[selectedUnits.Count];
        float angle = 180 / (selectedUnits.Count + 2);
        float curAngle = 0;

        float rotationDeg = GetRotationDegree(rightDirection, additionalRotation);

        for (int i = 1; i < selectedUnits.Count + 1; i++)
        {
            curAngle = angle * i + rotationDeg;
            ans[i - 1] = new Vector3(Mathf.Sin(Mathf.Deg2Rad * curAngle) * radius + targetLocation.x, 0f, Mathf.Cos(Mathf.Deg2Rad * curAngle) * radius + targetLocation.z);
        }
        return ans;
    }

    public static Vector3[] ArcDefensiveFormation(List<Unit> selectedUnits, Vector3 targetLocation, Vector3 rightDirection, float radius)
    {
        return ArcFormation(selectedUnits, targetLocation, rightDirection, radius, 90);
    }

    public static Vector3[] ArcOffensiveFormation(List<Unit> selectedUnits, Vector3 targetLocation, Vector3 rightDirection, float radius)
    {
        return ArcFormation(selectedUnits, targetLocation, rightDirection, radius, -90);
    }

    public static Vector3[] CircleFormation(List<Unit> selectedUnits, Vector3 targetLocation,Vector3 rightDirection, float radius)
    {
        if (selectedUnits.Count == 1)
        {
            return new Vector3[] { targetLocation };
        }

        Vector3[] ans = new Vector3[selectedUnits.Count];
        float angle = 360 / selectedUnits.Count;
        float curAngle = 0;

        float rotationDeg = GetRotationDegree(rightDirection, 0);

        for (int i = 0; i < selectedUnits.Count; i++)
        {
            curAngle = angle * i + rotationDeg;
            ans[i] = new Vector3(Mathf.Sin(Mathf.Deg2Rad * i * angle) * radius + targetLocation.x, 0f, Mathf.Cos(Mathf.Deg2Rad * angle * i) * radius + targetLocation.z);
        }

        return ans;
    }

    public static Vector3[] RowFormation(List<Unit> selectedUnits, Vector3 targetLocation, Vector3 rightDirection, float radius)
    {
        if (selectedUnits.Count == 1)
        {
            return new Vector3[] { targetLocation };
        }

        Vector3[] ans = new Vector3[selectedUnits.Count];
        float rotationDeg = GetRotationDegree(rightDirection, 90);

        ans[0] = new Vector3(Mathf.Sin(Mathf.Deg2Rad * rotationDeg) * radius + targetLocation.x, 0f, Mathf.Cos(Mathf.Deg2Rad * rotationDeg) * radius + targetLocation.z);
        ans[selectedUnits.Count-1] = new Vector3(Mathf.Sin(Mathf.Deg2Rad * (180 + rotationDeg)) * radius + targetLocation.x, 0f, Mathf.Cos(Mathf.Deg2Rad * (180 + rotationDeg)) * radius + targetLocation.z);
        Vector3 delta = (ans[selectedUnits.Count - 1] - ans[0])/ (selectedUnits.Count - 1);

        for (int i = 1; i < selectedUnits.Count - 1; i++)
        {
            ans[i] = new Vector3(ans[0].x+i*delta.x, 0f, ans[0].z + i * delta.z);
        }
        return ans;
    }

    public static Vector3[] PointFormation(List<Unit> selectedUnits, Vector3 targetLocation, Vector3 rightDirection, float radius)
    {
        Vector3[] ans = new Vector3[selectedUnits.Count];
        
        for (int i = 0; i < selectedUnits.Count; i++)
        {
            ans[i] = new Vector3(targetLocation.x, 0f, targetLocation.z);
        }

        return ans;
    }

    public static void SetCircleFormationActive()
    {
        foreach (Player player in GameManager.Instance.playersHolder.allPlayers)
        {
            if (player.photonView.IsMine)
            {
                player.gameObject.GetComponentInChildren<mouseController>().selectedGroupMovement = CircleFormation;
                //smallChosenIcon = smallCircleFormation;
                return;
            }
        }
    }

    public static void SetOffensiveArcFormationActive()
    {
        foreach (Player player in GameManager.Instance.playersHolder.allPlayers)
        {
            if (player.photonView.IsMine)
            {
                player.gameObject.GetComponentInChildren<mouseController>().selectedGroupMovement = ArcOffensiveFormation;
                //smallChosenIcon = smallArcOffensiveFormation;
                return;
            }
        }
    }

    public static void SetDefensiveArcFormationActive()
    {
        foreach (Player player in GameManager.Instance.playersHolder.allPlayers)
        {
            if (player.photonView.IsMine)
            {
                player.gameObject.GetComponentInChildren<mouseController>().selectedGroupMovement = ArcDefensiveFormation;
                //smallChosenIcon = smallArcDefensiveFormation;
                return;
            }
        }
    }

    public static void SetRowFormationActive()
    {
        foreach (Player player in GameManager.Instance.playersHolder.allPlayers)
        {
            if (player.photonView.IsMine)
            {
                player.gameObject.GetComponentInChildren<mouseController>().selectedGroupMovement = RowFormation;
                //smallChosenIcon = smallRowFormation;
                return;
            }
        }
    }

    private static void PrintArray(Vector3[] array)
    {
        string message = "Points: ";
        foreach(Vector3 point in array)
        {
            message += point;
        }
        print(message);
    }

}
