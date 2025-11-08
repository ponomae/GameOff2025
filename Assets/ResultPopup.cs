using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResultPopup : MonoBehaviour
{
    public static ResultPopup Instance { get; private set; }
    public bool IsOpen => root.activeSelf;

    [SerializeField] private GameObject root;   // ResultPopup 자체 또는 내부 루트
    [SerializeField] private Image art;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    private void Start()
    {
        if (root != null)
            root.SetActive(false); // Awake보다 Start에서 비활성화
    }
    public void Show(CuteTarget target)
    {
        if (target != null)
        {
            if (art) art.sprite = target.PreviewSprite;
            if (nameText) nameText.text = target.DisplayName;
            if (descText) descText.text = target.Description;
        }
        root.SetActive(true);
    }

    public void Close()
    {
        root.SetActive(false);   // 아이들 배경으로 복귀
    }
}