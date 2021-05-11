using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;

public class YellowFellowGame : MonoBehaviour
{
    //Public attributes of the game
    [SerializeField]
    GameObject highScoreUI;

    [SerializeField]
    GameObject mainMenuUI;

    [SerializeField]
    GameObject gameUI;

    [SerializeField]
    GameObject winUI;

    [SerializeField]
    Fellow playerObject;

    [SerializeField]
    GameObject pointsGameObject;

    [SerializeField]
    GameObject livesGameObject;

    [SerializeField]
    GameObject levelsGameObject;

    [SerializeField]
    GameObject highscoreGameObject;

    [SerializeField]
    GameObject lamps;

    [SerializeField]
    GameObject bigLight;

    [SerializeField]
    GameObject soundManagerObject;

    SoundManager soundManager;
   
    //Private arrays of various gameObjects
    GameObject[] pellets;
    GameObject[] powerups;
    GameObject[] ghosts;
    GameObject[] speedPowerups;

    //Private attributes of game
    bool dead = false;
    bool levelCompleted = false;
    int levelsBeat = 0;

    


    enum GameMode
    {
        InGame,
        MainMenu,
        HighScores,
        Win
    }

    GameMode gameMode = GameMode.MainMenu;

    // Start is called before the first frame update
    void Start()
    {
        //Starts Main Menu and stores different gameObjects in there corresponding arrays
        StartMainMenu();
        pellets = GameObject.FindGameObjectsWithTag("Pellet");
        powerups = GameObject.FindGameObjectsWithTag("Powerup");
        ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        speedPowerups = GameObject.FindGameObjectsWithTag("Speed Powerup");
        
        soundManager = soundManagerObject.GetComponent<SoundManager>();
        playerObject.enabled = false;
        //Activate the ghosts
        foreach(GameObject i in ghosts)
        {
            i.gameObject.SetActive(false);
        }

        
        
    }

    // Update is called once per frame
    void Update()
    {
        //Updates UI
        pointsGameObject.GetComponent<Text>().text = playerObject.GetScore().ToString();
        levelsGameObject.GetComponent<Text>().text = levelsBeat.ToString();

        switch(gameMode)
        {
            case GameMode.MainMenu:     UpdateMainMenu(); break;
            case GameMode.HighScores:   UpdateHighScores(); break;
            case GameMode.InGame:       UpdateMainGame(); break;
            case GameMode.Win:          UpdateWin(); break;
        }
        //Checks i the player has completed the level
        if(playerObject.PelletsEaten() == pellets.Length && levelCompleted == false)
        {
            levelCompleted = true;
            playerObject.ResetFellowForNextLevel();
            playerObject.enabled = false;
            levelsBeat++;
            //Starts the Win UI
            StartWin();
        }
        //Checks if player has lost a life
        if (int.Parse(livesGameObject.GetComponent<Text>().text) > playerObject.GetLives())
        {
            foreach (GameObject i in ghosts)
            {
                i.gameObject.SetActive(true);
                Ghost ghost = i.GetComponent<Ghost>();
                ghost.GoToStartPosition();
            }
        }
        //Updates Life UI element
        livesGameObject.GetComponent<Text>().text = playerObject.GetLives().ToString();

    }

    void UpdateMainMenu()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            soundManager.PlayMenuSound();
            StartGame();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            soundManager.PlayMenuSound();
            StartHighScores();
        }
    }

    void UpdateHighScores()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            soundManager.PlayMenuSound();
            StartMainMenu();
        }
    }

    void UpdateMainGame()
    {
        
        dead = playerObject.GetStatus();
        if (dead)
        {
            soundManager.PlayDeathSound();
            AddPlayerScore();
            StartHighScores();
            playerObject.RestoreFellowsLife();
            playerObject.enabled = false;
            
        }
    }

    void UpdateWin()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            soundManager.PlayMenuSound();
            StartGame();
        }
    }

    void StartMainMenu()
    {
        gameMode                        = GameMode.MainMenu;
        mainMenuUI.gameObject.SetActive(true);
        highScoreUI.gameObject.SetActive(false);
        gameUI.gameObject.SetActive(false);
        winUI.gameObject.SetActive(false);
    }


    void StartHighScores()
    {
        gameMode                = GameMode.HighScores;
        mainMenuUI.gameObject.SetActive(false);
        highScoreUI.gameObject.SetActive(true);
        gameUI.gameObject.SetActive(false);
        winUI.gameObject.SetActive(false);
        //Restores Fellow for future games and disables ghosts
        playerObject.RestoreFellowsLife();
        foreach (GameObject i in ghosts)
        {
            i.gameObject.SetActive(false);
        }

    }

    void StartWin()
    {
        gameMode                = GameMode.Win;
        winUI.gameObject.SetActive(true);
        mainMenuUI.gameObject.SetActive(false);
        highScoreUI.gameObject.SetActive(false);
        gameUI.gameObject.SetActive(false);
        soundManager.PlayCompleteSound();

        //Disables Ghosts
        foreach (GameObject i in ghosts)
        {
            i.gameObject.SetActive(false);
            i.GetComponent<Ghost>().SpeedUp();
        }

        //Activates Speed Powerups
        if (levelsBeat > 0)
        {
            foreach (GameObject i in speedPowerups)
            {
                i.gameObject.SetActive(true);
            }
        }

        //Switches lights
        if (bigLight.gameObject.activeSelf)
        {
            NightLights();
        }
        else
        {
            DayLights();
        }

    }

    void StartGame()
    {
        gameMode                = GameMode.InGame;
        mainMenuUI.gameObject.SetActive(false);
        highScoreUI.gameObject.SetActive(false);
        winUI.gameObject.SetActive(false);
        gameUI.gameObject.SetActive(true);
        //Renables player and resets them
        playerObject.enabled = true;
        playerObject.ResetPelletsEaten();
        playerObject.PowerupReset();
        levelCompleted = false;

        //Restore the pellets 
        foreach (GameObject i in pellets)
        {
            i.SetActive(true);
        }

        //Restore Powerups
        foreach (GameObject i in powerups)
        {
            i.SetActive(true);
        }

        //Restore Ghosts
        foreach (GameObject i in ghosts)
        {
            i.gameObject.SetActive(true);
            Ghost ghost = i.GetComponent<Ghost>();
            ghost.RestoreGhostForNextLevel();
        }

        //Updates highest score UI Element
        int highestScore = highScoreUI.GetComponent<HighScoreTable>().GetHighestScore();
        highscoreGameObject.GetComponent<Text>().text = highestScore.ToString();


    }

    //Adds Player score to leader board
    public void AddPlayerScore()
    {
        StreamWriter streamWriter = new StreamWriter("scores.txt", true);
        streamWriter.WriteLine(System.Environment.UserName
        +" " + playerObject.GetScore());
        streamWriter.Close();

    }

    //Turns on day lighting
    public void DayLights()
    {
        bigLight.gameObject.SetActive(true);
        lamps.gameObject.SetActive(false);
    }

    //Turns on night lighting
    public void NightLights()
    {
        bigLight.gameObject.SetActive(false);
        lamps.gameObject.SetActive(true);
    }
}
