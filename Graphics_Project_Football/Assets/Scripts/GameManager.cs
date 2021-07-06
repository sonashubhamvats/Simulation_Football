using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class GameManager : MonoBehaviour
{
    public int rand;

    public static GameManager instance;

    public TextMeshProUGUI score;

    public int scoreR,scoreG;

    public type sideWhichMadeTheGoal;

    public int maximumNoOfInterceptors,curr_count_of_interceptors;

    public BallMovement ballMovement;

    public bool goal;

    public GameObject[] goalSequenceObjets,offSequenceObjects; 

    public Sprite start,forward;

    public GameObject TimeControllerButtonG,TimeControllerButtonO;

    public int tempStartIndexForOffCase;

  

    public GameObject lineGoalR,lineGoalG,lineOff;

    public Transform breadthMarker,tempMarker;
    public List<GameObject> list_of_interceptors=new List<GameObject>();
    private void Awake() {
        tempStartIndexForOffCase=0;
        rand=Random.Range(0,2);
        if(instance==null)
        {
            instance=this;
        }
        else
        {
            Destroy(gameObject);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }


    // Update is called once per frame
    void Update()
    {

        if(Input.GetKeyDown(KeyCode.X)&&ballMovement.tempDec>(ballMovement.pos_ball_movement_transform.Count-1))
        {
            
        }
        
        if(goal)
        {
            AudioManager.instance.playGoalH();
            for(int i=0;i<goalSequenceObjets.Length;i++)
            {
                goalSequenceObjets[i].SetActive(true);
            }
            if(sideWhichMadeTheGoal==type.red)
            {
                scoreR++;
                lineGoalR.SetActive(true);
            }
            else
            {
                scoreG++;
                lineGoalG.SetActive(true);
            }
            score.text=scoreR+"-"+scoreG;
            goal=false;
        }
        KeepCheckingForReturnToGoalOfBall();
        KeepCheckingForEndOfRewindInGoal();
        KeepCheckingForTheOffPlayerToTouchTheBall();
        KeepCheckForEndOfRewindInOff();
    }
    public void rewindOff()
    {
        if(FindObjectOfType<CharacterBehaviourScript>().state==state.off)
        {
            FindObjectOfType<BallMovement>().rewind=true;
            CharacterBehaviourScript[] characterBehaviourScripts= FindObjectsOfType<CharacterBehaviourScript>();
            
            for(int j=0;j<characterBehaviourScripts.Length;j++)
            {
                if(characterBehaviourScripts[j].state==state.off)
                    characterBehaviourScripts[j].state=state.rewind;
            }    

            for(int j=0;j<offSequenceObjects.Length;j++)
            {
                offSequenceObjects[j].SetActive(false);
            }            
        }
        
    }
    void KeepCheckForEndOfRewindInOff()
    {
        int c=0;
        GoalKeeperScript[] goalKeeperScripts=FindObjectsOfType<GoalKeeperScript>();
        for(int i=0;i<goalKeeperScripts.Length;i++)
        {
            if(!goalKeeperScripts[i].moveTowardsGoalPoint)
            {
                c++;
            }
        }
        if(FindObjectOfType<CharacterBehaviourScript>().state==state.rewind&&ballMovement.tempDec>(ballMovement.pos_ball_movement_transform.Count-1)&&ballMovement.tempDec!=0&&c==2)
            TimeControllerButtonO.SetActive(true);
    }
    public void forwardOff()
    {
        if(TimeControllerButtonO.GetComponent<Image>().sprite==start)
        {
            FindObjectOfType<BallMovement>().forward=true;
            CharacterBehaviourScript[] characterBehaviourScripts= FindObjectsOfType<CharacterBehaviourScript>();
            
            for(int j=0;j<characterBehaviourScripts.Length;j++)
            {
                if(characterBehaviourScripts[j].state==state.rewind)
                    characterBehaviourScripts[j].state=state.forward;
            }   
            TimeControllerButtonO.GetComponent<Image>().sprite=forward;
        }
        else
        {
            FindObjectOfType<BallMovement>().executeForward=true;
        }
    }
    void KeepCheckingForTheOffPlayerToTouchTheBall()
    {
        if(ballMovement.endposition!=null)
        {
            if(Vector3.Distance(ballMovement.transform.position,ballMovement.endposition.position)<=1f)
            {
                
                if(FindObjectOfType<CharacterBehaviourScript>().state==state.off&&!ballMovement.rewind&&!ballMovement.forward)
                {
                    Debug.Log("Here in off ");
                    
                    for(int i=tempStartIndexForOffCase;i<offSequenceObjects.Length;i++)
                    {
                        offSequenceObjects[i].SetActive(true);
                        if(tempStartIndexForOffCase==0)
                        {
                            lineOff.SetActive(true);
                            var pos=lineOff.transform.position;
                            if(ballMovement.transform.position.z<tempMarker.position.z)
                            {
                                pos.z=ballMovement.transform.position.z+2f;
                            }
                            else
                            {
                                pos.z=ballMovement.transform.position.z-2f;
                            }
                            lineOff.transform.position=pos;

                            tempStartIndexForOffCase++;
                        }
                    }
                }
                    
            }

        }

    }
    void KeepCheckingForReturnToGoalOfBall()
    {
        GoalKeeperScript[] goalKeeperScripts=FindObjectsOfType<GoalKeeperScript>();
        for(int i=0;i<goalKeeperScripts.Length;i++)
        {
            if(goalKeeperScripts[i].moveTowardsGoalPoint&&FindObjectOfType<CharacterBehaviourScript>().state!=state.rewind)
            {
                if(ballMovement.endposition!=null)
                {
                    //only when ball is with the goalkeeper
                    if(Vector3.Distance(ballMovement.endposition.position,ballMovement.transform.position)<=1f)
                    {
                        for(int j=1;j<goalSequenceObjets.Length;j++)
                        {
                            goalSequenceObjets[j].SetActive(true);
                        }
                    }

                }
            }
        }

    }
    void KeepCheckingForEndOfRewindInGoal()
    {
        GoalKeeperScript[] goalKeeperScripts=FindObjectsOfType<GoalKeeperScript>();
        for(int i=0;i<goalKeeperScripts.Length;i++)
        {
            if(goalKeeperScripts[i].moveTowardsGoalPoint&&FindObjectOfType<CharacterBehaviourScript>().state==state.rewind)
            {
                //checking if the ball is at its initial position
                if(ballMovement.tempDec>(ballMovement.pos_ball_movement_transform.Count-1))
                {
                    TimeControllerButtonG.SetActive(true);
                }
            }
        }
    }
    public void ContinueAfterGoal()
    {
        AudioManager.instance.playKickSound();
        GoalKeeperScript[] goalKeeperScripts=FindObjectsOfType<GoalKeeperScript>();
        for(int i=0;i<goalKeeperScripts.Length;i++)
        {
            if(goalKeeperScripts[i].moveTowardsGoalPoint)
            {
                CharacterBehaviourScript[] characterBehaviourScripts= FindObjectsOfType<CharacterBehaviourScript>();
                if(characterBehaviourScripts[0].state!=state.rewind||characterBehaviourScripts[0].state!=state.forward)
                {
                    for(int j=0;j<characterBehaviourScripts.Length;j++)
                    {
                        
                        characterBehaviourScripts[j].agent.speed=3;
                        characterBehaviourScripts[j].state=state.idle;
                    }
                    //passing the ball
                    if(goalKeeperScripts[i].type==type.red)
                    {
                        int rand=Random.Range(0,goalKeeperScripts[i].parentDefenderR.transform.childCount-1);
                        Debug.Log(rand);
                        ballMovement.endposition=goalKeeperScripts[i].parentDefenderR.transform.GetChild(rand);
                        ballMovement.moving=true;
                    }
                    else
                    {
                        int rand=Random.Range(0,goalKeeperScripts[i].parentDefenderG.transform.childCount-1);
                        ballMovement.endposition=goalKeeperScripts[i].parentDefenderG.transform.GetChild(rand);
                        ballMovement.moving=true;

                    }

                    for(int j=0;j<goalKeeperScripts.Length;j++)
                    {
                        goalKeeperScripts[j].moveTowardsGoalPoint=false;
                    }
                    for(int j=0;j<goalSequenceObjets.Length;j++)
                    {
                        goalSequenceObjets[j].SetActive(false);
                    }
                    lineGoalG.SetActive(false);
                    lineGoalR.SetActive(false);
                    break;
                    
                }
            }        

        }
        
    }
    public void ContinueAfterOff()
    {
        AudioManager.instance.playKickSound();
        CharacterBehaviourScript[] characterBehaviourScripts=FindObjectsOfType<CharacterBehaviourScript>();
        if(characterBehaviourScripts[0].state==state.off)
        {
            for(int i=0;i<characterBehaviourScripts.Length;i++)
            {
                if(characterBehaviourScripts[i].greenOffPlayer!=null)
                {
                    characterBehaviourScripts[i].greenOffPlayer.GetComponent<CharacterBehaviourScript>().onceTheEndPosSetForOff=false;
                    if(characterBehaviourScripts[i].name!=characterBehaviourScripts[i].greenOffPlayer.name)
                    {
                        FindObjectOfType<BallMovement>().moving=false;
                        FindObjectOfType<BallMovement>().startStoringThePos=false;
                        FindObjectOfType<BallMovement>().transform.position=new Vector3(characterBehaviourScripts[i].transform.position.x,ballMovement.transform.position.y,
                        characterBehaviourScripts[i].transform.position.z+1f);
                        break;
                    }
                    
                }
                else
                {
                    characterBehaviourScripts[i].redOffPlayer.GetComponent<CharacterBehaviourScript>().onceTheEndPosSetForOff=false;

                    if(characterBehaviourScripts[i].name!=characterBehaviourScripts[i].redOffPlayer.name)
                    {
                        FindObjectOfType<BallMovement>().moving=false;
                        FindObjectOfType<BallMovement>().startStoringThePos=false;
                        FindObjectOfType<BallMovement>().transform.position=new Vector3(characterBehaviourScripts[i].transform.position.x,ballMovement.transform.position.y,
                        characterBehaviourScripts[i].transform.position.z+1f);
                        break;
                    }

                }
            } 
            for(int i=0;i<characterBehaviourScripts.Length;i++)
            {
                characterBehaviourScripts[i].state=state.idle;
                characterBehaviourScripts[i].agent.speed=3;
                characterBehaviourScripts[i].redOffPlayer=null;
                characterBehaviourScripts[i].greenOffPlayer=null;
                characterBehaviourScripts[i].GetComponent<CharacterBehaviourScript>().onceTheEndPosSetForOff=false;
            }
            lineOff.SetActive(false);
                

        }
        for(int j=0;j<offSequenceObjects.Length;j++)
        {
            offSequenceObjects[j].SetActive(false);
        }

    }
    public void Rewind()
    {
        GoalKeeperScript[] goalKeeperScripts=FindObjectsOfType<GoalKeeperScript>();
        for(int i=0;i<goalKeeperScripts.Length;i++)
        {
            if(goalKeeperScripts[i].moveTowardsGoalPoint)
            {
                //only when ball is with the goalkeeper
                if(Vector3.Distance(ballMovement.endposition.position,ballMovement.transform.position)<=2f)
                {
                    //call rewind for all 
                    CharacterBehaviourScript[] characterBehaviourScripts= FindObjectsOfType<CharacterBehaviourScript>();
                    
                    for(int j=0;j<characterBehaviourScripts.Length;j++)
                    {
                        if(characterBehaviourScripts[j].state==state.toGoal)
                            Debug.Log("Rewind set");
                            characterBehaviourScripts[j].state=state.rewind;
                    }    
                    for(int j=0;j<goalSequenceObjets.Length;j++)
                    {
                        goalSequenceObjets[j].SetActive(false);
                    }

                }
            }
        }       
    }
    public void Forward()
    {
        Debug.Log("Rewind set");
        if(TimeControllerButtonG.GetComponent<Image>().sprite==start)
        {
            GoalKeeperScript[] goalKeeperScripts=FindObjectsOfType<GoalKeeperScript>();
            for(int i=0;i<goalKeeperScripts.Length;i++)
            {
                if(goalKeeperScripts[i].moveTowardsGoalPoint)
                {
                    //checking if the ball is at its initial position
                    if(ballMovement.tempDec>(ballMovement.pos_ball_movement_transform.Count-1))
                    {
                        //forward
                        CharacterBehaviourScript[] characterBehaviourScripts= FindObjectsOfType<CharacterBehaviourScript>();
                        
                        for(int j=0;j<characterBehaviourScripts.Length;j++)
                        {
                            if(characterBehaviourScripts[j].state==state.rewind)
                                characterBehaviourScripts[j].state=state.forward;
                        }    

                    }
                }
            }
            TimeControllerButtonG.GetComponent<Image>().sprite=forward;
        }
        else
        {
            ballMovement.executeForward=true;
        }
    }

}
