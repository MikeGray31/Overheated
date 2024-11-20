using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScriptPool : MonoBehaviour
{
    public static LevelScriptPool Instance { get; private set; }

    private List<Queue<LevelSectionScript>> levelSections;


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        levelSections = new List<Queue<LevelSectionScript>>();
    }


    public void CreateLevelSectionPool(List<LevelSectionScript> piecesToSpawn)
    {
        levelSections.Clear();
        //Debug.Log("Creating level section pool!");
        foreach(LevelSectionScript l in piecesToSpawn)
        {
            Queue<LevelSectionScript> queue = new Queue<LevelSectionScript>();
            for(int i = 0; i < 5; i++)
            {
                LevelSectionScript newPiece = Instantiate(l);
                newPiece.gameObject.SetActive(false);
                queue.Enqueue(newPiece);
            }
            levelSections.Add(queue);
        }
    }

    public LevelSectionScript GetLevelSection(int index, LevelSectionScript prefab)
    {
        if(index < 0 || index >= levelSections.Count)
        {
            Debug.Log("index not within bounds of List!");
            return null;
        }

        Queue<LevelSectionScript> queue = levelSections[index];

        if(queue.Count == 0)
        {
            Debug.Log("No LevelSections in Queue! Creating a new one!");
            LevelSectionScript newPiece = Instantiate(prefab);
            queue.Enqueue(newPiece);
        }

        LevelSectionScript levelPiece = queue.Dequeue();
        //Debug.Log("levelPiece = " + levelPiece.ToString());
        //levelPiece.gameObject.SetActive(true);
        return levelPiece;
    }

    public void ReturnLevelSectionToQueue(LevelSectionScript levelPiece)
    {
        int index = levelPiece.indexInLevelDataList;
        if(index >= 0 && index < levelSections.Count)
        {
            levelSections[index].Enqueue(levelPiece);
            levelPiece.gameObject.SetActive(false);
        }
    }
}
