using LiteNetLib;
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
    public static string serverDirectory = "SceneSyncherServer"; // directory for server values to be stored

    // change to all events
    public delegate void ConnectionRecieveEvent();
    public event ConnectionRecieveEvent developerUpdate;

    NetManager client;
    NetPeer server = null;

    public bool isRunning { get => null != server; }

    int threadSleepTime = 15; // ms
    public Developer myDeveloper = null;
    bool displayDebugMessages;

    bool shouldRun;

    bool updatePending; // ???

    private Client() { }

    public Client(string devName, NetworkSettings set)
    {
        if (null == set) { set = new NetworkSettings(); }
        Debug.Assert(null != set);

        shouldRun = true;

        // change
        settings = set;

        listener = new EventBasedNetListener();
        client = new NetManager(listener);

        developers = new Developer[settings.maxPeerCount];
        for(int i = 0; i < settings.maxPeerCount; i++) { developers[i] = null; }
        myDeveloper = new Developer(devName);
        myDeveloper.SetProjectName(Application.productName);

        developerUpdate += DeveloperUpdateNotification;
    }

    public override void Start()
    {
        client.Start();
        client.Connect(settings.serverAddress, settings.port, settings.connectionKey);
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
            Thread.Sleep(threadSleepTime);
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
        if (null == EditorWindow.focusedWindow) { myDeveloper.SetCurrentTab("null"); }      
        else { myDeveloper.SetCurrentTab(EditorWindow.focusedWindow.titleContent.text); }
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
            type = DataRecieveType.developerInitialize,
            other = myDeveloper
        };
        bf.Serialize(ms, dat);
        byte[] data = ms.ToArray();

        server.Send(data, DeliveryMethod.ReliableOrdered);
    }

    void SendDeveloperData()
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();

        NetworkData dat = new NetworkData
        {
            type = DataRecieveType.developerAdd,
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
        if(null == server)
        {
            if (displayDebugMessages) { Debug.Log("Cannot send data, not connected to server"); }
            return;
        }

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
            case DataRecieveType.developerInitialize:
                InitializeNewDeveloper(rec, sender);
                break;
            case DataRecieveType.developerAdd:
                AddNewDeveloper(rec);
                break;
            case DataRecieveType.developerUpdate:
                UpdateExistingDeveloper(rec);
                developerUpdate.Invoke();
                break;
            case DataRecieveType.developerDelete:
                developers[(int)rec.other] = null;
                if (displayDebugMessages) { Debug.Log("Developer " + (int)rec.other + " disconnected"); }
                break;
            case DataRecieveType.developerMessage:
                HandleDeveloperMessage((string)rec.other);         
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

    void InitializeServer(NetworkData rec, NetPeer sender)
    {
        server = sender;
        if (displayDebugMessages) { Debug.Log("Server said: " + (string)rec.other); }
        SendInitialData(); // initialze server 
    }

    void InitializeNewDeveloper(NetworkData rec, NetPeer sender)
    { 
        myDeveloper = (Developer)rec.other;
        developers[myDeveloper.GetArrIdx()] = myDeveloper;
        if (displayDebugMessages) { Debug.Log("Developer initialized to " + myDeveloper.GetArrIdx()); }

        SendDeveloperData(); // give server the developer
    }

    void AddNewDeveloper(NetworkData rec)
    {
        Developer newDev = (Developer)rec.other;
        developers[newDev.GetArrIdx()] = newDev;

        Debug.Assert(null != developers[newDev.GetArrIdx()]);
        if (displayDebugMessages) { Debug.Log("Added new developer: " + newDev.GetName()); }
    }

    void UpdateExistingDeveloper(NetworkData rec)
    {
        Debug.Assert(null != developers);

        Developer newDev = (Developer)rec.other;
        developers[newDev.GetArrIdx()] = newDev;

        Debug.Assert(null != developers[newDev.GetArrIdx()]);
        if (displayDebugMessages) { Debug.Log("Updated dev " + newDev.GetArrIdx() + ": " + newDev.GetName()); }
    }

    void HandleDeveloperMessage(string msg) { Debug.Log(msg); }

    #endregion

    public void SendMessage(string message)
    {
        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();

        NetworkData dat = new NetworkData
        {
            type = Network.DataRecieveType.developerMessage,
            other = myDeveloper.GetName() + " said: " + message
        };
        bf.Serialize(ms, dat);
        byte[] data = ms.ToArray();

        server?.Send(data, DeliveryMethod.ReliableOrdered);
    }

    public void DisplayDebugMessages(bool val) { displayDebugMessages = val; }

    void DeveloperUpdateNotification() { if (displayDebugMessages) { Debug.Log("Developer update event"); } }
}