using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIOverlay : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] private Button homeButton;
    [SerializeField] private Button nextButton;     // 처음엔 숨김
    [SerializeField] private Button bookButton;

    [Header("Scene Names (optional)")]
    [SerializeField] private string homeSceneName = "Home";
    [SerializeField] private string collectionSceneName = "Collection";
    [SerializeField] private string nextSceneName = ""; // 비우면 buildIndex+1

    private int startCount;
    private bool shownNext;

    private void Awake()
    {
        if (nextButton) nextButton.gameObject.SetActive(false);
        homeButton.onClick.AddListener(GoHome);
        bookButton.onClick.AddListener(OpenBook);
        if (nextButton) nextButton.onClick.AddListener(GoNext);
    }

    private void Start()
    {
        startCount = CollectionManager.Instance ? CollectionManager.Instance.Count : 0;

        if (CollectionManager.Instance)
            CollectionManager.Instance.OnChanged += OnCollectionChanged;
    }

    private void OnDestroy()
    {
        if (CollectionManager.Instance)
            CollectionManager.Instance.OnChanged -= OnCollectionChanged;
    }

    private void OnCollectionChanged(int count, int total)
    {
        // 이 씬에서 처음으로 하나라도 늘면 Next 버튼 표시
        if (!shownNext && count > startCount)
        {
            shownNext = true;
            if (nextButton) nextButton.gameObject.SetActive(true);
        }
    }

    private void GoHome()
    {
        if (!string.IsNullOrEmpty(homeSceneName)) SceneManager.LoadScene(homeSceneName);
        else Debug.LogWarning("Home scene name not set.");
    }

    private void OpenBook()
    {
        if (!string.IsNullOrEmpty(collectionSceneName)) SceneManager.LoadScene(collectionSceneName);
        else Debug.LogWarning("Collection scene name not set.");
    }

    private void GoNext()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
            return;
        }
        var cur = SceneManager.GetActiveScene();
        int next = cur.buildIndex + 1;
        if (next < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(next);
        else Debug.Log("[Overlay] No next scene.");
    }
}