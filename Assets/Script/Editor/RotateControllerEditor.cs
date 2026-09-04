using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using System.Linq;

// Báo cho Unity biết script này dùng để tùy chỉnh Inspector của RotateController
[CustomEditor(typeof(RotateController))]
public class RotateControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        RotateController controller = (RotateController)target;
        GUILayout.Space(15);

        if (GUILayout.Button("Tự động chỉnh Pivot và Collider", GUILayout.Height(30)))
        {
            controller.gameObject.layer = LayerMask.NameToLayer("Rotate");
            FixPivotAndCollider(controller);
        }
    }

    void FixPivotAndCollider(RotateController controller)
    {
        if (controller.targetPivot == null)
        {
            EditorUtility.DisplayDialog("Thiếu thông tin", "Vui lòng kéo 1 Cube vào ô 'Target Pivot' trước khi bấm!", "OK");
            return;
        }

        Transform parent = controller.transform;
        Transform pivotTarget = controller.targetPivot;
        
        Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Fix Pivot and Collider");

        // --- ĐIỂM SỬA: TÍNH TOÁN TÂM TUYỆT ĐỐI ---
        Vector3 absoluteCenter = pivotTarget.position;
        Renderer pivotRenderer = pivotTarget.GetComponent<Renderer>();
        
        if (pivotRenderer != null)
        {
            absoluteCenter = pivotRenderer.bounds.center;
        }
        else if (pivotTarget.GetComponent<Collider>() != null)
        {
            absoluteCenter = pivotTarget.GetComponent<Collider>().bounds.center;
        }
        // -----------------------------------------

        // --- 1. DỜI PIVOT  ---       
        Transform[] children = new Transform[parent.childCount];
        for (int i = 0; i < parent.childCount; i++)
        {
            children[i] = parent.GetChild(i);
        }
        parent.DetachChildren();
        
        // Sử dụng absoluteCenter cho mọi trường hợp. 
        // Bỏ đoạn '+ Vector3.down * .5f' vì absoluteCenter đã nằm chính giữa.
        if(controller.rotationAxis.Equals(Axis.y))
        {
            parent.position = absoluteCenter;
        }
        else
        {
            // Nếu bạn test thử trục khác mà thấy vẫn cần offset thì có thể sửa lại thành: 
            // parent.position = absoluteCenter + Vector3.down * .5f;
            parent.position = absoluteCenter; 
        }
        
        foreach (Transform child in children)
        {
            child.SetParent(parent);
        }

        // --- 2. DỌN DẸP COLLIDER CŨ ---
        Collider[] parentColliders = parent.GetComponents<Collider>();
        foreach (Collider col in parentColliders)
        {
            Undo.DestroyObjectImmediate(col);
        }

        Debug.Log("Hoàn tất! Pivot dời vào giữa tâm và Collider cũ đã được xóa.");
    }
}