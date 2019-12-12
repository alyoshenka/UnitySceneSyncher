using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevDisplay : MonoBehaviour
{
    Developer[] developers;

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(222, 0, 255);
        foreach (Developer dev in developers)
        {
            if(null == dev) { continue; }

            Gizmos.DrawSphere(dev.GetPosition(), 5);
        }
    }
}
