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
    static string connectMsg = "Connect";
    static string disconnectMsg = "Disconnect";

    string serverAddress = "localhost";
    string devName = "your_name_here";
    string buttonMsg = "Connect";
    string messageMsg = "your_message_here";

    Color displayColor = Color.black;
    Color buttonColor = Color.black;

    bool connected { get => client != null; } // currently connected to server
    bool sendWithInspectorUpdate = false; // send updates to server on every InspectorGUI update
    public static bool displayDebugMessages = false;

    public Client client;
    Thread clientThread;

    [MenuItem("Window/UnitySceneSyncher/Client")]
    static void Init()
    {
        ClientWindow win = (ClientWindow)GetWindow(typeof(ClientWindow));
        win.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Client Settings", EditorStyles.boldLabel);

        // Client Settings
        devName = EditorGUILayout.TextField("Your Name", devName);

        // Display Color
        displayColor = EditorGUILayout.ColorField("Display Color", displayColor);
        GUIStyle colorButtonStyle = new GUIStyle(GUI.skin.button);
        colorButtonStyle.normal.textColor = buttonColor;
        if (GUILayout.Button("Set Color", colorButtonStyle)) { SetDisplayColor(displayColor); }

        // Connect
        if (GUILayout.Button(new GUIContent(buttonMsg)))
        {
            if (!connected) { Connect(); }
            else { Disconnect(); }
        }

        if (GUILayout.Button(new GUIContent("Push Transform"))) { client.SendData(); }

        // Settings
        sendWithInspectorUpdate = EditorGUILayout.Toggle("Update With Inspector?", sendWithInspectorUpdate);
        displayDebugMessages = EditorGUILayout.Toggle("Display Debug Messages?", displayDebugMessages);
        client?.DisplayDebugMessages(displayDebugMessages);

        // Display
        if (GUILayout.Button(new GUIContent("Display Devs in Scene"))) { DisplayDevsInScene(); }
        if (GUILayout.Button(new GUIContent("Display Netorked Scene Hierarchy"))) { DisplaySceneHierarchy(); }

        messageMsg = EditorGUILayout.TextField("Send a message", messageMsg);
        if (GUILayout.Button(new GUIContent("Send"))) { client.SendMessage(messageMsg); }
    }

    #region Window Actions

    void SetDisplayColor(Color newColor)
    {
        client?.myDeveloper.SetDisplayColor(newColor);
        buttonColor = newColor;
    }

    void Connect()
    {
        client = new Client(devName, SettingsWindow.settings);
        client.DisplayDebugMessages(displayDebugMessages);
        DisplayHierarchy.client = client;
        client.myDeveloper.SetDisplayColor(buttonColor);

        //DisplayWindow dispWin = ((DisplayWindow)GetWindow(typeof(DisplayWindow)));
        //dispWin.client = client;
        //dispWin.Hide();

        client.Start();
        clientThread = new Thread(client.Run);
        clientThread.Start();

        InitializeDisplay();
        buttonMsg = disconnectMsg;
        if (displayDebugMessages) { Debug.Log("Trying to connect\n" + client.settings.DisplayString()); }
    }

    void InitializeDisplay()
    {
        DisplayWindow disp = (DisplayWindow)GetWindow(typeof(DisplayWindow));
        disp.SetupClient(client);
    }

    void Disconnect()
    {
        buttonMsg = connectMsg;

        client?.Stop();
        clientThread?.Join();

        client = null;
        DisplayHierarchy.client = null;
        DevDisplay.client = null;

        ((DisplayWindow)GetWindow(typeof(DisplayWindow))).ClearClient();

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
        DevDisplay.client = client;
    }

    void DisplaySceneHierarchy()
    {
        DisplayHierarchy currentHierarchy = FindObjectOfType<DisplayHierarchy>();
        if (null == currentHierarchy)
        {
            GameObject go = new GameObject("HierarchyDisplay");
            go.AddComponent<DisplayHierarchy>();
            go.transform.SetAsFirstSibling(); // instantiate to top
            currentHierarchy = go.GetComponent<DisplayHierarchy>(); // make singleton instance
            DisplayHierarchy.client = client;
            currentHierarchy.StartDisplaying();
        }
    }

    #endregion

    public void SetSettings(Network.NetworkSettings set)
    {
        if (null == client) { client = new Client(devName, set); }
    }

    private void OnDestroy()
    {
        if (displayDebugMessages) { Debug.Log("Disconnected from server"); } // put in different place

        client?.Stop();
    }

    private void OnInspectorUpdate()
    {
        if (sendWithInspectorUpdate) { client?.SendData(); }
    }
}