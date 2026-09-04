using Unity.Android.Gradle.Manifest;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Main_Menu))]
public class Main_Menu_Editor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.BeginHorizontal();
        if(GUILayout.Button("Quay menu", GUILayout.Height(30f)))
        {
            Main_Menu main_Menu= (Main_Menu)target;
            main_Menu.LoadSCeneMenu();
        }
        GUILayout.EndHorizontal();
    }
}
