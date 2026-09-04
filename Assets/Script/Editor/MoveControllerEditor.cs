using UnityEngine;
using UnityEditor;
// Xóa dòng using Unity.VisualScripting; đi cho nhẹ file nếu bạn không dùng đến nhé

[CustomEditor(typeof(MoveController))] 
public class MoveControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        MoveController controller = (MoveController)target;
        
        GUILayout.Space(15);
        
        if (GUILayout.Button("Auto Căn chỉnh box collider và Pivot", GUILayout.Height(30))) 
        {
            Transform parent = controller.transform;
            Transform pivot = controller.targetPivot;

            if (pivot == null)
            {
                EditorUtility.DisplayDialog("Lỗi", "Bạn chưa kéo vật thể vào ô Target Pivot!", "OK");
                return;
            }

            Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Auto Adjust");
            
            int moveLayerID = LayerMask.NameToLayer("Move");
            controller.gameObject.layer = moveLayerID;

            Transform[] moveChildren = new Transform[parent.childCount];
            for (int i = 0; i < parent.childCount; i++)
            {
                moveChildren[i] = parent.GetChild(i);
            }
            
            parent.DetachChildren();
            parent.position = pivot.position;
            foreach (Transform child in moveChildren)
            {
                child.SetParent(parent);
            }

            BoxCollider box = parent.GetComponent<BoxCollider>();
            if (box != null)
            {   
                Undo.DestroyObjectImmediate(box);
            }


            Debug.Log("Hoàn tất setup Move! Pivot, Layer và Collider đã bọc khít.");
        }
    }
}