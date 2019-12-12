using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;

/// <summary>
/// type of data sent across the network
/// </summary>
public enum NetworkDataType
{
    // something something about these being bitwise
    // so i can say which bit of info i am sending


    developer, // full developer data
    message // developer message (string)
}

/// <summary>
/// used to determine the number of bytes to read
/// from the recieved data
/// </summary>
public struct NetworkDataSize
{
    public NetworkDataType type;
    public int size;

    public NetworkDataSize(NetworkDataType _type, int _size)
    {
        type = _type;
        size = _size;
    }

    public static NetworkDataSize[] array = new NetworkDataSize[]
    {
        new NetworkDataSize(NetworkDataType.developer, 306), // check that this works
        new NetworkDataSize(NetworkDataType.message, 0)
    };
}

#region Serializable Structs

[System.Serializable]
public struct Vector3_S
{
    public float x, y, z;

    public Vector3_S(Vector3 orig)
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
public struct Quaternion_S
{
    public float x, y, z, w;

    public Quaternion_S(Quaternion orig)
    {
        x = orig.x;
        y = orig.y;
        z = orig.z;
        w = orig.w;
    }

    public Quaternion ToQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }
}

#endregion
