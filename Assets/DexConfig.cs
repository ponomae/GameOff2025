using UnityEngine;
using System.Collections.Generic;

public class DexConfig : MonoBehaviour
{
    [System.Serializable]
    public class Entry
    {
        public string id;               // "desk", "rug", ...
        public string displayName;      // "Deskuma"
        [TextArea(2,5)] public string description;
        public Sprite previewSprite;
    }

    public List<Entry> entries = new List<Entry>();   // 인스펙터에서 직접 입력
    public int Count => entries.Count;
    public Entry Get(int i) => (i>=0 && i<entries.Count) ? entries[i] : null;
}