using System.Collections;
using UnityEngine;

public class Fellow : MonoBehaviour
{
    //Public attributes of the Fellow
    [SerializeField] float speed = 1f;
    [SerializeField] int lives = 3;
    [SerializeField] int pointsPerPellet = 100;
    [SerializeField] float powerupDuration = 10.0f; //How  long  should  powerups  last?
    float powerupTime = 0.0f;   //How  long is left on the  current  powerup?
    bool dead = false;
    string directionGoing = "right";
    

    int score = 0;
    int pelletsEaten = 0;
    

    void Start()
    {
        //Activate the gameobject & set its position
        gameObject.SetActive(true);
        gameObject.transform.position = new Vector3((float)7.5, (float)0.5, 4);
    }

    private void OnTriggerEnter(Collider other)
    {
        //When Fellow eats pellet
        if (other.gameObject.CompareTag("Pellet"))
        {
            pelletsEaten++;
            score += pointsPerPellet;
        }

        //When Fellow eats powerup (start powerup)
        if (other.gameObject.CompareTag("Powerup"))
        {
            powerupTime = powerupDuration;
            other.gameObject.SetActive(false);
        }

        //When Fellow eats speed powerup
        if(other.gameObject.CompareTag("Speed Powerup"))
        {
            speed += 0.08f;
            other.gameObject.SetActive(false);
        }

        //When Fellow hits a ghost when not powered up
        if (other.gameObject.CompareTag("Ghost") && !PowerupActive())
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
            StartCoroutine(ResetFellow());
        }
        //When Fellow hits a ghost when powered up
        else if (other.gameObject.CompareTag("Ghost") && PowerupActive())
        {
            Ghost ghost = other.gameObject.GetComponent<Ghost>();
            if (ghost != null)
            {
                ghost.Eaten();
            }
        }
    }

    public bool PowerupActive()
    {
        return powerupTime > 0.0f;
    }

    public void PowerupReset()
    {
        powerupTime = 0.0f;
        powerupTime = 0.0f;
    }

    public int PelletsEaten()
    {
        return pelletsEaten;
    }

    public void ResetPelletsEaten()
    {
        pelletsEaten = 0;
    }

    //Update method for moving the Fellow
    private void FixedUpdate()
    {
        Rigidbody b = GetComponent<Rigidbody>();
        Vector3 velocity = b.velocity;

        if (Input.GetKey(KeyCode.A))
        {
            velocity.x -= speed;
            directionGoing = "left";
        }
        if (Input.GetKey(KeyCode.D))
        {
            velocity.x += speed;
            directionGoing = "right";
        }
        if (Input.GetKey(KeyCode.W))
        {
            velocity.z += speed;
        }
        if (Input.GetKey(KeyCode.S))
        {
            velocity.z -= speed;
        }
        b.velocity = velocity;
    }

    private void Update()
    {
        //Updates power-up time
        powerupTime = Mathf.Max(0.0f, powerupTime - Time.deltaTime);

        //Checks if the Fellow is dead
        if(lives < 0)
        {
            dead = true;
        }
    }

    //Resetting the Fellow when he loses a life
    private IEnumerator ResetFellow()
    {
        lives--;
        yield return new WaitForSeconds(1);
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        gameObject.transform.position = new Vector3((float)7.5, (float)0.5, 4);
    }

    public bool GetStatus()
    {
        return dead;
    }

    public int GetScore()
    {
        return score;
    }

    public int GetLives()
    {
        return lives;
    }

    public void ResetStatus()
    {
        dead = false;
    }

    public void ResetScore()
    {
        score = 0;
    }

    public void ResetLives()
    {
        lives = 3;
    }

    public string GetDirectionGoing()
    {
        return directionGoing;
    }

    public void RestoreFellowsLife()
    {
        speed = 1f;
        ResetLives();
        ResetScore();
        ResetStatus();
    }

    public void ResetFellowForNextLevel()
    {
        speed = 1f;
        gameObject.transform.position = new Vector3((float)7.5, (float)0.5, 4);
    }
}
