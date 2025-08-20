# Unity Localization Tool

A tiny, no-frills localization pipeline for Unity UI (TextMeshProUGUI) and world-space TMP.

It scans your project for texts, assigns stable keys, round-trips through a CSV, and exports one JSON per language for runtime lookup.

**Download:** grab the prebuilt Unity package from the project’s GitHub [Releases](https://github.com/) page and import it into your project.

---

## 🔹 Highlights

### Editor Window (Tools → Localization)
- Scan Scenes and Prefabs under `Assets/` and auto-attach `LocalizedText` with a stable key (no `Packages/` popups).
- **Export CSV** → translators fill it.
- **Import CSV (overwrite)** → replaces the table with exactly what’s in your CSV (supports multiline, quotes, `<color>` tags).
- **Export JSON per language (raw)** → no fallback, no filtering, empty CSV cells remain empty in JSON so you can spot gaps.

<img width="454" height="605" alt="image" src="https://github.com/user-attachments/assets/b9f8e8b2-6835-43ee-8f0f-493672421d87" />


### Runtime
- `LocalizationManager` loads `Resources/Localization/<lang>.json`.
- `LocalizedText` listens for language changes and updates UI automatically.
- **TMP extras**: auto-size + left/right padding built into `LocalizedText` (for nice buttons).

---

## 📄 Format

### CSV Header:
```
key,en,ro,fr,de,es,it,ja,ru
```

### JSON:
```json
{
  "items": [
    { "key": "mainmenu.play", "value": "JOUER" }
  ]
}
```

---

## ✅ Requirements

- Unity 2019.4+ (tested on 2020/2021/2022/2023)
- TextMeshPro (Unity package)
- Newtonsoft.Json – install via Package Manager:

```
Name: Newtonsoft Json
ID: com.unity.nuget.newtonsoft-json
```

---

## 📦 Installation (from Release package)

1. Download the `.unitypackage` from Releases.
2. In Unity: `Assets → Import Package → Custom Package…` and import all.

### You’ll get:
- `Editor/LocalizationEditorWindow.cs`
- `Runtime/LocalizationManager.cs`
- `Runtime/LocalizedText.cs` (with TMP autosize & padding)
- `Resources/Localization/` (created on export)

---

## 🚀 Quick Start (Workflow)

### Scan & Key
- Open **Tools → Localization**
- Choose what to scan (Scenes/Prefabs)
- Click **Scan & Assign Keys**
- The tool adds `LocalizedText` components and fills their **Key** (Fallback uses the existing text).

### Export CSV
- Click **Export CSV** → produces `localization.csv`
- Open in your spreadsheet editor; **keep the first column (`key`) untouched**

### Translate
- Fill language columns (e.g., ro, fr, de, es, it, ja, ru)
- Use quotes for multiline cells; tags like `<color=#xxxxxx>` are fine

### Import CSV (Overwrite)
- Back in the window: click **Import CSV → Update Table**
- This **wipes and replaces** the table values for the languages present in your CSV header

### Export JSONs (Raw)
- Click **Export JSON per Language**
- Files written to `Assets/Resources/Localization/<lang>.json`
- Empty CSV cells remain `""` in JSON

### Load a language at runtime
```csharp
// run this once (e.g., splash/menu)
LocalizationManager.Instance.LoadLanguage("fr"); // "en", "ro", "de", "es", "it", "ja", "ru"
```

---

## 🧩 Using `LocalizedText` (runtime component)

Attach to any **TextMeshProUGUI** (UI) or **TextMeshPro** (3D). Set:

- `Key` – must match a key in your JSON
- `Fallback` – editor/backup text shown if the key is missing

### TMP quality-of-life options (inspector):
- Enable TMP Auto Size: on/off
- Min / Max Size: autosize range
- Left / Right Padding: uses TMP margins to avoid edge hugging

> For UI, use `TextMeshProUGUI` (CanvasRenderer).  
> For 3D/world text, use `TextMeshPro` (MeshRenderer + MeshFilter).  
> UI texts will never have a Mesh Filter by design.

---

## 📐 CSV Rules

Header must start with `key`. Other columns are treated as languages.

### Example:
```
key,en,fr
gamescene.tutorial,"Line 1\nLine 2","Ligne 1\nLigne 2"
```

Nothing is filtered: numbers, “Menu”, “Settings”, emoji, `<color>` tags—all exported as-is.

---

## 📁 JSON Structure

Example file path:
```
Assets/Resources/Localization/fr.json
```

### Example content:
```json
{
  "items": [
    { "key": "mainmenu._ui_canvas_main_buttoncontainer_play_text_tmp_", "value": "JOUER" },
    { "key": "mainmenu._ui_canvas_main_buttoncontainer_settings_text_tmp_", "value": "PARAMÈTRES" },
    { "key": "mainmenu._ui_canvas_main_buttoncontainer_exit_text_tmp_", "value": "SORTIE" }
  ]
}
```

---

## 🛠 Best Practices

- Script Execution Order: set `LocalizationManager` earlier than your UI (e.g., -100)
- Only scan `Assets/` to avoid read-only package popups
- Keep keys stable; avoid renaming after shipping

---

## 🧯 Troubleshooting

### ❌ Texts remain English
Did you call `LoadLanguage("<lang>")` at runtime? Or maybe the key is missing in the JSON?

### ❓ JSON shows `[xx translation of '...']`
These were placeholder values. Re-import the CSV in overwrite mode and re-export.

### 📏 CSV import issues
Use the built-in importer – it handles multiline/quoted cells. Make sure your spreadsheet program does too.

### 🔧 Missing Mesh Filter
UI uses `TextMeshProUGUI` – no Mesh Filter by design. Use `TextMeshPro` for 3D.

### 🚫 Read-only popup
Make sure you only scan scenes/prefabs under `Assets/`.

---

## 📎 Dependencies

- **TextMeshPro** (Unity Package)
- **Newtonsoft.Json** (`com.unity.nuget.newtonsoft-json`) – used for reading/writing JSON
