using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Item")]
public class Item : ScriptableObject
{
    [Header("Base info")]

    public string itemName;
    [Multiline] public string itemDescription;
    public ItemType type;
    public GameObject gameObject;

    [Header("Inventory Settings")]
    
    public InventoryItem inventoryItem;
}

public enum ItemType 
{ 
    Food,
    Water,
    Filter,
    Scrap,
    Gun
}
