/*
    Controls the character selection screen
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CharSelection : MonoBehaviour
{
    //Holds the name of the character that is currently selected
    public string charSelected;
    //Boolean to keep track if player 1 selected a character yet, if false then player 1 selected their character already
    public bool player1Select;
    //The button to lock in which character the player has chosen
    public Button select;
    //The Text Game Object that displays which character is currently selected
    public TextMeshProUGUI charLabel;
    //The Text Game Object that displays which player is selecting their character
    public TextMeshProUGUI selectLabel;
    //The Audio Source that has the announcer say the name of the character selected
    public AudioSource audioSource;
    //The Audio clip of the announcer saying "Mario"
    public AudioClip AnnouncerMario;

    /**/
    /*
    CharSelection::Start()

    NAME
        CharSelection::Start() - Start is called before the first frame update

    Synopsis
        void CharSelection::Start();

    Description
        Sets Player1Select to true since player 1 selects first

    RETURNS
        Returns nothing
    */
    /**/
    void Start()
    {
        player1Select = true;
    }

    /**/
    /*
    CharSelection::Select()

    NAME
        CharSelection::Select() - Submits the character the player has selected

    Synopsis
        void CharSelection::Select();

    Description
        This function is called when the Select button is clicked. If player 1 is selecting it locks in the character player 1 has currently selected. It makes 
        the select button uninteractable so player 2 must select a character before locking in. It changes the label to say player 2 is now picking. If it is Player 1 has already
        been selected then it locks in player 2's selected character and loads the stage selector scene.

    RETURNS
        Returns nothing
    */
    /**/
    public void Select()
    {
        if(player1Select)
        {
            //Sets player 1's character in PlayerPref so the battlescene knows who player 1 is using
            PlayerPrefs.SetString("Player1Char", charSelected);
            //sets the select button to false
            select.interactable = false;
            //clears the character selection label
            charLabel.text = "";
            //Changes the display for player 2 to select
            selectLabel.text = "Player 2 Select Your Character";
            //Sets player1select to false since player 1 is no longer selecting
            player1Select = false;
        }
        else
        {
            //Sets player 2's character in PlayerPref so the battlescene knows who player 2 is using
            PlayerPrefs.SetString("Player2Char", charSelected);
            //Loads the Stage selector scene
            SceneManager.LoadScene("StageSelector");
        }
    }

    /**/
    /*
    CharSelection::MarioButton()

    NAME
        CharSelection::MarioButton() - Hovers Mario as the currently selected character

    Synopsis
        void CharSelection::MarioButton();

    Description
        This function is called when the Mario button is clicked. It makes the Select button interactable and sets the character selected to Mario. Plays the
        announcer to call out "Mario".

    RETURNS
        Returns nothing
    */
    /**/
    public void MarioButton()
    {
        //Makes the select button interactable
        select.interactable = true;
        charLabel.text = "Mario";
        charSelected = "Mario";
        //Plays the sound clip of the announcer saying "Mario"
        PlaySound(AnnouncerMario);
    }

    /**/
    /*
    CharSelection::PlaySound()

    NAME
        CharSelection::PlaySound() - Plays the sound clip that is passed to it

    Synopsis
        void CharSelection::PlaySound(AudioClip clip);
            clip     --> The audio clip to be played

    Description
        Plays the sound clip that is passed to it

    RETURNS
        Returns nothing
    */
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
}
