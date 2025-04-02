[System.Serializable]
public class UtilityPoint
{
    public int utilityValue;
    public UtilityType utilityType;

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
        
        foreach(string text in uTextColection) 
        { 
            if(text == "{value}") 
            {
                utilityText += " " + utilityValue.ToString();
                continue;
            }

            utilityText += " " + text;
        }
        return utilityText;
    }
}

public enum UtilityType 
{ 
    Food,
    Water,
    Filter
}
