using UnityEngine;
using UnityEngine.EventSystems;

public class IdleClickCatcher : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private bool useAimMarker = false;
    [SerializeField] private RectTransform aimMarker; // + 버튼 UI (옵션)

    public void OnPointerDown(PointerEventData e)
    {
        if (useAimMarker && aimMarker != null)
        {
            // + 버튼을 클릭 지점에 배치하고 보이기
            PlaceRectAtScreen(aimMarker, e.position);
            aimMarker.gameObject.SetActive(true);
        }
        else
        {
            // 즉시 카메라 UI 열기
            CameraUIManager.Instance.OpenAt(e.position, "중앙에 맞춰 찍어보세요");
        }
    }

    private void PlaceRectAtScreen(RectTransform rect, Vector2 screenPos)
    {
        var canvas = rect.GetComponentInParent<Canvas>();
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform, screenPos,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out var local);
        rect.anchoredPosition = local;
    }
}