using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/InventoryItemInfo")]
public class InventoryItemInfo : ScriptableObject
{
    public ItemInfo itemInfo;
    [SerializeField] private GameObject inventoryItem;
}
