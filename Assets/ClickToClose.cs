using UnityEngine;

public class TutorialPanel : MonoBehaviour
{
    public GameObject tutorialPanel;

    void Start()
    {
        ShowTutorial();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HideTutorial();
        }
    }

    void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
        Time.timeScale = 0f; // Pause the game
    }

    void HideTutorial()
    {
        tutorialPanel.SetActive(false);
        Time.timeScale = 1f; // Resume the game
    }
}