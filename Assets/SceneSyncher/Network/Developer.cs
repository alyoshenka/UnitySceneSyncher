using UnityEngine;
using System.Runtime.InteropServices;
using UnityEditor;

namespace Network
{
    /// <summary>
    /// instance of networked peer
    /// </summary>
    [System.Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public class Developer
    {
        public static int maxNameSize = 20; // maximum string size

        char[] displayName;
        int arrIdx; 
        float4_S displayColor;

        float3_S position;
        float4_S rotation;
        char[] projectName; // we can now see if they are even in the same project
        char[] currentTab;
        int selectedGOID; // id of selected GameObject;

        private Developer() { }

        public Developer(string name)
        {
            position = new float3_S(Vector3.zero);
            rotation = new float4_S(Quaternion.identity);
            displayColor = new float4_S(Color.black);

            displayName = new char[maxNameSize];
            displayName = name.ToCharArray(0, Mathf.Min(name.Length, maxNameSize));

            arrIdx = -1;
        }

        #region Get/Setters

        public void SetArrIdx(int idx) { arrIdx = idx; }

        public int GetArrIdx() { return arrIdx; }

        public void SetPosition(Vector3 pos) { position = new float3_S(pos); }

        public Vector3 GetPosition() { return position.ToVector3(); }

        public void SetRotation(Quaternion rot) { rotation = new float4_S(rot); }

        public Quaternion GetRotation() { return rotation.ToQuaternion(); }

        public void SetName(string name) { displayName = name.ToCharArray(0, Mathf.Min(name.Length, maxNameSize)); }

        public string GetName() { return new string(displayName); }

        public void SetProjectName(string name) { projectName = name.ToCharArray(0, Mathf.Min(name.Length, maxNameSize)); }

        public string GetProjectName() { return new string(projectName); }

        public void SetCurrentTab(string tab) { currentTab = tab.ToCharArray(0, Mathf.Min(tab.Length, maxNameSize)); }

        public string GetCurrentTab() { return new string(currentTab); }

        public void SetDisplayColor(Color col) { displayColor = new float4_S(col); }

        public Color GetDisplayColor() { return displayColor.ToColor(); }

        public void SetSelectedGameObject(GameObject go) { selectedGOID = null == go ? 0 : go.GetInstanceID(); }

        public void SetSelectedGameObjectID(int id) { selectedGOID = id; }

        public int GetSelectedGameObjectIndex() { return selectedGOID; }

        #endregion

        public string DisplayString()
        {
            string s = "Nam: " + GetName() + "\n";
            s += "Prj: " + GetProjectName() + "\n";
            s += "Idx: " + GetArrIdx() + "\n";
            s += "Pos: " + GetPosition() + "\n";
            s += "Rot: " + GetRotation() + "\n";
            s += "Win: " + GetCurrentTab() + "\n";
            s += "Col: " + GetDisplayColor() + "\n";
            s += "Obj: " + GetSelectedGameObjectIndex();
            return s;
        }
    }

}
