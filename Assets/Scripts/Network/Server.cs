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

// SendToAll
// disconnect function

// serparate server application and settings manager ?

// what to do if connection is denied

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
            LoadSettings();

            developers = new Developer[settings.maxPeerCount];
            for(int i = 0; i < settings.maxPeerCount; i++) { developers[i] = null; }

            listener = new EventBasedNetListener();
            server = new NetManager(listener);
        }

        public override void Start()
        {
            server.Start(settings.port);

            string startDisplay = "Server started";
            startDisplay += "\nAdr: " + settings.serverAddress;
            startDisplay += "\nCnt: " + settings.maxPeerCount;
            startDisplay += "\nKey: " + settings.connectionKey;
            startDisplay += "\nPrt: " + settings.port;
            Console.WriteLine(startDisplay);

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
            if (server.PeersCount < settings.maxPeerCount) { request.AcceptIfKey(settings.connectionKey); }
            else
            {
                request.Reject();
                Console.WriteLine("Denied request: server full");
                return; // bad logic
            }

            if(request.Result == ConnectionRequestResult.Accept)
            {
                Console.WriteLine("Accepted request: " + (settings.maxPeerCount - server.PeersCount) + " open connections");
            }
            else {  Console.WriteLine("Denied request: wrong key"); }
        }

        public void PeerConnected(NetPeer peer)
        {
            NetworkData sendData = new NetworkData
            {
                type = DataRecieveType.serverInitialize,
                other = "Welcome" // peer is already connected
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
                case DataRecieveType.developerInitialize:
                    InitializeNewDeveloper(rec, sender);
                    break;
                case DataRecieveType.developerAdd:
                    AddNewDeveloper(rec);
                    break;
                case DataRecieveType.developerUpdate:
                    UpdateExistingDeveloper(rec, sender);
                    break;
                case DataRecieveType.developerMessage:
                    HandleDeveloperMessage(developers[sender.Id] == null ? "null" : developers[sender.Id].GetName(), (string)rec.other);
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }
        }

        #region Data Reactions

        void InitializeNewDeveloper(NetworkData rec, NetPeer origin)
        {
            // initialize
            Developer newDev = (Developer)rec.other;
            newDev.SetArrIdx(server.PeersCount - 1);
            Console.WriteLine("Created new developer: " + newDev.GetName());

            // send it back
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream ms = new MemoryStream();

            NetworkData dat = new NetworkData
            {
                type = DataRecieveType.developerInitialize,
                other = newDev
            };
            bf.Serialize(ms, dat);
            byte[] data = ms.ToArray();

            origin.Send(data, DeliveryMethod.ReliableOrdered);
        }

        void AddNewDeveloper(NetworkData rec)
        {
            Developer newDev = (Developer)rec.other;
            Console.WriteLine("Added new developer: " + newDev.GetName());
            developers[newDev.GetArrIdx()] = newDev;

            // ok now this is actually terrible
            foreach (NetPeer peer in server.ConnectedPeerList)
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();

                NetworkData dat = new NetworkData
                {
                    type = DataRecieveType.developerAdd,
                    other = developers[peer.Id]
                };
                bf.Serialize(ms, dat);
                byte[] data = ms.ToArray();

                peer.Send(data, DeliveryMethod.ReliableOrdered);
            }
        }

        void UpdateExistingDeveloper(NetworkData rec, NetPeer sender)
        {
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
        }

        void HandleDeveloperMessage(string name, string msg)
        {
            Console.WriteLine(name + " said: " + msg);
        }

        #endregion

        public void DisplayAllConnections()
        {
            Console.WriteLine("Connections: " + server.PeersCount);
            foreach (Developer dev in developers)
            {
                Console.WriteLine(dev?.DisplayString());
            }
        }

        public void LoadSettings()
        {
            settings = new NetworkSettings();
            string s = AppDomain.CurrentDomain.BaseDirectory;
            settings.Load("../../../../../settings.json");
        }
    }
}

