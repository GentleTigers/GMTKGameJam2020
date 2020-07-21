using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointCollection : MonoBehaviour {

    public static float WAYPOINT_RADIUS = 0.2f;

    private readonly List<Waypoint> waypoints = new List<Waypoint>();
    public List<Waypoint> Waypoints => waypoints;

    private readonly List<GameObject> waypointGameObjects = new List<GameObject>();
    public List<GameObject> WaypointGameObjects => waypointGameObjects;
    private int index = 0;

    // Start is called before the first frame update
    void Start() {
        Waypoints.AddRange(gameObject.GetComponentsInChildren<Waypoint>());
        foreach (var waypoint in Waypoints) {
            WaypointGameObjects.Add(waypoint.gameObject);
        }
    }

    internal GameObject getNextWaypoint() {
        if (WaypointGameObjects.Count > 0) {
            GameObject waypoint = WaypointGameObjects[index++];
            if (index >= WaypointGameObjects.Count) {
                index = 0;
            }
            return waypoint;
        }
        return null;
    }

    internal GameObject getCurrentWaypoint() {
        if (WaypointGameObjects.Count > 0) {
            return WaypointGameObjects[index];
        }
        return null;
    }
}
