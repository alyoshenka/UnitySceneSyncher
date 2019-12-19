using UnityEngine;
using UnityEditor;

public class PeerDisplayWindow : EditorWindow
{
    Client client;

    [MenuItem("Window/UnitySceneSyncher/PeerDisplay")]
    static void Init()
    {
        PeerDisplayWindow win = (PeerDisplayWindow)GetWindow(typeof(PeerDisplayWindow));
        win.Show();
    }  

    private void Awake()
    {
       client = ((ClientWindow)GetWindow(typeof(ClientWindow), false, null, false)).client;
    }

    private void OnGUI()
    {
        if(null == client) { client = ((ClientWindow)GetWindow(typeof(ClientWindow), false, null, false)).client; }
        if(null == client) { return; }

        foreach(Network.Developer dev in client.developers)
        {
            GUILayout.Label(dev.GetName());
        }
    }
}
