using DG.Tweening;
using UnityEngine;

public class GameManager_Controll2 : MonoBehaviour
{
    public static GameManager_Controll2 instance;

    [SerializeField] SpecialLevelManager2 level_First;
    [SerializeField] Player player_level_First;
    [SerializeField] GameObject Lv1;
    
    [SerializeField] SpecialLevelManager2 level_Second;
    [SerializeField] Player player_level_Second;
    
    [SerializeField] Vector3 targetRotate1;

    void Awake()
    {
        if(instance==null) instance=this;
    }
    public void SetUPAnimation()
    {
        level_First.enabled=false;
        player_level_Second.enabled=false;

        GameManager.instance=level_Second;
        Player.instance=player_level_Second;
        level_Second.enabled=true;
        player_level_Second.enabled=true;
        DoAnimation();
    }

    void DoAnimation()
    {
        Sequence s= DOTween.Sequence();
        s.Append(transform.DORotate(targetRotate1,1f));
        s.OnComplete(()=>Lv1.SetActive(false));
        
    }



}
