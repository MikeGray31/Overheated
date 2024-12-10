using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //ScriptableObjects Data -------------------------------------------------
    [SerializeField] protected PlayerData Data;
    [SerializeField] protected PlayerControlsData ControlsData;


    //Components -------------------------------------------------------------
    [Space(20)]
    [SerializeField] protected Collider2D playerCollider;
    [SerializeField] protected Rigidbody2D rb;

    [SerializeField] protected Transform CameraTarget;

    //Visuals
    [SerializeField] protected PlayerVisualsScript visuals;

    [SerializeField] protected ParticleSystem deathExplosion;

    //StressedState fields and events ----------------------------------------
    public bool StressStateOn { get; private set; }
    public event Action<bool> OnStressStateChanged;

    private float StressKeyLastPressedTime = 0;

    public float StressLevel { get; private set; }

    //movement-related fields ------------------------------------------------
    private Vector2 movementInput;
    
    private Vector2 TargetVector;
    private bool isFacingRight;


    //Jump-related fields ----------------------------------------------------
    private float LastOnGroundTime;
    private float LastPressedJumpTime;
    
    private bool isJumping;
    private bool DoubleJumpAvailable;
    private bool isFastFalling;

    private bool isWallJumping;
    private float wallJumpStartTime;
    private int lastWallJumpDir; // -1 for left or 1 for right

    //private bool isJumpCut;
    //private bool isJumpFalling;


    private float LastOnWallTime;
    private float LastOnWallRightTime;
    private float LastOnWallLeftTime;

    [Space(20)]
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Vector2 groundCheckSize; //set this in the inspector
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private Transform RightWallCheckPoint;
    [SerializeField] private Transform LeftWallCheckPoint;
    [SerializeField] private Vector2 wallCheckSize; //set this in the inspector

    // misc. -----------------------------------------------------------------

    private bool dead;
    public event Action<PlayerController> OnDeath;

    public event Action onWin;

    // Methods ---------------------------------------------------------------

    // Start is called before the first frame update
    void Start()
    {
        //playerRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        StressStateOn = false;
        TargetVector = new Vector2(0, 0);
        
        isFacingRight = true;

        isJumping = false;
        //isJumpCut = false;
        isFastFalling = false;
        isWallJumping = false;

        DoubleJumpAvailable = true;
    }

    // Update is called once per frame
    void Update()
    {
        GetStressKeyInput();
        DetermineStressState();
        ManageStressLevel();
        CalculateTimers();
        AllCollisionChecks();

        movementInput.x = Input.GetAxisRaw("Horizontal");
        movementInput.y = Input.GetAxisRaw("Vertical");

        TargetVector = GetMovementInput();
        GetJumpInputs();
        DetermineJump();
        DetermineWallJump();
        DetermineDoubleJump();
        ResetIsJumping();
        DetermineGravity();
        UpdateFacingRight();

        UpdateVisuals();
    }

    private void FixedUpdate()
    {
        //Debug.Log("Is FixedUpdate Being called?");
        if (isWallJumping)
        {
            ApplyMovementForce(Data.wallJumpRunLerp);
        }
        else
        {
            ApplyMovementForce(1);
        }
        
        //ApplyJumpForce();
    }

    public void AllCollisionChecks()
    {
        //on ground
        if (isGrounded() && !isJumping)
        {
            //Debug.Log("Hit the ground!");
            LastOnGroundTime = Data.coyoteTime;
            DoubleJumpAvailable = true;
        }

        //on right wall
        BoxCastDrawer.BoxCastAndDraw(RightWallCheckPoint.position, wallCheckSize, 0, Vector2.down, 0f, groundLayer);
        if (Physics2D.OverlapBox(RightWallCheckPoint.position, wallCheckSize, 0, groundLayer)  && isFacingRight && !isWallJumping)
        {
            LastOnWallRightTime = Data.coyoteTime;
        }
        //on left wall
        BoxCastDrawer.BoxCastAndDraw(LeftWallCheckPoint.position, wallCheckSize, 0, Vector2.down, 0f, groundLayer);
        if (Physics2D.OverlapBox(LeftWallCheckPoint.position, wallCheckSize, 0, groundLayer) && !isFacingRight && !isWallJumping)
        {
            LastOnWallLeftTime = Data.coyoteTime;
        }
        
        LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);
    }

    public void CalculateTimers()
    {
        StressKeyLastPressedTime -= Time.deltaTime;

        LastOnGroundTime -= Time.deltaTime;
        LastPressedJumpTime -= Time.deltaTime;

        LastOnWallTime -= Time.deltaTime;
        LastOnWallLeftTime -= Time.deltaTime;
        LastOnWallRightTime -= Time.deltaTime;
        
    }

    

    // ------------------------------------------------------------------

    #region Stress State Methods

    public void GetStressKeyInput()
    {
        if (Input.GetKey(ControlsData.StressModeKey))
        {
            StressKeyLastPressedTime = Data.StressKeyLastPressedTimeMax;
        }
    }

    public void DetermineStressState()
    {
        if (StressKeyLastPressedTime > 0)
        {
            if (!StressStateOn)
            {
                //Debug.Log("this should only run once;");
                StressStateOn = true;
                ManagerStressColorVisuals();
                OnStressStateChanged?.Invoke(StressStateOn);
            }
        }
        else
        {
            if (StressStateOn)
            {
                //Debug.Log("this should only run once;");
                StressStateOn = false;
                ManagerStressColorVisuals();
                OnStressStateChanged?.Invoke(StressStateOn);

            }
        }
       
    }

    public void ManageStressLevel()
    {
        if (StressStateOn)
        {
            StressLevel += Data.StressIncreaseRate * Time.deltaTime;
        }
        else if( !StressStateOn && StressLevel > 0)
        {
            StressLevel -= Data.RelaxRate * Time.deltaTime;
            if(StressLevel < 0) 
            { 
                StressLevel = 0; 
            }
        }

        if(StressLevel >= Data.StressThreshold)
        {
            Die();
        }
    }

    public bool GetStressedModeOn()
    {
        return StressStateOn;
    }

    #endregion

    // ------------------------------------------------------------------

    #region Movement Methods

    public void UpdateFacingRight()
    {
        if (movementInput.x > 0)
        {
            isFacingRight = true;
        }
        else if (movementInput.x < 0)
        {
            isFacingRight = false;
        }
    }

    public Vector2 GetMovementInput()
    {
        Vector2 MovementInput = new Vector2(0, 0);
        if (StressStateOn)
        {
            MovementInput.x = movementInput.x * Data.moveSpeedStressed;
        }
        else
        {
            MovementInput.x = movementInput.x * Data.moveSpeedRelaxed;
        }
        return MovementInput;
    }

    public void ApplyMovementForce(float lerpAmount)
    {
        float targetSpeed = TargetVector.x;
        targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount);
        //Debug.Log("targetSpeedFloat: " + targetSpeed);
        float SpeedDiff = targetSpeed - rb.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.accerelation : Data.deceleration;
        float movement = Mathf.Pow(Mathf.Abs(SpeedDiff) * accelRate, Data.VelPower) * Mathf.Sign(SpeedDiff);
        
        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    #endregion

    // ------------------------------------------------------------------

    #region Jump Methods

    //Jump Methods
    public void GetJumpInputs()
    {
        if (Input.GetKeyDown(ControlsData.JumpKey))
        {
            OnJumpInput();
        }
        /*if (Input.GetKeyUp(ControlsData.JumpKey))
        {
            OnJumpUpInput();
        }*/

        if (movementInput.y < 0) 
        {
            isFastFalling = true;
        }
        else 
        { 
            isFastFalling = false; 
        }
    }

    public void OnJumpInput()
    {
        LastPressedJumpTime = Data.LastPressedJumpTimeStart;
    }

    public void OnJumpUpInput()
    {
        if (CanJumpCut())
        {
            //isJumpCut = true;
        }
    }

    public bool CanJumpCut()
    {
        return isJumping && rb.velocity.y > 0;
    }

    public void DetermineJump()
    {
        if (LastOnGroundTime > 0 && LastPressedJumpTime > 0 && !isJumping)
        {
            //Debug.Log("LastOnGround Time > 0 => " + (LastOnGroundTime > 0) + "\nLastPressedJumpTime > 0 => " +(LastPressedJumpTime > 0) + "\n!isJumping => " + (!isJumping));
            isWallJumping = false;
            Jump();
        }
    }

    public void DetermineDoubleJump()
    {
        if(LastOnGroundTime <= 0 && LastPressedJumpTime > 0 && DoubleJumpAvailable  && !isWallJumping)
        {
            DoubleJump();
        }
    }

    public void Jump()
    {
        //Ensures we can't call Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        float jumpForce;
        if (StressStateOn)
        {
            jumpForce = Data.jumpForceStressed;
        }
        else
        {
            jumpForce = Data.jumpForceRelaxed;
        }

        //more force if falling.
        if (rb.velocity.y < 0)
        {
            jumpForce -= rb.velocity.y;
        }

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        VisualJumpUp();
        isJumping = true;
    }

    public void DoubleJump()
    {
        Debug.Log("Double jump called!");
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        DoubleJumpAvailable = false;

        float jumpForce;
        if (StressStateOn)
        {
            jumpForce = Data.doubleJumpForceStressed;
        }
        else
        {
            jumpForce = Data.doubleJumpForceRelaxed;
        }

        jumpForce -= rb.velocity.y;

        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        isJumping = true;
    }

    public void ResetIsJumping()
    {
        if (isJumping)
        {
            if (rb.velocity.y < 0f || !Input.GetKey(ControlsData.JumpKey)) 
            {
                //Debug.Log("setting isJumping to false!");
                isJumping = false;
            }
        }
    }
    #endregion

    // ------------------------------------------------------------------

    #region WallJump Methods

    public void DetermineWallJump()
    {
        if (isWallJumping && Time.time - wallJumpStartTime > Data.wallJumpTime)
        {
            //Debug.Log("Setting isWallJumping to false!");
            isWallJumping = false;
        }


        if (CanWallJump() && LastPressedJumpTime > 0)
        {
            //Debug.Log("Should be wall jumping!");
            isWallJumping = true;
            isJumping = true;
            DoubleJumpAvailable = true;
            lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;
            WallJump(lastWallJumpDir);
        }
    }

    public void WallJump(int dir)
    {
        //Ensures we can't call Wall Jump multiple times from one press
        LastPressedJumpTime = 0;
        LastOnGroundTime = 0;
        LastOnWallRightTime = 0;
        LastOnWallLeftTime = 0;
        wallJumpStartTime = Time.time;

        Vector2 force;
        if (StressStateOn)
        {
            force = new Vector2(Data.wallJumpForceStressed.x, Data.wallJumpForceStressed.y);
        }
        else
        {
            force = new Vector2(Data.wallJumpForceRelaxed.x, Data.wallJumpForceRelaxed.y);
        }

        force.x *= dir; //apply force in opposite direction of wall

        if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
            force.x -= rb.velocity.x;

        if (rb.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
            force.y -= rb.velocity.y;

        //Unlike in the run we want to use the Impulse mode.
        //The default mode will apply are force instantly ignoring mass
        rb.AddForce(force, ForceMode2D.Impulse);
    }

    public bool CanWallJump()
    {
        bool result = LastPressedJumpTime > 0 && 
                      LastOnWallTime > 0 && 
                      LastOnGroundTime <= 0 &&
                      (!isWallJumping || (LastOnWallRightTime > 0 && lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && lastWallJumpDir == -1));
        //if (result) Debug.Log("Can Wall Jump!");
        return result;
    }

    public bool CanDoubleJump()
    {
        return true;
    }

        #endregion

        // ------------------------------------------------------------------

        #region Gravity Methods

        // Gravity Methods
        public void DetermineGravity()
        {

        if (!isJumping)
        {
            if (isFastFalling)
            {
                SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFastFallSpeed));
                //Debug.Log("Should be fast falling!  Gravity Scale = " + rb.gravityScale);
            }
            else
            {
                SetGravityScale(Data.gravityScale * Data.fallGravityMult);
                rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -Data.maxFallSpeed));
            }
        }
