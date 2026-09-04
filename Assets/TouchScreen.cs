using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TouchScreen : MonoBehaviour
{
    [Header("Blinking Text")]
    [SerializeField] TextMeshProUGUI touchScreenText;
    [SerializeField] float blinkSpeed = 2f; 

    [Header("Scene Transition")]
    [SerializeField] Image fadeImage;
    [SerializeField] float fadeDuration = 1f;
    [SerializeField] string sceneToLoad = "MenuScene";

    private bool isLoading = false;

    void Start()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
            fadeImage.raycastTarget = false;
        }
    }

    void Update()
    {
        if (!isLoading)
        {
            Color color = touchScreenText.color;
            color.a = Mathf.PingPong(Time.time * blinkSpeed, 1f); 
            touchScreenText.color = color;
        }
    }

    public void LoadScene()
    {
        if (isLoading) return;
        isLoading = true;
        StartCoroutine(FadeAndLoadRoutine());
    }

    IEnumerator FadeAndLoadRoutine()
    {
        if (fadeImage != null)
        {
            fadeImage.raycastTarget = true;
            float elapsedTime = 0f;
            Color c = fadeImage.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                c.a = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
                fadeImage.color = c;
                yield return null;
            }

            c.a = 1f;
            fadeImage.color = c;
        }

        SceneManager.LoadScene(sceneToLoad);
    }
}