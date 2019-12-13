﻿using System.Collections;
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

    Color displayColor = Color.black;
    Color buttonColor = Color.black;

    bool connected = false;
    bool sendWithInspectorUpdate = false;
    bool displayDebugMessages = false;

    Client client;
    Thread clientThread;

    [MenuItem("Window/UnitySceneSyncher/ClientWindow")]
    static void Init()
    {
        ClientWindow win = CreateInstance<ClientWindow>(); // allows mutliple
        win.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Client Settings", EditorStyles.boldLabel);

        // Client Settings
        serverAddress = EditorGUILayout.TextField("Server Address", serverAddress);
        devName = EditorGUILayout.TextField("Your Name", devName);

        // Display Color
        displayColor = EditorGUILayout.ColorField("Display Color", displayColor);
        GUIStyle colorButtonStyle = new GUIStyle(GUI.skin.button);
        colorButtonStyle.normal.textColor = buttonColor;
        if (GUILayout.Button("Set Color", colorButtonStyle)) { SetDisplayColor(displayColor); }

        // Connect
        if (GUILayout.Button(new GUIContent(buttonMsg)))
        {
            connected = !connected;
            if (connected) { Connect(); }
            else { Disconnect(); }
        }

        if (GUILayout.Button(new GUIContent("Push Transform")))
        {
            client?.SendData();
        }

        sendWithInspectorUpdate = EditorGUILayout.Toggle("Update With Inspector?", sendWithInspectorUpdate);
        displayDebugMessages = EditorGUILayout.Toggle("Display Debug Messages?", displayDebugMessages);
        client?.DisplayDebugMessages(displayDebugMessages);

        // Display
        if (GUILayout.Button(new GUIContent("Display Devs in Scene"))) { DisplayDevsInScene(); }
        if(GUILayout.Button(new GUIContent("Display Netorked Scene Hierarchy"))) { DisplaySceneHierarchy(); }

        messageMsg = EditorGUILayout.TextArea(messageMsg);
        if(GUILayout.Button(new GUIContent("Send"))) { client.SendMessage(messageMsg);  }
    }

    #region Window Actions

    void SetDisplayColor(Color newColor)
    {
        client?.myDeveloper.SetDisplayColor(newColor);
        buttonColor = newColor;
    }

    void Connect()
    {
        buttonMsg = disconnectMsg;
        if (displayDebugMessages) { Debug.Log("Connected to " + serverAddress + " as " + devName); }

        client = new Client(devName, serverAddress);
        client.Start();
        clientThread = new Thread(client.Run);
        clientThread.Start();
    }

    void Disconnect()
    {
        buttonMsg = connectMsg;

        client?.Stop();
        clientThread?.Join();

        if (displayDebugMessages) { Debug.Log("Disconnected from " + serverAddress); }
    }

    void DisplayDevsInScene()
    {
        DevDisplay currentDisplay = FindObjectOfType<DevDisplay>();
        if (null == currentDisplay)
        {
            GameObject go = new GameObject("DevDisplay");
            go.AddComponent<DevDisplay>();
            go.transform.SetAsFirstSibling(); // instantiate to top
            currentDisplay = go.GetComponent<DevDisplay>(); // make singleton instance
            currentDisplay.CheckForClient(); // warn if not connected
        }
        currentDisplay.client = client;
    }

    void DisplaySceneHierarchy()
    {
        MyHeirarchy currentHierarchy = FindObjectOfType<MyHeirarchy>();
        if (null == currentHierarchy)
        {
            GameObject go = new GameObject("HierarchyDisplay");
            go.AddComponent<MyHeirarchy>();
            go.transform.SetAsFirstSibling(); // instantiate to top
            currentHierarchy = go.GetComponent<MyHeirarchy>(); // make singleton instance
            MyHeirarchy.client = client;
            currentHierarchy.StartDisplaying();
        }
    }

    #endregion

    private void OnDestroy()
    {
        if (displayDebugMessages) { Debug.Log("Disconnected from server"); } // put in different place

        client.Stop();
    }

    private void OnInspectorUpdate()
    {
        if (sendWithInspectorUpdate) { client?.SendData(); }
    }
}
