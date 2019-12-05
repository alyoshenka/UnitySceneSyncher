using System;

using LiteNetLib;
using LiteNetLib.Utils;
using System.Threading;

using UnityEngine;

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

    public Client()
    {
        shouldRun = true;

        hostIP = "localhost";
        port = 9050;
        connectionKey = "SomeConnectionKey";

        listener = new EventBasedNetListener();
        client = new NetManager(listener);
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
        developer.num = 5; // actually initialize developer

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        bf.Serialize(ms, developer);
        byte[] transform = ms.ToArray();

        server.Send(transform, DeliveryMethod.ReliableOrdered);
        Debug.Log(transform.Length + " sent");
    }
}