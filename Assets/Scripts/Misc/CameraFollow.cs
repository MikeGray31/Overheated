using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] protected Transform target;

    [SerializeField] private float followSpeed;
    [SerializeField] private float followDistanceX;
    [SerializeField] private float followDistanceY;


    private float xPosFrozen;
    private float yPosFrozen;
    [SerializeField] private bool freezeXPos = true;
    [SerializeField] private bool freezeYPos = false;

    // Start is called before the first frame update
    void Start()
    {
        target = FindFirstObjectByType<PlayerController>().GetCameraTarget();
        xPosFrozen = transform.position.x;
        yPosFrozen = transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        if(target != null)
        {
            FollowingWithFollowDistance();
            //FollowingWithoutFollowDistance();
        }
    }

    public void FreezeXPosition(bool freeze)
    {
        FreezeXPosition(freeze, transform.position.x);
    }

    public void FreezeXPosition(bool freeze, float xPosToFreezeTo)
    {
        
        freezeXPos = freeze;
        if (freeze) xPosFrozen = xPosToFreezeTo;
    }

    public void FreezeYPosition(bool freeze)
    {
        FreezeYPosition(freeze, transform.position.y);
    }

    public void FreezeYPosition(bool freeze, float yPosToFreezeTo)
    {
        freezeYPos = freeze;
        if (freeze) yPosFrozen = yPosToFreezeTo;
    }

    public void FollowingWithoutFollowDistance()
    {
        Vector3 newPos = new Vector3(target.position.x, target.position.y, -10);
        transform.position = Vector3.Slerp(transform.position, newPos, followSpeed * Time.deltaTime);
        //Debug.Log("camera x = " + transform.position.x + "\nnewPos.x = " + newPos.x);
    }

    public void FollowingWithFollowDistance()
    {
        Vector3 newPos = new Vector3(transform.position.x, transform.position.y, -10f);

        float XDiff = target.position.x - transform.position.x;
        float YDiff = target.position.y - transform.position.y;

        bool moveThisFrame = false;

        if (!freezeXPos)
        {
            if (XDiff > followDistanceX)
            {
                newPos.x = target.position.x - followDistanceX;
                moveThisFrame = true;
            }
            else if (XDiff < -followDistanceX)
            {
                newPos.x = target.position.x + followDistanceX;
                moveThisFrame = true;
            }
        }

        if (!freezeYPos) 
        {
            if (YDiff > followDistanceY)
            {
                newPos.y = target.position.y - followDistanceY;
                moveThisFrame = true;
            }
            else if (YDiff < -followDistanceY)
            {
                newPos.y = target.position.y + followDistanceY;
                moveThisFrame = true;
            }
        }
        

        //newPos.x = Mathf.Clamp(newPos.x, initialXPos - 0.05f, initialXPos + 0.05f);

        if (moveThisFrame)
        {
            transform.position = Vector3.Slerp(transform.position, newPos, followSpeed * Time.deltaTime);
            transform.position = newPos;
        }
    }


    //public void 
}
