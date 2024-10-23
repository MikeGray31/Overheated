using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //singleton instance
    public static GameManager Instance { get; private set; }

    // References To Other Game Objects
    private PlayerController player;
    private UIController ui;
    private LevelManager levelManager;
    public LavaScript lava { get; private set; }

    // stressState Fields
    public bool gameIsStressed { get; private set; }

    [SerializeField] private float winDistance;
    private float winDistanceIncreaseRate;

    //gameState fields
    public bool gamePaused { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        

        player = FindFirstObjectByType<PlayerController>();
        ui = FindFirstObjectByType<UIController>();
        OnLevelStart();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SubsribeToPlayerEvents();
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        UnsubsribeFromPlayerEvents();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log("New scene loaded: " + scene.name);
        UnsubsribeFromPlayerEvents();
        player = FindFirstObjectByType<PlayerController>();
        SubsribeToPlayerEvents();

        ui = FindFirstObjectByType<UIController>();
        OnLevelStart();

    }

    public void SubsribeToPlayerEvents()
    {
        if (player != null)
        {
            player.OnStressStateChanged += OnStressStateChanged;
            player.OnDeath += OnPlayerDeath;
            player.onWin += OnPlayerWin;
        }
        /*else
        {
            Debug.LogWarning("GameManager.player is null!");
        }*/
    }

    public void UnsubsribeFromPlayerEvents()
    {
        if (player != null)
        {
            player.OnStressStateChanged -= OnStressStateChanged;
            player.OnDeath -= OnPlayerDeath;
            player.onWin -= OnPlayerWin;
        }
        /*else
        {
            Debug.LogWarning("GameManager.player is null!");
        }*/
    }

    public void OnLevelStart()
    {
        levelManager = FindFirstObjectByType<LevelManager>();
        if(levelManager != null)
        {
            winDistance = levelManager.GetData().minimumWinHeight;
            winDistanceIncreaseRate = levelManager.GetData().winDistanceIncreaseRate;
        }
        else
        {
            winDistance = 100f;
            winDistanceIncreaseRate = 10f;
        }

        lava = FindFirstObjectByType<LavaScript>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckForPauseKey();
        StressStateEffects();
        CheckSpawnLevelSection();
    }
    
    public void CheckSpawnLevelSection()
    {
        if(levelManager != null && winDistance > levelManager.GetHighestLevelSection().transform.position.y)
        {
            levelManager.SpawnLevelSection();
        }
    }

    public void OnStressStateChanged(bool NewStressState)
    {
        //Debug.Log("player's stressStateOn is: " + NewStressState);
        gameIsStressed = NewStressState;
    }

    public void StressStateEffects()
    {
        if (gameIsStressed)
        {
            winDistance += winDistanceIncreaseRate * Time.deltaTime;
        }
    }

    public void OnPlayerDeath(PlayerController player)
    {
        GameOver();
    }

    public void OnPlayerWin()
    {
        WinGame();
    }

    public float GetWinDistance()
    {
        return winDistance;
    }

    public float GetPlayerStressLevel() 
    {
        if (player != null) return player.StressLevel;
        else return 0f;
    }
    public float GetPlayerStressLevelMax()
    {
        if (player != null) return player.GetMaxStressLevel();
        else return 100f;
    }

    public float GetPlayerHeight()
    {
        if (player != null) return player.transform.position.y;
        else return 0f;
    }

    #region GameStateMethods

    //----------- UI win, game over, and pause methods for UI and Timescale below ----------------

    public void GameOver()
    {
        //Debug.Log("Game Over!");
        ui.SetgameOverUIActive(true);
        //Time.timeScale = 0f;
    }

    public void WinGame()
    {
        //Debug.Log("Player Wins!");
        ui.SetwinGameUIActive(true);
        Time.timeScale = 0f;
    }

    public void CheckForPauseKey()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!gamePaused) Pause();
            else Resume();
        }
    }

    public void Resume()
    {
        //Debug.Log("Should be Resumed!");
        ui.SetPauseUIActive(false);
        Time.timeScale = 1f;
        gamePaused = false;
    }

    public void Pause()
    {
        //Debug.Log("Should be paused!");
        ui.SetPauseUIActive(true);
        Time.timeScale = 0f;
        gamePaused = true;
    }

    public void Restart()
    {
        //Debug.Log("PauseScreen: " + (pauseGameUI != null) + " | GameOverScreen: " + (gameOverUI != null) + " | WinScreen: " + (winGameUI != null));
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Resume();
    }

    public void QuitGame()
    {
        //Debug.Log("Quitting game...");
        Application.Quit();
    }


    public void StartLevel()
    {
        SceneManager.LoadScene("TestLevel");
    }

    #endregion
}
