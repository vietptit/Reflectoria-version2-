using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;


public enum Axis
{
    x,
    y,
    z
}

public class BaseBlock: MonoBehaviour
{
    protected Camera mainCamera;
     [Tooltip("Kéo 1 Cube con vào đây để làm tâm xoay")]
    public Transform targetPivot; 

    protected virtual void Awake()
    {
        if (mainCamera == null)
        {
            mainCamera=GameObject.Find(" Camera_Base").GetComponent<Camera>();
        }
    }

    protected virtual void Update()
    {
        
    }

    protected virtual bool CanInteract_Enter()
    {
        if (Pointer.current!=null&&Pointer.current.press.wasPressedThisFrame&&!Player.instance.isMoving)
        {
            Vector2 mouseScreenPosition = Pointer.current.position.ReadValue();
            
            Ray ray = mainCamera.ScreenPointToRay(mouseScreenPosition);
            RaycastHit hit;
            int layerMaskId= ~LayerMask.GetMask("Mirror","OutMirror");
            if (Physics.Raycast(ray, out hit,Mathf.Infinity,layerMaskId))
            {
                if (hit.transform.IsChildOf(this.transform)||hit.transform==this.transform)
                {
                    return true;
                }
            }
            return false;
        }
        else return false;
    }
}