using System;

using LiteNetLib;
using LiteNetLib.Utils;
using System.Threading;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using System.Diagnostics;

// need initial push on connection
//      need to send name/id data right after connection to connect name/id to peer

// make sure everything is closed/disconnected correctly

// only send info when it changes
//      different update rates for different windows?

namespace Server
{
    class Server : NetworkConnection
    {
        NetManager server;

        // TODO: file io or gui for settings

        const int maxPeersCount = 10;
        bool shouldRun;

        public Server()
        {
            shouldRun = true;

            developers = new Developer[maxPeersCount];
            for(int i = 0; i < maxPeersCount; i++) { developers[i] = null; }

            port = 9050;
            connectionKey = "SomeConnectionKey";

            listener = new EventBasedNetListener();
            server = new NetManager(listener);
        }

        public override void Start()
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
                Console.WriteLine("test push");

                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();
                bf.Serialize(ms, DataRecieveType.developerAdd);
                byte[] data = ms.ToArray();
                Console.WriteLine("Len: " + data.Length);
                peer.Send(data, DeliveryMethod.ReliableOrdered);
                return;


                Console.WriteLine("We got connection: " + peer.EndPoint); // Show peer ip
                NetDataWriter writer = new NetDataWriter();                 // Create writer class
                writer.Put("Hello client!");                                // Put some string
                peer.Send(writer, DeliveryMethod.ReliableOrdered);             // Send with reliability

                // setup should happen here

                // Console.WriteLine("new dev: " + devs[server.PeersCount].GetName());
            };

            listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
            {
                Console.WriteLine(developers[peer.Id].GetName() + " disconnected with: " + disconnectInfo.Reason.ToString());
                developers[peer.Id] = null;

                // check how peer array reacts
            };


            listener.NetworkReceiveEvent += (peer, reader, deliveryMethod) =>
            {
                // get type of data
                Console.WriteLine("data recieved");
                try
                {
                    Console.Clear();
                    // Console.WriteLine("decoding " + NetworkDataSize.array[0].size + " bytes");

                    byte[] data = new byte[reader.AvailableBytes];
                    reader.GetBytes(data, 0, reader.AvailableBytes);

                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Binder = new DeserializationBinder();
                    MemoryStream ms = new MemoryStream(data);
                    Developer dev = (Developer)bf.Deserialize(ms);

                    developers[peer.Id] = dev;
                    Console.WriteLine("peer id: " + peer.Id);

                    DisplayAllConnections();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            };

        }

        public override void Run()
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

        public override void Stop()
        {
            shouldRun = false;

            server.Stop();
        }

        public void DisplayAllConnections()
        {
            Console.WriteLine("Connections: " + server.PeersCount);
            foreach (Developer dev in developers)
            {
                Console.WriteLine(dev?.DisplayString());
            }
        }
    }
}

