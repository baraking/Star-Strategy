using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerButtons
{
    #region cameraMovement
    public static readonly KeyCode FORWARD = KeyCode.W;
    public static readonly KeyCode BACKWARDS = KeyCode.S;
    public static readonly KeyCode LEFT = KeyCode.A;
    public static readonly KeyCode RIGHT = KeyCode.D;

    public static readonly KeyCode ROTATE_LEFT = KeyCode.Q;
    public static readonly KeyCode ROTATE_RIGHT = KeyCode.E;

    public static readonly KeyCode FASTER_MOVEMENT = KeyCode.LeftShift;
    #endregion

    #region mouseActions
    public static readonly KeyCode LEFT_CLICK = KeyCode.Mouse0;
    public static readonly KeyCode RIGHT_CLICK = KeyCode.Mouse1;
    #endregion
}
