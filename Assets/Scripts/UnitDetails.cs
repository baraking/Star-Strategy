using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//model
//...

[CreateAssetMenu(fileName ="New Unit", menuName ="Units")]
public class UnitDetails : ScriptableObject
{
    public new string name;
    public int max_hp;
    public int cur_hp;

    public float speed;

    public Sprite icon;
}
