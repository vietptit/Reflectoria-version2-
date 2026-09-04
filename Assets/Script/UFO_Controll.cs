using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class UFO_Controll : MonoBehaviour
{
    public static UFO_Controll instance;
    public float height = 1.5f;
    public float duration = 1f;

    // SỬA LỖI 1: Đổi kiểu từ Mesh thành Renderer
    [SerializeField] Renderer effect_Render; 
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); 
            return;
        }
    }
    
    public void Active()
    {
        Player player = FindAnyObjectByType<Player>();
        if (player == null) return; 
        Sequence s = DOTween.Sequence();
        Vector3 hoverPos = player.transform.position + Vector3.up * height; 
        Vector3 escapePos = hoverPos + transform.forward * 50f;      
        s.Append(transform.DOMove(hoverPos, duration));
        s.Append(effect_Render.material.DOFloat(10f, "_FresnelPower", duration));
        s.JoinCallback(()=>AudioManager.instance.PlayAudioWhiskAwayPlayer());
        s.Join(player.transform.DOMove(hoverPos, duration));
        s.Join(player.transform.DOScale(Vector3.zero, duration));
        s.Append(effect_Render.material.DOFloat(0f, "_FresnelPower", duration));
        
        s.AppendCallback(()=>AudioManager.instance.PlayAudioDashSpaceShip());
        s.Append(transform.DOMove(escapePos, duration).SetEase(Ease.InBack, 1f));
        s.JoinCallback(()=>AudioManager.instance.StopAudioWhiskAwayPlayer());
        s.OnComplete(()=>Main_Menu.instance.LoadSCeneMenu());
    }


}