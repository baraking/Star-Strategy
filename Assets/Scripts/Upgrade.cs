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

    public void ApplyUpgrade()
    {
        for(int i = 0; i < upgradeDetails.unitIndex.Length; i++)
        {
            print(upgradeDetails.GetType().GetProperty(upgradeDetails.parameter[i]));
            upgradeDetails.GetType().GetProperty(upgradeDetails.parameter[i]);
        }
    }

}
