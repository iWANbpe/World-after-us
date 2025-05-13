using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	[HideInInspector] public Image invItemImage { get { return GetComponent<Image>(); } }
	[HideInInspector] public bool hasPlace = true;
	[HideInInspector] public string sizeCode;
	public InventoryItemInfo invItemInfo;

	private GameObject canvas;
	public void Initialization()
	{
		canvas = GameObject.Find("Canvas");

		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).GetComponent<InventoryItemCage>().Initialization();
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		hasPlace = false;
		transform.SetAsLastSibling();
		GetComponent<Image>().raycastTarget = false;
		SetChildrenRaycastTarget(false);
		ClearSlots();

		PlayerController.Instance.invItemHoldObject = gameObject;
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
			OccupySlots();
		}

		GetComponent<Image>().raycastTarget = true;
		SetChildrenRaycastTarget(true);
		PlayerController.Instance.invItemHoldObject = null;
	}

	public void ClearSlots()
	{
		ChildCagesCallFunction("ClearSlot", null);
	}

	private void OccupySlots()
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).GetComponent<InventoryItemCage>().OccupySlot();
		}
	}

	private void SetChildrenRaycastTarget(bool statement)
	{
		ChildCagesCallFunction("SetRayCastTarget", FuncParams(statement));
	}

	private void ChildCagesCallFunction(string funcName, object[] funcParams)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject child = transform.GetChild(i).gameObject;
			if (child.GetComponent<InventoryItemCage>())
			{
				MethodInfo method = child.GetComponent<InventoryItemCage>().GetType().GetMethod(funcName);
				method.Invoke(child.GetComponent<InventoryItemCage>(), funcParams);
			}
		}
	}

	private object[] FuncParams(params object[] objects)
	{
		return objects;
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

	public void SetPosition(Vector2 invItemPos)
	{
		transform.position = invItemPos;
		GetComponent<Image>().raycastTarget = true;
		SetChildrenRaycastTarget(true);
		OccupySlots();
	}

	public void SetRotation(float angle)
	{
		transform.localEulerAngles = new Vector3(0f, 0f, angle);
	}

	public void Rotate(float angle)
	{
		transform.localEulerAngles += new Vector3(0f, 0f, angle);
	}

	public void CreateSizeCode()
	{
		GameObject previousCage = null;
		Vector3 deltaBetweenCages;
		int cageRowCount = 0;

		for (int i = 0; i < transform.childCount; i++)
		{
			GameObject child = transform.GetChild(i).gameObject;
			if (previousCage == null)
			{
				cageRowCount += 1;
				previousCage = child;
				continue;
			}

			deltaBetweenCages = child.GetComponent<RectTransform>().localPosition - previousCage.GetComponent<RectTransform>().localPosition;

			if (deltaBetweenCages.y == 0)
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

	public void SetVisibility(bool visibility)
	{
		float alpha = visibility ? 1f : 0f;
		Color invItemColor = GetComponent<Image>().color;
		invItemColor.a = alpha;
		GetComponent<Image>().color = invItemColor;
	}

	public void AddOccupationSlots(List<GameObject> occupationSlots)
	{
		for (int i = 0; i < transform.childCount; i++)
		{
			transform.GetChild(i).GetComponent<InventoryItemCage>().OccupySlot(occupationSlots[i]);
		}
	}
}