using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New LevelData", menuName = "Level/Level Data")]
public class LevelData : ScriptableObject
{
    [Header("Win Height Values")]
    public float minimumWinHeight = 100f;
    public float winDistanceIncreaseRate = 10f;

    [Space(20)]
    [Header("LevelPrefabs")]
    public List<LevelSectionScript> piecesToSpawn;
}
