using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum GameStatus {
    MENU,
    PLAYING,
    HIGHSCORE,
    GAMEOVER
}

public class ScoreTracker : MonoBehaviour {

    public static ScoreTracker Instance { get; private set; }


    private GameStatus gameStatus = GameStatus.MENU;
    public GameStatus GameStatus {
        get => gameStatus; set {
            GameStatus oldValue = gameStatus;
            gameStatus = value;
            OnGameStatusChanged(oldValue, value);
        }
    }
    [SerializeField] private Canvas HUD;
    [SerializeField] private Text timerText;
    public float Timer { get; private set; }
    private bool timerIsRunning = false;

    public event EventHandler<GameStatusEventArgs> GameStatusChanged;

    protected virtual void OnGameStatusChanged(GameStatus oldStatus, GameStatus newStatus) {
        GameStatusChanged?.Invoke(this, new GameStatusEventArgs(oldStatus, newStatus));

        Debug.Log("OnGameStatusChanged: " + oldStatus + ", " + newStatus);

        if (newStatus == GameStatus.MENU) {
            ResetTimer();
            HUD.gameObject.SetActive(false);
        } else if (newStatus == GameStatus.PLAYING) {
            HUD.gameObject.SetActive(true);
            StartTimer();
        }
    }

    public void ResetTimer() {
        timerIsRunning = false;
        Timer = 0;
    }

    public void StartTimer() {
        timerIsRunning = true;
    }

    public void PauseTimer() {
        timerIsRunning = false;
    }


    private void Awake() {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("ScoreTracker");

        if (objs.Length > 1) {
            Destroy(this.gameObject);
        } else {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
        }

        
    }

    private void OnActiveSceneChanged(Scene arg0, Scene arg1) {
        Debug.Log("OnActiveSceneChanged: " + arg0.buildIndex + ", " + arg1.buildIndex);
        if (arg1.buildIndex != 0) {
            GameStatus = GameStatus.PLAYING;
        } else if (arg1.buildIndex == 0) {
            GameStatus = GameStatus.MENU;
        }
        // TODO Add for Highscore and Gameover screen.
    }

    void Start() {
        Debug.Log("ScoreTracker Start");
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    void Update() {
        if (timerIsRunning) {
            Timer += Time.deltaTime;
            timerText.text = "Time: " + Timer; // TODO: better formatting
        }
    }

}




public class GameStatusEventArgs {
    GameStatus oldStatus;
    GameStatus newStatus;

    public GameStatusEventArgs(GameStatus oldStatus, GameStatus newStatus) {
        this.oldStatus = oldStatus;
        this.newStatus = newStatus;
    }
}