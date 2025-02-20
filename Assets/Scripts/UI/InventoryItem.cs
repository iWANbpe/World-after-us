using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [HideInInspector] public Image invItemImage { get { return GetComponent<Image>(); } }
    [HideInInspector] public bool hasPlace = true;
    [HideInInspector] public string sizeCode;
    public InventoryItemInfo invItemInfo;

    private void Awake()
    {
        StartCoroutine(SizeCodeCoroutine());
    }

    private void Update()
    {
        print(sizeCode);
    }
    public void OnBeginDrag(PointerEventData eventData) 
    {
        hasPlace = false;
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
            hasPlace = true;
        }

        GetComponent<Image>().raycastTarget = true;
        SetChildrenRaycastTarget(true);
    }

    public void ClearSlots() 
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
        int slotsCount;

        for (slotsCount = 0; slotsCount < transform.childCount; slotsCount++)
        {
            GameObject child = transform.GetChild(slotsCount).gameObject;
            if (child.GetComponent<InventoryItemCage>())
            {
                medianX += child.GetComponent<InventoryItemCage>().avaliableInventorySlotPosition.x;
                medianY += child.GetComponent<InventoryItemCage>().avaliableInventorySlotPosition.y;
            }
        }

        medianX /= slotsCount;
        medianY /= slotsCount;

        return new Vector2(medianX, medianY);
    }

    private void CreateSizeCode() 
    {
        GameObject previousCage = null;
        Vector3 deltaBetweenCages;
        int cageRowCount = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;

            if(previousCage == null) 
            {
                cageRowCount += 1;
                previousCage = child;
                continue;
            }

            deltaBetweenCages = child.GetComponent<RectTransform>().localPosition - previousCage.GetComponent<RectTransform>().localPosition;

            if(deltaBetweenCages.y == 0) 
            {
                cageRowCount += 1;
            }

            else 
            {
                sizeCode += cageRowCount;
                cageRowCount = 1;
            }
            previousCage = child;
        }

        sizeCode += cageRowCount;
    }

    private IEnumerator SizeCodeCoroutine() 
    {
        yield return new WaitForEndOfFrame();
        CreateSizeCode();
    }
}
