using UnityEngine;

public class FinalLevel5Manager : GameManager
{
    protected override void IAmWinThenTriggerEvent()
    {
        base.IAmWinThenTriggerEvent();
        Debug.Log("I am winner");
    }
}
