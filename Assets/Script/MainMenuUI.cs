using System;
using UnityEngine;
using UnityEngine.Video;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] GameObject menuCV;
    [SerializeField] GameObject canvasSound;
    public void PauseGame(int i)
    {
        Time.timeScale=i;
    }

    public void LocgicSetting_Button()
    {
        if (menuCV.activeSelf)
        {
            menuCV.SetActive(false);
            PauseGame(1);
        }
        else
        {
            menuCV.SetActive(true);
            PauseGame(0);
        }
    }

    public void LogicContinue_Button()
    {
        menuCV.SetActive(false);
        PauseGame(1);
    }

    public void LogicMenu_Button()
    {
        PauseGame(1);
        menuCV.SetActive(false);
        Main_Menu.instance.LoadSCeneMenu();
    }

    public void LogicSound_Button()
    {
        menuCV.SetActive(false);
        PauseGame(1);
        canvasSound.SetActive(true);
    }

    public void LogicQuit_Button()
    {
        // 1. Lệnh này sẽ đóng ứng dụng khi game đã được build (Android, iOS, PC...)
        Application.Quit();

        // 2. Đoạn này giúp dừng game khi bạn đang test ngay trong màn hình Unity Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
