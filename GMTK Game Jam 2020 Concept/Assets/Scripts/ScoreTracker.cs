using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public enum GameStatus {
    MENU,
    PLAYING,
    HIGHSCORE,
    GAMEOVER
}


public class ScoreTracker : MonoBehaviour {

    public static ScoreTracker Instance { get; private set; }


    public int mainMenuScene;
    public int firstPlayScene;
    public int lastPlayScene;
    public int highscoreScene;
    public int gameOverScene;

    private GameStatus gameStatus = GameStatus.MENU;
    public GameStatus GameStatus {
        get => gameStatus; set {
            if (gameStatus == value) {
                return;
            }
            GameStatus oldValue = gameStatus;
            gameStatus = value;
            OnGameStatusChanged(oldValue, value);
        }
    }

    [SerializeField]
    public float DelayedStartPerLevel { get; } = 0.5f;


    /* EVENTS */

    public event EventHandler<GameStatusEventArgs> GameStatusChanged;

    protected virtual void OnGameStatusChanged(GameStatus oldStatus, GameStatus newStatus) {
        GameStatusChanged?.Invoke(this, new GameStatusEventArgs(oldStatus, newStatus));

        Debug.Log("OnGameStatusChanged: " + oldStatus + ", " + newStatus);

        if (newStatus == GameStatus.MENU) {
            ResetTimer();
        }
        if (newStatus == GameStatus.PLAYING) {
            HUD.gameObject.SetActive(true);
        }
        if (newStatus != GameStatus.PLAYING) {
            HUD.gameObject.SetActive(false);
        }/*
        if (newStatus == GameStatus.HIGHSCORE) {
            ShowHighscore();
        }
        if (newStatus != GameStatus.HIGHSCORE) {
            HideHighscore();
        }*/
    }

    private void OnActiveSceneChanged(Scene arg0, Scene arg1) {
        Debug.Log("OnActiveSceneChanged: " + arg0.buildIndex + ", " + arg1.buildIndex);
        if (arg1.buildIndex == mainMenuScene) {
            GameStatus = GameStatus.MENU;
        } else if (arg1.buildIndex >= firstPlayScene && arg1.buildIndex <= lastPlayScene) {
            GameStatus = GameStatus.PLAYING;
        } else if (arg1.buildIndex == highscoreScene) {
            GameStatus = GameStatus.HIGHSCORE;
        } else if (arg1.buildIndex == gameOverScene) {
            GameStatus = GameStatus.GAMEOVER;
        } else {
            Debug.Log("Scene index error: " + arg1.buildIndex);
        }

        if (GameStatus == GameStatus.PLAYING) {
            PauseTimer();
        }
    }


    /* TIMER */

    [SerializeField] private Canvas HUD;
    [SerializeField] private TextMeshProUGUI timerText;

    public float ScoreTimer { get; private set; }
    public string ScoreTimerAsText {
        get {
            if (ScoreTimer >= 60) {
                if (ScoreTimer >= 10 * 60) {
                    return TimeSpan.FromSeconds(ScoreTimer).ToString(@"hh\:mm\:ss\:ff");
                }
                return TimeSpan.FromSeconds(ScoreTimer).ToString(@"m\:ss\:ff");
            }
            return TimeSpan.FromSeconds(ScoreTimer).ToString(@"ss\:ff");
        }
    }
    private bool timerIsRunning = false;


    public void ResetTimer() {
        timerIsRunning = false;
        ScoreTimer = 0;
    }

    public void StartTimer() {
        timerIsRunning = true;
    }

    public void PauseTimer() {
        timerIsRunning = false;
    }


    public static bool IsAnyMovingKeyDown() {
        return Input.GetKeyDown(KeyCode.UpArrow) ||
            Input.GetKeyDown(KeyCode.DownArrow) ||
            Input.GetKeyDown(KeyCode.LeftArrow) ||
            Input.GetKeyDown(KeyCode.RightArrow) ||
            Input.GetKeyDown(KeyCode.W) ||
            Input.GetKeyDown(KeyCode.A) ||
            Input.GetKeyDown(KeyCode.S) ||
            Input.GetKeyDown(KeyCode.D);
    }


    /* START */

    private void Awake() {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("ScoreTracker");

        if (objs.Length > 1) {
            Destroy(this.gameObject);
        } else {
            DontDestroyOnLoad(this.gameObject);
            Instance = this;
            HUD.gameObject.SetActive(false);
        }


    }

    void Start() {
        Debug.Log("ScoreTracker Start");
        SceneManager.activeSceneChanged += OnActiveSceneChanged;
    }

    void Update() {
        if (timerIsRunning) {
            ScoreTimer += Time.deltaTime;
            timerText.text = "<mark=#a1a1a1aa>Time: " + ScoreTimerAsText + "</mark>"; // TODO: better formatting
        } else {
            if (GameStatus == GameStatus.PLAYING) {
                if (Time.timeSinceLevelLoad > DelayedStartPerLevel && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)) {
                    Debug.Log("ScoreTracker: ButtonDown");
                    StartTimer();
                }
            }
        }
    }

}




public class GameStatusEventArgs {
    public GameStatusEventArgs(GameStatus oldStatus, GameStatus newStatus) {
        this.OldStatus = oldStatus;
        this.NewStatus = newStatus;
    }

    public GameStatus NewStatus { get; }

    public GameStatus OldStatus { get; }
}