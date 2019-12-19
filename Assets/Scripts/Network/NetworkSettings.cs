using System;
using System.Collections.Generic;
using System.Text;

using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;

using UnityEngine;

// if file not found, regen with default settings

namespace Network
{
    [DataContract]
    public class NetworkSettings
    {
        public const string fileLocation = "settings.json";

        public const string defServerAddress = "localhost";
        public const int defMaxPeerCount = 10;
        public const string defConnectionKey = "SomeConnectionKey";
        public const int defPort = 9050;

        [DataMember] public string serverAddress;
        [DataMember] public int maxPeerCount;
        [DataMember] public string connectionKey;
        [DataMember] public int port;

        [IgnoreDataMember] public bool saved; // is the current instance saved?

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
        public void Load(string fileName)
        {
            try
            {
                Stream stream = File.OpenRead(fileName);
                stream.Position = 0;
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(NetworkSettings));
                NetworkSettings newSet = ser.ReadObject(stream) as NetworkSettings;
                SetToNew(newSet);
                stream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error\n" + e);
                DefaultInitialize();
            }
        }

        public void Load() { Load(fileLocation); }

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

        public string DisplayString()
        {
            string s = "Adr: " + serverAddress;
            s += "\nCnt: " + maxPeerCount;
            s += "\nKey: " + connectionKey;
            s += "\nPrt: " + port;
            return s;
        }
    }
}
