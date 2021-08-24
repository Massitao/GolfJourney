using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager instance;         // Game Manager Instance In-Game

    [Header("InGame Vars")]
    
    [SerializeField] bool startTimer;           // Shall the In-Game Timer Begin?
    public float gameTimer;                     // Stores In-Game Time spent

    public bool gameIsPaused;                   // Is Game Paused?
    public bool reachedEnding;                  // Did the Player / Ball reach the ending (hole)?

    [SerializeField] Ball theBall;              // Store Ball
    [SerializeField] LevelManager lvlManager;   // Store Level Manager
    [SerializeField] EndingTrigger hole;        // Store Ending Trigger

    [Header("Pause Values")]
    
    [SerializeField] float lerpPauseDuration;   // Duration of Pause Transition
    float lerpTimer;                            // Time stored until reaching same value as lerpPauseDuration

    int unpause = 1, pause = 0;                 // Values of Pause and Unpausing (Time Scale)


    [Header("Debug")]
    
    [SerializeField] bool debugMode;            // Enable Debug Mode
    [SerializeField] float debugTime;           // New Value for In-Game Time Scale


	// Use this for initialization
	private void Awake ()
    {
        instance = this;                        // Set Instance to GameObject containing this script   
	}

    private void Start()
    {
        theBall = Ball.instance;                // Set Ball with its Instance
        lvlManager = LevelManager.instance;     // Set Level Manager with its Instance  
        hole = EndingTrigger.instance;          // Set Ending Trigger with its Instance
    }

    // Update is called once per frame
    void Update () {

        // Game Timer Start if the ball isn't null and Timer didn't start
        if (theBall != null && !startTimer)
        {
            GameTimerStart();
        }

        // Game Has Started
        if (startTimer)
        {
            // GameTimerCount
            if (!reachedEnding)
            {
                GameTimerCount();
            }

            // EndGameHandling
            else
            {
                EndGameHandling();
            }    
        }

        // Debug Mode
        if (debugMode)
            Time.timeScale = debugTime;

        // Can Pause if not in Debug Mode
        else
        PauseHandling();
	}

    // Enables Game Timer()
    void GameTimerStart()
    {
        if (theBall.firstTouched)
            startTimer = true;
    }

    // Add time to timer
    void GameTimerCount()
    {
        gameTimer += Time.deltaTime;
    }


    // Pause and Unpause behaviour
    void PauseHandling()
    {
        // If Game is Paused
        if (gameIsPaused)
        {
            // If Time Scale isn't equals to Pause Value (0)
            if (Time.timeScale != pause)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, pause, lerpTimer / lerpPauseDuration);              // Lerp Time Scale to Pause Value, with LerpDuration

                lerpTimer += Time.deltaTime;                                                                    // Lerp Timer. Helps lerping withing LerpDuration

                // If Time Scale is less or equals to 0.1f
                if (Time.timeScale <= 0.1f)
                {
                    Time.timeScale = pause;                                                                     // Directly Set Time Scale to Pause Value
                    lerpTimer = 0;                                                                              // Reset Lerp Timer
                }

            }
        }
        // If Game is NOT Paused
        else
        {
            // If Time Scale isn't equals to Unpause Value (1)
            if (Time.timeScale != unpause)
            {
                Time.timeScale = Mathf.Lerp(Time.timeScale, unpause, lerpTimer / lerpPauseDuration);            // Lerp Time Scale to Pause Value, with LerpDuration

                lerpTimer += Time.deltaTime;                                                                    // Lerp Timer. Helps lerping withing LerpDuration

                // If Time Scale is less or equals to 0.9f
                if (Time.timeScale >= 0.9f)
                {
                    Time.timeScale = unpause;                                                                   // Directly Set Time Scale to Unpause Value
                    lerpTimer = 0;                                                                              // Reset Lerp Timer
                }
            }
        }
    }

    // Sets Bools to properly End Game
    void EndGameHandling()
    {
        gameIsPaused = false;
        theBall.constrainInput = true;
    }


    // Set gameIsPaused to true
    public void Pause()
    {
        gameIsPaused = true;
    }
    // Set gameIsPaused to false
    public void UnPause()
    {
        gameIsPaused = false;
        Time.timeScale = 0.01f;         // NECESSARY. Game Manager is scaled with Time Scale
    }


    // If Player temporarly exits app
    private void OnApplicationPause()
    {
        if (!gameIsPaused)
        {
            Pause();
        }
    }


    // Reset Game Scene and Values
    public void Reset()
    {
        UnPause();                                                              // Unpause Game    
        hole.enabled = false;                                                   // Disable Ending Trigger. Avoiding possible crashes

        lvlManager.LoadLevel(lvlManager.scene.name);                            // Reload Level
    }

    // Exits Game
    public void ExitToMenu()
    {
        UnPause();                                                              // Unpause Game

        hole.GetComponent<EndingTrigger>().enabled = false;                     // Disable Ending Trigger. Avoiding possible crashes

        lvlManager.LoadLevel("Menu");                                           // Load Menu Scene
    }
}
