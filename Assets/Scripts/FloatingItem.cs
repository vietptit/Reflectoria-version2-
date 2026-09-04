using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    [Header("Cài đặt Lơ lửng (Floating)")]
    [Tooltip("Độ cao di chuyển lên/xuống (Biên độ)")]
    public float floatAmplitude = 0.25f; 
    
    [Tooltip("Tốc độ di chuyển lên/xuống")]
    public float floatSpeed = 2f;        

    [Header("Cài đặt Xoay (Tùy chọn)")]
    public bool canRotate = true;        // Có cho phép item xoay hay không
    public float rotationSpeed = 50f;    // Tốc độ xoay (độ/giây)

    // Lưu trữ vị trí ban đầu
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // 1. Xử lý lơ lửng (Floating)
        // Tạo một tọa độ Y mới dựa trên vị trí ban đầu + hàm Sin(thời gian)
        float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        
        // Cập nhật vị trí mới cho item
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // 2. Xử lý tự xoay tròn (Rotation) để item trông đẹp mắt hơn
        if (canRotate)
        {
            // Xoay quanh trục Y (trục dọc)
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }
}