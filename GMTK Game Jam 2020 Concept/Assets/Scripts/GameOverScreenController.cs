using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverScreenController : MonoBehaviour {

    
    [SerializeField] private Text gameOverText;

    public void ShowGameOver() {
        try {
            gameObject.SetActive(true);
            //Time.timeScale = 0;
        } catch {

        }
        
    }

    public void HideGameOver() {
        try {
            gameObject.SetActive(false);
            Time.timeScale = 1;
        } catch {

        }
    }

    private void Awake() {
        ScoreTracker.Instance.GameStatusChanged += OnGameStatusChanged;
        gameObject.SetActive(false);
    }


    private void Update() {
        if (ScoreTracker.Instance.GameStatus == GameStatus.GAMEOVER) {
            if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Cancel")) {
                SceneManager.LoadScene(ScoreTracker.Instance.mainMenuScene);
            }
        }
    }

    private void OnGameStatusChanged(object sender, GameStatusEventArgs e) {
        Debug.Log("GameOverScreenController::OnGameStatusChanged");
        if (e.NewStatus == GameStatus.GAMEOVER) {
            ShowGameOver();
        } else {
            HideGameOver();
        }
    }
}
