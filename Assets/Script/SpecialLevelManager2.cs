using DG.Tweening;
using UnityEngine;

public class SpecialLevelManager2 : GameManager
{
    public bool isFinish{get;private set;}=false; 
    protected override void IAmWinThenTriggerEvent()
    {
        isFinish=true;
        GameManager_Controll2.instance.SetUPAnimation();
    }
}
