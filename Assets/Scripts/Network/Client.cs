using System;

using LiteNetLib;
using LiteNetLib.Utils;
using System.Threading;

using UnityEngine;
using UnityEditor;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


/// <summary>
/// Client network connection
/// </summary>
public class Client : Network.NetworkConnection
{
    NetManager client;
    NetPeer server = null;

    Developer myDeveloper;

    public DevDisplay display;

    string hostIP;

    bool shouldRun;

    private Client() { }

    public Client(string devName, string ip)
    {
        shouldRun = true;

        hostIP = ip;
        port = 9050;
        connectionKey = "SomeConnectionKey";

        listener = new EventBasedNetListener();
        client = new NetManager(listener);

        myDeveloper = new Developer(devName);
    }

    public override void Start()
    {
        client.Start();
        client.Connect(hostIP, port, connectionKey);

        Debug.Log("Client started");

        // move to serparate function
        listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
        {
            /*
            byte[] type = new byte[1];
            dataReader.GetBytes(type, 1);
            Debug.Log("got: " + type[0]);
            return;
            */

            Debug.Log("We got: " + dataReader.GetString(100 /* max length of string */));
            dataReader.Recycle();

            if(null == server)
            {
                server = fromPeer;
                Debug.Log("Server initialized");
            }

            // recieve all dev data here
            //      if correct send type
        };
    }

    public override void Stop()
    {
        shouldRun = false;

        client.Stop(); // bad ordering?
    }

    public override void Run()
    {
        while (shouldRun)
        {
            client.PollEvents();
            Thread.Sleep(15);
        }
    }

    /// <summary>
    /// set current data
    /// </summary>
    void SetData()
    {
        myDeveloper.SetPosition(SceneView.lastActiveSceneView.camera.transform.position);
        myDeveloper.SetRotation(SceneView.lastActiveSceneView.camera.transform.rotation);
        myDeveloper.SetCurrentTab(EditorWindow.focusedWindow.titleContent.text);
    }

    /// <summary>
    /// send the data across the network
    /// </summary>
    public void SendData()
    {
        Debug.Assert(null != server); // init server in construction

        SetData();

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, myDeveloper);
        byte[] data = ms.ToArray();

        server.Send(data, DeliveryMethod.ReliableOrdered);
    }
}