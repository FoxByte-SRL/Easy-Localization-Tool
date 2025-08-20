using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[ExecuteAlways]
public class LocalizedText : MonoBehaviour
{
    [SerializeField] public string Key;
    [TextArea] [SerializeField] private string _fallback;

    // ───── TMP auto-size options ─────
    [Header("TMP Auto Size")]
    [SerializeField] private bool _enableTmpAutoSize = true;
    [SerializeField] private float _tmpMinSize = 18f;
    [SerializeField] private float _tmpMaxSize = 120f;

    // ───── Horizontal bounds (padding inside the label) ─────
    [Header("Horizontal Bounds (Padding)")]
    [SerializeField] private float _leftPadding = 12f;
    [SerializeField] private float _rightPadding = 12f;

    TMP_Text _tmp;
    Text _uiText;

    void OnEnable()
    {
        Cache();
        ApplyBounds();        // apply both in Edit & Play
        ApplyTmpAutoSize();

        if (Application.isPlaying)
            StartCoroutine(EnsureSubscribedThenApply());
        else
            Apply();          // editor fallback preview
    }

    void OnDisable()
    {
        if (!Application.isPlaying) return;
        var mgr = LocalizationManager.Instance;
        if (mgr != null) mgr.OnLanguageChanged -= Apply;
    }

    void OnValidate()
    {
        Cache();
        ApplyBounds();
        ApplyTmpAutoSize();
        if (!Application.isPlaying) Apply();
    }

    void Cache()
    {
        if (_tmp == null) _tmp = GetComponent<TMP_Text>();
        if (_uiText == null) _uiText = GetComponent<Text>();
    }

    IEnumerator EnsureSubscribedThenApply()
    {
        while (LocalizationManager.Instance == null) yield return null;
        var mgr = LocalizationManager.Instance;
        mgr.OnLanguageChanged -= Apply;
        mgr.OnLanguageChanged += Apply;
        Apply();
    }

    public void SetFallbackForEditor(string text) => _fallback = text;

    public void Apply()
    {
        string value = null;

        if (Application.isPlaying && LocalizationManager.Instance != null && !string.IsNullOrEmpty(Key))
            LocalizationManager.Instance.TryGet(Key, out value);

        var finalText = string.IsNullOrEmpty(value) ? _fallback : value;

        if (_tmp != null)
        {
            _tmp.text = finalText;
            ApplyBounds();
            ApplyTmpAutoSize();
            _tmp.ForceMeshUpdate(true);
        }
        else if (_uiText != null)
        {
            _uiText.text = finalText;
        }
    }

    // ─────────────────────────────────
    void ApplyTmpAutoSize()
    {
        if (_tmp == null) return;

        _tmp.enableAutoSizing = _enableTmpAutoSize;
        if (_enableTmpAutoSize)
        {
            if (_tmpMinSize <= 0f) _tmpMinSize = 1f;
            if (_tmpMaxSize < _tmpMinSize) _tmpMaxSize = _tmpMinSize;

            _tmp.fontSizeMin = _tmpMinSize;
            _tmp.fontSizeMax = _tmpMaxSize;
        }

        // sensible defaults for fitting inside a button
        _tmp.enableWordWrapping = true;
        if (_tmp.overflowMode == TextOverflowModes.Truncate || _tmp.overflowMode == TextOverflowModes.Overflow)
            _tmp.overflowMode = TextOverflowModes.Ellipsis; // optional; keeps it tidy if still too long
    }

    void ApplyBounds()
    {
        if (_tmp == null) return;

        // TMP uses a Vector4 margin: (left, top, right, bottom)
        var m = _tmp.margin;
        m.x = _leftPadding;
        m.z = _rightPadding;
        _tmp.margin = m;
    }
}
