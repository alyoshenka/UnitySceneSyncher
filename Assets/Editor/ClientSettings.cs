using UnityEngine;
using UnityEditor;

public class ClientSettingsWindow : EditorWindow
{
    static Network.NetworkSettings settings;

    private void Awake()
    {
        settings = new Network.NetworkSettings();
        settings.Load();
    }

    [MenuItem("Window/UnitySceneSyncher/ClientSettings")]
    static void Init()
    {
        ClientSettingsWindow win = (ClientSettingsWindow)GetWindow(typeof(ClientSettingsWindow));
        win.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Connection Settings", EditorStyles.boldLabel);

        // settings
        if(null == settings) { settings = new Network.NetworkSettings(); }

        EditorGUI.BeginChangeCheck();
        settings.serverAddress = EditorGUILayout.TextField("Server Address", settings.serverAddress);
        settings.maxPeerCount = EditorGUILayout.IntField("Max Peer Count", settings.maxPeerCount);
        settings.connectionKey = EditorGUILayout.TextField("Connection Key", settings.connectionKey);
        settings.port = EditorGUILayout.IntField("Port", settings.port);
        if (EditorGUI.EndChangeCheck()) { settings.saved = false; }

        // status
        GUILayout.Label("Status: " + (settings.saved ? "saved" : "unsaved"), EditorStyles.label, null);

        // save
        if (GUILayout.Button("Save"))
        {
            settings.Save();
            ((ClientWindow)GetWindow(typeof(ClientWindow), false, null, false)).SetSettings(settings);
        }

        if (GUILayout.Button("Reload")) { settings.Load(); }
    }
}
