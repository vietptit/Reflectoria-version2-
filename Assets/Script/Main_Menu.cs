using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Main_Menu : MonoBehaviour
{
    public static Main_Menu instance;
    public List<GameObject> levelPrefab = new List<GameObject>();

    [Header("Transition Overlay")]
    public GameObject fadeObject;

    [Header("Transition Settings")]
    public float transitionDuration = 2.0f; 
    public float fadeOutTime = 2f;       
    GameObject pendingLevel;
   
    private AsyncOperation asyncLoad;
    
    // Thêm biến này để "nhớ" Level nào cần tạo sau khi Load xong Scene
    private int pendingLevelIndex = -1; 

    [SerializeField] Button UI_Return_click;
    [SerializeField] LevelManager levelCurrent;
    [SerializeField] GameObject CanVasBuyToUnlocked;
    public bool isPC;
    void Awake()
    {
        if (instance == null)
            instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        

        if (fadeObject != null)
        {
            fadeObject.SetActive(false);
        }

        if(isPC)
            UnlockAllLevel();
    }

    
    public void SetUpLevel(int idx)
    {
        DestroyChildExceptCanvas();

        if (idx < levelPrefab.Count)
        {
            // Lưu lại index của Level để dùng sau
            pendingLevelIndex = idx;
            LoadScene("Level", idx);
        }
    }

    private void DestroyChildExceptCanvas()
    {
        if (transform.childCount > 0)
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
            {
                Transform child = transform.GetChild(i);
                if (child.GetComponent<Canvas>() == null)
                {
                    Destroy(child.gameObject);
                }
            }
        }
    }

    public void LoadScene(string sceneName, int idx)
    {
        if (fadeObject != null)
        {
            UpdateShaderFade(0f);
            fadeObject.SetActive(true); 
        }
        
        StartCoroutine(LoadSceneWithTransition(sceneName, idx));
    }

    IEnumerator LoadSceneWithTransition(string sceneName, int idx)
    {
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        asyncLoad.allowSceneActivation = false; 

        float elapsedTime = 0f;

        while (elapsedTime < transitionDuration || asyncLoad.progress < 0.9f)
        {
            elapsedTime += Time.deltaTime;
            float fadeProgress = Mathf.Clamp01(elapsedTime / transitionDuration);
            UpdateShaderFade(fadeProgress);
            yield return null; 
        }

        UpdateShaderFade(1.0f); 
        
       
        SceneManager.sceneLoaded += OnSceneLoaded;
        asyncLoad.allowSceneActivation = true; 
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoaded; 
        
        
        if (pendingLevelIndex >= 0 && pendingLevelIndex < levelPrefab.Count)
        {
            pendingLevel = Instantiate(levelPrefab[pendingLevelIndex], transform);
        }
        
        UI_Return_click.gameObject.SetActive(false);
        StartCoroutine(FadeOutBlack());
    }

    IEnumerator FadeOutBlack()
    {
        if (fadeObject != null)
        {
            Image fadeImage = fadeObject.GetComponent<Image>();
            
            if (fadeImage != null && fadeImage.material != null)
            {
                float elapsedTime = 0f;
                float currentFade = fadeImage.material.GetFloat("_FadeProgress"); 

                while (elapsedTime < fadeOutTime)
                {
                    elapsedTime += Time.deltaTime;
                    float fadeProgress = Mathf.Lerp(currentFade, 0f, elapsedTime / fadeOutTime);
                    UpdateShaderFade(fadeProgress);
                    yield return null;
                }

                UpdateShaderFade(0f);
                fadeObject.SetActive(false);
            }
        }
    }

    private void UpdateShaderFade(float fadeValue)
    {
        Image fadeImage = fadeObject.GetComponent<Image>();
        fadeImage.material.SetFloat("_FadeProgress", fadeValue);  
    }

    public void LoadSCeneMenu()
    {
        DestroyChildExceptCanvas();
        LoadScene("MenuScene",-1);
        AudioManager.instance.SwitchMenu();
        Debug.Log("Chuyển sang scene menu");
    }

    public void UI_Return_Click() // gans vafo button, chuc nang laf khi ma clck button return thif tra lai camera vaf an
    {
        levelCurrent.LogicButtonReturn();
        UI_Return_click.gameObject.SetActive(false);
        levelCurrent=null;
        
    }

    public void SetActiveButton_Return(LevelManager levelManager)
    {
        UI_Return_click.gameObject.SetActive(true);
        SetLevelCurrent(levelManager);
    }

    void SetLevelCurrent(LevelManager levelManager)
    {
        levelCurrent=levelManager;
    }

    public int GetCurrentLevelUnlocked() => SaveLoadFile.instance.GetLevel();
    public void SaveLevel()
    {
        int current=GetCurrentLevelUnlocked();
        if (pendingLevelIndex >=current)
        {
            current++;
            SaveLoadFile.instance.SaveLevel(current);
        }
    }


    [ContextMenu("Unlock all level")]
    public void UnlockAllLevel()
    {
         SaveLoadFile.instance.SaveLevel(999);
        SceneManager.LoadScene("MenuScene");
    }

    



    public void SetActiveCanvasBuyToUnlocked() => CanVasBuyToUnlocked.SetActive(true);
}