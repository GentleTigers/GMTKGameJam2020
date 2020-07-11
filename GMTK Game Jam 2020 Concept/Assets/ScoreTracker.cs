using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTracker : MonoBehaviour {

    private void Awake() {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("ScoreTracker");

        if (objs.Length > 1) {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start() {
        Debug.Log("ScoreTracker Start");
    }

    // Update is called once per frame
    void Update() {

    }
}
