[System.Serializable]
public class UtilityPoint
{
	public int utilityValue;
	public UtilityType utilityType;

	private int minUtilityValue = 0;
	private int maxUtilityValue = 100;

	public int minValue { get { return minUtilityValue; } }
	public int maxValue { get { return maxUtilityValue; } }

	public UtilityPoint(int utilityValue, UtilityType utilityType)
	{
		this.utilityValue = utilityValue;
		this.utilityType = utilityType;
	}

	public string GetUtilityText()
	{
		string utilityText = Localization.Instance.GetText("UIStringTable", "add" + utilityType.ToString() + "Text");
		string[] uTextColection = utilityText.Split(" ");
		utilityText = "";

		foreach (string text in uTextColection)
		{
			if (text == "{value}")
			{
				utilityText += " " + utilityValue.ToString();
				continue;
			}

			utilityText += " " + text;
		}
		return utilityText;
	}

	public void ChangeMaxValue(int value) { maxUtilityValue = value; }
	public void ChangeMinValue(int value) { minUtilityValue = value; }


}

public enum UtilityType
{
	Food,
	Water,
	Filter
}