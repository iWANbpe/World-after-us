using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public bool hasePlace = true;
    public InventoryItemInfo invItemInfo;

    public void OnBeginDrag(PointerEventData eventData) 
    {
        hasePlace = false;
        GetComponent<Image>().raycastTarget = false;
        SetChildrenRaycastTarget(false);
        ClearSlots();
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData) 
    {
        if (AvailableSpace())
        {
            transform.position = NewPosition();
            hasePlace = true;
        }

        GetComponent<Image>().raycastTarget = true;
        SetChildrenRaycastTarget(true);
    }

    private void ClearSlots() 
    { 
        for(int i = 0; i < transform.childCount; i++) 
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.GetComponent<InventoryItemCage>()) 
            {
                child.GetComponent<InventoryItemCage>().ClearSlot();
            }
        }
    }

    private void SetChildrenRaycastTarget(bool statement) 
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.GetComponent<InventoryItemCage>())
            {
                child.GetComponent<InventoryItemCage>().SetRayCastTarget(statement);
            }
        }
    }

    private bool AvailableSpace() 
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.GetComponent<InventoryItemCage>() && !child.GetComponent<InventoryItemCage>().IsAboveFreeSlot())
            {
                return false;
            }
        }

        return true;
    }

    private Vector2 NewPosition() 
    {
        float medianX = 0, medianY = 0;
        int slotsCount = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (child.GetComponent<InventoryItemCage>())
            {
                medianX += child.GetComponent<InventoryItemCage>().avaliableInventorySlotPosition.x;
                medianY += child.GetComponent<InventoryItemCage>().avaliableInventorySlotPosition.y;
                slotsCount++;
            }
        }

        medianX /= slotsCount;
        medianY /= slotsCount;

        return new Vector2(medianX, medianY);
    }
}
