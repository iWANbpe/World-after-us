using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryItemCage : MonoBehaviour
{
    [HideInInspector] public EventSystem eventSystem;
    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private GameObject occupiedSlot;
    public Vector3 avaliableInventorySlotPosition { get{ return occupiedSlot.transform.position; } }

    private void Awake()
    {
        raycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
        eventSystem = GameObject.Find("/EventSystem").GetComponent<EventSystem>();
    }

    public void SetRayCastTarget(bool statement)
    {
        GetComponent<Image>().raycastTarget = statement;
    }

    public void ClearSlot() 
    {
        if (occupiedSlot) 
        {
            occupiedSlot.GetComponent<InventorySlot>().isOccupied = false;
            occupiedSlot = null;
        }
        
    }

    public bool IsAboveFreeSlot() 
    {
        List<RaycastResult> resultsList = new List<RaycastResult>();
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = transform.position;

        raycaster.Raycast(pointerEventData, resultsList);

        foreach(RaycastResult result in resultsList) 
        {
            if (result.gameObject.GetComponent<InventorySlot>() && result.gameObject.GetComponent<InventorySlot>().isOccupied == false) 
            {
                result.gameObject.GetComponent<InventorySlot>().isOccupied = true;
                occupiedSlot = result.gameObject;
                return true;
            }
        }

        return false;
    }
}
