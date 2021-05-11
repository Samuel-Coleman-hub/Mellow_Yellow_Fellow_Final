using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostHouse : MonoBehaviour
{
    //Method triggered when ghost enters Ghost House
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ghost"))
        {
            Ghost ghost = other.gameObject.GetComponent<Ghost>();
            StartCoroutine(InGhostHouse(ghost));
        }
    }

    //Coroutine making the ghost wait 5 seconds before returning from Ghoust House
    private IEnumerator InGhostHouse(Ghost ghost)
    {
        yield return new WaitForSeconds(5);
        if (ghost != null)
        {
            ghost.ReturnFromEaten();
        }
    }
}
