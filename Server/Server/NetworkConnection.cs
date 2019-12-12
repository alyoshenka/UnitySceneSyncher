using LiteNetLib;

namespace Network
{
    public abstract class NetworkConnection
    {
        protected const int maxPeerCount = 10;

        protected EventBasedNetListener listener;
        protected int port;
        protected string connectionKey;

        protected Developer[] developers;

        public abstract void Start();
        public abstract void Run();
        public abstract void Stop();
    }
}
