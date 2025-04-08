using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemInfoIntractable")]
public class ItemInfoIntractable : ItemInfo
{
    public ItemType type;
    public FWF itemUtility;

    [Header("Inventory Settings")]
    
    [SerializeField] private InventoryItemInfo inventoryItemInfo;
    public override GameObject InstantiateItem(Vector3 pos, Quaternion rotation)
    {
        GameObject item = Instantiate(gameObject, pos, rotation);
        item.GetComponent<Item>().isIntractable = true;
        return item;
    }
    public override InventoryItemInfo GetInventoryItemInfo()
    {
        return inventoryItemInfo;
    }

    public override string GetLocalizedItemName() 
    { 
        return Localization.Instance.GetText("ItemStringTable", itemName + "Name");
    }

    public override string GetLocalizedItemDescription() 
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
