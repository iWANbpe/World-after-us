using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject infoItemText;
    [SerializeField] private GameObject infoPanel;
    public bool infoItemTextIsActive { get{ return infoItemText.activeSelf; } }

    private Image itemImage;
    private TMP_Text itemNameText;
    private TMP_Text itemTypeText;
    private TMP_Text itemEffectsText;
    private TMP_Text ItemDescriptionText;

    private void Awake()
    {
        itemImage = infoPanel.transform.Find("ItemImage").gameObject.GetComponent<Image>();
        itemNameText = infoPanel.transform.Find("ItemInfoPanel").Find("ItemNameText").gameObject.GetComponent<TMP_Text>();
        itemTypeText = infoPanel.transform.Find("ItemInfoPanel").Find("ItemTypeText").gameObject.GetComponent<TMP_Text>();
        itemEffectsText = infoPanel.transform.Find("ItemInfoPanel").Find("ItemEffectsText").gameObject.GetComponent<TMP_Text>();
        ItemDescriptionText = infoPanel.transform.Find("ItemDescriptionText").gameObject.GetComponent<TMP_Text>();
        
        infoPanel.SetActive(false);
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
}
