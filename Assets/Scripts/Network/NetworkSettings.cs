using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.Serialization.Json;
using System.IO;

// if file not found, regen with default settings

namespace Network
{
    public class NetworkSettings
    {
        public const string fileLocation = "settings.json";

        public const string defServerAddress = "localhost";
        public const int defMaxPeerCount = 10;
        public const string defConnectionKey = "SomeConnectionKey";
        public const int defPort = 9050;

        public string serverAddress;
        public int maxPeerCount;
        public string connectionKey;
        public int port;

        [NonSerialized] public bool saved; // is the current instance saved?

        public NetworkSettings() { DefaultInitialize(); }

        /// <summary>
        /// saves data to file
        /// </summary>
        public void Save()
        {
            using (Stream stream = File.OpenWrite(fileLocation))
            {           
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(NetworkSettings));
                ser.WriteObject(stream, this);
            }

            saved = true;
        }

        /// <summary>
        /// loads data from file
        /// </summary>
        public void Load()
        {
            using (Stream stream = File.OpenRead(fileLocation))
            {
                if(null != stream)
                {
                    DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(NetworkSettings));
                    NetworkSettings newSet = (NetworkSettings)ser.ReadObject(stream);
                    SetToNew(newSet);
                }
                else { DefaultInitialize(); }
            }
        }

        /// <summary>
        /// sets all values to defaults
        /// </summary>
        public void DefaultInitialize()
        {
            serverAddress = defServerAddress;
            maxPeerCount = defMaxPeerCount;
            connectionKey = defConnectionKey;
            port = defPort;
        }

        /// <summary>
        /// set values to those of other Settings
        /// </summary>
        void SetToNew(NetworkSettings newSet)
        {
            serverAddress = newSet.serverAddress;
            maxPeerCount = newSet.maxPeerCount;
            connectionKey = newSet.connectionKey;
            port = newSet.port;
        }
    }
}
