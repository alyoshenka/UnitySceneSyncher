using System;

using LiteNetLib;
using System.Threading;
using LiteNetLib.Utils;

namespace LiteNetLibTest
{
    class Client
    {
        static NetPeer server;

        static void Main(string[] args)
        {
            EventBasedNetListener listener = new EventBasedNetListener();
            NetManager client = new NetManager(listener);
            client.Start();
            client.Connect("localhost", 9050, "Key");
            listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod) =>
            {
                Console.WriteLine("We got {0}", dataReader.GetString(100));
                dataReader.Recycle();

                server = fromPeer;
            };

            // Thread.Sleep(200);

            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                client.PollEvents();
                // Thread.Sleep(15);

                string msg = Console.ReadLine();
                if(Console.ReadKey().Key == ConsoleKey.Enter)
                {
                    NetDataWriter writer = new NetDataWriter();
                    writer.Put(msg);
                    server?.Send(writer, DeliveryMethod.ReliableOrdered);
                }
            }

            client.Stop();
        }
    }
}

