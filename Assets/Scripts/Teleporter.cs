using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Teleporter : MonoBehaviour
{
    [SerializeField] GameObject leftTeleporter;
    [SerializeField] GameObject rightTeleporter;

    private void OnTriggerEnter(Collider other)
    {
        //When Player enters a teleporter
        if (other.gameObject.CompareTag("Player"))
        {
            //If collided with left teleporter move to right
            if (gameObject == leftTeleporter)
            {

                other.transform.position = new Vector3((rightTeleporter.transform.position.x - 1), rightTeleporter.transform.position.y,
                    rightTeleporter.transform.position.z);
            }
            //If collided with right teleporter move to left
            else if (gameObject == rightTeleporter)
            {
                other.transform.position = new Vector3((leftTeleporter.transform.position.x + 1), leftTeleporter.transform.position.y,
                    leftTeleporter.transform.position.z);
            }
        }
        //When a Ghost enters a teleporter
        else if (other.gameObject.CompareTag("Ghost"))
        {
            NavMeshAgent agent = other.gameObject.GetComponent<NavMeshAgent>();
            if(agent != null)
            {
                //If collided with left teleporter move to right
                if (gameObject == leftTeleporter)
                {
                    agent.Warp(new Vector3((rightTeleporter.transform.position.x - 1), rightTeleporter.transform.position.y,
                        rightTeleporter.transform.position.z));
                    
                }
                //If collided with right teleporter move to left
                else if (gameObject == rightTeleporter)
                {
                    agent.Warp(new Vector3((leftTeleporter.transform.position.x + 1), leftTeleporter.transform.position.y,
                        leftTeleporter.transform.position.z));
                }

            }
        }
    }
}
