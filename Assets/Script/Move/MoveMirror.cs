using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MoveMirror : MonoBehaviour
{
    public Axis  moveAxis = Axis.x;
    bool isDragging = false;
    float dragThreshold = 0.2f;
    float distanceToCamera;
    
    Vector3 cameraPosStart;
    bool isMoving;
    
    Camera mainCam;

    void Start()
    {
        mainCam = Camera.main; 
    }
    
    void Update()
    {
       
      
        if (Pointer.current.press.wasReleasedThisFrame)
        {
            isDragging = false;
            isMoving = false; 
        }

        
        if (Pointer.current.press.wasPressedThisFrame && !Player.instance.isMoving)
        {
            Ray ray = mainCam.ScreenPointToRay(Pointer.current.position.ReadValue());
            
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject) 
                {
                    isDragging = true;
                    distanceToCamera = mainCam.WorldToScreenPoint(transform.position).z;
                    cameraPosStart = GetPointerWorldPosition(); 
                }
            }
        }
        
        if (isDragging && !isMoving && !Player.instance.isMoving)
        {
            Vector3 currentCamPos = GetPointerWorldPosition(); 
            Vector3 offset = currentCamPos - cameraPosStart;

            if (offset.magnitude >= dragThreshold)
            {
                Vector3 targetPos = transform.position;
                switch (moveAxis)
                {
                    case Axis.x: targetPos.x += offset.x; break;
                    case Axis.y: targetPos.y += offset.y; break;
                    case Axis.z: targetPos.z += offset.z; break;
                }

                Move(targetPos);
            }
        }
    }

    void Move(Vector3 targetPos)
    {
        isMoving = true;
        transform.DOMove(targetPos, 0.001f).OnComplete(() => 
        {
            cameraPosStart = GetPointerWorldPosition(); 
            isMoving = false;
        });
    }

    Vector3 GetPointerWorldPosition()
    {
        Vector2 pointerPos = Pointer.current.position.ReadValue();
        return mainCam.ScreenToWorldPoint(new Vector3(pointerPos.x, pointerPos.y, distanceToCamera));
    }
}