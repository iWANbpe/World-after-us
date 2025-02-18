using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [HideInInspector]public Dictionary<string, Queue<GameObject>> itemDictionary;

    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
        itemDictionary = new Dictionary<string, Queue<GameObject>>();
    }

    public void SpawnItem(ItemInfo itemInfo, Vector3 position, Quaternion rotation) 
    {
        if (itemDictionary.ContainsKey(itemInfo.itemName) && itemDictionary[itemInfo.itemName].Count > 0) 
        {
            GameObject itemFromDictionary = itemDictionary[itemInfo.itemName].Dequeue();

            itemFromDictionary.SetActive(true);
            itemFromDictionary.transform.position = position;
            itemFromDictionary.transform.rotation = rotation;

            itemFromDictionary.GetComponent<Rigidbody>().velocity = Vector3.zero;
            itemFromDictionary.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        }
        else 
        {
            itemInfo.InstaniateItem(position, rotation);
            
            if (!itemDictionary.ContainsKey(itemInfo.itemName)) 
                itemDictionary.Add(itemInfo.itemName, new Queue<GameObject>());
        }
    }

    public void DespawnItem(ItemInfo itemInfo, GameObject itemObj) 
    {
        itemObj.SetActive(false);

        if (!itemDictionary.ContainsKey(itemInfo.itemName)) 
            itemDictionary.Add(itemInfo.itemName, new Queue<GameObject>());

        itemDictionary[itemInfo.itemName].Enqueue(itemObj);
    }

    public void AddInventoryItem(InventoryItemInfo invItemInfo, Vector2 position) 
    {
        if (itemDictionary.ContainsKey(invItemInfo.invItemName) && itemDictionary[invItemInfo.invItemName].Count > 0)
        {
            GameObject itemFromDictionary = itemDictionary[invItemInfo.invItemName].Dequeue();

            itemFromDictionary.SetActive(true);
            itemFromDictionary.transform.position = position;
        }
        else
        {
            invItemInfo.AddItemToInventory(position);

            if (!itemDictionary.ContainsKey(invItemInfo.invItemName))
                itemDictionary.Add(invItemInfo.invItemName, new Queue<GameObject>());
        }
    }

    public void DeleteInventoryItem(InventoryItemInfo invItemInfo, GameObject invItem) 
    {
        invItem.GetComponent<InventoryItem>().ClearSlots();
        invItem.SetActive(false);

        if (!itemDictionary.ContainsKey(invItemInfo.invItemName))
            itemDictionary.Add(invItemInfo.invItemName, new Queue<GameObject>());

        itemDictionary[invItemInfo.invItemName].Enqueue(invItem);
    }
}
