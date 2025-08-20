using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationTable", menuName = "Localization/Table")]
public class LocalizationTable : ScriptableObject
{
    // Languages in the table (first is the source/default, e.g., "en")
    public List<string> Languages = new() { "en" };

    // One row per key
    public List<Row> Rows = new();

    [Serializable]
    public class Row
    {
        public string Key;
        // Same length/order as Languages
        public List<string> Values = new();
    }

    public int IndexOfLang(string lang) => Languages.FindIndex(l => string.Equals(l, lang, StringComparison.OrdinalIgnoreCase));

    public string Get(string key, string lang)
    {
        int li = IndexOfLang(lang);
        if (li < 0) return null;
        var row = Rows.Find(r => string.Equals(r.Key, key, StringComparison.OrdinalIgnoreCase));
        if (row == null || row.Values.Count <= li) return null;
        return row.Values[li];
    }

    public void EnsureLanguage(string lang)
    {
        int li = IndexOfLang(lang);
        if (li >= 0) return;
        Languages.Add(lang);
        foreach (var r in Rows) r.Values.Add(string.Empty);
    }

    public void Upsert(string key, string defaultText, string defaultLang)
    {
        EnsureLanguage(defaultLang);
        int defIdx = IndexOfLang(defaultLang);

        var row = Rows.Find(r => r.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
        if (row == null)
        {
            row = new Row { Key = key, Values = new List<string>(new string[Languages.Count]) };
            Rows.Add(row);
        }
        while (row.Values.Count < Languages.Count) row.Values.Add(string.Empty);
        if (string.IsNullOrEmpty(row.Values[defIdx])) row.Values[defIdx] = defaultText ?? string.Empty;
    }
}
