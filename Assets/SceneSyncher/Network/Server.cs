﻿using System;
using LiteNetLib;
using System.Threading;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;

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
        public int threadSleepTime = 15; // ms

        NetManager server;

        public Server()
        {
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
            server.PollEvents(); 
            Thread.Sleep(threadSleepTime);
        }

        public override void Stop()
        {
            server.Stop();
            Console.WriteLine("Server stopped");
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

            SendConnectedDeveloperData(peer);
        }

        /// <summary>
        /// gives the peer the existing developers
        /// </summary>
        void SendConnectedDeveloperData(NetPeer peer)
        {
            foreach(Developer dev in developers)
            {
                if(null != dev)
                {
                    NetworkData sendData = new NetworkData
                    {
                        type = DataRecieveType.developerAdd,
                        other = dev
                    };

                    BinaryFormatter bf = new BinaryFormatter();
                    MemoryStream ms = new MemoryStream();
                    bf.Serialize(ms, sendData);
                    byte[] data = ms.ToArray();
                    Console.WriteLine("Sending developer " + dev.GetName() + " to " + peer.EndPoint);
                    peer.Send(data, DeliveryMethod.ReliableOrdered);
                }
            }
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
                    HandleDeveloperMessage((string)rec.other);
                    break;
                case DataRecieveType.developerDelete:
                    DeleteDeveloper(rec);
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

            foreach(NetPeer peer in server.ConnectedPeerList) { peer.Send(data, DeliveryMethod.ReliableOrdered); }
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
                    other = newDev
                };
                bf.Serialize(ms, dat);
                byte[] data = ms.ToArray();

                peer.Send(data, DeliveryMethod.ReliableOrdered);
            }
        }

        void UpdateExistingDeveloper(NetworkData rec, NetPeer sender)
        {
            Developer updateDev = (Developer)rec.other;
            developers[updateDev.GetArrIdx()] = updateDev;
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
                    other = developers[updateDev.GetArrIdx()]
                };
                bf.Serialize(ms, dat);
                byte[] data = ms.ToArray();

                peer.Send(data, DeliveryMethod.ReliableOrdered);
            }
        }

        void DeleteDeveloper(NetworkData rec)
        {
            int toDelete = ((Developer)rec.other).GetArrIdx();
            developers[toDelete] = null;

            // put into a function
            // ok now this is actually terrible
            foreach (NetPeer peer in server.ConnectedPeerList)
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();

                NetworkData dat = new NetworkData
                {
                    type = DataRecieveType.developerDelete,
                    other = toDelete
                };
                bf.Serialize(ms, dat);
                byte[] data = ms.ToArray();

                peer.Send(data, DeliveryMethod.ReliableOrdered);
            }
        }

        void HandleDeveloperMessage(string msg)
        {
            Console.WriteLine(msg);

            foreach(NetPeer peer in server.ConnectedPeerList)
            {
                BinaryFormatter bf = new BinaryFormatter();
                MemoryStream ms = new MemoryStream();

                NetworkData dat = new NetworkData
                {
                    type = DataRecieveType.developerMessage,
                    other = msg
                };
                bf.Serialize(ms, dat);
                byte[] data = ms.ToArray();

                peer.Send(data, DeliveryMethod.ReliableOrdered);
            }
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
            settings.Load(LocationVariables.settingsLocation);
        }
    }
}

