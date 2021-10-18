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

    public void ApplyUpgrade(int playerNumber)
    {
        for(int i = 0; i < upgradeDetails.unitIndex.Length; i++)
        {

            if (upgradeDetails.parameter[i] == UpgradeDetails.PossibleParameters.Max_hp)
            {
                UnitDetails tmp = (UnitDetails)GameManager.Instance.playersHolder.allPlayers[playerNumber].PlayerRaceData.myFactionSpeciefcPurchasablesList[upgradeDetails.unitIndex[i]];
                float prevMax_hp = tmp.max_hp;
                tmp.max_hp *= (int)upgradeDetails.newValueMultiplier[i];

                float hpToAdd = tmp.max_hp - prevMax_hp;
                //set for all units
                //update all units cur health by +=hpToAdd
            }
            else if (upgradeDetails.parameter[i] == UpgradeDetails.PossibleParameters.Damage)
            {
                WeaponDetails tmp = (WeaponDetails)GameManager.Instance.playersHolder.allPlayers[playerNumber].PlayerRaceData.myFactionSpeciefcPurchasablesList[upgradeDetails.unitIndex[i]];
                tmp.damage *= (int)upgradeDetails.newValueMultiplier[i];
            }
        }
    }

}
