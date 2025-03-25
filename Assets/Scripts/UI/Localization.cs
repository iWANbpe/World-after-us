using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class Localization : MonoBehaviour
{
    [SerializeField] private int languageId;

    public static Localization Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance == this)
        {
            Destroy(gameObject);
        }

        SetLanguage(languageId);
    }

    IEnumerator SetLanguage(int languageId) 
    {
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[languageId];
    }

    public string GetText(string tableName, string key) 
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString(tableName, key);
    }
}
