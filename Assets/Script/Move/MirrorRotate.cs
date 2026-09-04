using UnityEngine;

public class MirrorRotate : MonoBehaviour
{
    [Tooltip("Kéo vật thể gốc vào đây")]
    public Transform targetObject;

    [Tooltip("Kéo 1 Cube con vào đây để làm tâm xoay cho cái bóng")]
    public Transform targetPivot;

    [Header("Tùy chỉnh Gương")]
    [Tooltip("Bật công tắc này nếu gương đặt song song (xoay cùng chiều)")]
    public bool isParallelMirror = false;

    private Vector3 initialTargetRot;
    private Vector3 initialMirrorRot;

    void Start()
    {
        if (targetObject != null)
        {          
            initialTargetRot = targetObject.eulerAngles;
            initialMirrorRot = transform.eulerAngles;
        }
    }

    void Update()
    {
        if (targetObject == null) return;
        Vector3 currentTargetRot = targetObject.eulerAngles;
     
        float deltaX = Mathf.DeltaAngle(initialTargetRot.x, currentTargetRot.x);
        float deltaY = Mathf.DeltaAngle(initialTargetRot.y, currentTargetRot.y);
        float deltaZ = Mathf.DeltaAngle(initialTargetRot.z, currentTargetRot.z);

       
        float directionMultiplier = isParallelMirror ? 1f : -1f;
       
        float rotX = deltaX * directionMultiplier;
        float rotY = deltaY * directionMultiplier;
        float rotZ = deltaZ * directionMultiplier;

        float finalX = Mathf.Repeat(initialMirrorRot.x + rotX, 360f);
        float finalY = Mathf.Repeat(initialMirrorRot.y + rotY, 360f);
        float finalZ = Mathf.Repeat(initialMirrorRot.z + rotZ, 360f);

        transform.eulerAngles = new Vector3(finalX, finalY, finalZ);
    }
}