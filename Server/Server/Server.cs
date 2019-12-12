using System;

using LiteNetLib;
using LiteNetLib.Utils;
using System.Threading;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using System.Diagnostics;

// need initial push on connection
//      need to send name/id data right after connection to connect name/id to peer

namespace Server
{
    class Server
    {
        EventBasedNetListener listener;
        NetManager server;

        // TODO: file io or gui for settings

        int port;
        string connectionKey;

        int maxPeersCount;
        bool shouldRun;

        Developer[] devs;

        public Server()
        {
            shouldRun = true;
            maxPeersCount = 10;

            devs = new Developer[maxPeersCount];
            for(int i = 0; i < maxPeersCount; i++) { devs[i] = null; }

            port = 9050;
            connectionKey = "SomeConnectionKey";

            listener = new EventBasedNetListener();
            server = new NetManager(listener);
        }

        public void Start()
        {
            server.Start(port);

            Console.WriteLine("server started");

            // move to separate functions
            listener.ConnectionRequestEvent += request =>
            {
                if (server.PeersCount < maxPeersCount) { request.AcceptIfKey(connectionKey); }
                else { request.Reject(); }
            };

            listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("We got connection: " + peer.EndPoint); // Show peer ip
                NetDataWriter writer = new NetDataWriter();                 // Create writer class
                writer.Put("Hello client!");                                // Put some string
                peer.Send(writer, DeliveryMethod.ReliableOrdered);             // Send with reliability

                // setup should happen here

                // Console.WriteLine("new dev: " + devs[server.PeersCount].GetName());
            };

            listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
            {
                Console.WriteLine(devs[peer.Id].GetName() + " disconnected with: " + disconnectInfo.Reason.ToString());

                // check how peer array reacts
            };


            listener.NetworkReceiveEvent += (peer, reader, deliveryMethod) =>
            {
                // get type of data
                Console.WriteLine("data recieved");
                try
                {
                    Debug.Assert(null == devs[1]);
                    Console.Clear();
                    // Console.WriteLine("decoding " + NetworkDataSize.array[0].size + " bytes");
                    byte[] data = new byte[NetworkDataSize.array[0].size];
                    reader.GetBytes(data, 0, NetworkDataSize.array[0].size);

                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Binder = new DeserializationBinder();
                    MemoryStream ms = new MemoryStream(data);
                    Developer dev = (Developer)bf.Deserialize(ms);

                    devs[peer.Id] = dev;
                    Console.WriteLine("peer id: " + peer.Id);

                    Debug.Assert(null == devs[1]);

                    DisplayAllConnections();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
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

        public void DisplayAllConnections()
        {
            Console.WriteLine("Connections: " + server.PeersCount);
            foreach(Developer dev in devs)
            {
                Console.WriteLine(dev?.DisplayString());
            }
        }

        public void Stop()
        {
            shouldRun = false;
        }
    }
}

