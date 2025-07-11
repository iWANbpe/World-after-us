using UnityEngine;

public abstract class ItemInfo : ScriptableObject
{
	[Header("Base info")]
	public string itemName;
	public Item item;
	public abstract GameObject InstantiateItem(Vector3 pos, Quaternion rotation);
}