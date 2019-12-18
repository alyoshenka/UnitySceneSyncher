using LiteNetLib;

namespace Network
{
    public abstract class NetworkConnection
    {
        protected EventBasedNetListener listener;
        public NetworkSettings settings;

        public Developer[] developers;

        public abstract void Start();
        public abstract void Run();
        public abstract void Stop();
        public abstract void DealWithRecievedData(NetworkData rec, NetPeer sender);
    }
}
