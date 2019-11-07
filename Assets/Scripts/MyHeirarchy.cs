using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

// https://unity3d.college/2017/09/04/customizing-hierarchy-bold-prefab-text/

[InitializeOnLoad]
public class MyHeirarchy : MonoBehaviour
{

    private static Vector2 offset = Vector2.up * 2;

    static MyHeirarchy()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HandleHeirarchyWindowItemOnGUI;
    }

    private static void HandleHeirarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        Color fontColor = Color.blue;
        Color backgroundColor = Color.gray;

        var obj = EditorUtility.InstanceIDToObject(instanceID);
        if(null != obj)
        {
            var prefabType = PrefabUtility.GetPrefabType(obj);
            if(prefabType == PrefabType.PrefabInstance)
            {
                fontColor = Color.white;
            }

            Rect offsetRect = new Rect(selectionRect.position + offset, selectionRect.size);
            EditorGUI.DrawRect(selectionRect, backgroundColor);
            EditorGUI.LabelField(offsetRect, obj.name, new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = fontColor },
                fontStyle = FontStyle.Bold
            });
        }
    }
}
