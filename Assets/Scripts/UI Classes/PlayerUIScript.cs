using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIScript : MonoBehaviour
{
    [SerializeField] private Slider stressBar;
    [SerializeField] private Slider progressBar;


    // Start is called before the first frame update
    void Start()
    {
        if (stressBar != null)
        {
            stressBar.minValue = 0;
            stressBar.maxValue = 1;
        }

        if (progressBar != null)
        {
            progressBar.minValue = 0;
            progressBar.maxValue = 1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStressBar();
        UpdateProgressBar();
    }

    public void UpdateStressBar()
    {
        if(stressBar != null)
            stressBar.value = Mathf.Clamp(GameManager.Instance.GetPlayerStressLevel() / GameManager.Instance.GetPlayerStressLevelMax(), 0, 1);
    }

    public void UpdateProgressBar()
    {
        if (progressBar != null)
            progressBar.value = Mathf.Clamp(GameManager.Instance.GetPlayerHeight() / GameManager.Instance.GetWinDistance(), 0, 1);
    }
}
