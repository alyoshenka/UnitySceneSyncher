using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class CaptureSceneView : MonoBehaviour
{

    void Update()
    {
        Debug.Log(SceneView.lastActiveSceneView.position);
    }
    void OnGUI()
    {
        Debug.Log(SceneView.lastActiveSceneView.position);
    }
}
