using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	[Header("UIMouse")]
	public float doubleClickCoolDown;
	[Header("Pointer")]
	[SerializeField] private float shootPointerPartSpawnPos;
	[SerializeField] private float shootPointerPartNormalPos;
	[SerializeField] private float shootPointerPartMoveSpeed;
	[Header("Objects")]
	[SerializeField] private GameObject playerPanel;
	[SerializeField] private GameObject inventoryPanel;
	[SerializeField] private GameObject infoItemText;
	[SerializeField] private GameObject infoPanel;
	[SerializeField] private GameObject canvas;
	[Header("MessagePanel settings")]
	[SerializeField] private GameObject messagePanel;
	[SerializeField] private Vector2 messageStartCoordinate;
	[SerializeField] private float messageOffScreenDistance;
	[SerializeField] private int maxMessagesCount;
	[SerializeField] private float messageLifetime;
	[SerializeField] private float messageMoveSpeed;
	[SerializeField] private float messageMoveDuration;

	[Header("FWF bars")]
	[SerializeField] private List<GameObject> foodBarList = new List<GameObject>();
	[SerializeField] private List<GameObject> waterBarList = new List<GameObject>();
	[SerializeField] private List<GameObject> filterBarList = new List<GameObject>();

	public bool infoItemTextIsActive { get { return infoItemText.activeSelf; } }

	private Dictionary<string, GameObject> invSubPanels = new Dictionary<string, GameObject>();

	private Image itemImage;
	private TMP_Text itemNameText;
	private TMP_Text itemTypeText;
	private TMP_Text ItemUtilityText;
	private TMP_Text ItemDescriptionText;
	private GameObject shootPointer;

	private List<ShootPointerPart> shootPointerParts = new List<ShootPointerPart>();

	private List<MessagePanel> messageList = new List<MessagePanel>();
	private Queue<MessagePanel> messagePool = new Queue<MessagePanel>();
	private float screenWidth;
	private Vector2 messageSpawnStartCoordinate;

	private Dictionary<UtilityType, List<GameObject>> fwfBarDictionary = new Dictionary<UtilityType, List<GameObject>>();

	public static PlayerUI Instance;
	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}

		invSubPanels.Add(inventoryPanel.transform.Find("InventorySubPanel").name, inventoryPanel.transform.Find("InventorySubPanel").gameObject);

		itemImage = infoPanel.transform.Find("ItemImage").gameObject.GetComponent<Image>();
		itemNameText = infoPanel.transform.Find("ItemBaseInfoPanel").Find("ItemNameText").gameObject.GetComponent<TMP_Text>();
		itemTypeText = infoPanel.transform.Find("ItemBaseInfoPanel").Find("ItemTypeText").gameObject.GetComponent<TMP_Text>();
		ItemUtilityText = infoPanel.transform.Find("ItemUtilityText").gameObject.GetComponent<TMP_Text>();
		ItemDescriptionText = infoPanel.transform.Find("ItemDescriptionText").gameObject.GetComponent<TMP_Text>();

		infoPanel.SetActive(false);

		screenWidth = canvas.GetComponent<RectTransform>().rect.width;
		messageSpawnStartCoordinate = new Vector2(screenWidth + messageOffScreenDistance, messageStartCoordinate.y);

		fwfBarDictionary.Add(UtilityType.Food, foodBarList);
		fwfBarDictionary.Add(UtilityType.Water, waterBarList);
		fwfBarDictionary.Add(UtilityType.Filter, filterBarList);

		shootPointer = playerPanel.transform.Find("Pointer").Find("ShootPointer").gameObject;
		InitializeShootPointer(shootPointer);
	}

	public void EnableInfoItemText(string itemName)
	{
		infoItemText.SetActive(true);
		infoItemText.GetComponent<TMP_Text>().text = itemName;
	}

	public void DisableInfoItemText()
	{
		infoItemText.SetActive(false);
	}

	public void EnableInfoPanel(InventoryItemInfo invItemInfo)
	{
		infoPanel.SetActive(true);
		itemImage.sprite = invItemInfo.invItemImage.sprite;

		itemNameText.text = invItemInfo.itemInfo.GetLocalizedItemName();
		itemTypeText.text = Localization.Instance.GetText("UIStringTable", "typeText") + " " + invItemInfo.itemInfo.type;
		ItemUtilityText.text = invItemInfo.itemInfo.GetItemFWF().GetUtilityText();
		ItemDescriptionText.text = invItemInfo.itemInfo.GetLocalizedItemDescription();
	}

	public void DisableInfoPanel()
	{
		infoPanel.SetActive(false);
	}

	private void InitializeShootPointer(GameObject shootPointer)
	{
		for (int i = 1; i < shootPointer.transform.childCount + 1; i++)
		{
			Transform sPointPart = shootPointer.transform.GetChild(i - 1).gameObject.GetComponent<Transform>();

			float curPointPos = i % 2 == 0 ? -shootPointerPartSpawnPos : shootPointerPartSpawnPos;
			float targetPointPos = i % 2 == 0 ? -shootPointerPartNormalPos : shootPointerPartNormalPos;

			if (i <= 2)
			{
				ShootPointerPart sPointerPart = new ShootPointerPart(sPointPart, new Vector2(sPointPart.localPosition.x, curPointPos), new Vector2(sPointPart.localPosition.x, targetPointPos));
				shootPointerParts.Add(sPointerPart);
			}
			else
			{
				ShootPointerPart sPointerPart = new ShootPointerPart(sPointPart, new Vector2(curPointPos, sPointPart.localPosition.y), new Vector2(targetPointPos, sPointPart.localPosition.y));
				shootPointerParts.Add(sPointerPart);
			}

			shootPointerParts[i - 1].ChangePosition(shootPointerParts[i - 1].curPosition);
		}
	}

	private IEnumerator ShootPointerAnimation()
	{
		float time = 0f;

		while (time < 1)
		{
			foreach (ShootPointerPart shootPointerPart in shootPointerParts)
			{
				shootPointerPart.ChangePosition(Vector2.Lerp(shootPointerPart.curPosition, shootPointerPart.targetPosition, time));
			}
			yield return null;
			time += Time.deltaTime * shootPointerPartMoveSpeed;
		}
	}

	private void ResetShootPointer()
	{
		foreach (ShootPointerPart shootPointerPart in shootPointerParts)
		{
			shootPointerPart.ChangePosition(shootPointerPart.curPosition);
		}
	}

	public void SetShootPointer(bool state)
	{
		shootPointer.SetActive(state);
		if (state) StartCoroutine(ShootPointerAnimation());
		else ResetShootPointer();
	}

	public void PlayerPanelMessage(string messageText)
	{
		if (messageList.Count > 0 && messageList.Count < maxMessagesCount)
		{
			MoveUpMessagesList();
		}
		else if (messageList.Count >= maxMessagesCount)
		{
			MessagePanel oldMessage = messageList[0].GetComponent<MessagePanel>();
			oldMessage.HideMessage();
			MoveUpMessagesList();
		}

		MessagePanel newMessage = SpawnMessagePanel();
		newMessage.SetTextMessage(messageText);
		newMessage.AddMoving(messageStartCoordinate, messageMoveSpeed, messageMoveDuration);
		messageList.Add(newMessage);
	}

	private void MoveUpMessagesList()
	{
		for (int i = 0; i < messageList.Count; i++)
		{
			Vector2 newPos = new Vector2(messageStartCoordinate.x, messageStartCoordinate.y + (messagePanel.GetComponent<RectTransform>().rect.height * (messageList.Count - i)));
			messageList[i].AddMoving(newPos, messageMoveSpeed, messageMoveDuration);
		}
	}

	public void RemoveMessageFromMessageList(MessagePanel messagePanel)
	{
		messageList.Remove(messagePanel);
	}

	public MessagePanel SpawnMessagePanel()
	{
		MessagePanel newMessage;

		if (messagePool.Count > 0)
		{
			newMessage = messagePool.Dequeue();
			newMessage.gameObject.SetActive(true);
			newMessage.gameObject.transform.position = messageSpawnStartCoordinate;
		}

		else
		{
			newMessage = Instantiate(messagePanel, messageSpawnStartCoordinate, Quaternion.identity, playerPanel.transform).GetComponent<MessagePanel>();
			newMessage.disappearancePos.x = screenWidth + messageOffScreenDistance;
		}

		newMessage.SetMessageLifetime(messageLifetime);
		return newMessage;
	}

	public void AddMessageToPool(MessagePanel messagePanel)
	{
		messagePool.Enqueue(messagePanel);
	}

	public void UpdateFWFBars(FWF fwfValue)
	{
		foreach (UtilityPoint uPoint in fwfValue.utilityPoints)
		{
			if (fwfBarDictionary.ContainsKey(uPoint.utilityType))
			{
				for (int i = 0; i < fwfBarDictionary[uPoint.utilityType].Count; i++)
				{
					GameObject bar = fwfBarDictionary[uPoint.utilityType][i];
					bar.GetComponentInChildren<Slider>().value = uPoint.utilityValue / 100f;
				}

			}
		}
	}
}

public class ShootPointerPart
{
	public Transform shootPointerPartTransform;
	public Vector2 curPosition;
	public Vector2 targetPosition;

	public ShootPointerPart(Transform transform, Vector2 curPosition, Vector2 targetPosition)
	{
		shootPointerPartTransform = transform;
		this.curPosition = curPosition;
		this.targetPosition = targetPosition;
	}

	public void ChangePosition(Vector2 newPos)
	{
		shootPointerPartTransform.localPosition = newPos;
	}
}