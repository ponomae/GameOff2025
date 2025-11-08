using UnityEngine;
using System.Collections.Generic;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance { get; private set; }

    private HashSet<string> collected = new HashSet<string>();
    [SerializeField] private List<string> allIds = new List<string> { "bed", "desk" }; // 전체 대상 ID 목록
    [SerializeField] private bool resetOnStart = true;
    public int Count => collected.Count;
    public int Total => allIds.Count;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (resetOnStart) ResetAll();  // ← 게임 시작마다 초기화
        Load();
    }

    public bool Collect(string id)
    {
        if (collected.Contains(id)) return false;
        collected.Add(id);
        Save();
        return true;
    }

    private void Save()
    {
        PlayerPrefs.SetString("collected", string.Join(",", collected));
        PlayerPrefs.Save();
    }

    private void Load()
    {
        collected.Clear();
        var s = PlayerPrefs.GetString("collected", "");
        if (string.IsNullOrEmpty(s)) return;
        foreach (var id in s.Split(','))
        {
            if (!string.IsNullOrEmpty(id)) collected.Add(id);
        }
    }
    public void ResetAll()
    {
        collected.Clear();
        PlayerPrefs.DeleteKey("collected");
        PlayerPrefs.Save();
    }
}