using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class 
    LanguageManager : MonoBehaviour
{
    public static LanguageManager Instance { get; private set; }

    public string CurrentLanguage { get; private set; } = "en"; // Default language

    private Dictionary<string, Dictionary<string, string>> _translations;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadTranslations(string language)
    {
        _translations = LoadTranslationsFromFile(language); // Load the translations from the file
        CurrentLanguage = language;
        UpdateUI();
    }

    public string GetTranslation(string key)
    {
        if (_translations != null && _translations.ContainsKey(CurrentLanguage) && _translations[CurrentLanguage].ContainsKey(key))
        {
            return _translations[CurrentLanguage][key];
        }
        return key; // Fallback to key if translation is not found
    }

    private void UpdateUI()
    {
        // Notify all UI elements to update their text
        // Implement this based on your UI management
    }

    private Dictionary<string, Dictionary<string, string>> LoadTranslationsFromFile(string language)
    {
        // Load the JSON file
        TextAsset jsonFile = Resources.Load<TextAsset>("languages");
        if (jsonFile == null)
        {
            Debug.LogError("Language file not found!");
            return new Dictionary<string, Dictionary<string, string>>();
        }

        // Deserialize JSON
        var allTranslations = JsonUtility.FromJson<TranslationsWrapper>(jsonFile.text);
        return allTranslations.translations;
    }

    [System.Serializable]
    private class TranslationsWrapper
    {
        public Dictionary<string, Dictionary<string, string>> translations;
    }
}
