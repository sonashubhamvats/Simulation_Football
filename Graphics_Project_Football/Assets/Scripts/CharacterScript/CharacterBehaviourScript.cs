using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public enum type
{
    red,green
}
public enum position
{
    defender,attacker
}
public enum state
{
    idle,possession,off,toGoal,intecepting,avoid,movingTowardsBall,rewind,forward
}
public class CharacterBehaviourScript : MonoBehaviour
{
    private int tempDec=0;
    public Transform goalPointR,goalPointG;

    [SerializeField]
    private List<Vector3> pos_during_rewind=new List<Vector3>();

    public GoalKeeperScript redGoalKeeper,greenGoalKeeper;

    public GameObject greenOffPlayer,redOffPlayer;
    public type type;

    public position position;
    public bool start;
    public NavMeshAgent agent;

    public state state;
    public float gravity;

    public GameObject football;

    public float distance_b_w_Ball;
    public float possessionTimer=0f;

    public float possessionMaxTime;

    public float currentPossesionMaxTime;

    public bool startPossesionTimer;



    public bool onceTheEndPosSetForOff;

    private BallMovement ballMovement;
    public GameObject[] TeamPlayerParent;
    private CharacterController thisCharacter;

    public float interceptingMaximumTimer,currInterceptingTime,defenderInterceptDistance;

    public bool startInterceptingTimer,preventFromIntercepting;

