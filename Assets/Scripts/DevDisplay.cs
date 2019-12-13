using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// displays dev data in the current scene
/// </summary>
public class DevDisplay : MonoBehaviour
{
    public int devDisplaySize = 2; // make editor
    public static Client client;

    public void CheckForClient()
    {
        if (null == client) { Debug.LogWarning("Cannot find list of devs"); }
    }

    private void OnDrawGizmos()
    {
        if(null != client)
        {
            foreach (Network.Developer dev in client.developers)
            {
                if (null == dev) { continue; }

                Gizmos.color = dev.GetDisplayColor();
                Gizmos.DrawSphere(dev.GetPosition(), devDisplaySize);
            }
        }
    }
}
