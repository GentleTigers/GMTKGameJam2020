using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointCollection : MonoBehaviour {

    public static float WAYPOINT_RADIUS = 0.2f;

    /// <summary>
    /// If set to true and loop set to false, then the Human will look at this direction if the movement ends.
    /// </summary>
    [SerializeField] private Vector2 endLookDirection;
    [SerializeField] private bool useEndLookDirection;
    [SerializeField] private bool loop = true;
    private bool loopEnded = false;
    public bool Loop { get => loop; private set => loop = value; }
    public bool LoopEnded { get => loopEnded; set => loopEnded = value; }
    public bool UseEndLookDirection { get => useEndLookDirection; set => useEndLookDirection = value; }
    public Vector2 EndLookDirection { get => endLookDirection; set => endLookDirection = value; }


    private readonly List<Waypoint> waypoints = new List<Waypoint>();
    private readonly List<GameObject> waypointGameObjects = new List<GameObject>();
    public List<Waypoint> Waypoints => waypoints;
    public List<GameObject> WaypointGameObjects => waypointGameObjects;


    private int index = 0;


    // Start is called before the first frame update
    void Start() {
        Waypoints.AddRange(gameObject.GetComponentsInChildren<Waypoint>());
        foreach (var waypoint in Waypoints) {
            WaypointGameObjects.Add(waypoint.gameObject);
        }
    }

    internal GameObject GetNextWaypoint() {
        if (WaypointGameObjects.Count > 0) {
            GameObject waypoint = WaypointGameObjects[index++];
            if (index >= WaypointGameObjects.Count) {
                if (loop) {
                    index = 0;
                } else {
                    loopEnded = true;
                    return null;
                }
            }
            if (Waypoints[index].Active == false) {
                return GetNextWaypoint();
            }
            return waypoint;
        }
        Debug.LogWarning("Should never reach this point! Always have at least one Waypoint in a WaypointCollection!");
        return null;
    }

    internal GameObject getCurrentWaypointObject() {
        while (Waypoints[index].Active == false) {
            index++;
        }
        if (WaypointGameObjects.Count > 0) {
            return WaypointGameObjects[index];
        }
        return null;
    }

    internal Waypoint getCurrentWaypoint() {
        if (WaypointGameObjects.Count > 0) {
            return Waypoints[index];
        }
        return null;
    }
}
