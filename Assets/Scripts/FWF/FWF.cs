using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FWF
{
	public List<UtilityPoint> utilityPoints = new List<UtilityPoint>();

	public FWF(int foodValue, int waterValue, int filterValue)
	{
		if (foodValue > 0) utilityPoints.Add(new UtilityPoint(foodValue, UtilityType.Food));
		if (waterValue > 0) utilityPoints.Add(new UtilityPoint(waterValue, UtilityType.Water));
		if (filterValue > 0) utilityPoints.Add(new UtilityPoint(filterValue, UtilityType.Filter));
	}

	public void Add(FWF fwf)
	{
		foreach (UtilityPoint utilityPointMain in utilityPoints)
		{
			foreach (UtilityPoint utilityPointAdd in fwf.utilityPoints)
			{
				if (utilityPointMain.utilityType == utilityPointAdd.utilityType)
				{
					utilityPointMain.utilityValue += utilityPointAdd.utilityValue;
					utilityPointMain.utilityValue = Mathf.Clamp(utilityPointMain.utilityValue, utilityPointMain.minValue, utilityPointMain.maxValue);
				}
			}
		}
	}

	public void Decrease(FWF fwf)
	{
		foreach (UtilityPoint utilityPointMain in utilityPoints)
		{
			foreach (UtilityPoint utilityPointAdd in fwf.utilityPoints)
			{
				if (utilityPointMain.utilityType == utilityPointAdd.utilityType)
				{
					utilityPointMain.utilityValue -= utilityPointAdd.utilityValue;
					utilityPointMain.utilityValue = Mathf.Clamp(utilityPointMain.utilityValue, utilityPointMain.minValue, utilityPointMain.maxValue);
				}
			}
		}
	}

	public UtilityPoint GetUtilityPoint(UtilityType utilityType)
	{
		foreach (UtilityPoint uPoint in utilityPoints)
		{
			if (uPoint.utilityType == utilityType) return uPoint;
		}
		return null;
	}

	public string GetUtilityText()
	{
		string uText = "";

		foreach (UtilityPoint uPoint in utilityPoints)
		{
			uText += uPoint.GetUtilityText() + "\n";
		}
		return uText;
	}
}