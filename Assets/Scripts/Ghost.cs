using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering.PostProcessing;

public class Ghost : MonoBehaviour
{
    //Public attributes of Ghost
    [SerializeField] Fellow player;
    [SerializeField] Material scaredMaterial;
    [SerializeField] Material alternativeMaterial;
    [SerializeField] int scoreToStart;
    [SerializeField] bool speedIncreases = false;
    [SerializeField] bool ambushing = false;
    [SerializeField] bool direct = false;
    
    //Private attributes of Ghost
    Material normalMaterial;
    NavMeshAgent agent;
    Vector3 ghostHouseDes = new Vector3((float)7.46000004, (float)0.409000009, (float)6.32999992);
    bool hiding = false; //A new  member  variable!
    bool eaten = false;
    bool leftGhostHouse = false;
    int scoreAtLevelStart;
    Vector3 startPos;


    // Start is called before the first frame update
    void Start()
    {
        //Stores the start position of ghost
        startPos = transform.position;
        normalMaterial = GetComponent<Renderer>().material;
        //Starts StartGhost()
        StartCoroutine(StartGhost());
        scoreAtLevelStart = player.GetScore();
    }

    //For restoring and starting the ghosts behaviours
    private IEnumerator StartGhost()
    {
        while((player.GetScore()- scoreAtLevelStart) != scoreToStart)
        {
            yield return null;
        }
        agent = GetComponent<NavMeshAgent>();
        agent.destination = PickRandomPosition();
        leftGhostHouse = true;
    }

    //Move to a random position
    Vector3 PickRandomPosition()
    {
        Vector3 destination = transform.position;
        Vector2 randomDirection = UnityEngine.Random.insideUnitCircle * 8.0f;
        destination.x += randomDirection.x;
        destination.z += randomDirection.y;

        NavMeshHit navHit;
        NavMesh.SamplePosition(destination, out navHit, 8.0f, NavMesh.AllAreas); 
        
        return navHit.position;
    }

    //Finds hiding spot away from Fellow
    Vector3 PickHidingPlace()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position ). normalized;
        
        NavMeshHit navHit;
        NavMesh.SamplePosition(transform.position -(directionToPlayer * 8.0f), out navHit, 8.0f, NavMesh.AllAreas);
        return navHit.position;
    }

    // Update is called once per frame
    void Update()
    {
        //Ghost moves normally unless going to ghost eaten
        if (leftGhostHouse)
        {
            if (!eaten)
            {
                NormalEnemyBehaviour();
            }
            else
            {
                agent.destination = ghostHouseDes;
            }
        }
        
        //Increases ghost speed for those with speedIncreases as true
        if (speedIncreases)
        {
            switch (player.GetScore() - scoreAtLevelStart)
            {
                case var _ when player.GetScore() > 1000:
                    agent.speed += (float)0.5;
                    break;

                case var _ when player.GetScore() > 2500:
                    agent.speed += (float)0.5;
                    break;

                case var _ when player.GetScore() > 4500:
                    agent.speed += (float)1;
                    break;
            }
        }
        
    }

    //The normal movement behaviour of the ghost
    private void NormalEnemyBehaviour()
    {
        if (CanSeePlayer())
        {
            //If not ambushing goes straight to players location
            if (!ambushing)
            {
                agent.destination = player.transform.position;
            }
            else
            {
                //If ambushing attempts to go where player will go next
                switch (player.GetDirectionGoing())
                {
                    case ("right"):
                        agent.destination = new Vector3(player.transform.position.x + 2, player.transform.position.y, player.transform
                    .position.z);
                        break;
                    case ("left"):
                        agent.destination = new Vector3(player.transform.position.x - 2, player.transform.position.y, player.transform
                    .position.z);
                        break;
                }
                
            }
        }
        else
        {
            if (agent.remainingDistance < 0.5f && !direct)
            {
                agent.destination = PickRandomPosition();
            }
            else if (direct)
            {
                agent.destination = player.transform.position;
            }
        }

        //If powerup is active ghost will change to scared material
        if (player.PowerupActive())
        {
            GetComponent<Renderer>().material = scaredMaterial;
        }
        else
        {
            GetComponent<Renderer>().material = normalMaterial;
        }

        if (player.PowerupActive())
        {

            if (!hiding || agent.remainingDistance < 0.5f)
            {
                hiding = true;
                agent.destination = PickHidingPlace();
                GetComponent<Renderer>().material = scaredMaterial;
            }
        }
        else
        {
            //Ghost will hide from player
            if (hiding)
            {
                GetComponent<Renderer>().material = normalMaterial;
                hiding = false;
            }

            if (agent.remainingDistance < 0.5f)
            {
                agent.destination = PickRandomPosition();
                hiding = false;
                GetComponent<Renderer>().material = normalMaterial;
            }
        }
    }

    bool CanSeePlayer()
    {
        Vector3 rayPos = transform.position;
        Vector3 rayDir = (player.transform.position - rayPos).normalized;
        RaycastHit info;
        if (Physics.Raycast(rayPos, rayDir, out info))
        {
            if (info.transform.CompareTag("Fellow"))
            {
                //the  ghost  can  see  the  player!
                return true;
            }
        }
        return false;
    }

    //When eaten changes to transparent material
    public void Eaten()
    {
        eaten = true;
        GetComponent<Renderer>().material = alternativeMaterial;
        GetComponent<CapsuleCollider>().isTrigger = true;
        
    }

    //Restoring ghost from being eaten
    public void ReturnFromEaten()
    {
        eaten = false;
        GetComponent<Renderer>().material = normalMaterial;
        GetComponent<CapsuleCollider>().isTrigger = true;
    }

    //Restoring the ghost for the next level
    public void RestoreGhostForNextLevel()
    {
        gameObject.transform.position = startPos;
        hiding = false;
        GetComponent<Renderer>().material = normalMaterial;
        normalMaterial = GetComponent<Renderer>().material;
        scoreAtLevelStart = player.GetScore();
        leftGhostHouse = false;
        StartCoroutine(StartGhost());
    }

    //Moves to starting location
    public void GoToStartPosition()
    {
        gameObject.GetComponent<NavMeshAgent>().Warp(startPos);
    }

    //Increases ghost speed
    public void SpeedUp()
    {
        gameObject.GetComponent<NavMeshAgent>().speed += 0.1f;
    }



}
