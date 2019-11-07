using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// https://docs.unity3d.com/ScriptReference/EditorWindow.html

public class MyWindow : EditorWindow
{
    string str = "boop";
    bool isThing;
    bool mine = true;
    float num = 1.23f;

    [MenuItem("Window/MyWindow")]
    static void Init()
    {
        MyWindow win = (MyWindow)EditorWindow.GetWindow(typeof(MyWindow));
        win.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("Base Settings", EditorStyles.boldLabel);
        str = EditorGUILayout.TextField("Text Field", str);

        isThing = EditorGUILayout.BeginToggleGroup("Setting", isThing);
        mine = EditorGUILayout.Toggle("Switch", mine);
        num = EditorGUILayout.Slider("Scale", num, 0, 2);
        EditorGUILayout.EndToggleGroup();

        if(GUILayout.Button("button"))
        {
            Debug.Log("boop");
        }
    }
}
