using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemInfo")]
public class ItemInfo : ScriptableObject
{
    [Header("Base info")]

    public string itemName;
    public ItemType type;
    public FWF itemUtility;
    public GameObject gameObject;

    [Header("Inventory Settings")]
    
    public InventoryItemInfo inventoryItemInfo;

    public void InstaniateItem(Vector3 position, Quaternion rotation) 
    {
        Instantiate(gameObject, position, rotation);
    }

    public string GetLocalizedItemName() 
    { 
        return Localization.Instance.GetText("ItemStringTable", itemName + "Name");
    }

    public string GetLocalizedItemDescription() 
    { 
        return Localization.Instance.GetText("ItemStringTable", itemName + "Description");
    }
}

public enum ItemType 
{ 
    Food,
    Water,
    Filter,
    Scrap,
    Gun
}
