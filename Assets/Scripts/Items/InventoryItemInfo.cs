using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObjects/InventoryItemInfo")]
public class InventoryItemInfo : ScriptableObject
{
	public ItemInfoIntractable itemInfo;
	public GameObject inventoryItem;
	public string invItemName { get { return inventoryItem.name; } }
	public Image invItemImage { get { return inventoryItem.GetComponent<InventoryItem>().invItemImage; } }

	public GameObject AddItemToInventory(Vector2 position, Transform parent)
	{
		GameObject invItem = Instantiate(inventoryItem, position, Quaternion.identity, parent);
		invItem.GetComponent<InventoryItem>().Initialization();
		return invItem;
	}
}