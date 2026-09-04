using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;
using System.Collections.Generic;

public class DragBlock : BaseBlock
{
    [Header("Cài đặt Drag Block")]
    [SerializeField] LayerMask waypointLayer;
    [SerializeField] float timeDur = 0.2f; 
    [SerializeField] Vector3 offset = new Vector3(0, 1, 0);
    
    [Tooltip("Khoảng cách vuốt 3D (0.5 = nửa ô Unity)")]
    [SerializeField] float dragThreshold3D = 0.5f; 

    private bool isDraging;
    private bool isMoving;
    private Sequence moveSequence;
    
    private Plane virtualGroundPlane; 
    private Vector3 startDragWorldPos; 

    protected override void Update()
    {
        base.Update();

        if (Mouse.current.leftButton.wasReleasedThisFrame && isDraging)
        {
            isDraging = false;
        }

        if (CanInteract_Enter())
        {
            isDraging = true;
            virtualGroundPlane = new Plane(Vector3.up, transform.position);

           
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (virtualGroundPlane.Raycast(ray, out float enterDist))
            {
                startDragWorldPos = ray.GetPoint(enterDist);
            }
        }


        if (isDraging && !isMoving)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (virtualGroundPlane.Raycast(ray, out float enterDist))
            {
                Vector3 currentDragWorldPos = ray.GetPoint(enterDist);
                
                Vector3 swipeVector3D = currentDragWorldPos - startDragWorldPos;
                swipeVector3D.y = 0; 
                if (swipeVector3D.magnitude > dragThreshold3D)
                {
                    Vector3 swipeDir = swipeVector3D.normalized;

                    if (FindBestNeightbor3D(swipeDir))
                    {
                        startDragWorldPos = currentDragWorldPos; 
                    }
                    else
                    {
                        startDragWorldPos = currentDragWorldPos; 
                    }
                }
            }
        }
    }

    private bool FindBestNeightbor3D(Vector3 swipeDir3D)
    {
        if (Physics.Raycast(transform.position + Vector3.up * .1f, Vector3.down, out RaycastHit hitInfo, 1.5f, waypointLayer))
        {
            Waypoint current = hitInfo.transform.GetComponent<Waypoint>();
            if (current == null) return false;

            List<Waypoint> waypoints = current.GetListWayPont();
            
            float ValueClose = -1f; 
            Waypoint bestMatch = null; 

            foreach (var hit in waypoints)
            {
                if (!hit.isActive) continue;

                
                Vector3 dirToNeighbor = (hit.transform.position - transform.position);
                dirToNeighbor.y = 0; 
                dirToNeighbor = dirToNeighbor.normalized;

                
                float amount = Vector3.Dot(dirToNeighbor, swipeDir3D);

                if (amount > 0.5f && amount > ValueClose)
                {
                    ValueClose = amount;
                    bestMatch = hit;
                }
            }

            if (bestMatch != null)
            {
                Move(bestMatch.transform);
                return true; 
            }
        }
        return false; 
    }

    void Move(Transform target)
    {
        isMoving = true; 
        
        if (moveSequence != null) moveSequence.Kill();
        moveSequence = DOTween.Sequence();

        moveSequence.Append(transform.DOMove(target.position + offset, timeDur).SetEase(Ease.Linear));

        moveSequence.OnComplete(() =>
        {
            isMoving = false; 
        });
    }
}