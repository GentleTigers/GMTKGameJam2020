using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class World : MonoBehaviour {


    [SerializeField] private float degressionTotalTime = 1f;
    [SerializeField] private float imuneToHealthyTotalTime = 5f;
    public float DegressionTotalTime { get => degressionTotalTime; set => degressionTotalTime = value; }
    public float ImuneToHealthyTotalTime { get => imuneToHealthyTotalTime; set => imuneToHealthyTotalTime = value; }

    [SerializeField] private int numberOfInfectedNeededForWin;
    public int NumberOfInfectedNeededForWin {
        get => numberOfInfectedNeededForWin > 0 ? numberOfInfectedNeededForWin : HumanGOsInThisLevel.Count;
        set => numberOfInfectedNeededForWin = value;
    }

    private List<GameObject> humanGOsInThisLevel;
    public List<GameObject> HumanGOsInThisLevel {
        get {
            if (humanGOsInThisLevel == null) {
                humanGOsInThisLevel = new List<GameObject>();
                for (int i = 0; i < transform.childCount; i++) {
                    GameObject child = transform.GetChild(i).gameObject;
                    Human human = child.GetComponent<Human>();
                    if (human.Status != HumanStatus.Doctor || human.Status != HumanStatus.Dead) {
                        humanGOsInThisLevel.Add(child);
                    }
                }
                
            } 
            return humanGOsInThisLevel;
        }
    }




    // Start is called before the first frame update
    void Start() {
        foreach (var human in GetHumenInThisLevel()) {
            human.StatusChanged += OnStatusChanged;
            human.StageChanged += OnStageChanged;
        }
    }

    private void Update() {
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.I) && Input.GetKeyDown(KeyCode.N)) { // CHEATCODE
            DoLevelWin();
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
            ScoreTracker.Instance.GameStatus = GameStatus.GAMEOVER;
        } else if (CheckForLevelWin()) {
            DoLevelWin();
        }
    }

    private void DoLevelWin() {
        LoadNextLevel();
        Debug.Log("LEVEL WON!");
    }

    private bool CheckForLevelWin() {
        int infected = 0;
        foreach (var human in GetHumenInThisLevel()) {
            if (human.Status == HumanStatus.Infected) {
                infected++;
            }
        }
        return infected >= NumberOfInfectedNeededForWin;
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
        enoughNotDeadHumans = notDeadHumans >= NumberOfInfectedNeededForWin;

        return !(enoughNotDeadHumans && atLeastOneInfectedLeft);
    }



    private void LoadNextLevel() {
        try {
            Debug.Log(SceneManager.GetSceneByBuildIndex(SceneManager.GetActiveScene().buildIndex + 1));
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        } catch (Exception ex) {
            SceneManager.LoadScene(0);
        }
    }

    public List<Human> GetHumenInThisLevel() {
        List<Human> humanList = new List<Human>();
        foreach (var humanGO in HumanGOsInThisLevel) {
            humanList.Add(humanGO.GetComponent<Human>());
        }
        return humanList;
    }
}
