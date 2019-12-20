using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Threading;

using System.Diagnostics;
using System.Runtime;

// todo: server data
//  num connections
//  ping
//  etc

public class ServerWindow : EditorWindow
{
    static string start = "Start Server";
    static string stop = "Stop Server";
    static string buttonMsg = start;

    static bool serverRunning = false;
    static Process p;

    [MenuItem("Window/UnitySceneSyncher/Server")]
    static void Init()
    {
        ServerWindow win = (ServerWindow)GetWindow(typeof(ServerWindow));
        win.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Insert server warning here");

        if (GUILayout.Button(new GUIContent(buttonMsg)))
        {
            if (!serverRunning)
            {
                if (ClientWindow.displayDebugMessages) { UnityEngine.Debug.Log("Started server"); }
                buttonMsg = stop;
                serverRunning = true;

                // runs from main project file
                p = System.Diagnostics.Process.Start("Network.exe");
            }
            else
            {
                if (ClientWindow.displayDebugMessages) { UnityEngine.Debug.Log("Stopped server"); }
                buttonMsg = start;
                serverRunning = false;

                try { p?.Kill(); }
                catch (System.InvalidOperationException)
                {
                    if (ClientWindow.displayDebugMessages) { UnityEngine.Debug.Log("Server already closed"); }
                }
                catch(System.Exception e) { UnityEngine.Debug.LogError(e); }
            }            
        }
    }
}
