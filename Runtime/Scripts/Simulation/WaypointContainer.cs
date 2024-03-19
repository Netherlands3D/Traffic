using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Netherlands3D.Coordinates;
using SimpleJSON;
using System.Linq;
using Netherlands3D.Traffic.Simulation;
using System;


namespace Netherlands3D.Traffic.Simulation
{
    public class WaypointContainer : MonoBehaviour
    {
        public List<Transform> waypoints;

        private void Awake()
        {
            foreach(Transform tr in gameObject.GetComponentsInChildren<Transform>())
            {
                if (tr != transform) // Only add the child transforms
                {
                    waypoints.Add(tr);
                }
            }
        }
    }
}