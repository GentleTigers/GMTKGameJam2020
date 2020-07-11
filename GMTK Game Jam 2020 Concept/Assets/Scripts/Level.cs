using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {

    public List<GameObject> humanGOsInThisLevel;
    public GameObject levelManager;
    public LevelManager LevelManager { get { return levelManager.GetComponent<LevelManager>(); } }

    // Start is called before the first frame update
    void Start() {
        foreach (var human in GetHumenInThisLevel()) {
            human.StatusChanged += OnStatusChanged;
            human.StageChanged += OnStageChanged;
        }
    }

    

    private void OnStatusChanged(object sender, StatusChangedEventArgs e) {
        OnHumanChanged();
    }

    private void OnStageChanged(object sender, StageChangedEventArgs e) {
        OnHumanChanged();
    }

    private void OnHumanChanged() {
        if (CheckForGameOver()) {
            Debug.Log("GAME OVER");
        } else if (CheckForLevelWin()) {
            Debug.Log("LEVEL WON!");
        }
    }

    private bool CheckForLevelWin() {
        foreach (var human in GetHumenInThisLevel()) {
            if (human.Status != HumanStatus.Infected) {
                return false;
            }
        }
        return true;
    }

    private bool CheckForGameOver() {
        foreach (var human in GetHumenInThisLevel()) {
            if (human.Status == HumanStatus.Infected) {
                return false;
            }
        }
        return true;
    }


    public List<Human> GetHumenInThisLevel() {
        List<Human> humanList = new List<Human>();
        foreach (var humanGO in humanGOsInThisLevel) {
            humanList.Add(humanGO.GetComponent<Human>());
        }
        return humanList;
    }
}
