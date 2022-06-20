/*
    Controls the background music that plays in the main menu, character select, and stage select screens
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    //The audio source that contains the music
    private AudioSource menuMusic;

    /**/
    /*
    MenuMusic::Awake()

    NAME
        MenuMusic::Awake() - Called when the GameObject the script is attached to is created

    Synopsis
        void MenuMusic::Awake();

    Description
        Attaches menuMusic to the actual AudioSource in the scene. Also makes the audiosource not get destroyed when going to a different scene.

    RETURNS
        Returns nothing
    */
    /**/
    private void Awake()
    {
        //Makes the audiosource remain after going to a differnt scene
        DontDestroyOnLoad(transform.gameObject);
        //Attaches menuMusic to the actual AudioSource in the scene
        menuMusic = GetComponent<AudioSource>();
    }

    /**/
    /*
    MenuMusic::PlayMusic()

    NAME
        MenuMusic::PlayMusic() - Starts playing the music

    Synopsis
        void MenuMusic::PlayMusic();

    Description
        Starts playing the music unless it is already playing.

    RETURNS
        Returns nothing
    */
    /**/
    public void PlayMusic()
    {
        if (!menuMusic.isPlaying)
        {
            menuMusic.Play();
        }
    }

    /**/
    /*
    MenuMusic::StopMusic()

    NAME
        MenuMusic::StopMusic() - Stops the music from playing

    Synopsis
        void MenuMusic::StopMusic();

    Description
        Stops the music from playing

    RETURNS
        Returns nothing
    */
    /**/
    public void StopMusic()
    {
        menuMusic.Stop();
    }
}
