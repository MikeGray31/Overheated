using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LavaData", menuName = "Lava/LavaData")]
public class LavaData : ScriptableObject
{
    public float MinimumSpeed = 2f;
    public float MinimumSpeedDistance = 7f;
    public float DistanceMultiplier = 0.3f;
    public float MinSpeedIncreaseRate = 0.05f;

    [Space(20)]
    [Header("Affects the player")]
    public float StressLevelLavaIncreaseRate = 15f;
}
