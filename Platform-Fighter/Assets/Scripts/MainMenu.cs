/*
    The script controlling the main menu
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    /**/
    /*
    MainMenu::Start()

    NAME
        MainMenu::Start() - Start is called before the first frame update

    Synopsis
        void MainMenu::Start();

    Description
        Deletes PlayerPref data and plays the menu music

    RETURNS
        Returns nothing
    */
    /**/
    void Start()
    {
        PlayerPrefs.DeleteAll();
        //Finds the game object with the tag "MenuMusic" and plays the PlayMusic function
        GameObject.FindGameObjectWithTag("MenuMusic").GetComponent<MenuMusic>().PlayMusic();
    }

    /**/
    /*
    MainMenu::StartGame()

    NAME
        MainMenu::StartGame() - Loads the character selection scene

    Synopsis
        void MainMenu::StartGame();

    Description
        This function is called when the start button is pressed. It loads the character selection scene.

    RETURNS
        Returns nothing
    */
    /**/
    public void StartGame()
    {
        SceneManager.LoadScene("CharSelection");
    }

    /**/
    /*
    MainMenu::EndGame()

    NAME
        MainMenu::EndGame() - Quits out of the game

    Synopsis
        void MainMenu::EndGame();

    Description
        This function is called when the quit button is pressed. It closes out of the program.

    RETURNS
        Returns nothing
    */
    /**/
    public void EndGame()
    {
        Application.Quit();
    }
}
