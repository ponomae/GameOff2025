using UnityEngine;
using UnityEngine.SceneManagement;

public class DexBackButton : MonoBehaviour
{
    [SerializeField] private string fallbackSceneName = "Home"; 
    // 이전 씬 정보가 없을 때 돌아갈 기본 씬 (옵션)

    public void OnBack()
    {
        string target = SceneMemory.LastSceneName;

        if (string.IsNullOrEmpty(target))
        {
            // 도감이 바로 실행되었거나, 이전 씬 정보가 없는 경우
            target = fallbackSceneName;
        }

        SceneManager.LoadScene(target);
    }
}