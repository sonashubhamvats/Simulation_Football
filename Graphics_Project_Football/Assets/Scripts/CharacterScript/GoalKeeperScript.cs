using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalKeeperScript : MonoBehaviour
{
    public float speed;
    public BallMovement ballMovement;
    public bool moveTowardsGoalPoint;

    public type type;

    public Transform parentDefenderR,parentDefenderG;

    public Transform GoalPoint,originalPoint;

    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(moveTowardsGoalPoint)
        {
            var pos=transform.position;
            pos.x=Mathf.Lerp(pos.x,GoalPoint.position.x,speed*Time.deltaTime);
            pos.z=Mathf.Lerp(pos.z,GoalPoint.position.z,Time.deltaTime*speed);
            transform.position=pos;
        }
        else
        {
            var pos=transform.position;
            pos.x=Mathf.Lerp(pos.x,originalPoint.position.x,speed*Time.deltaTime);
            pos.z=Mathf.Lerp(pos.z,originalPoint.position.z,Time.deltaTime*speed);
            transform.position=pos;

        }
        if(moveTowardsGoalPoint)
        {

        }
    }
}
