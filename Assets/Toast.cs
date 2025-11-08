using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class Toast : MonoBehaviour
{
    private static Toast _instance;
    [SerializeField] private TextMeshProUGUI toastText;
    [SerializeField] private CanvasGroup group;
    [SerializeField] private float showTime = 1.2f;

    private void Awake()
    {
        _instance = this;
        if (group) group.alpha = 0f;
    }

    public static void Show(string msg)
    {
        if (_instance == null) return;
        _instance.StopAllCoroutines();
        _instance.StartCoroutine(_instance.Run(msg));
    }

    private IEnumerator Run(string msg)
    {
        if (toastText) toastText.text = msg;
        // fade in
        float t = 0f;
        while (t < 0.15f)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(0,1,t/0.15f);
            yield return null;
        }
        group.alpha = 1f;
        yield return new WaitForSecondsRealtime(showTime);
        // fade out
        t = 0f;
        while (t < 0.2f)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(1,0,t/0.2f);
            yield return null;
        }
        group.alpha = 0f;
    }
}