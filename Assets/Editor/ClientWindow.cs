using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;

// disconnect if closed window?????
// reload status when open window????

/// <summary>
/// displays Unity client GUI
/// </summary>
public class ClientWindow : EditorWindow
{
    string serverAddress = "localhost";

    string connectedMsg = "connected to server";
    string disconnectedMsg = "waiting to connect";
    string connectMsg = "Connect";
    string disconnectMsg = "Disconnect";

    string buttonMsg = "Connect";

    bool connected = false;

    static Client client;
    static Thread clientThread;

    static int i;

    [MenuItem("Window/UnitySceneSyncher/ClientWindow")]
    static void Init()
    {
        ClientWindow win = (ClientWindow)GetWindow(typeof(ClientWindow));
        win.Show();

        i = 0;
    }

    private void OnGUI()
    {
        GUILayout.Label("Client Settings", EditorStyles.boldLabel);

        serverAddress = EditorGUILayout.TextField("Server Address", serverAddress);

        if (GUILayout.Button(new GUIContent(buttonMsg)))
        {
            connected = !connected;
            if (connected)
            {
                buttonMsg = disconnectMsg;

                Debug.Log("connected to server at " + serverAddress);

                client = new Client();
                client.Start();
                clientThread = new Thread(client.Run);
                clientThread.Start();
            }
            else
            {
                buttonMsg = connectMsg;

                Debug.Log("disconnected from " + serverAddress);

                client.Stop();
                clientThread.Join();
            }
        }

        if (GUILayout.Button(new GUIContent("Push Transform")))
        {
            client.PushTransform();
        }
    }

    private void OnDestroy()
    {
        Debug.Log("window closed");
    }

    private void OnInspectorUpdate()
    {
        
    }
}
