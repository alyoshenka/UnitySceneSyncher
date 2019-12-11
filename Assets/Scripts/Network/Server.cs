using System;

using LiteNetLib;
using LiteNetLib.Utils;
using System.Threading;

using UnityEngine;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

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

        listener.NetworkReceiveEvent += (peer, reader, deliveryMethod) =>
        {
            // get type of data
            Debug.Log("data recieved");
            try
            {
                Debug.Log("decoding " + NetworkDataSize.array[0].size + " bytes");
                byte[] data = new byte[NetworkDataSize.array[0].size];
                reader.GetBytes(data, 0, NetworkDataSize.array[0].size);

                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream(data);
                Developer dev = (Developer)bf.Deserialize(ms);

                Debug.Log("Recieved nam: " + dev.GetName());
                Debug.Log("Recieved pos: " + dev.GetPosition());
                Debug.Log("Revieved rot: " + dev.GetRotation().eulerAngles);
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        };

    }

    public void Run()
    {
        while (shouldRun)
        {
            server.PollEvents(); // get events

            // process events

            // SendToAll
            //      send data to clients

            Thread.Sleep(15);
        }

        server.Stop();
    }

    public void Stop()
    {
        shouldRun = false;
    }
}
