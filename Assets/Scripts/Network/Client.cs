using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

using System.Threading;

public class Client : MonoBehaviour
{
    const int listenPort = 11000;

    UdpClient listener;
    IPEndPoint groupEP;

    Thread worker;

    private void Start()
    {
        listener = new UdpClient(listenPort);
        groupEP = new IPEndPoint(IPAddress.Any, listenPort);

        worker = new Thread(Go);
        worker.Start();

        Debug.Log("listener started");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { worker.Abort(); }  
    }

    void Go()
    {
        try
        {
            while (true)
            {
                Debug.Log("Waiting for broadcast");
                byte[] bytes = listener.Receive(ref groupEP);

                Debug.Log("Recieved from " + groupEP);
                Debug.Log("Message: " + Encoding.ASCII.GetString(bytes, 0, bytes.Length));
            }
        }
        catch (SocketException e) { Debug.LogError(e); }
        finally { listener.Close(); }

        Debug.Log("listener closed");
    }

    void Boop()
    {
        while (true) { Debug.Log("boop"); }
    }
}
