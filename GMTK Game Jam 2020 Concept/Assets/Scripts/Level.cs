using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour {


    [SerializeField] private float degressionTotalTime = 1f;
    [SerializeField] private float imuneToHealthyTotalTime = 5f;
    public float DegressionTotalTime { get => degressionTotalTime; set => degressionTotalTime = value; }
    public float ImuneToHealthyTotalTime { get => imuneToHealthyTotalTime; set => imuneToHealthyTotalTime = value; }

    public List<GameObject> humanGOsInThisLevel;

    [SerializeField] private int numberOfInfectedNeededForWin;


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
        int infected = 0;
        foreach (var human in GetHumenInThisLevel()) {
            if (human.Status == HumanStatus.Infected) {
                infected++;
            }
        }
        return infected >= numberOfInfectedNeededForWin;
    }

    private bool CheckForGameOver() {
        bool atLeastOneInfectedLeft = false;
        bool enoughNotDeadHumans = false;

        int notDeadHumans = 0;
        foreach (var human in GetHumenInThisLevel()) {
            if (human.Status == HumanStatus.Infected) {
                atLeastOneInfectedLeft = true;
            }
            if (human.Status != HumanStatus.Dead) {
                notDeadHumans++;
            }
        }
        enoughNotDeadHumans = notDeadHumans >= numberOfInfectedNeededForWin;

        return !(enoughNotDeadHumans && atLeastOneInfectedLeft);
    }


    public List<Human> GetHumenInThisLevel() {
        List<Human> humanList = new List<Human>();
        foreach (var humanGO in humanGOsInThisLevel) {
            humanList.Add(humanGO.GetComponent<Human>());
        }
        return humanList;
    }
}
