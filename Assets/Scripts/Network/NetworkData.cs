using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;

/// <summary>
/// type of data sent across the network
/// </summary>
public enum NetworkDataType
{
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
        new NetworkDataSize(NetworkDataType.developer, 127), // check that this works
        new NetworkDataSize(NetworkDataType.message, 0)
    };
}
