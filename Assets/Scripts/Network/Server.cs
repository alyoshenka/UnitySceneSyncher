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

namespace Network
{
    public class Server : NetworkConnection
    {
        NetManager server;

        // TODO: file io or gui for settings
        bool shouldRun;

        public Server()
        {
            shouldRun = true;

            developers = new Developer[maxPeerCount];
            for(int i = 0; i < maxPeerCount; i++) { developers[i] = null; }

            port = 9050;
            connectionKey = "SomeConnectionKey";

            listener = new EventBasedNetListener();
            server = new NetManager(listener);
        }

        public override void Start()
        {
            server.Start(port);

            Console.WriteLine("Server started");

            listener.ConnectionRequestEvent += ConnectionRequest;
            listener.PeerConnectedEvent += PeerConnected;
            listener.PeerDisconnectedEvent += PeerDisconnected;
            listener.NetworkReceiveEvent += NetworkRecieve;
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

        #region Events

        public void ConnectionRequest(ConnectionRequest request)
        {
            Console.WriteLine("Recieved connection request from: " + request);
            if (server.PeersCount < maxPeerCount)
            {
                request.AcceptIfKey(connectionKey);
                Console.WriteLine("Accepted request: " + (maxPeerCount - server.PeersCount) + " open connections");
            }
            else
            {
                request.Reject();
                Console.WriteLine("Denied request: server full");
            }
        }

        public void PeerConnected(NetPeer peer)
        {
            NetworkData sendData = new NetworkData
            {
                type = DataRecieveType.serverInitialize,
                other = "Welcome!"
            };

            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();
            bf.Serialize(ms, sendData);
            byte[] data = ms.ToArray();
            Console.WriteLine("Connection from: " + peer.EndPoint);
            peer.Send(data, DeliveryMethod.ReliableOrdered);
        }

        public void PeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
        {
            Console.WriteLine(developers[peer.Id]?.GetName() + " disconnected with: " + disconnectInfo.Reason.ToString());
            developers[peer.Id] = null;

            // check how peer array reacts
        }

        public void NetworkRecieve(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            try
            {
                byte[] data = reader.GetRemainingBytes();

                BinaryFormatter bf = new BinaryFormatter();
                bf.Binder = new DeserializationBinder();
                MemoryStream ms = new MemoryStream(data);
                NetworkData rec = (NetworkData)bf.Deserialize(ms);

                Console.WriteLine("Recieved data of type: " + rec.type);
                DealWithRecievedData(rec, peer);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error:\n" + e);
            }
        }

        #endregion

        public override void DealWithRecievedData(NetworkData rec, NetPeer sender)
        {
            switch (rec.type)
            {
                case DataRecieveType.developerAdd:
                    Developer newDev = (Developer)rec.other;
                    Console.WriteLine("Added new developer: " + newDev.GetName());
                    developers[sender.Id] = newDev;

                    // ok now this is actually terrible
                    foreach(NetPeer peer in server.ConnectedPeerList)
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream ms = new MemoryStream();

                        NetworkData dat = new NetworkData
                        {
                            type = DataRecieveType.developerUpdate,
                            other = developers[peer.Id]
                        };
                        bf.Serialize(ms, dat);
                        byte[] data = ms.ToArray();

                        sender.Send(data, DeliveryMethod.ReliableOrdered);
                    }
                    break;
                case DataRecieveType.developerUpdate:
                    Developer updateDev = (Developer)rec.other;
                    developers[sender.Id] = updateDev;
                    Console.WriteLine(updateDev.DisplayString());

                    // put into a function
                    // ok now this is actually terrible
                    foreach (NetPeer peer in server.ConnectedPeerList)
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        MemoryStream ms = new MemoryStream();

                        NetworkData dat = new NetworkData
                        {
                            type = DataRecieveType.developerUpdate,
                            other = developers[peer.Id]
                        };
                        bf.Serialize(ms, dat);
                        byte[] data = ms.ToArray();

                        sender.Send(data, DeliveryMethod.ReliableOrdered);
                    }
                    break;
                case DataRecieveType.developerMessage:
                    Console.WriteLine(developers[sender.Id]?.GetName() + " said: " + (string)rec.other);
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
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

