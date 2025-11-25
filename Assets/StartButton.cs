using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    [SerializeField] private string firstSceneName = "MyRoom"; 
    // 인스펙터에서 첫 씬 이름 입력

    public void OnStartButtonPressed()
    {
        SceneManager.LoadScene(firstSceneName);
        BGMManager.Instance.PlayBGM();
    }
}