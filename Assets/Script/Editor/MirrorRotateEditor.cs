using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MirrorRotate))]
public class MirrorRotateEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        MirrorRotate controller = (MirrorRotate)target;
        GUILayout.Space(15);

        if (GUILayout.Button("Tự động chỉnh Pivot và Collider", GUILayout.Height(30)))
        {
            controller.gameObject.layer = LayerMask.NameToLayer("Rotate");
            FixPivotAndCollider(controller);
        }
    }

    void FixPivotAndCollider(MirrorRotate controller)
    {
        if (controller.targetPivot == null)
        {
            EditorUtility.DisplayDialog("Thiếu thông tin", "Vui lòng kéo 1 Cube vào ô 'Target Pivot' trước khi bấm!", "OK");
            return;
        }

        Transform parent = controller.transform;
        Transform pivotTarget = controller.targetPivot;
        
        Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Fix Pivot and Collider");

        // --- ĐIỂM SỬA CHÍNH TẠI ĐÂY ---
        // Lấy tâm hình học (center) của targetPivot thay vì Transform gốc của ProBuilder
        Vector3 absoluteCenter = pivotTarget.position;
        Renderer pivotRenderer = pivotTarget.GetComponent<Renderer>();
        
        // Ưu tiên lấy tâm của Mesh hiển thị
        if (pivotRenderer != null)
        {
            absoluteCenter = pivotRenderer.bounds.center;
        }
        // Nếu không có Renderer thì lấy tâm của Collider
        else if (pivotTarget.GetComponent<Collider>() != null)
        {
            absoluteCenter = pivotTarget.GetComponent<Collider>().bounds.center;
        }
        // ------------------------------

        Transform[] children = new Transform[parent.childCount];
        for (int i = 0; i < parent.childCount; i++)
        {
            children[i] = parent.GetChild(i);
        }
        parent.DetachChildren();
        
        // Cập nhật vị trí của Parent vào đúng tâm tuyệt đối
        parent.position = absoluteCenter;
        
        foreach (Transform child in children)
        {
            child.SetParent(parent);
        }

        

        Bounds bounds = new Bounds(parent.position, Vector3.zero);
        bool hasBounds = false;
        
        foreach (Renderer r in parent.GetComponentsInChildren<Renderer>())
        {
            if (!hasBounds)
            {
                bounds = r.bounds;
                hasBounds = true;
            }
            else
            {
                bounds.Encapsulate(r.bounds);
            }
        }

        

        Debug.Log("Hoàn tất! Pivot và Collider của vật thể phản chiếu đã được căn chỉnh vào giữa tâm.");
    }
}