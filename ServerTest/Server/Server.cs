using System;

using LiteNetLib;
using System.Threading;
using LiteNetLib.Utils;

namespace LiteNetLibTest
{
    class Server
    {
        static void Main(string[] args)
        {
            EventBasedNetListener listener = new EventBasedNetListener();
            NetManager server = new NetManager(listener);
            server.Start(9050);

            listener.ConnectionRequestEvent += request =>
            {
                if (server.PeersCount < 10) { request.AcceptIfKey("Key"); }
                else { request.Reject(); }
            };

            listener.PeerConnectedEvent += peer =>
            {
                Console.WriteLine("We got connection: {0}", peer.EndPoint);
                NetDataWriter writer = new NetDataWriter();
                writer.Put("Hello client!");
                peer.Send(writer, DeliveryMethod.ReliableOrdered);
            };

            listener.NetworkReceiveEvent += Listener_NetworkReceiveEvent;

            while (true)
            {
                server.PollEvents();
                Thread.Sleep(15);
            }

            server.Stop();
        }

        private static void Listener_NetworkReceiveEvent(NetPeer peer, NetPacketReader reader, DeliveryMethod deliveryMethod)
        {
            Console.WriteLine("Recieved message: {2} of type {0} from peer {1}", deliveryMethod.ToString(), peer.Id, reader.GetString());
        }
    }
}
