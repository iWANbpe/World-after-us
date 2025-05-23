using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "ScriptableObjects/InventoryItemInfo")]
public class InventoryItemInfo : ScriptableObject
{
	[HideInInspector] public ItemInfoIntractable itemInfo;
	public InventoryItem inventoryItem;
	public string invItemName { get { return inventoryItem.name; } }
	public Image invItemImage { get { return inventoryItem.invItemImage; } }

	public GameObject AddItemToInventory(Vector2 position, Transform parent)
	{
		GameObject invItem = Instantiate(inventoryItem.gameObject, position, Quaternion.identity, parent);
		invItem.GetComponent<InventoryItem>().Initialization();
		return invItem;
	}
}