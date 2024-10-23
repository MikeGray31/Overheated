using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSectionScript : MonoBehaviour
{
    [SerializeField] private Transform nextSpawnTransform;
    public int indexInLevelDataList;

    public Vector3 GetnextSpawnVector()
    {
        return nextSpawnTransform.position;
    }

    private void Update()
    {
        if (GameManager.Instance.lava.transform.position.y - 40 > this.transform.position.y) 
        {
            LevelScriptPool.Instance.ReturnLevelSectionToQueue(this);
        }
    }
}
