using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSectionScript : MonoBehaviour
{
    [SerializeField] private Transform nextSpawnTransform;

    public Vector3 GetnextSpawnVector()
    {
        return nextSpawnTransform.position;
    }
}
