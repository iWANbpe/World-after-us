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

	public virtual bool CanInteract()
	{
		return false;
	}

	public virtual void ItemInteraction() { }
	public virtual InventoryItemInfo GetInventoryItemInfo() { return null; }
	public virtual string GetLocalizedItemName() { return null; }
	public virtual string GetLocalizedItemDescription() { return null; }
	public virtual FWF GetItemFWF() { return null; }
}
