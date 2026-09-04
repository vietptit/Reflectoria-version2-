using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Level : MonoBehaviour, IPointerDownHandler
{
    public int idx=0;
    public bool unlock=true;
    int levelCurrentUnlocked=0;
    public static Action IamChosen;
    
    LevelManager myDad;
    public static bool isSelect=false;

    void Awake()
    {
        
        myDad=GetComponentInParent<LevelManager>();
        isSelect=false;
    }


    public void OnPointerDown(PointerEventData eventData)
    {

        

        AudioManager.instance.PlayAudioClick();
        if(isSelect) return;
        if (!unlock)
        {
            Debug.Log("Bạn chưa mở khóa màn chơi này");
            return;
        }

        isSelect=true; // block chir an mot lan

        Sequence s= DOTween.Sequence();
        s.Append(myDad.AnimReturnLineRenderer());
        s.AppendCallback(()=>Main_Menu.instance.SetUpLevel(idx));
        Debug.Log("Sang lv moi");
        AudioManager.instance.SwitchPlay();
    }

    public void LoadLevel()
    {
        levelCurrentUnlocked=Main_Menu.instance.GetCurrentLevelUnlocked();
        if(idx<=levelCurrentUnlocked)
            unlock=true;
        else
        {
            unlock=false;
        }
    }
    

    

}
