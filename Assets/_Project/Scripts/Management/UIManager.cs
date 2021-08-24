using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public static UIManager instance;               // UI Instance

    GameManager gameManager;                        // Game Manager Instance
    Ball theBall;                                   // Ball Instance


    // All UI Animators
    [SerializeField] Animator statMenuAnimator, statTextAnimator, pauseMenuAnimator, endMenuAnimator;
    // All Text Components in UI to change
    [SerializeField] Text timerUI, launchesUI = null;
    // Checkpoint Reset Image
    [SerializeField] Image backToCheckPointImage;


    bool isStatMenuOpen = true;                     // Is Stat Button Open? (Showing Stats)
    bool isAnimationStillGoing = false;             // Is It Still Closing / Opening?

    bool isPausedCheck;                             // Checks this bool and compares it with Game Manager. Used to stop always updating PauseAnimHandles
    bool updatePauseHandlesOnce = true;             // Do not keep updating values after Pause / Unpause and already updated values once.

    bool updateEndGameOnce = true;                  // Do not keep updating values after game ended and already updated values once.

    // Use this for initialization
    private void Awake()
    {
        instance = this;                            // Set UI Instance to GameObject containing this script
    }

    private void Start()
    {
        gameManager = GameManager.instance;         // Store Game Manager Instance
        theBall = Ball.instance;                    // Store Ball Instance

        isPausedCheck = gameManager.gameIsPaused;   // Set Checker to Game Manager Pause Value
    }

    // Update is called once per frame
    private void Update()
    {
        PauseAnimHandles();
        UpdateStatTexts();
        EndGameHandles();
    }

    // Pause / Unpause UI Menus Behaviour
    private void PauseAnimHandles()
    {
        // If Checker is not equal to Game Pause State, enable One Update
        if (isPausedCheck != gameManager.gameIsPaused)
        {
            isPausedCheck = gameManager.gameIsPaused;       // Sets value to the same one as Game Pause State
            updatePauseHandlesOnce = true;                  // Sets to true. Enables Updating.
        }

        // If can update Values
        if (updatePauseHandlesOnce)
        {
            // PAUSE
            if (gameManager.gameIsPaused)
            {
                pauseMenuAnimator.gameObject.SetActive(true);
                pauseMenuAnimator.SetTrigger("Pause");    
                pauseMenuAnimator.ResetTrigger("Unpause");

                updatePauseHandlesOnce = false;             // Disables Updating Again
            }
            // Unpause
            else
            {
                // If Pause Menu GameObject is Active
                if (pauseMenuAnimator.gameObject.activeSelf)
                {
                    pauseMenuAnimator.SetTrigger("Unpause");
                    pauseMenuAnimator.ResetTrigger("Pause");

                    updatePauseHandlesOnce = false;             // Disables Updating Again
                }
            }
        }

        // If Game is Unpaused, Time Scale is greater or equals than 0.9f, and Pause Menu is Active, deactivate Pause Menu GameObject
        if (!gameManager.gameIsPaused && Time.timeScale >= 0.9f && pauseMenuAnimator.gameObject.activeSelf)
            pauseMenuAnimator.gameObject.SetActive(false);
    }

    // Sets Texts and Animations for Stats
    private void UpdateStatTexts()
    {
        // If Game is still going
        if (!gameManager.reachedEnding)
        {
            statMenuAnimator.SetBool("EndAnim", isAnimationStillGoing);                     // Sets bool for Ending Transition Animation. Prevents User from toying with the Stat Button

            // If the Ball is NOT null, set text to number of User Launches
            if (theBall.gameObject != null)
                launchesUI.text = theBall.timesBallLaunched.ToString();

            string minutes = Mathf.Floor(gameManager.gameTimer / 60).ToString("00");        
            string seconds = (gameManager.gameTimer % 60).ToString("00");

            timerUI.text = minutes + ":" + seconds;                                         // Sets Timer Text to Timer Values
        }
    }

    // End Game UI Handling
    private void EndGameHandles()
    {
        // If User reached the End and is allowed to Update Values Once
        if (gameManager.reachedEnding && updateEndGameOnce)
        {
            endMenuAnimator.SetTrigger("ShowEnding");                           // Shows End Menu

            statTextAnimator.SetBool("gameEnded", gameManager.reachedEnding);   // Start Stat Text Animation of End Game

            updateEndGameOnce = false;                                          // Do not update more
        }    
    }

    // Controls Opening and Closing Stat Button
    public void StatsAnimHandling()
    {
        isStatMenuOpen = (isStatMenuOpen) ? false : true;           // Sets to opposite value

        isAnimationStillGoing = true;                               // Locks player from pressing the button again until the animation is done

        statMenuAnimator.SetBool("Opened", isStatMenuOpen);         // Sets Stat Menu (Button) "Opened Bool" to Stat Menu State Value
        statTextAnimator.SetBool("isOpen", isStatMenuOpen);         // Sets Stat Text "Is Open" to Stat Menu State Value
    }

    // Checkpoint Handling
    public void SetCheckpoint()
    {
        theBall.SpawnCheckPoint();

        if (theBall.activeCheckPoint != null)
        {
            backToCheckPointImage.gameObject.SetActive(true);
        }
    }

    // Reset to checkpoint
    public void BackToCheckPoint()
    {
        theBall.ResetToCheckpoint();
    }



    // Animation Event To Properly stop locking player from pressing the Stat Button again
    public void EndStatMenuOpenClose()
    {
        isAnimationStillGoing = false;          // Player can press the button again
    }
}
