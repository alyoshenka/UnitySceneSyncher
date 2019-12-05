using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;

// todo: server data
//  num connections
//  ping
//  etc

public class ServerWindow : EditorWindow
{
    string serverAddress = "localhost";
    string serverMsg = "waiting to start server";
    string buttonMsg = "Start Server";

    bool serverRunning = false;

    static Server server;
    static Thread serverThread;

    [MenuItem("Window/UnitySceneSyncher/ServerWindow")]
    static void Init()
    {
        ServerWindow win = (ServerWindow)GetWindow(typeof(ServerWindow));
        win.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Server Settings", EditorStyles.boldLabel);
        GUILayout.Label(serverMsg, EditorStyles.label);

        serverAddress = EditorGUILayout.TextField("Server Address", serverAddress);

        if (GUILayout.Button(new GUIContent(buttonMsg)))
        {
            serverRunning = !serverRunning;
            if (serverRunning)
            {
                serverMsg = "server running";
                Debug.Log("started server at " + serverAddress);
                buttonMsg = "Stop Server";


                server = new Server();
                server.Start();
                serverThread = new Thread(server.Run);
                serverThread.Start();
            }
            else
            {
                serverMsg = "waiting to start server";
                Debug.Log("stopped server");
                buttonMsg = "Start Server";

                server.Stop();
                serverThread.Join();
            }            
        }
    }
}
