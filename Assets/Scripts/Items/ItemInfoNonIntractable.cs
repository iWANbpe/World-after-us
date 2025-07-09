using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/ItemInfoNonIntractable")]
public class ItemInfoNonIntractable : ItemInfo
{
	public override GameObject InstantiateItem(Vector3 pos, Quaternion rotation)
	{
		GameObject itemObj = Instantiate(item.gameObject, pos, rotation);

		if (itemObj.GetComponent<Item>().itemInfo != this)
			itemObj.GetComponent<Item>().itemInfo = this;

		return itemObj;
	}
}