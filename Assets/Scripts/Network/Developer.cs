using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using UnityEditor;

[System.Serializable]
[StructLayout(LayoutKind.Sequential)]
public class Developer
{
    public static int maxNameSize = 20;

    char[] displayName;
    Vector3_S position;
    Quaternion_S rotation;

    public Developer(string name)
    {
        position = new Vector3_S(Vector3.zero);
        rotation = new Quaternion_S(Quaternion.identity);

        displayName = new char[maxNameSize];
        displayName = name.ToCharArray(0, Mathf.Min(name.Length, maxNameSize));

        GameObject boop = new GameObject("boop");
    }

    public void CaptureData()
    {
        SetPosition(SceneView.lastActiveSceneView.camera.transform.position);
        SetRotation(SceneView.lastActiveSceneView.camera.transform.rotation);
    }

    #region Get/Setters

    public void SetPosition(Vector3 pos) { position = new Vector3_S(pos); }

    public Vector3 GetPosition() { return position.ToVector3(); }

    public void SetRotation(Quaternion rot) { rotation = new Quaternion_S(rot); }

    public Quaternion GetRotation() { return rotation.ToQuaternion(); }

    public void SetName(string name) { displayName = name.ToCharArray(0, Mathf.Min(name.Length, maxNameSize)); }

    public string GetName() { return new string(displayName); }

    #endregion
}
