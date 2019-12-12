using System;

namespace Network
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
            while (true) { server.Run(); }
            Console.ReadLine();
        }
    }
}
