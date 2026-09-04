using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VisualWaypoint))]
[CanEditMultipleObjects] 
public class VisualWaypointEditor : Editor
{
    private static GameObject targetModelPrefab;
    private static Vector3 visualRotation = Vector3.zero;
    private static float heightOffset = -.5f;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("--- CÔNG CỤ TÙY CHỈNH DIỆN MẠO HÀNG LOẠT ---", EditorStyles.boldLabel);

        targetModelPrefab = (GameObject)EditorGUILayout.ObjectField("Model / Prefab Mới", targetModelPrefab, typeof(GameObject), false);

        // ==========================================
        // KHU VỰC 1: ĐIỀU CHỈNH GÓC XOAY (LIVE PREVIEW)
        // ==========================================
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("1. Cài đặt Góc xoay", EditorStyles.boldLabel);
        
        // Theo dõi xem người dùng có gõ số trực tiếp vào ô không
        EditorGUI.BeginChangeCheck();
        visualRotation = EditorGUILayout.Vector3Field("Góc hiện tại", visualRotation);
        if (EditorGUI.EndChangeCheck()) ApplyRealtimeTransform();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Xoay Y (Trái/Phải)");
        if (GUILayout.Button("-90°")) { visualRotation.y -= 90f; ApplyRealtimeTransform(); }
        if (GUILayout.Button("+90°")) { visualRotation.y += 90f; ApplyRealtimeTransform(); }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Xoay X (Lên/Xuống)");
        if (GUILayout.Button("-90°")) { visualRotation.x -= 90f; ApplyRealtimeTransform(); }
        if (GUILayout.Button("+90°")) { visualRotation.x += 90f; ApplyRealtimeTransform(); }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.PrefixLabel("Xoay Z (Nghiêng)");
        if (GUILayout.Button("-90°")) { visualRotation.z -= 90f; ApplyRealtimeTransform(); }
        if (GUILayout.Button("+90°")) { visualRotation.z += 90f; ApplyRealtimeTransform(); }
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Reset Góc về (0, 0, 0)"))
        {
            visualRotation = Vector3.zero;
            ApplyRealtimeTransform();
        }

        // ==========================================
        // KHU VỰC 2: ĐIỀU CHỈNH ĐỘ CAO (LIVE PREVIEW)
        // ==========================================
        EditorGUILayout.Space(5);
        EditorGUILayout.LabelField("2. Cài đặt Độ cao", EditorStyles.boldLabel);
        
        EditorGUI.BeginChangeCheck();
        heightOffset = EditorGUILayout.FloatField("Chênh lệch độ cao (Y)", heightOffset);
        if (EditorGUI.EndChangeCheck()) ApplyRealtimeTransform();

        EditorGUILayout.Space(10);

        // ==========================================
        // KHU VỰC 3: NÚT TẠO MỚI DIỆN MẠO
        // ==========================================
        if (GUILayout.Button("Tạo mới / Áp dụng Prefab", GUILayout.Height(35)))
        {
            if (targetModelPrefab == null)
            {
                Debug.LogWarning("Vui lòng kéo một Prefab vào ô 'Model / Prefab Mới' trước khi bấm!");
                return;
            }

            foreach (var selectedObject in targets)
            {
                VisualWaypoint waypoint = selectedObject as VisualWaypoint;
                if (waypoint == null) continue;

                Undo.RegisterCompleteObjectUndo(waypoint.gameObject, "Change Visual");

                // 1. DỌN DẸP LƯỚI CHA BẢO VỆ
                Component pbShape = waypoint.GetComponent("ProBuilderShape");
                if (pbShape != null) Undo.DestroyObjectImmediate(pbShape);

                Component pbMesh = waypoint.GetComponent("ProBuilderMesh");
                if (pbMesh != null) Undo.DestroyObjectImmediate(pbMesh);

                MeshRenderer parentRenderer = waypoint.GetComponent<MeshRenderer>();
                if (parentRenderer != null) Undo.DestroyObjectImmediate(parentRenderer);

                MeshFilter parentFilter = waypoint.GetComponent<MeshFilter>();
                if (parentFilter != null) Undo.DestroyObjectImmediate(parentFilter);

                // 2. XÓA CON CŨ
                Transform oldVisual = waypoint.transform.Find("VisualModel");
                if (oldVisual != null)
                {
                    Undo.DestroyObjectImmediate(oldVisual.gameObject);
                }

                // 3. TẠO CON MỚI
                GameObject newVisual = (GameObject)PrefabUtility.InstantiatePrefab(targetModelPrefab, waypoint.transform);
                if (newVisual != null)
                {
                    newVisual.name = "VisualModel"; 
                    newVisual.transform.localPosition = new Vector3(0, heightOffset, 0);
                    newVisual.transform.localRotation = Quaternion.Euler(visualRotation);
                    newVisual.transform.localScale = Vector3.one; 
                }

                EditorUtility.SetDirty(waypoint.gameObject);
            }

            Debug.Log($"Đã khởi tạo diện mạo thành công cho {targets.Length} Waypoint!");
            GUIUtility.ExitGUI();
        }
    }

    // ==========================================
    // HÀM XỬ LÝ LIVE PREVIEW TRÊN SCENE
    // ==========================================
    private void ApplyRealtimeTransform()
    {
        foreach (var selectedObject in targets)
        {
            VisualWaypoint waypoint = selectedObject as VisualWaypoint;
            if (waypoint == null) continue;

            // Tìm xem bên trong Object này có con tên là "VisualModel" chưa
            Transform visual = waypoint.transform.Find("VisualModel");
            if (visual != null)
            {
                // Cho phép bấm Ctrl+Z đối với hành động xoay
                Undo.RecordObject(visual.transform, "Realtime Transform Adjust");
                
                // Cập nhật ngay lập tức Góc xoay và Độ cao
                visual.localRotation = Quaternion.Euler(visualRotation);
                visual.localPosition = new Vector3(0, heightOffset, 0);
                
                EditorUtility.SetDirty(visual.transform);
            }
        }
        
        // Yêu cầu màn hình Scene vẽ lại ngay lập tức để bạn thấy sự thay đổi
        SceneView.RepaintAll();
    }
}