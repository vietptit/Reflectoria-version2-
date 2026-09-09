using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveController : BaseBlock
{
    [Header("Cài đặt trục di chuyển")]
    public Axis moveAxis = Axis.x;

    private float distanceToCamera;
    private Vector3 offset;
    private bool isDragging = false;

    public float maxPosition;
    public float minPosition;

    private BoxCollider[] allColliders;

    protected virtual void Start()
    {
        allColliders = GetComponentsInChildren<BoxCollider>();
    }

    protected override void Update()
    {
        base.Update();
        
        
        if(CanInteract_Enter())
        {
            if(AudioManager.instance != null)
                AudioManager.instance.PlayAudioMove();
                
            Vector2 pointerScreenPosition = Pointer.current.position.ReadValue();
            isDragging = true; 
            distanceToCamera = mainCamera.WorldToScreenPoint(transform.position).z;
                  
            Vector3 pointerWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
                                                                            pointerScreenPosition.x, 
                                                                            pointerScreenPosition.y, 
                                                                            distanceToCamera));
            
            Vector3 pointerLocalPosition = transform.parent != null 
                                         ? transform.parent.InverseTransformPoint(pointerWorldPosition) 
                                         : pointerWorldPosition;
                                         
            offset = transform.localPosition - pointerLocalPosition;
        }
        
        
        if(Pointer.current.press.isPressed && isDragging)
        {
            Vector2 pointerScreenPosition = Pointer.current.position.ReadValue();
            Vector3 pointerWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(pointerScreenPosition.x, pointerScreenPosition.y, distanceToCamera));
            
            Vector3 pointerLocalPos = transform.parent != null 
                                    ? transform.parent.InverseTransformPoint(pointerWorldPosition) 
                                    : pointerWorldPosition;
                                    
            Vector3 targetPosition = pointerLocalPos + offset;
            Vector3 newPosition = transform.localPosition;

            switch (moveAxis)
            {
                case Axis.x:
                    if(targetPosition.x <= maxPosition && targetPosition.x >= minPosition) newPosition.x = targetPosition.x;
                    break;
                case Axis.y:
                    if(targetPosition.y <= maxPosition && targetPosition.y >= minPosition) newPosition.y = targetPosition.y;
                    break;
                case Axis.z:
                    if(targetPosition.z <= maxPosition && targetPosition.z >= minPosition) newPosition.z = targetPosition.z;
                    break;
            }

            Vector3 moveDirection = newPosition - transform.localPosition;
            float moveDistance = moveDirection.magnitude;

            if (moveDistance > 0.001f) 
            {
                Vector3 futureWorldPos = transform.parent != null ? transform.parent.TransformPoint(newPosition) : newPosition;
         
                Vector3 moveDeltaWorld = futureWorldPos - transform.position;
                
                Vector3 worldDirection = moveDeltaWorld.normalized;
                float worldDistance = moveDeltaWorld.magnitude;

                bool isBlocked = false;
                
                float shortestHitDistance = worldDistance; 

                foreach (BoxCollider col in allColliders)
                {
                    if (col == null) continue;

                    Vector3 colCenter = col.transform.TransformPoint(col.center);
                    Vector3 colHalfExtents = Vector3.Scale(col.size, col.transform.lossyScale) * 0.5f * 0.95f;

                    RaycastHit[] hits = Physics.BoxCastAll(colCenter, colHalfExtents, worldDirection, col.transform.rotation, worldDistance, ~0, QueryTriggerInteraction.Collide);
                    
                    foreach (RaycastHit hit in hits)
                    {
                        if (hit.transform != this.transform && !hit.transform.IsChildOf(this.transform))
                        {
                            // --- BẮT ĐẦU FIX LỖI DEADLOCK ---
                            if (hit.distance <= 0.001f) 
                            {
                                Vector3 dirToObstacle = (hit.collider.bounds.center - colCenter).normalized;
                                if (Vector3.Dot(worldDirection, dirToObstacle) <= 0.1f)
                                {
                                    continue; 
                                }
                            }
                            // --- KẾT THÚC FIX LỖI ---

                            isBlocked = true;
                            
                            if (hit.distance < shortestHitDistance)
                            {
                                shortestHitDistance = hit.distance;
                            }
                        }
                    }
                }

                if (isBlocked)
                {
                    float moveRatio = shortestHitDistance / worldDistance;
                    moveRatio = Mathf.Max(0f, moveRatio - 0.005f);
                    transform.localPosition = transform.localPosition + (moveDirection * moveRatio);
                }
                else
                {
                    transform.localPosition = newPosition;
                }
            }
        }

        
        if (Pointer.current.press.wasReleasedThisFrame || Player.instance.isMoving)
        {
            isDragging = false; 
        }
    }

}