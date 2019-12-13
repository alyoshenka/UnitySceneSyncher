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
    static string connectedMsg = "connected to server";
    static string disconnectedMsg = "waiting to connect";
    static string connectMsg = "Connect";
    static string disconnectMsg = "Disconnect";

    string serverAddress = "localhost";
    string devName = "your_name_here";
    string buttonMsg = "Connect";
    string messageMsg = "your_message_here";

    bool connected = false;
    bool sendWithInspectorUpdate = false;

    Client client;
    Thread clientThread;

    static int i;

    [MenuItem("Window/UnitySceneSyncher/ClientWindow")]
    static void Init()
    {
        ClientWindow win = CreateInstance<ClientWindow>(); // allows mutliple
        win.Show();

        i = 0;
    }

    private void OnGUI()
    {
        GUILayout.Label("Client Settings", EditorStyles.boldLabel);

        serverAddress = EditorGUILayout.TextField("Server Address", serverAddress);
        devName = EditorGUILayout.TextField("Your Name", devName);

        if (GUILayout.Button(new GUIContent(buttonMsg)))
        {
            connected = !connected;
            if (connected)
            {
                buttonMsg = disconnectMsg;

                Debug.Log("Connected to " + serverAddress + " as " + devName);

                client = new Client(devName, serverAddress);
                client.Start();
                clientThread = new Thread(client.Run);
                clientThread.Start();
            }
            else
            {
                buttonMsg = connectMsg;

                client?.Stop();
                clientThread?.Join();

                Debug.Log("Disconnected from " + serverAddress);
            }
        }

        if (GUILayout.Button(new GUIContent("Push Transform")))
        {
            client?.SendData();
        }

        sendWithInspectorUpdate = EditorGUILayout.Toggle("Update With Inspector?", sendWithInspectorUpdate);

        if(GUILayout.Button(new GUIContent("Display Devs in Scene")))
        {
            DevDisplay currentDisplay = FindObjectOfType<DevDisplay>();
            if(null == currentDisplay)
            {
                GameObject go = new GameObject("DevDisplay");
                go.AddComponent<DevDisplay>();
                currentDisplay = go.GetComponent<DevDisplay>();
            }
            currentDisplay.client = client;
        }

        messageMsg = EditorGUILayout.TextField("Send a message", messageMsg);
        if(GUILayout.Button(new GUIContent("Send")))
        {
            client.SendMessage(messageMsg);
        }
    }

    private void OnDestroy()
    {
        Debug.Log("window closed");

        // disconnect

        client.Stop();
    }

    private void OnInspectorUpdate()
    {
        if (sendWithInspectorUpdate) { client?.SendData(); }
    }
}
