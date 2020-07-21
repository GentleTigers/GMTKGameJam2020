using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoint : MonoBehaviour {

    [SerializeField] private int waitTime;
    [SerializeField] private Vector2 waitDirection;
    [SerializeField] private bool useWaitDirection;
    [SerializeField] private bool active;

    public int WaitTime { get => waitTime; }
    public Vector2 WaitDirection { get => waitDirection; }
    public bool UseWaitDirection { get => useWaitDirection; }
    public bool Active { get => active; }
}
