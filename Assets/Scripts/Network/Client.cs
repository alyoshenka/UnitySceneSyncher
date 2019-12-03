using System;

using LiteNetLib;
using LiteNetLib.Utils;
using System.Threading;

using UnityEngine;

public class Client
{
    EventBasedNetListener listener;
    NetManager client;

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
}