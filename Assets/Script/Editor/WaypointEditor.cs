using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Waypoint))]
[CanEditMultipleObjects]
public class WaypointEditor : Editor
{
    // Kích thước cạnh của khối (mặc định khối lập phương là 1x1x1)
    private static float blockSize = 1.0f;

    public override void OnInspectorGUI()
    {
        // 1. Vẽ giao diện mặc định của script Waypoint
        DrawDefaultInspector();

        EditorGUILayout.Space(15);
        EditorGUILayout.LabelField("--- CÔNG CỤ ĐIỀU CHỈNH PIVOT (6 MẶT) ---", EditorStyles.boldLabel);

        blockSize = EditorGUILayout.FloatField("Kích thước khối (Size)", blockSize);

        EditorGUILayout.Space(5);

        // HÀNG 1: TRỤC Y (TRÊN / DƯỚI)
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("⬆ Mặt Trên (+Y)", GUILayout.Height(30)))
        {
            ShiftPivot(Vector3.up);
        }
        if (GUILayout.Button("⬇ Mặt Dưới (-Y)", GUILayout.Height(30)))
        {
            ShiftPivot(Vector3.down);
        }
        GUILayout.EndHorizontal();

        // HÀNG 2: TRỤC X (TRÁI / PHẢI)
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("⬅ Mặt Trái (-X)", GUILayout.Height(30)))
        {
            ShiftPivot(Vector3.left);
        }
        if (GUILayout.Button("➡ Mặt Phải (+X)", GUILayout.Height(30)))
        {
            ShiftPivot(Vector3.right);
        }
        GUILayout.EndHorizontal();

        // HÀNG 3: TRỤC Z (TRƯỚC / SAU)
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("🔼 Mặt Trước (+Z)", GUILayout.Height(30)))
        {
            ShiftPivot(Vector3.forward);
        }
        if (GUILayout.Button("🔽 Mặt Sau (-Z)", GUILayout.Height(30)))
        {
            ShiftPivot(Vector3.back);
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox("Mẹo: Sau khi đổi Pivot, quả cầu Gizmo và mũi tên Transform sẽ nhảy ra đúng mặt bạn chọn, còn hình ảnh 3D vẫn đứng yên.", MessageType.Info);
    }

    /// <summary>
    /// Dịch chuyển Pivot sang một hướng chỉ định
    /// </summary>
    private void ShiftPivot(Vector3 localDirection)
    {
        float halfSize = blockSize * 0.5f;

        foreach (var selectedObject in targets)
        {
            Waypoint wp = selectedObject as Waypoint;
            if (wp == null) continue;

            // Đăng ký Undo để bấm Ctrl+Z được
            Undo.RegisterFullObjectHierarchyUndo(wp.gameObject, "Shift Waypoint Pivot");

            // Tính toán khoảng dịch chuyển trong không gian thế giới (World Space)
            Vector3 worldOffset = wp.transform.TransformDirection(localDirection * halfSize);

            // 1. Dời tâm cha sang mặt mới
            wp.transform.position += worldOffset;

            // 2. Bù trừ lại vị trí các Object con để hình ảnh đứng yên
            foreach (Transform child in wp.transform)
            {
                child.position -= worldOffset;
            }

            // 3. Bù trừ lại BoxCollider (nếu Collider nằm trực tiếp trên cha)
            BoxCollider boxCollider = wp.GetComponent<BoxCollider>();
            if (boxCollider != null)
            {
                boxCollider.center -= localDirection * halfSize;
            }

            EditorUtility.SetDirty(wp.gameObject);
        }

        // Vẽ lại SceneView ngay lập tức
        SceneView.RepaintAll();
    }
}