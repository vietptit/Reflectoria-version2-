using System;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour, IPointerDownHandler
{
    public IAPProductKey iAPProductKey;
    public List<Level> levels;
    public bool unlocked;
    [SerializeField] LineRenderer lineRenderer;
    
    [SerializeField] float durationPerSegment = 1f; 
    CinemachineCamera cinemachineCamera;
    Collider myCollider;
    UIBillboard uIBillboard;

    [SerializeField] Button UnLockedButton;
    

    void Awake()
    {
        cinemachineCamera=GetComponentInChildren<CinemachineCamera>();
        lineRenderer=GetComponentInChildren<LineRenderer>();
        myCollider=GetComponent<Collider>();
        uIBillboard=GetComponentInChildren<UIBillboard>();
        if (UnLockedButton != null)
        {
            UnLockedButton.onClick.AddListener(CanvasBuyToUnlocked);
            UnLockedButton.onClick.AddListener(()=>AudioManager.instance.PLayAudioClickButClickMouse());
        }
        if(uIBillboard!=null)
            uIBillboard.SetUp(iAPProductKey);
        
    }

    
    
    void Start()
    {
        levels = new List<Level>();
        levels.AddRange(GetComponentsInChildren<Level>());
        foreach(var hit in levels)
        {
            hit.LoadLevel();
        }
        StartLinerender();
        if(levels[0].unlock)
            unlocked=true;
        if (unlocked && uIBillboard != null)
        {
            uIBillboard.gameObject.SetActive(false);
        }
        
    }

    void StartLinerender()
    {
        if (levels.Count == 0) return;

        Sequence s = DOTween.Sequence();
        
        
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, levels[0].transform.position);

       
        for (int i = 1; i < levels.Count; i++)
        {
            int index = i; 
            if(levels[index].unlock==false) break;
            Vector3 targetPos = levels[index].transform.position;
            
          
            s.AppendCallback(() =>
            {
                lineRenderer.positionCount = index + 1;
                lineRenderer.SetPosition(index, lineRenderer.GetPosition(index - 1));
            });

           
            s.Append(DOTween.To(
                () => lineRenderer.GetPosition(index),     
                x => lineRenderer.SetPosition(index, x),   
                targetPos,                                
                durationPerSegment                   
            ).SetEase(Ease.Linear)); 
        }
        
        s.OnComplete(() => Debug.Log("Đã vẽ xong toàn bộ chòm sao!"));
    }


    public void SetPriority(int idx) => cinemachineCamera.Priority=idx;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (unlocked)
        {
            Main_Menu.instance.SetActiveButton_Return(this); // UI hien thi button return
            SetPriority(2); // phóng to camera
            if(myCollider!=null)    myCollider.enabled=false;
        }
        
    }
    public void LogicButtonReturn()
    {
       
        SetPriority(0);
        if(myCollider!=null)    myCollider.enabled=true;
    }


    public Tween AnimReturnLineRenderer()
    {
        Sequence sequence = DOTween.Sequence();
        
        for (int i = lineRenderer.positionCount - 1; i >= 1; i--)
        {
            
            int index = i; 

            sequence.Append(DOTween.To(
                () => lineRenderer.GetPosition(index),
                x => lineRenderer.SetPosition(index, x),
                lineRenderer.GetPosition(index - 1),
                durationPerSegment
            ).SetEase(Ease.Linear));
            sequence.AppendCallback(() => lineRenderer.positionCount--); 
        }
        
        return sequence; 
    }

    void CanvasBuyToUnlocked() => Main_Menu.instance.SetActiveCanvasBuyToUnlocked();
    public void UnLock()
    {
        unlocked=true;
        uIBillboard.gameObject.SetActive(false);

    }
}