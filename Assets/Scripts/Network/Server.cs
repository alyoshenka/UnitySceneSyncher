using System;

using LiteNetLib;
using LiteNetLib.Utils;
using System.Threading;

using UnityEngine;

public class Server
{
    EventBasedNetListener listener;
    NetManager server;

    // TODO: file io or gui for settings

    int port;
    string connectionKey;

    int maxPeersCount;

    bool shouldRun;

    public Server()
    {
        shouldRun = true;
        maxPeersCount = 10;

        port = 9050;
        connectionKey = "SomeConnectionKey";

        listener = new EventBasedNetListener();
        server = new NetManager(listener);
    }

    public void Start()
    {
        server.Start(port);

        Debug.Log("server started");

        // move to separate functions
        listener.ConnectionRequestEvent += request =>
        {
            if (server.PeersCount < maxPeersCount) { request.AcceptIfKey(connectionKey); }
            else { request.Reject(); }
        };

        listener.PeerConnectedEvent += peer =>
        {
            Debug.Log("We got connection: " + peer.EndPoint); // Show peer ip
            NetDataWriter writer = new NetDataWriter();                 // Create writer class
            writer.Put("Hello client!");                                // Put some string
            peer.Send(writer, DeliveryMethod.ReliableOrdered);             // Send with reliability
        };
    }

    public void Run()
    {
        while (shouldRun)
        {
            server.PollEvents();
            Thread.Sleep(15);
        }

        server.Stop();
    }

    public void Stop()
    {
        shouldRun = false;
    }
}
