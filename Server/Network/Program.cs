using System;

namespace Network
{
    class Program
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) => { server.Stop(); };

            server.Start();
            while (true) { server.Run(); }
        }
    }
}
