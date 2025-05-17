using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemInfo")]
public class ItemInfo : ScriptableObject
{
	[Header("Base info")]
	public string itemName;
	public GameObject gameObject;

	public virtual GameObject InstantiateItem(Vector3 pos, Quaternion rotation)
	{
		return Instantiate(gameObject, pos, rotation);
	}

	public virtual string GetLocalizedItemName()
	{
		return Localization.Instance.GetText("ItemStringTable", itemName + "Name");
	}

	public virtual InventoryItemInfo GetInventoryItemInfo() { return null; }
	public virtual string GetLocalizedItemDescription() { return null; }
	public virtual FWF GetItemFWF() { return null; }
}