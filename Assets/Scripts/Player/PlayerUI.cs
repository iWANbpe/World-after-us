using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject infoItemText;
    
    public void EnableInfoItemText(string itemName) 
    {
        infoItemText.SetActive(true);
        infoItemText.GetComponent<TMP_Text>().text = itemName;
    }

    public void DisableInfoItemText() 
    { 
        infoItemText.SetActive(false);
    }
}
