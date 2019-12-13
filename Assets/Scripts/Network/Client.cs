using System;

using LiteNetLib;
using LiteNetLib.Utils;
using System.Threading;

using UnityEngine;
using UnityEditor;

using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using Network;

/// <summary>
/// Client network connection
/// </summary>
public class Client : NetworkConnection
{
    NetManager client;
    NetPeer server = null;

    public bool isRunning { get => null != server; }

    public Developer myDeveloper;
    bool displayDebugMessages;

    string hostIP;
    bool shouldRun;

    bool updatePending; // ???

    private Client() { }

    public Client(string devName, string ip)
    {
        shouldRun = true;

        hostIP = ip;
        port = 9050;
        connectionKey = "SomeConnectionKey";

        listener = new EventBasedNetListener();
        client = new NetManager(listener);

        developers = new Developer[maxPeerCount];
        for(int i = 0; i < maxPeerCount; i++) { developers[i] = null; }
        myDeveloper = new Developer(devName);
    }

    public override void Start()
    {
        client.Start();
        client.Connect(hostIP, port, connectionKey);

        if (displayDebugMessages) { Debug.Log("Client started"); }

        listener.NetworkReceiveEvent += NetworkRecieve;
    }

    public override void Stop()
    {
        shouldRun = false;
        server = null;

        client.Stop(); // bad ordering?
    }

    public override void Run()
    {
        while (shouldRun)
        {
            client.PollEvents();
            Thread.Sleep(15);
        }
    }

    /// <summary>
    /// set current data
    /// </summary>
    void SetData()
    {
        Debug.Assert(null != myDeveloper);

        myDeveloper.SetPosition(SceneView.lastActiveSceneView.camera.transform.position);
        myDeveloper.SetRotation(SceneView.lastActiveSceneView.camera.transform.rotation);
        myDeveloper.SetCurrentTab(EditorWindow.focusedWindow.titleContent.text);

        myDeveloper.SetSelectedGameObject(Selection.activeGameObject);
    }

    /// <summary>
    /// initialize server data
    /// </summary>
    void SendInitialData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();

        NetworkData dat = new NetworkData
        {
            type = Network.DataRecieveType.developerAdd,
            other = myDeveloper
        };
        bf.Serialize(ms, dat);
        byte[] data = ms.ToArray();

        server.Send(data, DeliveryMethod.ReliableOrdered);
    }

    /// <summary>
    /// send the data across the network
    /// </summary>
    public void SendData()
    {
        Debug.Assert(null != server); // init server in construction

        SetData();

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();

        NetworkData dat = new NetworkData
        {
            type = Network.DataRecieveType.developerUpdate,
            other = myDeveloper
        };
        bf.Serialize(ms, dat);
        byte[] data = ms.ToArray();

        server.Send(data, DeliveryMethod.ReliableOrdered);
    }

    #region Events

    public void NetworkRecieve(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
    {
        byte[] data = new byte[reader.AvailableBytes];
        reader.GetBytes(data, 0, reader.AvailableBytes);

        BinaryFormatter bf = new BinaryFormatter();
        bf.Binder = new DeserializationBinder();
        MemoryStream ms = new MemoryStream(data);
        NetworkData rec = (NetworkData)bf.Deserialize(ms);

        if (displayDebugMessages) { Debug.Log("Recieved: " + rec.type); }
        DealWithRecievedData(rec, peer);

        reader.Recycle();
    }

    #endregion

    public override void DealWithRecievedData(NetworkData rec, NetPeer sender)
    {
        switch (rec.type)
        {
            case DataRecieveType.developerAdd:
                AddNewDeveloper(rec, sender.Id);
                break;
            case DataRecieveType.developerUpdate:
                UpdateExistingDeveloper(rec, sender.Id);
                break;
            case DataRecieveType.developerMessage:
                HandleDeveloperMessage(developers[sender.Id] == null ? developers[sender.Id].GetName() : "null", (string)rec.other);
                break;

            case DataRecieveType.serverInitialize:
                InitializeServer(rec, sender);
                break;
            default:
                Debug.Assert(false);
                break;
        }
    }

    #region Data Reactions

    void AddNewDeveloper(NetworkData rec, int id) { developers[id] = (Developer)rec.other; }

    void UpdateExistingDeveloper(NetworkData rec, int id) { developers[id] = (Developer)rec.other; }

    void HandleDeveloperMessage(string name, string msg) { Debug.Log(name + " said: " + msg); }

    void InitializeServer(NetworkData rec, NetPeer sender)
    {
        server = sender;
        if (displayDebugMessages) { Debug.Log("Message from server: " + (string)rec.other); }
        SendInitialData(); // initialze server 
    }

    #endregion

    public void SendMessage(string message)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();

        NetworkData dat = new NetworkData
        {
            type = Network.DataRecieveType.developerMessage,
            other = message
        };
        bf.Serialize(ms, dat);
        byte[] data = ms.ToArray();

        server?.Send(data, DeliveryMethod.ReliableOrdered);
    }

    public void DisplayDebugMessages(bool val) { displayDebugMessages = val; }
}