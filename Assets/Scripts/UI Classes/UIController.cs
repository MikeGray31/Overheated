using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIController : MonoBehaviour
{
    //UI GameObjects
    [SerializeField]
    private GameObject gameOverUI;
    [SerializeField]
    private GameObject winGameUI;
    [SerializeField]
    private GameObject pauseGameUI;
    [SerializeField]
    private GameObject HowToPlayUI;


    public void SetgameOverUIActive(bool setToThis)
    {
        if(gameOverUI != null) gameOverUI.SetActive(setToThis);
    }

    public void SetwinGameUIActive(bool setToThis)
    {
        if(winGameUI != null) winGameUI.SetActive(setToThis);
    }
    public void SetPauseUIActive(bool setToThis)
    {
        if(pauseGameUI != null) pauseGameUI.SetActive(setToThis);
    }

    public void SetHowToPlayUIActive(bool setToThis)
    {
        if (HowToPlayUI != null) HowToPlayUI.SetActive(setToThis);
    }


    public void ResumeButtonPressed()
    {
        GameManager.Instance.Resume();
    }

    public void RestartButtonPressed()
    {
        GameManager.Instance.Restart();
    }

    public void QuitButtonPressed()
    {
        GameManager.Instance.QuitGame();
    }

    public void StartButtonPressed()
    {
        GameManager.Instance.StartLevel();
    }

    public void DisplayControlsPressed()
    {
        SetHowToPlayUIActive(true);
        StartCoroutine(WaitForExplainControlsToDeactivate());
    }

    IEnumerator WaitForExplainControlsToDeactivate()
    {
        yield return new WaitUntil(() => Input.anyKeyDown);
        SetHowToPlayUIActive(false);
    }
}
