using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItemCage : MonoBehaviour
{
	[HideInInspector] public EventSystem eventSystem;
	private GraphicRaycaster raycaster;
	private PointerEventData pointerEventData;
	private GameObject occupiedSlot;
	public Vector3 avaliableInventorySlotPosition { get { return occupiedSlot.transform.position; } }

	private List<RaycastResult> resultsList = new List<RaycastResult>();
	public void Initialization()
	{
		raycaster = GameObject.Find("Canvas").GetComponent<GraphicRaycaster>();
		eventSystem = GameObject.Find("/EventSystem").GetComponent<EventSystem>();
	}

	public void SetRayCastTarget(bool statement)
	{
		GetComponent<Image>().raycastTarget = statement;
	}

	private void UpdatePointerEventData()
	{
		pointerEventData = new PointerEventData(eventSystem);
		pointerEventData.position = transform.position;

		resultsList = new List<RaycastResult>();
		raycaster.Raycast(pointerEventData, resultsList);
	}

	public void ClearSlot()
	{
		if (occupiedSlot)
		{
			occupiedSlot.GetComponent<InventorySlot>().isOccupied = false;
			occupiedSlot = null;
		}

	}

	public void OccupySlot()
	{
		if (occupiedSlot != null)
		{
			occupiedSlot.GetComponent<InventorySlot>().isOccupied = true;
		}
	}

	public void OccupySlot(GameObject slot)
	{
		occupiedSlot = slot;
		occupiedSlot.GetComponent<InventorySlot>().isOccupied = true;
	}

	public bool IsAboveFreeSlot()
	{
		UpdatePointerEventData();

		foreach (RaycastResult result in resultsList)
		{
			if (result.gameObject.GetComponent<InventorySlot>() && result.gameObject.GetComponent<InventorySlot>().isOccupied == false)
			{
				occupiedSlot = result.gameObject;
				return true;
			}
		}

		return false;
	}
}