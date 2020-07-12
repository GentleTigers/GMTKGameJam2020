using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class HighscoreScreenController : MonoBehaviour {


    /* HIGHSCORE */

    [SerializeField] private Canvas highscoreScreen;
    [SerializeField] private Text highscoreText;
    [SerializeField] private Text totalHighscoreText;


    public string CurrentScoreAsText {
        get => ScoreTracker.Instance.ScoreTimerAsText;
    }

    public float CurrentScore {
        get => ScoreTracker.Instance.ScoreTimer;
    }

    public void ShowHighscore() {
        highscoreText.text = "Your Time:\n" + CurrentScoreAsText;

        float savedScore = ScoreTracker.Instance.Higscore;
        if (savedScore < 0 || savedScore > CurrentScore) {
            ScoreTracker.Instance.Higscore = CurrentScore;
            totalHighscoreText.text = "NEW HIGHSCORE!\nPrevious Time: " + ScoreTracker.GetTimeInNiceFormat(savedScore);
        } else {
            totalHighscoreText.text = "Best Time:\n" + ScoreTracker.GetTimeInNiceFormat(savedScore);
        }
        highscoreScreen.gameObject.SetActive(true);
    }


    // Start is called before the first frame update
    void Start() {
        ShowHighscore();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetButtonDown("Submit") || Input.GetButtonDown("Cancel")) {
            SceneManager.LoadScene(ScoreTracker.Instance.mainMenuScene);
        }
    }
}