/*        else if (isJumping)
        {
            SetGravityScale(Data.gravityScale);
        }*/
        else
        {
            SetGravityScale(Data.gravityScale);
        }
        //Debug.Log("Current Gravity Scale: " + rb.gravityScale);
    }

    public void SetGravityScale(float scale)
    {
        rb.gravityScale = scale;
    }

    #endregion

    // ------------------------------------------------------------------

    #region Methods for Visuals

    public void UpdateVisuals()
    {
        visuals.UpdateVisuals(isGrounded(), rb.velocity.x, rb.velocity.y, isFacingRight);
        visuals.UpdateColorChangeFrequency(StressLevel, GetMaxStressLevel());
        //visuals.SetSpriteDirection(isFacingRight);
    }

    public void VisualJumpUp()
    {
        visuals.JumpUp();
    }

    public void ManagerStressColorVisuals()
    {
        visuals.StressColorChange(StressStateOn);
    }


    #endregion

    // ------------------------------------------------------------------

    public bool isGrounded()
    {
        BoxCastDrawer.BoxCastAndDraw(groundCheckPoint.position, groundCheckSize, 0, Vector2.down, 0f, groundLayer);
        return Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer);

        /*RaycastHit2D raycastHit = Physics2D.BoxCast(playerCollider.bounds.center, playerCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        BoxCastDrawer.BoxCastAndDraw(playerCollider.bounds.center, playerCollider.bounds.size, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;*/
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<LavaScript>() != null)
        {
            InLava(collision.GetComponent<LavaScript>().stressLevelLavaIncreaseRate);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<WinLineScript>() != null)
        {
            Debug.Log("Player touched the win line!");
            CheckWin();
        }
        if (collision.CompareTag("InstaKillArea"))
        {
            Die();
        }
    }

    public void InLava(float increaseRate)
    {
        //Debug.Log("Stress Levels should be skyrocketing right now!");
        StressLevel += increaseRate * Time.deltaTime;
    }
    
    public float GetMaxStressLevel()
    {
        if (Data != null) return Data.StressThreshold;
        else return 100;
    }

    public void Die()
    {
        if (!dead)
        {
            dead = true;
            OnDeath?.Invoke(this);
            ParticleSystem explosion = Instantiate(deathExplosion, transform.position, Quaternion.identity);
            Destroy(explosion, 3f);
            Destroy(this.gameObject, 0.01f);
        }
    }

    public void CheckWin()
    {
        onWin?.Invoke();
    }


    public Transform GetCameraTarget()
    {
        return CameraTarget;
    }
}
