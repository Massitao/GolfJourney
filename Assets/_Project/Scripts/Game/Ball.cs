using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(LineRenderer))]

public class Ball : MonoBehaviour {

    [Header("Ball Components")]
    
    public static Ball instance;       // Ball Instance. Allows other scripts access this script.

    Rigidbody2D ballRB;                // RigidBody
    LineRenderer dragLine;             // Line Renderer
    AudioSource ballASource;           // Audio Source


    [Header ("Ball, Finger and Radius Position")]
            
    // ALL POSITIONS STORED
    Vector3 ballPosition;               // Ball Position
    Vector3 lastPressedPosition;        // Last Finger Press Position
    Vector3 radiusPos;                  // Radius Length (Limits Finger Position)
    Vector3 fingerPos;                  // Finger Position


    [Header("Ball Power Launch")]
    
    // Power to launch ball into Input Direction
    [SerializeField] float launchPower = 15f;

    // DISTANCE BETWEEN BALLPOSITION AND LASTPRESSEDPOSITION;
    float distanceBallToFinger;

    // BODY VELOCITY
    float rbVelocity;                   // RigidBody Velocity
    float maxRBVel;                     // Maximum RigidBody Velocity Achieved


    [Header ("Ball Limits")]
    
    // Minimun distance between ballPos and lastPressedPos to launch ball
    [Range(0f, 0.5f)] [SerializeField] float minDistanceRequiredToLaunch = 0.25f;

    // Minimun Body Velocity required to let the player launch the ball again
    [Range(0f, 1f)] [SerializeField]   float maxVelocityAllowedToLaunchBall = 0.5f;

    // Radius Length
    [Range(0f, 4f)] [SerializeField]   float powerRadius = 4f;


    [Header("States And Info")]
    
    [HideInInspector]
    public bool firstTouched;                           // Check if Player Pressed the Ball for the first time after loading the level
    [HideInInspector]
    public int timesBallLaunched;                       // Times Ball Has Been Launched

    bool pressedOn = false;                             // Is Player Touching the Ball?
    bool isAiming = false;                              // Is Player Aiming? (Required to stop the ball from launching itself at start of the game)
    bool canLaunch = true;                              // Is Player able to launch the Ball?

    bool isInWater;                                     // Is Player inside Water?

    public bool constrainInput;                         // Stop Player from launching the Ball


    [Header("Checkpoint")]

    public GameObject checkPoint;                       // Allows Player To Respawn
    public GameObject activeCheckPoint;


    [Header("Particles")]
    
    // (ASIGNED IN EDITOR) Smoke Hit Particle System
    [SerializeField] ParticleSystem hitPS;

    // Launch Ready Warn Particle System
    [SerializeField] ParticleSystem launchReadyPS;

    [SerializeField] float warnDelayTime;               // Warn Delay Time for the first time
    float warnDelayTimer;                               // Warn Delay Timer
    bool warnOnce;                                      // First Warn Updates only once

    [SerializeField] int warnTime;                      // Warn Time for new warnings after the first one has triggered
    float warnTimer;                                    // Warn Timer


    [Header("Audio")]
    
    // Launch Audios
    [SerializeField] AudioClip strongLaunch;            // When Launch bar higher than 2/3 of the radius
    [SerializeField] AudioClip normalLaunch;            // When Launch bar is between 1/3 and 2/3 of the radius
    [SerializeField] AudioClip smallLaunch;             // When Launch bar is lower than 1/3 of the radius

    [Space(5)]

    // Impact Audios
    [SerializeField] AudioClip strongImpact;            // When Ball Impacts the ground with a velocity magnitude higher than 2/3 of the maximum velocity achieved by the RigidBody.
    [SerializeField] AudioClip normalImpact;            // When Ball Impacts the ground with a velocity magnitude between 1/3 and 2/3 of the maximum velocity achieved by the RigidBody.
    [SerializeField] AudioClip smallImpact;             // When Ball Impacts the ground with a velocity magnitude lower than 1/3 of the maximum velocity achieved by the RigidBody.



    // Use this for sooner initialization
    private void Awake()
    {
        // Sets Ball Instance
        instance = this;

        // Get RigidBody Component
        ballRB = gameObject.GetComponent<Rigidbody2D>();

        // Get LineRenderer Component
        dragLine = gameObject.GetComponent<LineRenderer>();

        // Get Launch Warn Particle System
        launchReadyPS = gameObject.GetComponentInChildren<ParticleSystem>();

        // Get Audio Source
        ballASource = gameObject.GetComponent<AudioSource>();
    }

