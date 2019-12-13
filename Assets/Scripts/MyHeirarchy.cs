using UnityEngine;
using UnityEditor;

using System.Collections;
using System.Collections.Generic;

// https://unity3d.college/2017/09/04/customizing-hierarchy-bold-prefab-text/

/// <summary>
/// shows interactions between other devs and hierarchy
/// </summary>
[ExecuteAlways]
public class MyHeirarchy : MonoBehaviour
{
    private static Vector2 offset = Vector2.up * 2;

    public static Client client;

    public void StartDisplaying() { EditorApplication.hierarchyWindowItemOnGUI += HandleHeirarchyWindowItemOnGUI; }

    public void StopDisplaying() { EditorApplication.hierarchyWindowItemOnGUI -= HandleHeirarchyWindowItemOnGUI; }

    private static void HandleHeirarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
    {
        Color fontColor = Color.blue;
        Color backgroundColor = Color.gray;

        var obj = EditorUtility.InstanceIDToObject(instanceID);
        if(null != obj)
        {
            // slightly terrible
            if(client != null)
            {
                foreach (Network.Developer dev in client.developers)
                {
                    if (dev != null && dev.GetSelectedGameObjectIndex() == instanceID)
                    {
                        backgroundColor = dev.GetDisplayColor();
                        break;
                    }
                }
            }

            var prefabType = PrefabUtility.GetPrefabAssetType(obj);
            if(prefabType == PrefabAssetType.Regular)
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

    private void OnDestroy() { StopDisplaying(); }
}
