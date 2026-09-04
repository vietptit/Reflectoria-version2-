using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;
    [SerializeField] LayerMask WaypointLayer;
    
    public Waypoint CurrentWaypoint;
    Sequence s;
    
    [SerializeField] float speed = 0.1f;

    [Header("Pre-Move Effects")]
    [SerializeField] float glowDuration = 0.3f;
    [SerializeField] float waveDelay = 0.03f; 
    
    [Header("Random Color Range")]
    [ColorUsage(true, true)] 
    [SerializeField] Color colorStart = Color.yellow;  
    [ColorUsage(true, true)] 
    [SerializeField] Color colorEnd = Color.green;     

    public bool isMoving = false;

    void OnEnable() => GameManager.LetGoPlayer += LetGo;
    void OnDisable() => GameManager.LetGoPlayer -= LetGo;

    void Awake()
    {
        if(instance == null) instance = this;
    }

    void Start()
    {
        if (Physics.Raycast(transform.position + Vector3.up * .1f, Vector3.down, out RaycastHit hitInfo, 2f, WaypointLayer))
        {
            CurrentWaypoint = hitInfo.transform.GetComponent<Waypoint>();
        }
    }

    void LetGo(List<Waypoint> resultPath)
    {
        if (resultPath == null || resultPath.Count == 0) return;
        if(AudioManager.instance!=null)
            AudioManager.instance.PlayAudioWhenFindPath();
        if (s != null) s.Kill(); 

        s = DOTween.Sequence(); 
        isMoving = true;

    
        float currentWaveTime = 0f;
        foreach(var wp in resultPath)
        {
            Renderer[] renderers = wp.GetComponentsInChildren<Renderer>();
            if (renderers.Length > 0)
            {
                Color randomGlowColor = Color.Lerp(colorStart, colorEnd, Random.value);
                foreach (var r in renderers)
                {
                    Material mat = r.material;
                    mat.EnableKeyword("_EMISSION");
                    s.Insert(currentWaveTime, mat.DOColor(randomGlowColor, "_EmissionColor", glowDuration / 2f)
                                      .SetLoops(2, LoopType.Yoyo)
                                      .SetEase(Ease.InOutSine));
                }
                currentWaveTime += waveDelay;
            }
        }

        
        if(AudioManager.instance!=null)
            AudioManager.instance.PlayAudioSpaceShipFadeIn(speed); 

        
        for (int i = 0; i < resultPath.Count; i++)
        {
            var hit = resultPath[i];
            
            // --- ĐIỂM SỬA CHÍNH: Lấy tọa độ mặt trên (Chấm xanh) thay vì tâm khối ---
            Vector3 targetPosition = hit.GetWalkPosition(); 
            // ------------------------------------------------------------------------

            if (i == resultPath.Count - 1 && resultPath.Count > 1)
            {
                s.Append(
                    transform.DOMove(targetPosition, speed*1.5f) // Đã sửa
                    .SetEase(Ease.OutQuad)
                    .OnStart(() => { CurrentWaypoint = hit; })
                );
            }
            else
                s.Append(
                    transform.DOMove(targetPosition, speed) // Đã sửa
                    .SetEase(Ease.Linear)
                    .OnStart(() => { CurrentWaypoint = hit; })
                );
            
            s.JoinCallback(()=> transform.SetParent(hit.transform));
            
            if(!hit.noRotate)
                // Đã sửa hàm LookAt để Player nhìn thẳng vào chấm xanh, không bị chúi đầu xuống đất
                s.Join(transform.DOLookAt(targetPosition, 0f)); 

            
            if (i == resultPath.Count - 1 && resultPath.Count > 1)
            {
                if(AudioManager.instance!=null)
                    s.Join(AudioManager.instance.GetSpaceShipFadeOutTween(speed));
            }
        }

        s.OnComplete(() => {
            if (GameManager.instance != null)
            {
                GameManager.instance.OnPlayerReachedDestination();
                isMoving = false;
                
                if(AudioManager.instance != null)
                    AudioManager.instance.StopAudioSpaceShip();
            }
        });
    }

    public Waypoint GetCurrentWayPoint() => CurrentWaypoint;
}