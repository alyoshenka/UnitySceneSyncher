using UnityEngine;
using System.Runtime.InteropServices;

namespace Network
{

    /// <summary>
    /// type of data sent across the network
    /// </summary>
    public enum DataRecieveType : byte
    {
        // something something about these being bitwise
        // so i can say which bit of info i am sending

        developerInitialize,    // initialize current developer
        developerAdd,           // add a new developer connection
        developerUpdate,        // update an existing developer connection
        developerMessage,       // send a developer message

        serverInitialize        // initialize server connection
    }
    
    [System.Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct NetworkData
    {
        public DataRecieveType type;
        public object other;
    }

    #region Serializable Structs

    [System.Serializable]
    public struct float3_S
    {
        public float x, y, z;

        public float3_S(Vector3 orig)
        {
            x = orig.x;
            y = orig.y;
            z = orig.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [System.Serializable]
    public struct float4_S
    {
        public float x, y, z, w;

        public float4_S(Quaternion orig)
        {
            x = orig.x;
            y = orig.y;
            z = orig.z;
            w = orig.w;
        }

        public float4_S(Color orig)
        {
            x = orig.r;
            y = orig.g;
            z = orig.b;
            w = orig.a;
        }

        public Quaternion ToQuaternion()
        {
            return new Quaternion(x, y, z, w);
        }

        public Color ToColor()
        {
            return new Color(x, y, z, w);
        }
    }

    #endregion

}