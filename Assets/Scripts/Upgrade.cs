using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : Purchasables
{

    public UpgradeDetails upgradeDetails;


    public override int[] GetPrerequisites()
    {
        return upgradeDetails.prerequisites;
    }

    public override int[] GetRequirements()
    {
        return upgradeDetails.requirements;
    }

    public override Sprite GetIcon()
    {
        return upgradeDetails.icon;
    }

}
