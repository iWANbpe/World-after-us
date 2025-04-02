using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject playerPanel;
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
    [SerializeField] private GameObject foodBar;
    [SerializeField] private GameObject waterBar;
    [SerializeField] private GameObject filterBar;
    public bool infoItemTextIsActive { get{ return infoItemText.activeSelf; } }

    private Image itemImage;
    private TMP_Text itemNameText;
    private TMP_Text itemTypeText;
    private TMP_Text itemEffectsText;
    private TMP_Text ItemDescriptionText;

    private List<MessagePanel> messageList = new List<MessagePanel>();
    private Queue<MessagePanel> messagePool = new Queue<MessagePanel>();
    private float screenWidth;
    private Vector2 messageSpawnStartCoordinate;

    private Dictionary<UtilityType, GameObject> fwfBarDictionary = new Dictionary<UtilityType, GameObject>();
    private void Awake()
    {
        itemImage = infoPanel.transform.Find("ItemImage").gameObject.GetComponent<Image>();
        itemNameText = infoPanel.transform.Find("ItemInfoPanel").Find("ItemNameText").gameObject.GetComponent<TMP_Text>();
        itemTypeText = infoPanel.transform.Find("ItemInfoPanel").Find("ItemTypeText").gameObject.GetComponent<TMP_Text>();
        itemEffectsText = infoPanel.transform.Find("ItemInfoPanel").Find("ItemEffectsText").gameObject.GetComponent<TMP_Text>();
        ItemDescriptionText = infoPanel.transform.Find("ItemDescriptionText").gameObject.GetComponent<TMP_Text>();
        
        infoPanel.SetActive(false);
        
        screenWidth = canvas.GetComponent<RectTransform>().rect.width;
        messageSpawnStartCoordinate = new Vector2(screenWidth + messageOffScreenDistance, messageStartCoordinate.y);

        fwfBarDictionary.Add(UtilityType.Food, foodBar);
        fwfBarDictionary.Add(UtilityType.Water, waterBar);
        fwfBarDictionary.Add(UtilityType.Filter, filterBar);
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
        itemEffectsText.text = invItemInfo.itemInfo.itemUtility.GetUtilityText();
        ItemDescriptionText.text = invItemInfo.itemInfo.GetLocalizedItemDescription();
    }

    public void DisableInfoPanel() 
    {
        infoPanel.SetActive(false);
    }

    public void PlayerPanelMessage(string messageText) 
    {
        if(messageList.Count > 0 && messageList.Count < maxMessagesCount) 
        {
            MoveUpMessagesList();
        }
        else if(messageList.Count >= maxMessagesCount) 
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
        foreach(UtilityPoint uPoint in fwfValue.utilityPoints) 
        {
            if (fwfBarDictionary.ContainsKey(uPoint.utilityType)) 
            {
                GameObject bar = fwfBarDictionary[uPoint.utilityType];
                bar.GetComponentInChildren<Slider>().value = uPoint.utilityValue / 100f;
            }
        }
    }
}
