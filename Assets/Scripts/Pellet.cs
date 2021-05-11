using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pellet : MonoBehaviour
{
    //Disables pellet when triggered
    private void OnTriggerEnter(Collider other)
    {
        gameObject.SetActive(false);
    }
}
