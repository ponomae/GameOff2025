using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CameraUIManager : MonoBehaviour
{
    public static CameraUIManager Instance { get; private set; }

    [Header("Refs")]
    [SerializeField] private GameObject cameraUIRoot;
    [SerializeField] private Image flashOverlay;
    [SerializeField] private TMP_Text collectionCounterText;
    [SerializeField] private Canvas rootCanvas;
    [SerializeField] private ResultPopup resultPopup;   // Inspector에서 비어있으면 GetPopup()이 찾음

    [Header("Aim / Hit")]
    [SerializeField] private RectTransform viewportRect;           // 프레임 이미지
    [SerializeField] private Vector2 viewportSize = new Vector2(540, 360);
    [SerializeField] private Vector2 margin = new Vector2(24, 24);
    [SerializeField] private string cuteLayerName = "Cute";
    [SerializeField] private float hitRadius = 0.18f;

    public bool IsOpen { get; private set; }
    private string currentHotspotId;
    private Vector2 aimScreenPos; // 현재 프레임 중심의 화면좌표(px)

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (flashOverlay) flashOverlay.color = new Color(1,1,1,0);
        if (cameraUIRoot) cameraUIRoot.SetActive(false);
    }

    private void Start()
    {
        UpdateCounterUI();  // 초기 UI
    }

    private void OnEnable()
    {
        if (CollectionManager.Instance != null)
            CollectionManager.Instance.OnChanged += HandleCollectionChanged;
    }

    private void OnDisable()
    {
        if (CollectionManager.Instance != null)
            CollectionManager.Instance.OnChanged -= HandleCollectionChanged;
    }

    // ------ 외부에서 호출 ------
    public void OpenAt(Vector2 screenPos, string hotspotId = null)
    {
        currentHotspotId = hotspotId;
        IsOpen = true;
        if (cameraUIRoot) cameraUIRoot.SetActive(true);

        aimScreenPos = ClampToCanvas(screenPos);
        PlaceFrameInstant(aimScreenPos);
    }

    public void CloseUI()
    {
        if (cameraUIRoot) cameraUIRoot.SetActive(false);
        StartCoroutine(ReenableHotspotsNextFrame());
        currentHotspotId = null;
    }

    public void Shutter()
    {
        if (!IsOpen) return;
        StartCoroutine(ShutterRoutine());
    }

    // ------ 내부 로직 ------
    private IEnumerator ShutterRoutine()
    {
        if (flashOverlay != null) yield return StartCoroutine(Flash());

        // 화면좌표 → 월드좌표
        Vector3 wp = Camera.main.ScreenToWorldPoint(
            new Vector3(aimScreenPos.x, aimScreenPos.y, -Camera.main.transform.position.z));
        Vector2 p = new Vector2(wp.x, wp.y);

        // Cute 레이어만 검사하고, 여러 개면 가장 가까운 것 선택
        int mask = LayerMask.GetMask(cuteLayerName);
        Collider2D[] hits = Physics2D.OverlapCircleAll(p, hitRadius, mask);

        CuteTarget best = null;
        float bestDist = float.MaxValue;

        foreach (var h in hits)
        {
            var ct = h.GetComponent<CuteTarget>();
            if (!ct) continue;
            Vector2 q = h.bounds.ClosestPoint(p);
            float d2 = (q - p).sqrMagnitude;
            if (d2 < bestDist) { bestDist = d2; best = ct; }
        }

        if (best != null)
        {
            bool ok = string.IsNullOrEmpty(currentHotspotId) || best.TargetId == currentHotspotId;
            if (ok)
            {
                CollectionManager.Instance?.Collect(best.TargetId);

                // 1) 먼저 닫고
                CloseUI();
                // 2) 첫 샷 경합 방지용 1프레임 대기
                yield return null;
                // 3) 팝업 표시
                var popup = GetPopup();
                if (popup != null) popup.Show(best);
                yield break;
            }
        }

        // 실패 시: 필요하면 토스트/로그
        Debug.Log("No CuteTarget or ID mismatch");
    }

    private IEnumerator Flash()
    {
        float t = 0f;
        while (t < 0.08f) { t += Time.unscaledDeltaTime; flashOverlay.color = new Color(1,1,1, Mathf.Lerp(0,1,t/0.08f)); yield return null; }
        t = 0f;
        while (t < 0.20f) { t += Time.unscaledDeltaTime; flashOverlay.color = new Color(1,1,1, Mathf.Lerp(1,0,t/0.20f)); yield return null; }
        flashOverlay.color = new Color(1,1,1,0);
    }

    private void PlaceFrameInstant(Vector2 screenPos)
    {
        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform,
            screenPos,
            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera,
            out local
        );
        if (viewportRect)
        {
            viewportRect.sizeDelta = viewportSize;
            viewportRect.anchoredPosition = local;
        }
    }

    private Vector2 ClampToCanvas(Vector2 screenPos)
    {
        float halfW = viewportSize.x * 0.5f + margin.x;
        float halfH = viewportSize.y * 0.5f + margin.y;
        float x = Mathf.Clamp(screenPos.x, halfW, Screen.width  - halfW);
        float y = Mathf.Clamp(screenPos.y, halfH, Screen.height - halfH);
        return new Vector2(x, y);
    }

    private IEnumerator ReenableHotspotsNextFrame()
    {
        yield return null; // same-click 전파 방지
        IsOpen = false;
    }

    private void HandleCollectionChanged(int count, int total)
    {
        if (collectionCounterText)
            collectionCounterText.text = $"Collected: {count}/{total}";
    }

    private void UpdateCounterUI()
    {
        if (collectionCounterText && CollectionManager.Instance)
            collectionCounterText.text = $"Collected: {CollectionManager.Instance.Count}/{CollectionManager.Instance.Total}";
    }

    private ResultPopup GetPopup()
    {
        if (resultPopup != null) return resultPopup;
        if (ResultPopup.Instance != null) return resultPopup = ResultPopup.Instance;
        return resultPopup = FindObjectOfType<ResultPopup>(true);
    }
}