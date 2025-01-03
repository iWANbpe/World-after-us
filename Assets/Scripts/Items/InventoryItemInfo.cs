using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObjects/InventoryItemInfo")]
public class InventoryItemInfo : ScriptableObject
{
    public ItemInfo itemInfo;
    [SerializeField] private GameObject inventoryItem;
    public string invItemName { get { return inventoryItem.name; } }
    public Image invItemImage { get { return inventoryItem.GetComponent<InventoryItem>().invItemImage; } } 

    public void AddItemToInventory(Vector3 position) 
    {
        GameObject inventory = GameObject.Find("Canvas").transform.Find("Inventory").gameObject;
        Instantiate(inventoryItem, position, Quaternion.identity, inventory.transform);
    }
}
