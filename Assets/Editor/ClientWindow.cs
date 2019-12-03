using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;

// disconnect if closed window?????
// reload status when open window????

public class ClientWindow : EditorWindow
{
    string serverAddress = "localhost";

    string connectedMsg = "connected to server";
    string disconnectedMsg = "waiting to connect";
    string connectMsg = "Connect";
    string disconnectMsg = "Disconnect";

    string statusMsg;
    string buttonMsg;

    bool connected = false;

    static Client client;
    static Thread clientThread;

    [MenuItem("Window/UnitySceneSyncher/ClientWindow")]
    static void Init()
    {
        ClientWindow win = (ClientWindow)GetWindow(typeof(ClientWindow));
        win.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Client Settings", EditorStyles.boldLabel);
        GUILayout.Label(statusMsg, EditorStyles.label);

        serverAddress = EditorGUILayout.TextField("Server Address", serverAddress);

        if (GUILayout.Button(new GUIContent(buttonMsg)))
        {
            connected = !connected;
            if (connected)
            {
                buttonMsg = disconnectMsg;
                statusMsg = connectedMsg;

                Debug.Log("connected to server at " + serverAddress);

                client = new Client();
                client.Start();
                clientThread = new Thread(client.Run);
                clientThread.Start();
            }
            else
            {
                buttonMsg = connectMsg;
                statusMsg = disconnectedMsg;

                Debug.Log("disconnected from " + serverAddress);

                client.Stop();
                clientThread.Join();
            }
        }
    }
}
