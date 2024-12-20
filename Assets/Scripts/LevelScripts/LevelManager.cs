using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelData Data;
    [SerializeField] private LevelSectionScript highestLevelSection;
    

    private void Awake()
    {
        LevelScriptPool.Instance.CreateLevelSectionPool(Data.piecesToSpawn);

        for (int i = 0; i < Data.piecesToSpawn.Count; i++)
        {
            LevelSectionScript l = Data.piecesToSpawn[i];
            l.indexInLevelDataList = i;
        }
        
        highestLevelSection = FindHighestLevelSection();
    }

    private void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.G))
        {
            SpawnLevelSection();
        }*/
    }

    public LevelSectionScript FindHighestLevelSection()
    {
        LevelSectionScript[] levelSections = FindObjectsOfType<LevelSectionScript>();
        if(levelSections.Length > 0)
        {
            LevelSectionScript currentHighest = levelSections[0];
            foreach (LevelSectionScript section in levelSections)
            {
                if (section.transform.position.y > currentHighest.transform.position.y)
                {
                    currentHighest = section;
                }
            }
            return currentHighest;
        }
        return null;
    }
    
    public void SpawnLevelSection()
    {
        //LevelSectionScript newSection = Instantiate(GetNewSection(), highestLevelSection.GetnextSpawnVector(), Quaternion.identity);
        
        int index = Random.Range(0, Data.piecesToSpawn.Count);
        //Debug.Log("LevelScriptPool exists => " + (LevelScriptPool.Instance != null));
        LevelSectionScript newSection = LevelScriptPool.Instance.GetLevelSection(index, Data.piecesToSpawn[index]);
        
        newSection.gameObject.SetActive(true);
        newSection.transform.position = highestLevelSection.GetnextSpawnVector();
        
        highestLevelSection = newSection;
    }

    public LevelSectionScript GetNewSection()
    {
        if(Data.piecesToSpawn.Count > 0)
        {
            int section = Random.Range(0, Data.piecesToSpawn.Count);
            return Data.piecesToSpawn[section];
        }
        else
        {
            Debug.LogWarning("no level pieces available");
            return null;
        }
    }

    public LevelData GetData()
    {
        return Data;
    }

    public LevelSectionScript GetHighestLevelSection()
    {
        return highestLevelSection;
    }
}
