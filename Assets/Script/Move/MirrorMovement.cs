using UnityEngine;

public class MirrorMovement : MonoBehaviour
{
    [Tooltip("Kéo vật thể gốc (vật bạn sẽ điều khiển) vào đây")]
    public Transform targetObject;

    [Header("Chọn trục muốn phản chiếu (chạy ngược lại)")]
    public bool mirrorX = true;
    public bool mirrorY = false;
    public bool mirrorZ = false;


    private Vector3 initialTargetPos;
    private Vector3 initialMirrorPos;

    void Start()
    {
        if (targetObject != null)
        {
            initialTargetPos = targetObject.position;
            initialMirrorPos = transform.position;
        }
        else
        {
            Debug.LogWarning("Bạn chưa gắn Target Object cho " + gameObject.name);
        }
    }

    void Update()
    {
        if (targetObject == null) return;
        Vector3 distanceMoved = targetObject.position - initialTargetPos;


        float moveX = mirrorX ? -distanceMoved.x : distanceMoved.x;
        float moveY = mirrorY ? -distanceMoved.y : distanceMoved.y;
        float moveZ = mirrorZ ? -distanceMoved.z : distanceMoved.z;


        transform.position = initialMirrorPos + new Vector3(moveX, moveY, moveZ);
    }
}