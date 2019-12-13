using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevDisplay : MonoBehaviour
{
    public Client client;

    private void OnDrawGizmos()
    {
        if(null == client)
        {
            Debug.LogWarning("Cannot find list of devs");
            return;
        }

        Gizmos.color = new Color(222, 0, 255);
        foreach (Network.Developer dev in client.developers)
        {
            if(null == dev) { continue; }

            Gizmos.DrawSphere(dev.GetPosition(), 5);
        }
    }
}
