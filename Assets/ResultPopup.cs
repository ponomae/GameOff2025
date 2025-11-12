// ResultPopup.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultPopup : MonoBehaviour
{
    public static ResultPopup Instance { get; private set; }
    public bool IsOpen => root && root.activeSelf;

    [SerializeField] private GameObject root;
    [SerializeField] private Image art;
    [SerializeField] private TMP_Text nameText, descText;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Debug.Log("[Popup] Awake");
    }

    void Start()
    {
        if (root) root.SetActive(false);
        Debug.Log("[Popup] Start, root off");
    }

    public void Show(CuteTarget t)
    {
        if (!t) { Debug.LogWarning("[Popup] Show called with null target"); return; }
        if (art) art.sprite = t.PreviewSprite;
        if (nameText) nameText.text = t.DisplayName;
        if (descText) descText.text = t.Description;
        if (root) root.SetActive(true);
        Debug.Log($"[Popup] Show: {t.name} ({t.TargetId})");
    }

    public void Close()
    {
        if (root) root.SetActive(false);
        Debug.Log("[Popup] Close");
    }
}