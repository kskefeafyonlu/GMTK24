using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManager : MonoBehaviour
{

    public GameObject mainMenuUI;
    public GameObject settingsUI;

    private void Awake()
    {
        mainMenuUI.SetActive(true);
        settingsUI.SetActive(false);
    }

    public void LoadScene(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleSettings()
    {
        mainMenuUI.SetActive(!mainMenuUI.activeSelf);
        settingsUI.SetActive(!settingsUI.activeSelf);
    }

    public void ReloadCurrentScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}

