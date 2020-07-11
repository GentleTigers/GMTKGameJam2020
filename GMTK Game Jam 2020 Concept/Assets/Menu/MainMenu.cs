﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    public int firstSceneBuildIndex;

    public void PlayGame() {
        SceneManager.LoadScene(firstSceneBuildIndex);
    }

    public void QuitGame() {
        Debug.Log("Application Quit!");
        Application.Quit();
    }

}