    // Use this for initialization
    private void Start()
    {
        fingerPos = ballPosition;
        lastPressedPosition = ballPosition;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateBallValues();
        LaunchBall();

        if (Input.GetKeyDown(KeyCode.E))
        {
            SpawnCheckPoint();
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            ResetToCheckpoint();
        }
    }


    // If Player Is Pressing the Ball
    private void OnMouseDown()
    {
        pressedOn = true;       // Player Has Pressed The Ball
    }

    // If Player Lifted the finger off the screen
    private void OnMouseUp()
    {
        pressedOn = false;      // Player Stopped Pressing The Screen
    }


    // Change Pitch from Audio Source Randomly
    private void AudioPitchChange()
    {
        ballASource.pitch = Random.Range(1 * 0.75f, 1 * 1.25f);
    }


    // Store Updated Values
    private void UpdateBallValues()
    {
        ballPosition = transform.position;                                                                  // Store Transform Position Of Ball
        fingerPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);                                    // Store Transform Position Of Mouse in WorldPoint
        dragLine.SetPosition(dragLine.positionCount - dragLine.positionCount, ballPosition);                // Set Origin Position to Ball Position

        distanceBallToFinger = Vector3.Distance(ballPosition, lastPressedPosition);                         // Store Ball to Finger Distance
        rbVelocity = ballRB.velocity.magnitude;                                                             // Store Body Velocity

