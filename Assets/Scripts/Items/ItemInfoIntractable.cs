using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemInfoIntractable")]
public class ItemInfoIntractable : ItemInfo, IInteract, ILocalizationItem
{
	public ItemType type;
	[SerializeField] private FWF itemUtility;

	[Header("Inventory Settings")]

	[SerializeField] private InventoryItemInfo inventoryItemInfo;

	public bool CanInteract()
	{
		return true;
	}

	public void Interact()
	{
		GameObject.Find("Player").GetComponent<PlayerController>().AddItemToInventory();
	}

	public override GameObject InstantiateItem(Vector3 pos, Quaternion rotation)
	{
		GameObject itemObj = Instantiate(item.gameObject, pos, rotation);

		if (inventoryItemInfo.itemInfo != this)
			inventoryItemInfo.itemInfo = this;

		if (inventoryItemInfo.inventoryItem.invItemInfo != inventoryItemInfo)
			inventoryItemInfo.inventoryItem.invItemInfo = inventoryItemInfo;

		if (itemObj.GetComponent<Item>().itemInfo != this)
			itemObj.GetComponent<Item>().itemInfo = this;
		return itemObj;
	}

	public string GetLocalizedItemName()
	{
		return Localization.Instance.GetText("ItemStringTable", itemName + "Name");
	}

	public string GetLocalizedItemDescription()
	{
		return Localization.Instance.GetText("ItemStringTable", itemName + "Description");
	}

	public InventoryItemInfo GetInventoryItemInfo()
	{
		return inventoryItemInfo;
	}

	public FWF GetItemFWF()
	{
		return itemUtility;
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