    public Transform middle_spawn_point,other_team_middle_spawn_point,other_team_end_spawn_point,breadth_spawn_point,our_end_spawn_point;
    // Start is called before the first frame update
    private void Awake() 
    {
        thisCharacter=GetComponent<CharacterController>();
        agent=GetComponent<NavMeshAgent>();
        ballMovement=football.GetComponent<BallMovement>();    
        gravity=2f;
        state=state.idle;
        start=true;
    }
    void Start()
    {
        if(GameManager.instance.rand==0)
        {
            if(type==type.red)
            {
                if(gameObject.name=="1")
                {
                    agent.SetDestination(football.transform.position);
                    state=state.movingTowardsBall;
                    
                }
            }

        }
        else
        {
            if(type==type.green)
            {
                if(gameObject.name=="1")
                {
                    agent.SetDestination(football.transform.position);
                    state=state.movingTowardsBall;
                }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        distance_b_w_Ball=Vector3.Distance(transform.position,football.transform.position);
        Movement();
        //when we are passing the ball once to start the game
        if(start)
        {
            AtStart();
        }
        else
        {
            if(state!=state.toGoal&&state!=state.off&&state!=state.rewind&&state!=state.forward)
            {
                Debug.Log("Here in normal");

                //a player is always ready to take the ball if that player has not passed just now
                if(state!=state.avoid&&state!=state.possession&&state!=state.off&&state!=state.toGoal&&state!=state.rewind&&state!=state.forward)
                {
                    if(distance_b_w_Ball<=agent.stoppingDistance)
                    {
                        //disable posession state for all other players
                        CharacterBehaviourScript[] characterBehaviourScripts= FindObjectsOfType<CharacterBehaviourScript>();
                    
                        for(int i=0;i<characterBehaviourScripts.Length;i++)
                        {
                            if(characterBehaviourScripts[i].state==state.possession)
                            {
                                characterBehaviourScripts[i].state=state.avoid;
                                characterBehaviourScripts[i].startPossesionTimer=false;
                                characterBehaviourScripts[i].possessionTimer=0;

                                //assigining the new path here itself as the idle statment will not work unless we are done with our path
                                float randx=Random.Range(middle_spawn_point.position.x,breadth_spawn_point.position.x);
                                float randz=0;
                                randz=Random.Range(characterBehaviourScripts[i].middle_spawn_point.position.z,characterBehaviourScripts[i].our_end_spawn_point.position.z);
                                var destination=new Vector3(randx,transform.position.y,randz);
                                characterBehaviourScripts[i].agent.SetDestination(destination);
                                StartCoroutine(characterBehaviourScripts[i].disableTheAvoid());
                        

                            }
        
                        }
                        //if the player has stopped
                        agent.isStopped=false;
                        agent.speed=6;
                        //if the player was a interceptor
                        //disable all the interceptors 
                        for(int i=0;i<GameManager.instance.list_of_interceptors.Count;i++)
                        {
                            GameManager.instance.list_of_interceptors[i].GetComponent<CharacterBehaviourScript>().currInterceptingTime=0;
                            GameManager.instance.list_of_interceptors[i].GetComponent<CharacterBehaviourScript>().startInterceptingTimer=false;
                            //prevent from intercepting
                            GameManager.instance.list_of_interceptors[i].GetComponent<CharacterBehaviourScript>().preventFromIntercepting=true;
                            StartCoroutine(GameManager.instance.list_of_interceptors[i].GetComponent<CharacterBehaviourScript>().AllowIntercepting());
                            GameManager.instance.list_of_interceptors[i].GetComponent<CharacterBehaviourScript>().state=state.idle;
                            //assigining the new path here itself as the idle statment will not work unless we are done with our path
                            float randx=Random.Range(middle_spawn_point.position.x,breadth_spawn_point.position.x);
                            float randz=0;
                            randz=Random.Range(GameManager.instance.list_of_interceptors[i].GetComponent<CharacterBehaviourScript>().middle_spawn_point.position.z
                            ,GameManager.instance.list_of_interceptors[i].GetComponent<CharacterBehaviourScript>().our_end_spawn_point.position.z);
                            var destination=new Vector3(randx,transform.position.y,randz);
                            GameManager.instance.list_of_interceptors[i].GetComponent<CharacterBehaviourScript>().agent.SetDestination(destination);

                        }
                        GameManager.instance.curr_count_of_interceptors=0;
                        GameManager.instance.list_of_interceptors.Clear();
                        //once the player recieves the ball it changes it state 
                        InitiateThePossesion();
                    }
                }
                if(startPossesionTimer)
                {
                    RunPossesionTimer();
                }
                if(state==state.idle)
                {
                    agent.speed=3;
                    //idle state move anywhere in your area
                    if(!agent.pathPending)
                    {
                        if(agent.remainingDistance<=agent.stoppingDistance)
                        {
                            if(agent.velocity.sqrMagnitude==0)
                            {
                                float randx=Random.Range(middle_spawn_point.position.x,breadth_spawn_point.position.x);
                                float randz=0;
                                randz=Random.Range(middle_spawn_point.position.z,our_end_spawn_point.position.z);
                                var destination=new Vector3(randx,transform.position.y,randz);
                                agent.SetDestination(destination);      

                            }
                        }
                    }
                    //these measures taken to prevent interceptors from clogging up
                    if(!preventFromIntercepting&&(GameManager.instance.curr_count_of_interceptors<GameManager.instance.maximumNoOfInterceptors))
                    {
                        //there is a one in 4 chance that the attacker will intercept the ball and for defender we will check if the distance is less then intercept
                        if(position==position.attacker)
                        {
                            int rand=Random.Range(0,4);
                            if(rand==0)
                            {
                                state=state.intecepting;
                                agent.SetDestination(football.transform.position);
                                //in pursuit speed
                                agent.speed=7;
                                startInterceptingTimer=true;
                                //increasing the no of interceptors
                                GameManager.instance.curr_count_of_interceptors++;
                                GameManager.instance.list_of_interceptors.Add(gameObject);
                            }
                            else
                            {
                                if(distance_b_w_Ball<=defenderInterceptDistance)
                                {
                                    state=state.intecepting;
                                    agent.SetDestination(football.transform.position);
                                    //in pursuit speed
                                    agent.speed=7;
                                    startInterceptingTimer=true;
                                    //increasing the no of interceptors
                                    GameManager.instance.curr_count_of_interceptors++;
                                    GameManager.instance.list_of_interceptors.Add(gameObject);
                                }
                            }
                        }

                    }
        
                }
                if(startInterceptingTimer)
                {
                    agent.SetDestination(football.transform.position);
                    StartInterceptingTimer();
                }
                //to simulate the goal
                if(Input.GetKeyDown(KeyCode.Space))
                {
                    if(resettingForGoalOrOff()) 
                        GameManager.instance.sideWhichMadeTheGoal=type;
                        ShootTowardsGoal();
                }
                //simulate off
                if(Input.GetKeyDown(KeyCode.O))
                {
                    GameManager.instance.tempStartIndexForOffCase=0;
                    if(resettingForGoalOrOff(false))
                        CreateOff();
                        
                }


            }
            else
            {
                if(state==state.toGoal)
                {
                    if(ballMovement.startStoringThePos)
                        storeThePositions();
                    else
                    {
                        agent.isStopped=true;
                        agent.ResetPath();
                    }
                }
                Debug.Log("Here in either in goal");
                if(state==state.rewind)
                {
                    RewindTheTime();
                    Debug.Log("Here in rewind");
                    ballMovement.rewind=true;
                   
                }
                if(state==state.forward)
                {
                    ballMovement.forward=true;
                    if(ballMovement.executeForward)
                        forwardTheTime();

                    Debug.Log("Forward time ");
                    
                }
                if(state==state.off)
                {
                    if(ballMovement.startStoringThePos)
                        storeThePositions();
                        
                    if(greenOffPlayer!=null)
                    {
                        if(!greenOffPlayer.GetComponent<CharacterBehaviourScript>().agent.pathPending)
                        {
                            if(greenOffPlayer.GetComponent<CharacterBehaviourScript>().agent.remainingDistance
                            <=greenOffPlayer.GetComponent<CharacterBehaviourScript>().agent.stoppingDistance)
                            {
                                if(greenOffPlayer.GetComponent<CharacterBehaviourScript>().agent.velocity.sqrMagnitude==0)
                                {
                                    if(!onceTheEndPosSetForOff)
                                    {
                                        Debug.Log("Passed");
                                        AudioManager.instance.playKickSound();
                                        //reached the destination now pass
                                        football.transform.parent=null;
                                        ballMovement.endposition=greenOffPlayer.transform;
                                        ballMovement.moving=true;
                                        ballMovement.startStoringThePos=true;
                                        onceTheEndPosSetForOff=true;
                                        agent.isStopped=true;
                                        agent.ResetPath();
                                    }

                                }
                            }
                        }
                    }
                    else if(redOffPlayer!=null)
                    {
                        if(!redOffPlayer.GetComponent<CharacterBehaviourScript>().agent.pathPending)
                        {
                            if(redOffPlayer.GetComponent<CharacterBehaviourScript>().agent.remainingDistance
                            <=redOffPlayer.GetComponent<CharacterBehaviourScript>().agent.stoppingDistance)
                            {
                                if(redOffPlayer.GetComponent<CharacterBehaviourScript>().agent.velocity.sqrMagnitude==0)
                                {
                                    if(!onceTheEndPosSetForOff)
                                    {
                                        Debug.Log("Passed");
                                        AudioManager.instance.playKickSound();
                                        //reached the destination now pass
                                        football.transform.parent=null;
                                        ballMovement.endposition=redOffPlayer.transform;
                                        ballMovement.moving=true;
                                        ballMovement.startStoringThePos=true;
                                        onceTheEndPosSetForOff=true;
                                        agent.isStopped=true;
                                        agent.ResetPath();

                                    }
                                }
                            }
                        }

                    }
                   
                    
                }

            }
        }
    }
    void CreateOff()
    {
        Debug.Log("In create off");
        float randx=Random.Range(middle_spawn_point.position.x,breadth_spawn_point.position.x);
        float randz=Random.Range(other_team_middle_spawn_point.position.z,other_team_end_spawn_point.position.z);
        if(type==type.red)
        {
            redOffPlayer.GetComponent<CharacterBehaviourScript>().agent.SetDestination(new Vector3(randx,transform.position.y,randz));
        }
        else
        {
            greenOffPlayer.GetComponent<CharacterBehaviourScript>().agent.SetDestination(new Vector3(randx,transform.position.y,randz));
        }
        
    }
    bool resettingForGoalOrOff(bool goal=true)
    {
        if(state==state.possession)
        {
            Debug.Log("In the state possesion");
            CharacterBehaviourScript[] characterBehaviourScripts= FindObjectsOfType<CharacterBehaviourScript>();

            
            int flag=0;
            for(int i=0;i<characterBehaviourScripts.Length;i++)
            {
                characterBehaviourScripts[i].pos_during_rewind.Clear();
                ballMovement.pos_ball_movement_transform.Clear();
                //assigining the off player
                if(characterBehaviourScripts[i].state!=state.possession&&type==characterBehaviourScripts[i].type&&flag==0&&!goal)
                {
                    CharacterBehaviourScript[] characterBehaviourScripts2= FindObjectsOfType<CharacterBehaviourScript>();
                    for(int j=0;j<characterBehaviourScripts2.Length;j++)
                    {
                        if(characterBehaviourScripts[i].type==type.red)
                        {
                            characterBehaviourScripts2[j].redOffPlayer=characterBehaviourScripts[i].gameObject;
                        }
                        else
                        {
                            characterBehaviourScripts2[j].greenOffPlayer=characterBehaviourScripts[i].gameObject;
                        }

                    }
                    flag=1;
                }
                if(goal)
                {
                    
                    //this statement should be executed only once
                    ballMovement.startStoringThePos=true;

                }
                
                //disable all possesion and interception and idle and make it avoid to allow a goal
                if(characterBehaviourScripts[i].state==state.possession)
                {
                    
                    characterBehaviourScripts[i].startPossesionTimer=false;
                    characterBehaviourScripts[i].possessionTimer=0;
                }
                if(characterBehaviourScripts[i].state==state.intecepting)
                {
                    
                    characterBehaviourScripts[i].currInterceptingTime=0;
                    characterBehaviourScripts[i].startInterceptingTimer=false;
                    //prevent from intercepting
                    characterBehaviourScripts[i].preventFromIntercepting=true;
                    //decreasing the no of interceptors in the Gameamnager script
                    GameManager.instance.curr_count_of_interceptors--;
                    //removing from the list
                    for(int j=0;j<GameManager.instance.list_of_interceptors.Count;j++)
                    {
                        if(GameManager.instance.list_of_interceptors[j].name==characterBehaviourScripts[i].gameObject.name&&
                        GameManager.instance.list_of_interceptors[j].tag==characterBehaviourScripts[i].gameObject.tag)
                        {                                    
                            //assigining the new path here itself as the idle statment will not work unless we are done with our path
                            GameManager.instance.list_of_interceptors.RemoveAt(j);
                            break;
                        }
                    }
                    StartCoroutine(characterBehaviourScripts[i].AllowIntercepting());
                }
                if(goal)
                {
                    characterBehaviourScripts[i].state=state.toGoal;
                    //assigining the new path here itself as the idle statment will not work unless we are done with our path
                    float randx=Random.Range(middle_spawn_point.position.x,breadth_spawn_point.position.x);
                    float randz=0;
                    randz=Random.Range(middle_spawn_point.position.z,characterBehaviourScripts[i].our_end_spawn_point.position.z);
                    var destination=new Vector3(randx,transform.position.y,randz);
                    characterBehaviourScripts[i].agent.SetDestination(destination);

                }
                else
                {
                    
                    
                    if(characterBehaviourScripts[i].state!=state.possession)
                    {
                        
                        Debug.Log("In not goal");
                        //assigining the new path here itself as the idle statment will not work unless we are done with our path
                        float randx=Random.Range(middle_spawn_point.position.x,breadth_spawn_point.position.x);
                        float randz=0;
                        if(type==characterBehaviourScripts[i].type)
                            randz=Random.Range(characterBehaviourScripts[i].middle_spawn_point.position.z,characterBehaviourScripts[i].our_end_spawn_point.position.z);
                        else
                            randz=Random.Range(characterBehaviourScripts[i].middle_spawn_point.position.z,characterBehaviourScripts[i].other_team_end_spawn_point.position.z);
                        var destination=new Vector3(randx,transform.position.y,randz);
                        characterBehaviourScripts[i].agent.SetDestination(destination);

                    }
                    else if(characterBehaviourScripts[i].state==state.possession)
                    {
                        agent.ResetPath();
                        agent.isStopped=true;
                    }
                    characterBehaviourScripts[i].state=state.off;

                }

                

            }
            return true;
        }
        return false;
    }
    void RewindTheTime()
    {
        if(pos_during_rewind.Count>tempDec)
        {
            Debug.Log("Ball rewind");
            //rewinding using the transform points
            transform.position=pos_during_rewind[tempDec];
            tempDec++;
        }
    }
    public void forwardTheTime()
    {
        if(tempDec>=1)
        {
            tempDec--;
            transform.position=pos_during_rewind[tempDec];
        }
        else
        {
            tempDec=0;

            int c=0;
            GoalKeeperScript[] goalKeeperScripts=FindObjectsOfType<GoalKeeperScript>();
            for(int i=0;i<goalKeeperScripts.Length;i++)
            {
                if(!goalKeeperScripts[i].moveTowardsGoalPoint)
                {
                    c++;
                }
            }

            Debug.Log("Assiging the toGoal state in the function forward time "+gameObject.name+" "+gameObject.GetComponent<CharacterBehaviourScript>().type);
            if(c==2)
                state=state.off;
            else
                state=state.toGoal;
        }
    }
    void storeThePositions()
    {
        
        var temp=transform.position;
        pos_during_rewind.Insert(0,temp);
        
        
    }
    void ShootTowardsGoal()
    {
        AudioManager.instance.playKickSound();
        football.transform.parent=null;
        if(type==type.red)
        {
            //forcing the goalkeepre to move towards the goal point
            greenGoalKeeper.moveTowardsGoalPoint=true;
            
            ballMovement.endposition=goalPointG;
            ballMovement.moving=true;
        }
        else
        {
            //forcing the goalkeepre to move towards the goal point
            redGoalKeeper.moveTowardsGoalPoint=true;

            ballMovement.endposition=goalPointR;
            ballMovement.moving=true;

        }
    }
    void StartInterceptingTimer()
    {
        if(currInterceptingTime<=interceptingMaximumTimer)
        {
            currInterceptingTime+=Time.deltaTime;
        }
        else
        {
            currInterceptingTime=0;
            startInterceptingTimer=false;
            //prevent from intercepting
            preventFromIntercepting=true;
            //decreasing the no of interceptors in the Gameamnager script
            GameManager.instance.curr_count_of_interceptors--;
            //removing from the list
            for(int i=0;i<GameManager.instance.list_of_interceptors.Count;i++)
            {
                if(GameManager.instance.list_of_interceptors[i].name==gameObject.name&&GameManager.instance.list_of_interceptors[i].tag==gameObject.tag)
                {
                   
                    //assigining the new path here itself as the idle statment will not work unless we are done with our path
                    float randx=Random.Range(middle_spawn_point.position.x,breadth_spawn_point.position.x);
                    float randz=0;
                    randz=Random.Range(GameManager.instance.list_of_interceptors[i].GetComponent<CharacterBehaviourScript>().middle_spawn_point.position.z
                    ,GameManager.instance.list_of_interceptors[i].GetComponent<CharacterBehaviourScript>().our_end_spawn_point.position.z);
                    var destination=new Vector3(randx,transform.position.y,randz);
                    GameManager.instance.list_of_interceptors[i].GetComponent<CharacterBehaviourScript>().agent.SetDestination(destination);
                    GameManager.instance.list_of_interceptors.RemoveAt(i);
                    break;
                }
            }
            StartCoroutine(AllowIntercepting());
            state=state.idle;
        }
    }
    public IEnumerator AllowIntercepting()
    {
        yield return new WaitForSeconds(4f);
        preventFromIntercepting=false;
    }
    void InitiateThePossesion()
    {
        state=state.possession;
        ballMovement.moving=false;
        ballMovement.endposition=null;

        //start the possesion timer
        int rand=Random.Range(0,2);
        currentPossesionMaxTime=Random.Range(2f,possessionMaxTime+1f);
        startPossesionTimer=true;
        
        //setting the parent as in possesion of the football
        football.transform.SetParent(this.transform);
        //setting a random position for the destination 
        SetTheDestination();
 
    }
    void SetTheDestination()
    {
        float randx=Random.Range(middle_spawn_point.position.x,breadth_spawn_point.position.x);
        float randz=0;
        if(type==type.green)
        {
            if(transform.position.z<middle_spawn_point.position.z)
            {
                //when the green player is on the opponent's side
                //one in a three chance that the player is gonna take their ball in thier own side lol
                int rand=Random.Range(0,3);
                if(rand==0)
                {
                    //yeeeeeeettttttt
                    randz=Random.Range(middle_spawn_point.position.z,our_end_spawn_point.position.z);
                }
                else
                {
                    //keep the ball on the oppnent's side
                    if(position==position.defender)
                    {
                        randz=Random.Range(middle_spawn_point.position.z,other_team_middle_spawn_point.position.z);
                    }
                    else
                    {
                        randz=Random.Range(other_team_middle_spawn_point.position.z,other_team_end_spawn_point.position.z);
                    }

                }
            }
            else
            {
                //when the green player is in their side
                if(position==position.defender)
                {
                    randz=Random.Range(middle_spawn_point.position.z,other_team_middle_spawn_point.position.z);
                }
                else
                {
                    randz=Random.Range(other_team_middle_spawn_point.position.z,other_team_end_spawn_point.position.z);
                }

            }
        }
        else
        {
            if(transform.position.z>middle_spawn_point.position.z)
            {
                //when the red player is on the opponent's side
                //one in a three chance that the player is gonna take their ball in thier own side lol
                int rand=Random.Range(0,3);
                if(rand==0)
                {
                    //yeeeeeeettttttt
                    randz=Random.Range(middle_spawn_point.position.z,our_end_spawn_point.position.z);
                }
                else
                {
                    //keep the ball on the oppnent's side
                    if(position==position.defender)
                    {
                        randz=Random.Range(middle_spawn_point.position.z,other_team_middle_spawn_point.position.z);
                    }
                    else
                    {
                        randz=Random.Range(other_team_middle_spawn_point.position.z,other_team_end_spawn_point.position.z);
                    }

                }
            }
            else
            {
                //when the red player is in their side
                if(position==position.defender)
                {
                    randz=Random.Range(middle_spawn_point.position.z,other_team_middle_spawn_point.position.z);
                }
                else
                {
                    randz=Random.Range(other_team_middle_spawn_point.position.z,other_team_end_spawn_point.position.z);
                }

            }
        }
        var destination=new Vector3(randx,transform.position.y,randz);
        agent.SetDestination(destination);                    

    }
    void RunPossesionTimer()
    {
        if(possessionTimer<currentPossesionMaxTime)
        {
            possessionTimer+=Time.deltaTime;
        }
        else
        {
            state=state.avoid;
            startPossesionTimer=false;
            possessionTimer=0;
            StartCoroutine(disableTheAvoid());
            PassToARandomPlayer();
        }
    }
    void AtStart()
    {
        if(state==state.movingTowardsBall)
        {
            //first posses the ball in the start
            if(!agent.pathPending)
            {
                if(agent.remainingDistance<=agent.stoppingDistance)
                {
                    if(agent.velocity.sqrMagnitude==0)
                    {
                        Debug.Log("Here");
                        state=state.possession;
                        //setting the parent as in possesion
                        football.transform.SetParent(this.transform);
                    }
                }
            }
        }
        if(state==state.possession)
        {
            PassToARandomPlayer();
            
            //the player should not be able to recieve the ball once it has just passed the ball
            state=state.avoid;
            //setting start =false everywhere and initializing the game
            CharacterBehaviourScript[] characterBehaviourScripts= FindObjectsOfType<CharacterBehaviourScript>();
        
            for(int i=0;i<characterBehaviourScripts.Length;i++)
            {
                characterBehaviourScripts[i].start=false;
            }
            
            StartCoroutine(disableTheAvoid());
        }

    }
    IEnumerator disableTheAvoid(int extra=0)
    {
        yield return new WaitForSeconds(2f+extra);
        if(state==state.avoid)
            state=state.idle;
    }
    void PassToARandomPlayer()
    {
        AudioManager.instance.playKickSound();
        //freeing the football
        football.transform.parent=null;
        //now pass the ball to only the players that are at some distance to the player

        List<Transform> tempListOfTransform=new List<Transform>();
        for(int i=0;i<2;i++)
        {
            for(int j=0;j<5;j++)
            {
                if(Vector3.Distance(transform.position,TeamPlayerParent[i].transform.GetChild(j).position)>15)
                {
                    tempListOfTransform.Add(TeamPlayerParent[i].transform.GetChild(j));
                }
            }
        }

        ballMovement.endposition=tempListOfTransform[Random.Range(0,tempListOfTransform.Count)];
        ballMovement.moving=true;

    }
    void ForStart()
    {

    }
    void Movement()
    {
        
        if(!thisCharacter.isGrounded)
        {
            thisCharacter.Move(new Vector3(0f,-gravity*Time.deltaTime,0f));
            //thisCharacter.Move(new Vector3(transform.position.x,transform.position.y-(gravity*Time.deltaTime),transform.position.z));
        }
    }
}
