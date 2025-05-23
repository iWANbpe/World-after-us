using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemInfo")]
public class ItemInfo : ScriptableObject
{
	[Header("Base info")]
	public string itemName;
	public Item item;

	public virtual GameObject InstantiateItem(Vector3 pos, Quaternion rotation)
	{
		GameObject itemObj = Instantiate(item.gameObject, pos, rotation);

		if (itemObj.GetComponent<Item>().itemInfo != this)
			itemObj.GetComponent<Item>().itemInfo = this;

		return itemObj;
	}

	public virtual string GetLocalizedItemName()
	{
		return Localization.Instance.GetText("ItemStringTable", itemName + "Name");
	}

	public virtual InventoryItemInfo GetInventoryItemInfo() { return null; }
	public virtual string GetLocalizedItemDescription() { return null; }
	public virtual FWF GetItemFWF() { return null; }
}