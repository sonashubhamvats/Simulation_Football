using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootballCameraScript : MonoBehaviour
{
    public Transform mvPoint1,mvPoint2;

    private int movPointer=0;
    public float tempTimer,speed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if(Input.GetKeyDown(KeyCode.F))
        {
            movPointer=movPointer==1?0:1;
        }        
        if(movPointer==0)
        {
            Vector3 pos=transform.position;
            Vector3 rot=transform.eulerAngles;
            pos.x=Mathf.Lerp(pos.x,mvPoint2.position.x,speed*Time.deltaTime);
            pos.y=Mathf.Lerp(pos.y,mvPoint2.position.y,speed*Time.deltaTime);
            pos.z=Mathf.Lerp(pos.z,mvPoint2.position.z,speed*Time.deltaTime);
            rot.x=Mathf.LerpAngle(rot.x,mvPoint2.transform.eulerAngles.x,speed*Time.deltaTime);
            rot.y=Mathf.LerpAngle(rot.y,mvPoint2.transform.eulerAngles.y,speed*Time.deltaTime);
            rot.z=Mathf.LerpAngle(rot.z,mvPoint2.transform.eulerAngles.z,speed*Time.deltaTime);

            transform.position=pos;
            transform.eulerAngles=rot;

            
        }
        else
        {
            Vector3 pos=transform.position;
            Vector3 rot=transform.eulerAngles;
            pos.x=Mathf.Lerp(pos.x,mvPoint1.position.x,speed*Time.deltaTime);
            pos.y=Mathf.Lerp(pos.y,mvPoint1.position.y,speed*Time.deltaTime);
            pos.z=Mathf.Lerp(pos.z,mvPoint1.position.z,speed*Time.deltaTime);
            rot.x=Mathf.LerpAngle(rot.x,mvPoint1.transform.eulerAngles.x,speed*Time.deltaTime);
            rot.y=Mathf.LerpAngle(rot.y,mvPoint1.transform.eulerAngles.y,speed*Time.deltaTime);
            rot.z=Mathf.LerpAngle(rot.z,mvPoint1.transform.eulerAngles.z,speed*Time.deltaTime);
            transform.position=pos;
            transform.eulerAngles=rot;


        }
 
       
    }
}
