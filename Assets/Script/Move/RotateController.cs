using DG.Tweening;
using UnityEngine;

public class RotateController : BaseBlock
{
    public Axis rotationAxis = Axis.y;

    [Header("Rotation Settings")]
    public float stepAngle = 90f; 
    public float duration = 0.3f; 

    private int direction = 1;       
    private bool isRotating = false; 
    private bool isBouncing = false; 
    
    private BoxCollider[] allColliders;
    private Tween currentTween; 

    protected virtual void Start()
    {
        allColliders = GetComponentsInChildren<BoxCollider>();
    }

    protected override void Update()
    {
        base.Update();
        
        if (CanInteract_Enter() && !isRotating)
        {
            if(AudioManager.instance!=null)
                AudioManager.instance.PlayAudioRotate();
            RotateTarget(); 
        }
    }

    void RotateTarget() 
    {
        isRotating = true;
        isBouncing = false;

        float rotateAmount = stepAngle * direction;
        Quaternion originalRotation = transform.rotation;
        
        // Xác định trục xoay
        Vector3 axis = Vector3.up;
        if (rotationAxis == Axis.x) axis = transform.right;
        else if (rotationAxis == Axis.y) axis = transform.up;
        else if (rotationAxis == Axis.z) axis = transform.forward;

        float currentLerp = 0f;

        // BÍ QUYẾT: Dùng Float Tween nhích góc từng frame, KHÔNG dùng Euler để tránh lỗi kẹt góc
        currentTween = DOVirtual.Float(0f, rotateAmount, duration, (val) =>
        {
            if (isBouncing) return;

            float delta = val - currentLerp;
            currentLerp = val;
            
           
            transform.Rotate(axis, delta, Space.World);

            if (CheckCollision())
            {
                isBouncing = true;
                currentTween.Kill();
                
                // Va chạm -> Chạy hiệu ứng nảy về vị trí cũ
                DOVirtual.Float(currentLerp, 0f, duration * 0.6f, (backVal) =>
                {
                    float backDelta = backVal - currentLerp;
                    currentLerp = backVal;
                    transform.Rotate(axis, backDelta, Space.World);
                })
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    transform.rotation = originalRotation; // Chốt góc gốc an toàn
                    isBouncing = false;
                    isRotating = false;
                    
                    // ĐỔI HƯỚNG: Lần click chuột tiếp theo sẽ xoay sang hướng ngược lại
                    direction *= -1; 
                });
            }
        })
        .SetEase(Ease.InOutQuart)
        .OnComplete(() =>
        {
            // Xoay trót lọt, kết thúc animation
            isBouncing = false;
            isRotating = false;
        });
    }

    // Hàm check va chạm dùng chung
    private bool CheckCollision()
    {
        foreach (BoxCollider col in allColliders)
        {
            if (col == null) continue;

            Vector3 colCenter = col.transform.TransformPoint(col.center);
            Vector3 colHalfExtents = Vector3.Scale(col.size, col.transform.lossyScale) * 0.5f * 0.9f; 

            Collider[] hits = Physics.OverlapBox(colCenter, colHalfExtents, col.transform.rotation, ~0, QueryTriggerInteraction.Collide);
            
            foreach (Collider hit in hits)
            {
                if (hit.transform != this.transform && !hit.transform.IsChildOf(this.transform))
                {
                    return true; 
                }
            }
        }
        return false; 
    }
}