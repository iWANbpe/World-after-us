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
    [SerializeField] private int maxMessagesCount;
    [SerializeField] private float messageSpeed;
    [SerializeField] private float messageOffScreenDistance;
    public bool infoItemTextIsActive { get{ return infoItemText.activeSelf; } }

    private Image itemImage;
    private TMP_Text itemNameText;
    private TMP_Text itemTypeText;
    private TMP_Text itemEffectsText;
    private TMP_Text ItemDescriptionText;

    private List<GameObject> messageList = new List<GameObject>();
    private float screenWidth;
    private Vector2 messageSpawnStartCoordinate;
    private void Awake()
    {
        itemImage = infoPanel.transform.Find("ItemImage").gameObject.GetComponent<Image>();
        itemNameText = infoPanel.transform.Find("ItemInfoPanel").Find("ItemNameText").gameObject.GetComponent<TMP_Text>();
        itemTypeText = infoPanel.transform.Find("ItemInfoPanel").Find("ItemTypeText").gameObject.GetComponent<TMP_Text>();
        itemEffectsText = infoPanel.transform.Find("ItemInfoPanel").Find("ItemEffectsText").gameObject.GetComponent<TMP_Text>();
        ItemDescriptionText = infoPanel.transform.Find("ItemDescriptionText").gameObject.GetComponent<TMP_Text>();
        
        infoPanel.SetActive(false);
        
        screenWidth = canvas.GetComponent<RectTransform>().rect.width;
        messageSpawnStartCoordinate = new Vector2(messageStartCoordinate.x + screenWidth + messageOffScreenDistance, messageStartCoordinate.y);
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
        itemNameText.text = invItemInfo.itemInfo.itemName;
        itemTypeText.text = "Type: " + invItemInfo.itemInfo.type;
        ItemDescriptionText.text = invItemInfo.itemInfo.itemDescription;
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
            GameObject oldMessage = messageList[0];
            Vector2 oldMessageNewPos = new Vector2(oldMessage.transform.position.x + screenWidth + messageOffScreenDistance, oldMessage.transform.position.y);
            
            messageList.Remove(oldMessage);
            oldMessage.GetComponent<MessagePanel>().targetPos = oldMessageNewPos;
            MoveUpMessagesList();
        }

        GameObject newMessage = Instantiate(messagePanel, messageSpawnStartCoordinate, Quaternion.identity, playerPanel.transform);
        newMessage.GetComponent<MessagePanel>().SetTextMessage(messageText);
        newMessage.GetComponent<MessagePanel>().moveSpeed = messageSpeed;
        newMessage.GetComponent<MessagePanel>().targetPos = messageStartCoordinate;
        messageList.Add(newMessage);
    }

    private void MoveUpMessagesList() 
    {
        for (int i = 0; i < messageList.Count; i++) 
        {
            Vector2 newPos = new Vector2(messageStartCoordinate.x, messageStartCoordinate.y + (messagePanel.GetComponent<RectTransform>().rect.height * (messageList.Count - i)));
            
            messageList[i].GetComponent<MessagePanel>().moveSpeed = messageSpeed;
            messageList[i].GetComponent<MessagePanel>().targetPos = newPos;
        }
    }
}
