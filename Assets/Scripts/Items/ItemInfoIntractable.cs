using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemInfoIntractable")]
public class ItemInfoIntractable : ItemInfo, IInteract
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

	public override InventoryItemInfo GetInventoryItemInfo()
	{
		return inventoryItemInfo;
	}

	public override string GetLocalizedItemDescription()
	{
		return Localization.Instance.GetText("ItemStringTable", itemName + "Description");
	}

	public override FWF GetItemFWF()
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