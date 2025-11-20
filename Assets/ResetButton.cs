using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetButton : MonoBehaviour
{
    [SerializeField] private string firstSceneName = "Home"; 
    // 인스펙터에서 첫 씬 이름 입력

    public void OnStartButtonPressed()
    {
        CollectionManager.Instance?.ResetCollection();
        SceneManager.LoadScene(firstSceneName);
    }
}