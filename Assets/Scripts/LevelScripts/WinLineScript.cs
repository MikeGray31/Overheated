using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLineScript : MonoBehaviour
{

    private Vector3 currentDestination;

    // Start is called before the first frame update
    void Start()
    {
        currentDestination = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentDestination();
        SetWinLinePosition();
    }

    public void GetCurrentDestination()
    {
        currentDestination.y = GameManager.Instance.GetWinDistance();
    }

    public void SetWinLinePosition()
    {
        float distance = Vector3.Distance(transform.position, currentDestination);
        if (distance > 0.05f)
        {
            float speed = Mathf.Max(4 * Mathf.Abs(currentDestination.y - transform.position.y), 2f);
            transform.position = Vector3.MoveTowards(transform.position, currentDestination, speed * Time.deltaTime);
        }
        
    }
}
