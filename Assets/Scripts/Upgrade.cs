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
                int prevMax_hp = (GameManager.Instance.playersHolder.allPlayers[playerNumber].PlayerRaceData.myFactionSpeciefcPurchasablesList[upgradeDetails.unitIndex[i]] as UnitDetails).max_hp;
                (GameManager.Instance.playersHolder.allPlayers[playerNumber].PlayerRaceData.myFactionSpeciefcPurchasablesList[upgradeDetails.unitIndex[i]] as UnitDetails).max_hp *= (int)upgradeDetails.newValueMultiplier[i];

                int hpToAdd = prevMax_hp * ((int)upgradeDetails.newValueMultiplier[i] - 1);

                //set for all units

                //update all units cur health by +=hpToAdd
                foreach(Unit unit in GameManager.Instance.playersHolder.allPlayers[playerNumber].playerUnits)
                {
                    if(unit.RACE_INDEX== upgradeDetails.unitIndex[i])
                    {
                        unit.HealUnit(hpToAdd);
                    }
                }

            }
            else if (upgradeDetails.parameter[i] == UpgradeDetails.PossibleParameters.Damage)
            {
                WeaponDetails tmp = (WeaponDetails)GameManager.Instance.playersHolder.allPlayers[playerNumber].PlayerRaceData.myFactionSpeciefcPurchasablesList[upgradeDetails.unitIndex[i]];
                tmp.damage *= (int)upgradeDetails.newValueMultiplier[i];
            }
        }
    }

}
