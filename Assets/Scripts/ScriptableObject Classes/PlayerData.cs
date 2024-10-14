using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New PlayerData", menuName = "Player/Player Data")]
public class PlayerData : ScriptableObject
{
    [Header("StressStateValues")]
    public float StressThreshold;
    public float StressIncreaseRate;
    public float RelaxRate;

    public float StressKeyLastPressedTimeMax;

    [Space(20)]
    [Header("Run values")]
    public float moveSpeedRelaxed;
   
    public float moveSpeedStressed;
    
    public float VelPower;
    public float accerelation;
    public float deceleration;

    [Space(20)]
    [Header("Gravity and Jump values")]
    public float LastPressedJumpTimeStart;
    public float coyoteTime;
    [Space(5)]
    public float jumpForceRelaxed;
    public float jumpForceStressed;
    public float doubleJumpForceRelaxed;
    public float doubleJumpForceStressed;
    [Space(5)]
    public float gravityStrength; //Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
    public float gravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).
                                                 //Also the value the player's rigidbody2D.gravityScale is set to.
    public float fallGravityMult; //Multiplier to the player's gravityScale when falling.
    public float maxFallSpeed; //Maximum fall speed (terminal velocity) of the player when falling.

    public float fastFallGravityMult; //Larger multiplier to the player's gravityScale when they are falling and a downwards input is pressed.
                                      //Seen in games such as Celeste, lets the player fall extra fast if they wish.
    public float maxFastFallSpeed; //Maximum fall speed(terminal velocity) of the player when performing a faster fall.

    [Header("Wall Jump")]
    public Vector2 wallJumpForceRelaxed; //The actual force (this time set by us) applied to the player when wall jumping.
    public Vector2 wallJumpForceStressed;
    [Space(5)]
    [Range(0f, 1f)] public float wallJumpRunLerp; //Reduces the effect of player's movement while wall jumping.
    [Range(0f, 1.5f)] public float wallJumpTime; //Time after wall jumping the player's movement is slowed for.
    public bool doTurnOnWallJump; //Player will rotate to face wall jumping direction

}
