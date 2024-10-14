using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaScript : MonoBehaviour
{
    [SerializeField]
    protected LavaData Data;

    [SerializeField]
    private float currentMinRisingSpeed;
    private float risingSpeed;

    public float stressLevelLavaIncreaseRate { get; private set; }

    private void Start()
    {
        stressLevelLavaIncreaseRate = Data.StressLevelLavaIncreaseRate;
        currentMinRisingSpeed = Data.MinimumSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateRisingSpeed();
        MoveLavaUp();
        IncreaseMinRisingSpeed();
    }

    public void CalculateRisingSpeed()
    {
        float playerDistanceFromLava = GameManager.Instance.GetPlayerHeight() - transform.position.y;
        //Debug.Log("playerDistanceFromLava = " + playerDistanceFromLava);
        if (playerDistanceFromLava <= Data.MinimumSpeedDistance)
        {
            if (playerDistanceFromLava < -9f)
            {
                //Debug.Log("Lava should stop!");
                risingSpeed = 0;
            }
            else if(playerDistanceFromLava < 0.5f)
            {
                risingSpeed = Data.MinimumSpeed;
            }
            else
            {
                risingSpeed = currentMinRisingSpeed;
            }
        }
        else if (playerDistanceFromLava > Data.MinimumSpeedDistance)
        {
            risingSpeed = Mathf.Max(playerDistanceFromLava * Data.DistanceMultiplier, Data.MinimumSpeed);
        }
        
        
    }

    public void MoveLavaUp()
    {
        transform.Translate(Vector2.up * risingSpeed * Time.deltaTime);
    }

    public void IncreaseMinRisingSpeed()
    {
        currentMinRisingSpeed += Data.MinSpeedIncreaseRate * Time.deltaTime;
    }
}
