using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//model
//...

[CreateAssetMenu(fileName ="New Unit", menuName ="Units")]
public class UnitDetails : ScriptableObject
{
    public new string name;
    public int hp;

    public Sprite icon;
}