        // Check Maximun RB Velocity Achieved
        if (rbVelocity >= maxRBVel)
        {
            maxRBVel = rbVelocity;                                                                          // Store Maximum RigidBody Velocity Achieved
        }
    }

    // Launch Behaviour and Conditions to enable Aiming and Launching
    private void LaunchBall()
    {
        // If Body Velocity is less or equal than the Maximun Velocity Allowed, allow to Aim and Launch. Also, Plays Warn PS
        if (rbVelocity <= maxVelocityAllowedToLaunchBall)
        {
            canLaunch = true;                                                                               // Player Is Allowed to Launch The Ball

            // Should Particle warn player once player reaches ground?
            if (warnOnce && !constrainInput)
            {
                // If Warn Delay Timer is greater or equal than Setted Warn Delay Time (Waits for Ball to fully allow player to Launch it (Makes sure ball won't move again))
                if (warnDelayTimer >= warnDelayTime)
                {
                    launchReadyPS.Play();                                                                       // Play Warn Particle System
                    warnOnce = false;                                                                           // Won't play again
                    warnDelayTimer = 0f;                                                                        // Reset Warn Delay Timer
                }
                else
                {
                    warnDelayTimer += Time.deltaTime;                                                           // Add Time to Warn Delay Timer
                }              
            }

            // Warn again if ball isn't pressed, warnTimer surpassed warnTime
            if (warnTimer >= warnTime && !pressedOn)
            {
                launchReadyPS.Play();                                                                       // Play Warn Particle System
                warnTimer = 0f;                                                                             // Reset Warn Timer
            }
            else
            {
                // Add Time to Warn Timer if player isn't pressing the ball and player has already been warned
                if (!pressedOn && !warnOnce)
                    warnTimer += Time.deltaTime;                                                            // Sum time to Warn Timer
                else
                    warnTimer = 0f;                                                                         // Reset Warn Timer
            }
        }
        else
        {
            // DragLine Related

            dragLine.enabled = false;                                                                       // Disable Line Renderer
            lastPressedPosition = ballPosition;                                                             // Reset Radius Position
            dragLine.SetPosition(dragLine.positionCount - 1, lastPressedPosition);                          // Set Last Element Position to Last Pressed Position


            // Ball State

            canLaunch = false;                                                                              // Player Is NOT Allowed to Launch The Ball
            

            // Warning Timers

            warnOnce = true;                                                                                // Warn Player with Particle System to update again
            warnTimer = 0f;                                                                                 // Reset Warn Timer
            warnDelayTimer = 0f;                                                                            // Reset Warn Delay Timer 
        }

        // If Player is allowed to Launch Ball
        if (canLaunch && !constrainInput)
        {
            // If Player pressed the ball
            if (pressedOn)
            {
                isAiming = true;                                                                             // Set Aiming to True

                radiusPos = fingerPos - ballPosition;                                                        // Store Position Distance between Finger and Ball
                radiusPos = Vector2.ClampMagnitude(radiusPos, powerRadius);                                  // Store Clamped Position Distance to Radius
                lastPressedPosition = ballPosition + radiusPos;                                              // Change Last Pressed Position To Clamped Values

                dragLine.enabled = true;                                                                     // Enable Line Renderer
                dragLine.SetPosition(dragLine.positionCount - 1, lastPressedPosition);                       // Set Last Element Position to Last Pressed Position

                // If Player did not touch the ball
                if (!firstTouched)
                {
                    firstTouched = true;                                                                     // Set firstTouched to true            
                }
            }
            else
            {
                // If Player was aiming
                if (isAiming)
                {
                    dragLine.enabled = false;                                                                // Disable Line Renderer

                    // If Distance between Ball To Finger meets Minimun Distance To Launch the Ball, applies force to the Ball (launches it)
                    if (distanceBallToFinger >= minDistanceRequiredToLaunch)
                    {
                        LaunchBallAudio();
                        // Apply force to inverse location
                        ballRB.AddForce((ballPosition - lastPressedPosition).normalized * launchPower * distanceBallToFinger);
                        // Increment times Player launched the Ball
                        timesBallLaunched++;
                    }

                    isAiming = false;                                                                        // Set Aiming To False
                    lastPressedPosition = ballPosition;                                                      // Reset Last Pressed Position
                }
            }
        }
        else
        {
            dragLine.enabled = false;                                                                       // Disable Line Renderer
            lastPressedPosition = ballPosition;                                                             // Reset Last Pressed Position To Avoid Launches
        }
    }

    // Launch Audio Behaviour
    private void LaunchBallAudio()
    {
        AudioPitchChange();

        // If Distance is greater than 75 % of total power radius
        if (distanceBallToFinger > powerRadius * 0.75f)
        {
            ballASource.PlayOneShot(strongLaunch);
        }
        // If Distance is between 30 - 75 % of total power radius
        else if (distanceBallToFinger >= powerRadius * 0.3f && distanceBallToFinger <= powerRadius * 0.75f)
        {
            ballASource.PlayOneShot(normalLaunch);
        }
        // If Distance is lower than 30 % of total power radius
        else
        {
            ballASource.PlayOneShot(smallLaunch);
        }
    }


    public void SpawnCheckPoint()
    {
        if (canLaunch)
        {
            if (activeCheckPoint != null)
            {
                activeCheckPoint.transform.position = this.transform.position;
            }
            else
            {
                activeCheckPoint = Instantiate(checkPoint, transform.position, Quaternion.identity);
            }
        }   
    }

    public void ResetToCheckpoint()
    {
        this.transform.position = activeCheckPoint.transform.position;
        ballRB.velocity = Vector2.zero;
    }

    // Water will call this Method - Sets isInWater to true
    public void InsideWater()
    {
        isInWater = true;
    }

    // Water will call this Method - Sets isInWater to false
    public void OutsideWater()
    {
        isInWater = false;
    }


    // Handles Ball Particles and Audio On Collision
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Particles
        
        // If Ball isn't in water, Instantiate Dust Particle
        if (!isInWater)
        {
            Vector3 pointHitColl = collision.GetContact(0).point;                                                           // Store Temporal Collision Hit Point
            Vector3 difference = pointHitColl - transform.position;                                                         // Store Temporal Vector3 Difference between Hit Point and Ball Transform's Position
            float rotateOnlyZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;                                    // Calculate Z rotation

            ParticleSystem dustPS = Instantiate(hitPS, transform.position, Quaternion.Euler(0f, 0f, rotateOnlyZ));          // Control Particle with a temporal Particle System Variable
            ParticleSystem.MainModule dustPSSize = dustPS.main;                                                             // Control Main Module of the new Controlled PS
            dustPSSize.startSizeMultiplier = rbVelocity / maxRBVel;                                                         // Multiplier is effected by RigidBody Velocity and Max Velocity Achieved
        }


        // Audio
        
        // Change Audio Pitch Randomly
        AudioPitchChange();

        // If Velocity is greater than 75 % of Maximun RigidBody Velocity
        if (rbVelocity > maxRBVel * 0.75f)
        {
            ballASource.PlayOneShot(strongImpact);
        }
        // If Velocity is between 30 - 75 % of Maximun RigidBody Velocity
        else if (rbVelocity >= maxRBVel * 0.3f && rbVelocity <= maxRBVel * 0.75f)
        {
            ballASource.PlayOneShot(normalImpact);
        }
        // If Velocity is lower than 30% of Maximun RigidBody Velocity
        else
        {
            ballASource.PlayOneShot(smallImpact);
        }
    }
}
