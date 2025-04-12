using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryControll : MonoBehaviour
{
    [SerializeField] private int slotsPanelWidth;
    [SerializeField] private int slotsPanelHeight;

    private GameObject inventory;
    private GameObject slotsPanel;
    private GameObject inventoryWarningPanel;
    private GameObject player;

    public List<GameObject> invItemList = new List<GameObject>();

    private GameObject [] inventorySlots;
    private List<GameObject> freeInventorySlots = new List<GameObject>();
    
    public static InventoryControll Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance == this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        inventoryWarningPanel = GameObject.Find("Canvas").transform.Find("Inventory").transform.Find("WarningPanel").gameObject;

        inventory = GameObject.Find("Canvas").transform.Find("Inventory").gameObject;
        slotsPanel = inventory.transform.Find("SlotsPanel").gameObject;
        player = GameObject.Find("Player").gameObject;

        inventoryWarningPanel.transform.Find("WarningPanelYesButton").GetComponent<Button>().onClick.AddListener(AproveItemThrowing);
        inventoryWarningPanel.transform.Find("WarningPanelNoButton").GetComponent<Button>().onClick.AddListener(CancelItemThrowing);
        AddSlots();
    }

    private void Start()
    {
        WarningPanelSetActivity(false);
    }

    private void AddSlots() 
    {
        List<GameObject> invSlotsList = new List<GameObject>();

        for (int i = 0; i < slotsPanel.transform.childCount; i++)
        {
            invSlotsList.Add(slotsPanel.transform.GetChild(i).gameObject); 
        }

        inventorySlots = invSlotsList.ToArray();
    }
    private void FindFreeSlots() 
    {
        freeInventorySlots.Clear();

        foreach(GameObject slot in inventorySlots) 
        {
            if (!slot.GetComponent<InventorySlot>().isOccupied) freeInventorySlots.Add(slot);
        }
    }

    public bool IsFreeSpaceForItem(GameObject invItem, out Vector2 invItemPlace) 
    {
        FindFreeSlots();
        invItemPlace = Vector2.zero;
        if (freeInventorySlots.Count == 0) return false;

        if (invItem.GetComponent<InventoryItem>().sizeCode == "")
            invItem.GetComponent<InventoryItem>().CreateSizeCode();

        string sizeCode = invItem.GetComponent<InventoryItem>().sizeCode;
        int rows = sizeCode.Length;
        char[] columns = sizeCode.ToCharArray();
        List<GameObject> placeSlots = new List<GameObject>();
        for(int slotIndex = 0; slotIndex < freeInventorySlots.Count; slotIndex++) 
        {
            int GlobalSlotIndex = System.Array.IndexOf(inventorySlots, freeInventorySlots[slotIndex]);

            placeSlots = TryPlace(GlobalSlotIndex, rows, columns);
            
            if(placeSlots != null) 
            {
                OccupySlots(placeSlots);
                invItem.GetComponent<InventoryItem>().AddOccupationSlots(placeSlots);

                invItemPlace = FindCenter(placeSlots);
                return true;
            }
        }

        return false;
    }
    
    private List<GameObject> TryPlace(int GlobalSlotIndex, int rows, char[] columns) 
    {
        List<GameObject> slotList = new List<GameObject>();

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < int.Parse(columns[row].ToString()); column++)
            {
                if (GlobalSlotIndex + column + (row * slotsPanelWidth) >= inventorySlots.Length || inventorySlots[GlobalSlotIndex + column + (row * slotsPanelWidth)].GetComponent<InventorySlot>().isOccupied)
                {
                    return null;
                }

                slotList.Add(inventorySlots[GlobalSlotIndex + column + (row * slotsPanelWidth)]);
            }
        }

        return slotList;
    }

    private void OccupySlots(List<GameObject> slotList) 
    { 
        for(int i = 0; i < slotList.Count; i++) 
        {
            freeInventorySlots.Remove(slotList[i]);
        }
    }

    private Vector2 FindCenter(List<GameObject> slotList) 
    {
        float medianX = 0, medianY = 0;
        int slotsCount;

        for (slotsCount = 0; slotsCount < slotList.Count; slotsCount++)
        {
            medianX += slotList[slotsCount].transform.position.x;
            medianY += slotList[slotsCount].transform.position.y;
        }

        medianX /= slotsCount;
        medianY /= slotsCount;


        return new Vector2(medianX, medianY);
    }

    public void WarningPanelSetActivity(bool activity) 
    {
        inventoryWarningPanel.SetActive(activity);
    }

    public bool WarningPanelActivity() 
    {
        return inventoryWarningPanel.activeSelf;
    }
    private void AproveItemThrowing() 
    {
        WarningPanelSetActivity(false);

        List<GameObject> fullInvItemList = new List<GameObject>(invItemList);

        foreach (GameObject invItem in fullInvItemList)
        {
            if (!invItem.GetComponent<InventoryItem>().hasPlace)
            {
                ObjectPooler.Instance.SpawnItem(invItem.GetComponent<InventoryItem>().invItemInfo.itemInfo, player.GetComponent<PlayerController>().playerGrabPoint.transform.position, Quaternion.identity);
                ObjectPooler.Instance.DeleteInventoryItem(invItem.GetComponent<InventoryItem>().invItemInfo, invItem);
            }
        }

        player.GetComponent<PlayerController>().InventoryChangeStatement(false);
    }

    private void CancelItemThrowing() 
    {
        WarningPanelSetActivity(false);
    }

    public bool AllItemsHavePlace() 
    { 
        foreach(GameObject invItem in invItemList) 
        {
            if (!invItem.GetComponent<InventoryItem>().hasPlace) 
            {
                return false;
            }
        }
        return true;
    }
}
