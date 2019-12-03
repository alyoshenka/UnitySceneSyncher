using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Threading;

public class NetworkTest : MonoBehaviour
{
    Client client;
    Server server;

    private void Start()
    {
        client = new Client();
        server = new Server();

        client.Start();
        server.Start();

        Thread clientThread = new Thread(client.Run);
        Thread serverThread = new Thread(server.Run);

        clientThread.Start();
        serverThread.Start();

        Debug.Log("started client and server threads");
    }
}
