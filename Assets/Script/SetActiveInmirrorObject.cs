using UnityEngine;

public class SetActiveInmirrorObject : MonoBehaviour
{
    [Tooltip("Thời gian (tính bằng giây) giữa mỗi lần F5 Collider")]
    public float timeInterval = 1f; 

    private float timer = 0f;
    private Collider myCollider;

    void Start()
    {
        // Lấy Collider đang gắn trên chính Object này
        myCollider = GetComponent<Collider>();
    }

    void Update()
    {
        if (myCollider == null) return;

        // Cộng dồn thời gian trôi qua mỗi frame
        timer += Time.deltaTime;

        // Nếu thời gian đếm được lớn hơn hoặc bằng mức cài đặt (1 giây)
        if (timer >= timeInterval)
        {
            // Tắt đi bật lại để "F5" vật lý
            myCollider.enabled = false;
            myCollider.enabled = true;

            // Reset đồng hồ đếm ngược về 0 để tính cho chu kỳ tiếp theo
            timer = 0f;
        }
    }
    
}
