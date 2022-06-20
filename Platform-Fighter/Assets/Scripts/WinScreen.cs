/*
   Displays which player won and what character they were using on the victory screen
   Leads the players back to the main menu with a button
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    //The text at the top of the screen saying who won
    public TextMeshProUGUI winnerText;
    //The Image of Mario to be displayed when someone playing Mario wins
    public Image marioImage;

    /**/
    /*
    WinScreen::Start()

    NAME
        WinScreen::Start() - Start is called before the first frame update

    Synopsis
        void WinScreen::Start();

    Description
        This function is called as soon as the victory screen scene is opened. It disables all the character images so none can be seen
        It checks which player lost and displays the player number that won up top. It then enables the image of the character that the winning 
        player is using. Then deletes all the PlayerPrefs data to reset the game.

    RETURNS
        Returns nothing
    */
    /**/
    void Start()
    {
        //Hides all character images
        marioImage.enabled = false;

        //Changes the text for player 1 winning and shows player 1's character
        if (PlayerPrefs.GetString("losingPlayer") == "player2")
        {
            winnerText.text = "Player 1 is the Winner!";
            if(PlayerPrefs.GetString("Player1Char") == "Mario")
            {
                marioImage.enabled = true;
            }
        }
        //Changes the text for player 2 winning and shows player 2's character
        else if (PlayerPrefs.GetString("losingPlayer") == "player1")
        {
            winnerText.text = "Player 2 is the Winner!";
            if (PlayerPrefs.GetString("Player2Char") == "Mario")
            {
                marioImage.enabled = true;
            }
        }
        //Deletes all the PlayerPrefs data so it's a full reset when sent to the main menu
        PlayerPrefs.DeleteAll();
    }

    /**/
    /*
    WinScreen::ToMenu

    NAME
        WinScreen::ToMenu - Sends the user back to the Main Menu

    Synopsis
        void WinScreen::ToMenu();

    Description
        This function is called when the user presses the "Main Menu" button. It sends them back to the main menu.

    RETURNS
        Returns nothing
    */
    /**/
    public void ToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
