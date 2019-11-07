using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class CaptureSceneView : MonoBehaviour
{
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(255, 0, 200, 1);
        Gizmos.DrawSphere(SceneView.lastActiveSceneView.camera.transform.position, 1);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(SceneView.lastActiveSceneView.camera.transform.position, 
                        SceneView.lastActiveSceneView.camera.transform.position 
                        + SceneView.lastActiveSceneView.camera.transform.rotation * Vector3.forward * 5);
    }
}
