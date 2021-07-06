using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowBall : MonoBehaviour
{
    public Transform endPosition,middlePosition,leftPosition,rightPosition;
  

    public Transform football,leftMarker,rightMarker;
    public float speed;

    public Camera this_camera,football_camera,goal_post_camera_r,goal_post_camera_g;

    // Start is called before the first frame update
    void Start()
    {
        endPosition=middlePosition;        
    }
    //f to change the cameera angle behind the football
    //q - main camera w-football camera

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            this_camera.enabled=true;
            football_camera.enabled=false;
            goal_post_camera_r.enabled=false;
            goal_post_camera_g.enabled=false;
        }
        else if(Input.GetKeyDown(KeyCode.W))
        {
            football_camera.enabled=true;
            this_camera.enabled=false;
            goal_post_camera_r.enabled=false;
            goal_post_camera_g.enabled=false;
        }
        else if(Input.GetKeyDown(KeyCode.E))
        {
            goal_post_camera_r.enabled=true;
            this_camera.enabled=false;
            football_camera.enabled=false;
            goal_post_camera_g.enabled=false;
        }
        else if(Input.GetKeyDown(KeyCode.R))
        {
            goal_post_camera_g.enabled=true;
            this_camera.enabled=false;
            football_camera.enabled=false;
            goal_post_camera_r.enabled=false;

        }
        if(football.position.z<leftMarker.position.z)
        {
            endPosition=leftPosition;
        }
        else if(football.position.z>rightMarker.position.z)
        {
            endPosition=rightPosition;
        }
        else
        {
            endPosition=middlePosition;
        }

        Lerping();
    }
    void Lerping()
    {
        var pos=transform.position;
        pos.x=Mathf.Lerp(pos.x,endPosition.position.x,speed*Time.deltaTime);
        pos.y=Mathf.Lerp(pos.y,endPosition.position.y,speed*Time.deltaTime);
        pos.z=Mathf.Lerp(pos.z,endPosition.position.z,speed*Time.deltaTime);
        transform.position=pos;

    }
}
