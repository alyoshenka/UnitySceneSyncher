using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Runtime.InteropServices;

/// <summary>
/// type of data sent across the network
/// </summary>
public enum DataRecieveType : byte
{
    // something something about these being bitwise
    // so i can say which bit of info i am sending


    developerAdd,       // add a new developer connection
    developerUpdate,    // update an existing developer connection
    developerMessage,   // send a developer message
    
    test
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
