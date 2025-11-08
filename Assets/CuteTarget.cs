using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CuteTarget : MonoBehaviour
{
    [SerializeField] private string targetId = "desk";
    [SerializeField] private string displayName = "Desk Spirit";
    [SerializeField] private string description = "점심의 나른한 신";
    [SerializeField] private Sprite previewSprite;   // 팝업에 표시할 스프라이트

    public string TargetId => targetId;
    public string DisplayName => displayName;
    public string Description => description;
    public Sprite PreviewSprite => previewSprite;
}