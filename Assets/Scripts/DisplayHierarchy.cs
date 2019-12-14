using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

// https://unity3d.college/2017/09/04/customizing-hierarchy-bold-prefab-text/

/// <summary>
/// shows interactions between other devs and hierarchy
/// </summary>
[ExecuteAlways]
public class DisplayHierarchy : MonoBehaviour
{
    private static Vector2 offset = Vector2.up * 2;

    public static Client client;
    public static float a = 0.2f;

    public void StartDisplaying() { EditorApplication.hierarchyWindowItemOnGUI += HandleHeirarchyWindowItemOnGUI; }

    public void StopDisplaying() { EditorApplication.hierarchyWindowItemOnGUI -= HandleHeirarchyWindowItemOnGUI; }

    private static void HandleHeirarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        if(null == client) { return; }

        Color backgroundColor = Color.gray;

        var obj = EditorUtility.InstanceIDToObject(instanceID);
        if(null != obj)
        {
            foreach (Network.Developer dev in client.developers)
            {
                if (dev != null && dev.GetSelectedGameObjectIndex() == instanceID)
                {
                    backgroundColor = dev.GetDisplayColor();
                    backgroundColor.a = a;
                    Rect offsetRect = new Rect(selectionRect.position + offset, selectionRect.size);
                    EditorGUI.DrawRect(selectionRect, backgroundColor);
                }
            }
        }
    }

    private void OnEnable() { StartDisplaying(); }

    private void OnDestroy() { StopDisplaying(); }
}
