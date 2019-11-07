using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class CaptureSceneView : MonoBehaviour
{
    void OnDrawGizmos()
    {
       
        foreach (SceneView view in SceneView.sceneViews)
        {
            // Gizmos.color = new Color(255, 0, 200, 1);
            // Gizmos.DrawSphere(view.camera.transform.position, 1);
            Gizmos.DrawIcon(view.camera.transform.position, "bulb.png", true);

            Gizmos.color = Color.red;
            Gizmos.DrawLine(view.camera.transform.position,
                            view.camera.transform.position
                            + view.camera.transform.rotation * Vector3.forward * 5);
        }

    }

    void Start()
    {
        Selection.selectionChanged += SelectionChanged;
    }

    void SelectionChanged()
    {
        if (null != Selection.activeGameObject) { Debug.Log(Selection.activeGameObject.name); }
    }
}
