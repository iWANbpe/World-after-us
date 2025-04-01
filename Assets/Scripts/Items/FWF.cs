using System.Collections.Generic;
using UnityEngine;

public class FWF
{
    public List<UtilityPoint> utilityPoints;

    public FWF(float foodValue, float waterValue, float filterValue) 
    {
        if (foodValue > 0f) utilityPoints.Add(new UtilityPoint(foodValue, UtilityType.Food));
        if (waterValue > 0f) utilityPoints.Add(new UtilityPoint(foodValue, UtilityType.Water));
        if (filterValue > 0f) utilityPoints.Add(new UtilityPoint(foodValue, UtilityType.Filter));
    }

    public void Add(FWF fwf) 
    { 
        foreach(UtilityPoint utilityPointMain in utilityPoints) 
        { 
            foreach(UtilityPoint utilityPointAdd in fwf.utilityPoints) 
            { 
                if(utilityPointMain.utilityType == utilityPointAdd.utilityType) 
                {
                    utilityPointMain.utilityValue += utilityPointAdd.utilityValue;
                }
            }
        }
    }

    public UtilityPoint GetUtilityPoint(UtilityType utilityType) 
    { 
        foreach(UtilityPoint uPoint in utilityPoints) 
        {
            if (uPoint.utilityType == utilityType) return uPoint;
        }
        return null;
    }
}
