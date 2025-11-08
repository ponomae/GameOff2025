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
    [SerializeField] private ResultPopup resultPopup;

    [Header("Aim UI")]
    [SerializeField] private RectTransform viewportRect;
    [SerializeField] private Vector2 viewportSize = new Vector2(540, 360);
    [SerializeField] private Vector2 margin = new Vector2(24, 24);

    private string currentHotspotId;
    private Vector2 aimScreenPos;
    public bool IsOpen { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        cameraUIRoot.SetActive(false);
        if (flashOverlay) flashOverlay.color = new Color(1,1,1,0);
        UpdateCounterUI();
    }
    private ResultPopup GetPopup()
    {
        if (resultPopup != null) return resultPopup;
        // Singleton 우선
        if (ResultPopup.Instance != null) { resultPopup = ResultPopup.Instance; return resultPopup; }
        // 비활성 포함 탐색
        resultPopup = FindObjectOfType<ResultPopup>(true);
        return resultPopup;
    }
    public void Open(string hotspotId)
    {
        // 기본 중앙 오픈
        OpenAt(new Vector2(Screen.width*0.5f, Screen.height*0.5f), hotspotId);
    }

    public void OpenAt(Vector2 screenPos, string hotspotId = null)
    {
        currentHotspotId = hotspotId;
        IsOpen = true;
        cameraUIRoot.SetActive(true);

        aimScreenPos = ClampToCanvas(screenPos);
        PlaceUIInstant(aimScreenPos);
    }

    public void CloseUI()
    {
        cameraUIRoot.SetActive(false);
        currentHotspotId = null;
        StartCoroutine(ReenableHotspotsNextFrame());
    }

    public void Shutter()
    {
        if (!IsOpen) return;
        StartCoroutine(ShutterRoutine());
    }

    private IEnumerator ShutterRoutine()
    {
        if (flashOverlay != null) yield return StartCoroutine(Flash());

        float radius = 0.8f; // 프레임의 중심 감도
        int mask = LayerMask.GetMask("Cute");
        
        Vector3 wp = Camera.main.ScreenToWorldPoint(
            new Vector3(aimScreenPos.x, aimScreenPos.y, -Camera.main.transform.position.z));
        
        Collider2D hit = Physics2D.OverlapCircle(new Vector2(wp.x, wp.y), radius, mask);
        
        if (hit)
        {
            var cute = hit.GetComponent<CuteTarget>();
            if (cute != null && (string.IsNullOrEmpty(currentHotspotId) || cute.TargetId == currentHotspotId))
            {
                // 수집 처리(선택)
                CollectionManager.Instance.Collect(cute.TargetId);

                // 1) 카메라 UI 닫기
                CloseUI();

                // 2) 결과 팝업 표시
                if (resultPopup != null) resultPopup.Show(cute);
                yield break;
            }
        }
        
        Debug.Log($"Overlap center: {wp}");
        if (hit) Debug.Log($"Hit object: {hit.name}");
        else Debug.Log("No hit");


    // 실패 시에도 카메라 닫고 끝내고 싶으면 아래 주석 해제
    // CloseUI();
    }

    private IEnumerator Flash()
    {
        float t=0f; while (t<0.08f){ t+=Time.unscaledDeltaTime; flashOverlay.color=new Color(1,1,1,Mathf.Lerp(0,1,t/0.08f)); yield return null; }
        t=0f; while (t<0.2f){ t+=Time.unscaledDeltaTime; flashOverlay.color=new Color(1,1,1,Mathf.Lerp(1,0,t/0.2f)); yield return null; }
        flashOverlay.color=new Color(1,1,1,0);
    }

    private void UpdateCounterUI()
    {
        if (collectionCounterText)
            collectionCounterText.text = $"Collected: {CollectionManager.Instance.Count}/{CollectionManager.Instance.Total}";
    }

    private void PlaceUIInstant(Vector2 screenPos)
    {
        Vector2 local;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rootCanvas.transform as RectTransform, screenPos,
            rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : rootCanvas.worldCamera,
            out local);

        viewportRect.sizeDelta = viewportSize;
        viewportRect.anchoredPosition = local;
    }

    private Vector2 ClampToCanvas(Vector2 screenPos)
    {
        float halfW = viewportSize.x * 0.5f + margin.x;
        float halfH = viewportSize.y * 0.5f + margin.y;
        float x = Mathf.Clamp(screenPos.x, halfW, Screen.width - halfW);
        float y = Mathf.Clamp(screenPos.y, halfH, Screen.height - halfH);
        return new Vector2(x, y);
    }
    private IEnumerator ReenableHotspotsNextFrame()
    {
        // 같은 클릭이 즉시 바닥에 전달되는 걸 방지
        yield return null; // wait 1 frame
        IsOpen = false;
    }   
}