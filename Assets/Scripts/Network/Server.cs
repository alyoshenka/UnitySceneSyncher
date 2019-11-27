using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

public class Server : MonoBehaviour
{
    Socket s;
    IPAddress broadcast;

    void Start()
    {
        s = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        broadcast = IPAddress.Parse("192.168.0.4");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            byte[] sendbuf = Encoding.ASCII.GetBytes("boop");
            IPEndPoint ep = new IPEndPoint(broadcast, 11000);

            s.SendTo(sendbuf, ep);

            Debug.Log("message sent");
        }
    }
}
