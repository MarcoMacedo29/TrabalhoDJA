using UnityEngine;
using TMPro;

[RequireComponent(typeof(RectTransform))]
public class BorderAutoSize : MonoBehaviour
{
    [Header("Target Input Field (assign in Inspector)")]
    public TMP_InputField inputField;

    [Header("Padding (px)")]
    public Vector2 padding = new Vector2(16, 8);

    [Header("Minimum Border Size (px)")]
    public Vector2 minSize = new Vector2(50, 30);

    private RectTransform _borderRT;
    private TMP_Text _textComponent;

    void Awake()
    {
        _borderRT = GetComponent<RectTransform>();

        if (inputField == null)
        {
            Debug.LogError($"[{nameof(BorderAutoSize)}] No TMP_InputField assigned!", this);
            enabled = false;
            return;
        }

        _textComponent = inputField.textComponent;
        if (_textComponent == null)
        {
            Debug.LogError($"[{nameof(BorderAutoSize)}] InputField.textComponent is null.", this);
            enabled = false;
        }
    }

    void LateUpdate()
    {
        if (_borderRT == null || _textComponent == null)
            return;

        // Measure current input text, or fallback to "0" if empty
        string txt = string.IsNullOrEmpty(inputField.text) ? "0" : inputField.text;

        // Get preferred size from TMP
        Vector2 size = _textComponent.GetPreferredValues(txt);

        // Add padding
        Vector2 targetSize = size + padding;

        // Enforce minimum size
        targetSize.x = Mathf.Max(targetSize.x, minSize.x);
        targetSize.y = Mathf.Max(targetSize.y, minSize.y);

        // Apply size
        _borderRT.sizeDelta = targetSize;
    }
}
