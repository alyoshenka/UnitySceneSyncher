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
public class Client
{
    EventBasedNetListener listener;
    NetManager client;
    NetPeer server = null;

    Developer developer;

    string hostIP;
    int port;
    string connectionKey;

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

        developer = new Developer(devName);
    }

    public void Start()
    {
        client.Start();
        client.Connect(hostIP, port, connectionKey);

        Debug.Log("client started");


        // move to serparate function
        listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
        {
            Debug.Log("We got: " + dataReader.GetString(100 /* max length of string */));
            dataReader.Recycle();

            if(null == server)
            {
                server = fromPeer;
                Debug.Log("server initialized");
            }
        };
    }

    public void Stop()
    {
        shouldRun = false;
    }

    public void Run()
    {
        Debug.Log("run client");

        while (shouldRun)
        {
            client.PollEvents();
            Thread.Sleep(15);
        }

        client.Stop();
    }

    // send transform data to server
    public void PushTransform()
    {
        developer.SetPosition(SceneView.lastActiveSceneView.camera.transform.position);
        developer.SetRotation(SceneView.lastActiveSceneView.camera.transform.rotation);

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, developer);
        byte[] transform = ms.ToArray();

        Debug.Log(developer.DisplayString());

        server.Send(transform, DeliveryMethod.ReliableOrdered);
        Debug.Log(transform.Length + " sent");
    }
}