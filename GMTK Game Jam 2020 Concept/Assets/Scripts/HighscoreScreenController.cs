using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HighscoreScreenController : MonoBehaviour {


    /* HIGHSCORE */

    [SerializeField] private Canvas highscoreScreen;
    [SerializeField] private Text highscoreText;

    public string Score {
        get => ScoreTracker.Instance.ScoreTimerAsText;
    }


    public void ShowHighscore() {
        highscoreText.text = "Your Score:\n" + Score;
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
