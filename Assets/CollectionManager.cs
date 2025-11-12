using UnityEngine;
using System;
using System.Collections.Generic;

public class CollectionManager : MonoBehaviour
{
    public static CollectionManager Instance { get; private set; }

    [SerializeField] private List<string> allIds = new List<string>(); // 전체 대상 ID 목록
    [SerializeField] private bool resetOnStart = true;

    private HashSet<string> collected = new HashSet<string>();
    public int Count => collected.Count;
    public int Total => allIds.Count;
    public bool Has(string id) => collected.Contains(id);

    public event Action<int,int> OnChanged;  // (count, total)

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        if (resetOnStart) ResetAll();
        else Load();

        OnChanged?.Invoke(Count, Total); // 초기 UI 갱신
    }

    public bool Collect(string id)
    {
        if (string.IsNullOrEmpty(id)) return false;
        bool added = collected.Add(id);
        if (added)
        {
            Save();
            OnChanged?.Invoke(Count, Total);
        }
        return added;
    }

    public void ResetAll()
    {
        collected.Clear();
        PlayerPrefs.DeleteKey("collected");
        PlayerPrefs.Save();
        OnChanged?.Invoke(Count, Total);
    }

    private void Save()
    {
        string data = string.Join(",", collected);
        PlayerPrefs.SetString("collected", data);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        collected.Clear();
        var data = PlayerPrefs.GetString("collected", "");
        if (!string.IsNullOrEmpty(data))
        {
            foreach (var id in data.Split(','))
                if (!string.IsNullOrEmpty(id)) collected.Add(id);
        }
    }
}