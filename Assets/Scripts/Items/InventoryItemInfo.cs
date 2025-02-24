using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObjects/InventoryItemInfo")]
public class InventoryItemInfo : ScriptableObject
{
    public ItemInfo itemInfo;
    public GameObject inventoryItem;
    public string invItemName { get { return inventoryItem.name; } }
    public Image invItemImage { get { return inventoryItem.GetComponent<InventoryItem>().invItemImage; } }
    public string invItemSizeCode { get { return inventoryItem.GetComponent<InventoryItem>().sizeCode; } }
    
    public void AddItemToInventory(Vector2 position) 
    {
        GameObject inventory = GameObject.Find("Canvas").transform.Find("Inventory").gameObject;
        
        GameObject invItem = Instantiate(inventoryItem, position, Quaternion.identity, inventory.transform);
        invItem.GetComponent<InventoryItem>().Initialization();
        invItem.GetComponent<InventoryItem>().SetPosition(position);
    }
}
