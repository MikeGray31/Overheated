using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVisualsScript : MonoBehaviour
{
    [SerializeField] private SpriteRenderer renderer;
    [SerializeField] private Animator animator;
    [SerializeField] private ParticleSystem smoke;

    private float stressColorChangeTimerMax = 0.5f;
    private bool stressedColorOn;
    private float stressColorTimer;

    // Start is called before the first frame update

    private void Awake()
    {
        animator.GetComponent<Animator>();
        renderer = GetComponent<SpriteRenderer>();
        
        stressColorTimer = 0f;
    }

    public void UpdateVisuals(bool isGrounded, float xVel, float yVel, bool isFacingRight)
    {

        animator.SetBool("grounded", isGrounded);
        if (isGrounded)
        {
            if(Mathf.Abs(xVel) > 0.05f)
            {
                animator.SetBool("Running", true);
            }
            else
            {
                animator.SetBool("Running", false);
            }
        }

        if (yVel < 0.05f)
        {
            animator.SetBool("JumpingUp", false);
        }
        //animator.SetBool("isJumpingUp", isJumpingUp);
        SetSpriteDirection(isFacingRight);
    }

    public void JumpUp()
    {
        animator.SetBool("JumpingUp", true);
    }

    public void SetSpriteDirection(bool isFacingRight)
    {
        if (isFacingRight)
        {
            renderer.flipX = false;
        }
        else
        {
            renderer.flipX = true;
        }
    }

    public void StressColorChange(bool stressed)
    {
        stressedColorOn = stressed;
    }

    public void UpdateColorChangeFrequency(float stressLevel, float MaxStressLevel)
    {
        float percentage = stressLevel / MaxStressLevel;
        stressColorChangeTimerMax = Mathf.Lerp(0.5f, 0.1f, percentage);
        //Debug.Log("colorChangeTimerMax = " + stressColorChangeTimerMax);
        UpdateSmokeAmount(stressLevel, MaxStressLevel);
    }


    public void UpdateSmokeAmount(float StressLevel, float MaxStressLevel)
    {
        smoke.emissionRate = Mathf.Lerp(0, 200, StressLevel / MaxStressLevel);
    }

    private void Update()
    {
        stressColorTimer += Time.deltaTime;


        if (stressedColorOn)
        {
            if (stressColorTimer >= (stressColorChangeTimerMax))
            {
                renderer.color = Color.white;
                stressColorTimer = 0f;
            }
            else if (stressColorTimer >= (stressColorChangeTimerMax - 0.099f))
            {
                renderer.color = Color.red;
            }
        }
        else
        {
            renderer.color = Color.white;
        }
        
    }
}
