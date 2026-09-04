using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DragBlock))] 
public class DragBlockEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        DragBlock controller = (DragBlock)target;
        
        GUILayout.Space(30);

        if (GUILayout.Button("Setup Pivot DragBlock", GUILayout.Height(30)))
        {
           
            if (controller.targetPivot == null)
            {
                Debug.LogWarning(" Bạn chưa gán Target Pivot! Vui lòng kéo 1 object vào ô Target Pivot trước khi bấm.");
                return;
            }

            Transform parent = controller.transform;
            Transform pivot = controller.targetPivot;

            
            Undo.RegisterFullObjectHierarchyUndo(parent.gameObject, "Setup Pivot DragBlock");

            Transform[] childs = new Transform[parent.childCount];
            for (int i = 0; i < parent.childCount; i++)
            {
                childs[i] = parent.GetChild(i);
                Collider collider = childs[i].GetComponent<Collider>();
                
                if (collider == null)
                {
                    Undo.AddComponent<BoxCollider>(childs[i].gameObject);
                }
            }  

            
            parent.DetachChildren();
            parent.position = pivot.position;

            for(int i=0;i<childs.Length;i++) 
            {
                childs[i].SetParent(parent);
                if(i!=childs.Length-1)
                    childs[i].gameObject.layer=LayerMask.NameToLayer("Default");
                else
                    childs[i].gameObject.layer=LayerMask.NameToLayer("Waypoint");
            }

            
            EditorUtility.SetDirty(parent.gameObject);
            Debug.Log("Setup Pivot thành công!");
        }
    }
}