using System;

namespace Network
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.Start();
            while (Console.ReadKey().Key != ConsoleKey.Escape) { server.Run(); }
            server.Stop();
            Console.ReadLine();
        }
    }
}
