using UnityEngine;

public class FaceToCamera : MonoBehaviour
{
    private Camera mainCamera;

    
    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (mainCamera != null)
        {
            transform.LookAt(mainCamera.transform);
        }
    }
}