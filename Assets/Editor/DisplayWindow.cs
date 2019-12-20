using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class DisplayWindow : EditorWindow
{
    public Client client;
    string[] displays;

    [MenuItem("Window/UnitySceneSyncher/Display")]
    static void Init()
    {
        DisplayWindow win = (DisplayWindow)GetWindow(typeof(DisplayWindow));
        win.Show();
    }

    private void ReloadDevelopers()
    {
        if(null == displays || null == client)
        {
            Debug.Assert(null != client);
            displays = new string[client.developers.Length];

            Debug.Log("Reinitialized display client");
        }

        for (int i = 0; i < client.developers.Length; i++)
        {
            if (null == client.developers[i]) { displays[i] = "open"; }
            else { displays[i] = client.developers[i].DisplayString(); }
        }

        EditorUtility.SetDirty(this); // force reload
    }

    private void Update()
    {
        if (null == client)
        {
            Client client = ((ClientWindow)GetWindow(typeof(ClientWindow))).client;
            if (null == client) { return; }
            client.developerUpdate += ReloadDevelopers;
        }
    }

    private void OnGUI()
    {
        if (null == client) { return; }

        for (int i = 0; i < displays.Length; i++)
        {
            GUILayout.Label("Developer");
            GUILayout.Label(displays[i]);
        }
    }
}
