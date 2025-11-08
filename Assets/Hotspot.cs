using UnityEngine;
using UnityEngine.EventSystems;

public class Hotspot : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] string hotspotId = "desk";

    public void OnPointerClick(PointerEventData e)
    {
        if (ResultPopup.Instance != null && ResultPopup.Instance.IsOpen) return;
        // Ignore if camera UI is open
        if (CameraUIManager.Instance != null && CameraUIManager.Instance.IsOpen)
        {
            // And let UI elements handle clicks first
            if (EventSystem.current != null &&
                EventSystem.current.IsPointerOverGameObject(e.pointerId))
                return;

            return; // camera is open → ignore hotspot
        }

        // Otherwise open camera at click position
        CameraUIManager.Instance.OpenAt(e.position, hotspotId);
    }
}