using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemInfo")]
public class ItemInfo : ScriptableObject
{
    [Header("Base info")]
    public string itemName;
    public GameObject gameObject;

    public virtual GameObject InstantiateItem(Vector3 pos, Quaternion rotation) 
    {
        GameObject item = Instantiate(gameObject, pos, rotation);
        item.GetComponent<Item>().isIntractable = false;
        return item;
    }

    public virtual InventoryItemInfo GetInventoryItemInfo() { return null; }
    public virtual string GetLocalizedItemName() { return null; }
    public virtual string GetLocalizedItemDescription() { return null; }
}
