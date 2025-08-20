using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalizationManager : MonoBehaviour
{
    public static LocalizationManager Instance { get; private set; }
    public string CurrentLanguage = "en"; // default
    public event Action OnLanguageChanged;

    private Dictionary<string, string> _table = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    [Serializable]
    public class KV
    {
        public string key;
        public string value;
    }

    [Serializable]
    public class SerializableDict
    {
        public List<KV> items = new List<KV>();
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        LoadLanguage(CurrentLanguage);
    }

    public void LoadLanguage(string lang)
    {
        CurrentLanguage = lang;
        _table.Clear();

        var textAsset = Resources.Load<TextAsset>($"Localization/{lang}");
        if (textAsset != null)
        {
            var dict = JsonConvert.DeserializeObject<SerializableDict>(textAsset.text);
            if (dict != null && dict.items != null)
            {
                foreach (var it in dict.items)
                    _table[it.key] = it.value;
            }
        }

        OnLanguageChanged?.Invoke();
    }

    public bool TryGet(string key, out string value) => _table.TryGetValue(key, out value);

    public static string ToJson(SerializableDict dict, bool pretty = true)
    {
        return JsonConvert.SerializeObject(dict, pretty ? Formatting.Indented : Formatting.None);
    }

    public static SerializableDict FromJson(string json)
    {
        return JsonConvert.DeserializeObject<SerializableDict>(json);
    }
}
