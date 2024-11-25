using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemInfo")]
public class ItemInfo : ScriptableObject
{
    [Header("Base info")]

    public string itemName;
    [Multiline] public string itemDescription;
    public ItemType type;
    public GameObject gameObject;

    [Header("Inventory Settings")]
    
    public InventoryItemInfo inventoryItemInfo;

    public void InstaniateItem(Transform itemTransform) 
    {
        GameObject item = Instantiate(gameObject, itemTransform);
        item.GetComponent<ItemInfoHolder>().itemInfo = this;
    }

    public void AddInventoryItem() 
    { 
        
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
