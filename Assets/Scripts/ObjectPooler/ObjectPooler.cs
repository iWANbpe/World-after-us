using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [HideInInspector]public Dictionary<string, Queue<GameObject>> itemDictionary;

    public static ObjectPooler Instance;
    [HideInInspector] public GameObject canvas;
 
    private void Awake()
    {
        if(Instance == null) 
        { 
            Instance = this;
        }
        else if(Instance == this) 
        {
            Destroy(gameObject);
        }

        canvas = GameObject.Find("Canvas");
        itemDictionary = new Dictionary<string, Queue<GameObject>>();
        DontDestroyOnLoad(gameObject);
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
            itemInfo.InstantiateItem(position, rotation);
            
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
    public GameObject InitializeInventoryItem(InventoryItemInfo invItemInfo) 
    {
        GameObject invItem;

        if (itemDictionary.ContainsKey(invItemInfo.invItemName) && itemDictionary[invItemInfo.invItemName].Count > 0)
        {
            invItem = itemDictionary[invItemInfo.invItemName].Dequeue();
            invItem.SetActive(true);
        }
        else
        {
            invItem = invItemInfo.AddItemToInventory(Vector3.zero, canvas.transform);

            if (!itemDictionary.ContainsKey(invItemInfo.invItemName))
                itemDictionary.Add(invItemInfo.invItemName, new Queue<GameObject>());
        }

        invItem.GetComponent<InventoryItem>().SetVisibility(false);
        return invItem;
    }

    public void AddInventoryItem(GameObject invItem, Vector2 position) 
    {
        GameObject inventory = GameObject.Find("Canvas").transform.Find("Inventory").gameObject;

        invItem.transform.SetParent(inventory.transform);
        invItem.GetComponent<InventoryItem>().SetVisibility(true);
        invItem.GetComponent<InventoryItem>().SetPosition(position);
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
