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
        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(true);
        }

        if (settingsUI != null)
        {
            settingsUI.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleSettings();
        }
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

        if (mainMenuUI != null)
        {
            mainMenuUI.SetActive(!mainMenuUI.activeSelf);
        }
        if (settingsUI != null)
        {
            settingsUI.SetActive(!settingsUI.activeSelf);
        }

    }

    public void ReloadCurrentScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}

