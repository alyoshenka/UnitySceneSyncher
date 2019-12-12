using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevDisplay : MonoBehaviour
{
    Network.Developer[] developers;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(222, 0, 255);
        foreach (Network.Developer dev in developers)
        {
            if(null == dev) { continue; }

            Gizmos.DrawSphere(dev.GetPosition(), 5);
        }
    }
}
