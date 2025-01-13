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

    public void InstaniateItem(Vector3 position, Quaternion rotation) 
    {
        Instantiate(gameObject, position, rotation);
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
