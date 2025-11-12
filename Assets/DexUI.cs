using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class DexUI : MonoBehaviour
{
    [Header("Config (in scene)")]
    [SerializeField] private DexConfig config;

    [Header("UI")]
    [SerializeField] private Image art;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descText;
    [SerializeField] private TMP_Text pageText;
    [SerializeField] private Button prevButton, nextButton;

    [Header("Placeholders (uncollected)")]
    [SerializeField] private Sprite unknownSprite;
    [SerializeField] private string unknownName = "Unknown";
    [SerializeField] private string unknownDesc = "Find this in the world.";

    [Header("Mode")]
    [SerializeField] private bool showCollectedOnly = true;
    private int index;
    private List<int> viewIndices = new List<int>(); // 실제 페이지에 쓸 인덱스들(필터링 결과)


    void Awake(){
        prevButton.onClick.AddListener(Prev);
        nextButton.onClick.AddListener(Next);
    }

    void Start()
    {
        RebuildView();   // ⬅️ 필터 재구성
        // 시작 페이지: 첫 소유 항목
        if (viewIndices.Count > 0) index = 0;
        Refresh();
    }
    public void Prev(){ index = Mathf.Max(0, index-1); Refresh(); }
    public void Next(){ index = Mathf.Min(viewIndices.Count-1, index+1); Refresh(); }
    private void RebuildView(){
        viewIndices.Clear();
        if (config == null || config.Count == 0) return;

        for (int i=0;i<config.Count;i++){
            var id = config.entries[i].id;
            bool owned = CollectionManager.Instance && CollectionManager.Instance.Has(id);
            if (showCollectedOnly){
                if (owned) viewIndices.Add(i);
            } else {
                viewIndices.Add(i);
            }
        }
        // 최소 1페이지는 보장(전부 미수집 + collectedOnly일 때)
        if (showCollectedOnly && viewIndices.Count==0) {
            // 아무것도 없으면 임시로 전체 첫 항목 한 장을 빈 페이지로 표시
            viewIndices.Add(0);
        }
    }

    private void Refresh(){
        if (config == null || viewIndices.Count == 0) return;

        var realIndex = viewIndices[index];
        var e = config.Get(realIndex);
        bool owned = CollectionManager.Instance && CollectionManager.Instance.Has(e.id);

        if (owned){
            art.sprite   = e.previewSprite;
            nameText.text = e.displayName;
            descText.text = e.description;
            art.color = Color.white;
        } else {
            // 블랭크(미수집) 표현
            art.sprite   = unknownSprite;     // 없으면 null 둬도 됨
            nameText.text = "";               // 완전 블랭크
            descText.text = "";               // 완전 블랭크
            art.color = new Color(1,1,1,0.25f); // 연하게(선택)
        }

        pageText.text = $"{index+1} / {viewIndices.Count}";
        prevButton.interactable = index > 0;
        nextButton.interactable = index < viewIndices.Count - 1;
    }

    // ⬇️ 외부에서 모드 전환하고 싶을 때 호출용(옵션)
    public void SetShowCollectedOnly(bool v){
        showCollectedOnly = v;
        index = 0;
        RebuildView();
        Refresh();
    }
}