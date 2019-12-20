using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DisplayWindow : EditorWindow
{
    Client client;
    string[] displays;

    [MenuItem("Window/UnitySceneSyncher/Display")]
    static void Init()
    {
        DisplayWindow win = (DisplayWindow)GetWindow(typeof(DisplayWindow));
        win.Show();
    }

    public void SetupClient(Client c)
    {
        client = c;
        displays = new string[client.developers.Length];
    }

    public void ClearClient() { client = null; }

    void UpdateClient()
    {
        for (int i = 0; i < client.developers.Length; i++)
        {
            if (null == client.developers[i]) { displays[i] = "open"; }
            else { displays[i] = client.developers[i].DisplayString(); }
        }
    }

    private void ReloadDevelopers()
    {
        UpdateClient();

        EditorUtility.SetDirty(this); // force reload
    }

    private void OnInspectorUpdate()
    {
        if(null != client) { UpdateClient(); }
    }

    private void OnGUI()
    {
        if(null == client) { return; }

        for (int i = 0; i < displays.Length; i++)
        {
            GUILayout.Label("Developer");
            GUILayout.Label(displays[i]);
        }
    }
}
