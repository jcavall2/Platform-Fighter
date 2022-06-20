/*
    Controls starting and ending the battle scene
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class BattleSettings : MonoBehaviour
{
    //The Mario GameObject for player 1
    public GameObject mario1;
    //The Mario GameObject for player 1
    public GameObject mario2;
    //The GameObject for the final destination stage
    public GameObject fdStage;
    //The Text at the top of the screen at the beginning and end of the fight
    public TextMeshProUGUI countdown;
    //The timer to countdown when the battle will start
    private float countdownTimer;
    //True if the battle has started false if it has not started yet
    private bool started = false;
    //The timer to delay the game ending and going to the victory screen
    private float endGameTimer;
    //Used to play the countdown and game end audio
    public AudioSource audioSource;
    //Clip of announcer saying 3
    public AudioClip countdown3;
    //Clip of announcer saying 2
    public AudioClip countdown2;
    //Clip of announcer saying 1
    public AudioClip countdown1;
    //Clip of announcer saying Go
    public AudioClip countdownGo;
    //Clip of announcer saying Game
    public AudioClip gameEnd;
    //True if the announcer already said 3 false if he has not
    private bool played3 = false;
    //True if the announcer already said 2 false if he has not
    private bool played2 = false;
    //True if the announcer already said 1 false if he has not
    private bool played1 = false;
    //True if the announcer already said Go false if he has not
    private bool playedGo = false;
    //True if the announcer already said Game false if he has not
    private bool playedGame = false;

    /**/
    /*
    BattleSettings::Start()

    NAME
        BattleSettings::Start() - Start is called before the first frame update

    Synopsis
        void BattleSettings::Start();

    Description
        Creates the stage depending on which one was selected during stage select. Starts the countdown timer and sets the end game timer. Destroys the menu music since the battle has it's own music.

    RETURNS
        Returns nothing
    */
    /**/
    void Start()
    {
        //Checks if Final Destination was the selected stage
        if(PlayerPrefs.GetString("stageSelected") == "Final Destination")
        {
            //Activates the Stage
            fdStage.SetActive(true);
        }
        countdownTimer = 5f;
        endGameTimer = 4f;
        //Destroys the menu music so there are not two when the user goes back to the main menu
        Destroy(GameObject.FindGameObjectWithTag("MenuMusic"));
    }

    /**/
    /*
    BattleSettings::Update()

    NAME
        BattleSettings::Update() - Update is called every frame

    Synopsis
        void BattleSettings::Update();

    Description
        If the fight has not started counts down to start the match. Once the countdown ends puts the fighters on the stage. After the match has started keeps checking to see if somebody lost. If so stops the fight
        and loads the victory screen

    RETURNS
        Returns nothing
    */
    /**/
    void Update()
    {
        //If the match has not started yet
        if(!started)
        {
            if(countdownTimer > 4)
            {
                countdown.text = "3";
                //Makes sure the announcer only says 3 once
                if(!played3)
                {
                    PlaySound(countdown3);
                    played3 = true;
                }
            }
            else if(countdownTimer > 3)
            {
                countdown.text = "2";
                //Makes sure the announcer only says 2 once
                if (!played2)
                {
                    PlaySound(countdown2);
                    played2 = true;
                }
            }
            else if(countdownTimer > 2)
            {
                countdown.text = "1";
                //Makes sure the announcer only says 1 once
                if (!played1)
                {
                    PlaySound(countdown1);
                    played1 = true;
                }
            }
            else if(countdownTimer > 1)
            {
                countdown.text = "GO";
                //Makes sure the announcer only says Go once
                if (!playedGo)
                {
                    PlaySound(countdownGo);
                    playedGo = true;
                }
            }
            //Once the timer has finished
            else
            {
                //Gets rid of the text so it's not in the way
                countdown.text = "";
                //Creates the fighters on the stage
                CreateFighters();
                return;
            }
            countdownTimer -= Time.deltaTime;
        }

        //If player 2 lost the battle
        if(PlayerPrefs.GetString("losingPlayer") == "player2")
        {
            //Disables player 1's script so they can no longer move
            mario1.GetComponent<MarioController>().enabled = false;
            //Disables player 2's script so they will not respawn
            mario2.GetComponent<MarioControllerP2>().enabled = false;
            countdown.text = "GAME";
            //Makes sure the announcer only says Game once
            if (!playedGame)
            {
                PlaySound(gameEnd);
                playedGame = true;
            }
            endGameTimer -= Time.deltaTime;
            //Once the timer ends load the victory screen
            if(endGameTimer < 0)
            {
                SceneManager.LoadScene("WinScreen");
            }
        }

        //If player 1 lost the battle
        else if (PlayerPrefs.GetString("losingPlayer") == "player1")
        {
            //Disables player 1's script so they will not respawn
            mario1.GetComponent<MarioController>().enabled = false;
            //Disables player 2's script sot hey can no longer move
            mario2.GetComponent<MarioControllerP2>().enabled = false;
            countdown.text = "GAME";
            //Makes sure the announcer only says Game once
            if (!playedGame)
            {
                PlaySound(gameEnd);
                playedGame = true;
            }
            endGameTimer -= Time.deltaTime;
            //Once the timer ends load the victory screen
            if (endGameTimer < 0)
            {
                SceneManager.LoadScene("WinScreen");
            }
        }
    }

    /**/
    /*
    BattleSettings::createFighters()

    NAME
        BattleSettings::createFighters() - Puts the fighters into the scene

    Synopsis
        void BattleSettings::createFighters();

    Description
        Creates each character depending which character was chosen by each player

    RETURNS
        Returns nothing
    */
    /**/
    public void CreateFighters()
    {
        //Sets Mario 1 to active if player 1 chose Mario
        if (PlayerPrefs.GetString("Player1Char") == "Mario")
        {
            mario1.SetActive(true);
        }

        //Sets Mario 2 to active if player 2 chose Mario
        if (PlayerPrefs.GetString("Player2Char") == "Mario")
        {
            mario2.SetActive(true);
        }
        started = true;
    }

    /**/
    /*
    BattleSettings::PlaySound()

    NAME
        BattleSettings::PlaySound() - Plays any sound clip passed to it

    Synopsis
        void BattleSettings::PlaySound(AudioClip clip);
            clip    --> The audio clip to be played

    Description
        Plays the audio clip that is passed to it

    RETURNS
        Returns nothing
    */
    /**/
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
