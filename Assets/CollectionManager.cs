using UnityEngine;
using System.Collections.Generic;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance { get; private set; }

    // 실제 수집된 ID들
    private HashSet<string> collected = new HashSet<string>();

    // 지금까지 수집한 개수
    public int Count => collected.Count;

    // 전체 개수 (Inspector에서 지정)
    [SerializeField] private int totalCount = 3;
    public int Total => totalCount;

    // UI 업데이트 이벤트
    public System.Action<int, int> OnChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Collect(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        if (collected.Add(id))
        {
            // Count, Total 전달
            OnChanged?.Invoke(collected.Count, totalCount);
        }
    }

    public bool Has(string id)
    {
        return collected.Contains(id);
    }
    public void ResetCollection()
    {
        collected.Clear();
        OnChanged?.Invoke(collected.Count, totalCount);
    }
}