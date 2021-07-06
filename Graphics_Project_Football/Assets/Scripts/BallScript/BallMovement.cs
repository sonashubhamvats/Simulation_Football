using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BallMovement : MonoBehaviour
{
    public static BallMovement instance;

    
    public List<Vector3> pos_ball_movement_transform=new List<Vector3>();

    public bool rewind;

    public bool startStoringThePos;

    public bool moving;

    public float speed;

    public int tempDec;

    public bool forward,executeForward;

    private CharacterBehaviourScript[] characterBehaviourScripts;

    public Transform endposition;
    private void Awake() 
    {
        moving=false;
        if(instance==null)
        {
            instance=this;
        }    
        else
        {
            Destroy(gameObject);
        }
        characterBehaviourScripts=FindObjectsOfType<CharacterBehaviourScript>();
    }
    // Start is called before the first frame update
    void Start()
    {
       tempDec=0;
       pos_ball_movement_transform.Clear();
    }

    // Update is called once per frame
    void Update()
    {
        if(moving)
        {
            var pos=transform.position;
            pos.x=Mathf.Lerp(pos.x,endposition.position.x,speed*Time.deltaTime);
            pos.z=Mathf.Lerp(pos.z,endposition.position.z,speed*Time.deltaTime);
            transform.position=pos;
            
        }
        if(startStoringThePos)
        {
            
            var temp=transform.position;
            pos_ball_movement_transform.Insert(0,temp);
            
            if(Vector3.Distance(transform.position,endposition.position)<1f)
            {
                Debug.Log("Here");
                startStoringThePos=false;
                moving=false;
                if(FindObjectOfType<CharacterBehaviourScript>().state==state.toGoal)
                {
                    GameManager.instance.goal=true;
                }
            }
 
        }
        if(rewind)
        {
            
            if(pos_ball_movement_transform.Count>tempDec)
            {
                Debug.Log("Ball rewind");
                //rewinding using the transform points
                transform.position=pos_ball_movement_transform[tempDec];
                tempDec++;
            }
            else
            {
                rewind=false;
            }
        }
        if(forward)
        {
            if(executeForward)
            {
                if(tempDec>=1)
                {
                    
                    tempDec--;
                    transform.position=pos_ball_movement_transform[tempDec];
                    
                }
                else
                {
                    
                    tempDec=0;
                    forward=false;
                    GameManager.instance.TimeControllerButtonG.GetComponent<Image>().sprite=GameManager.instance.start;
                    GameManager.instance.TimeControllerButtonG.SetActive(false);
                    GameManager.instance.TimeControllerButtonO.GetComponent<Image>().sprite=GameManager.instance.start;
                    GameManager.instance.TimeControllerButtonO.SetActive(false);
                    
                }

            }
            if(Input.GetKey(KeyCode.Mouse0))
                executeForward=true;
            else if(Input.GetKeyUp(KeyCode.Mouse0))
            {
                executeForward=false;
            }

        }
    }
}
