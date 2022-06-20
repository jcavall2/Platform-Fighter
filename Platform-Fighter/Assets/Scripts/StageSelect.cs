/*
   Has the user select which stage they would like to fight on
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class StageSelect : MonoBehaviour
{
    //Holds the name of the stage currently selected
    public string stageSelected;
    //The text object that says which stage is currently selected
    public TextMeshProUGUI stageLabel;
    //The button used to submit the stage selection
    public Button select;

    /**/
    /*
    StageSelect::FDSelect()

    NAME
        StageSelect::FDSelect()

    Synopsis
        void StageSelect::FDSelect();

    Description
        This function is called when the user clicks the "Final Destination" button. It changes the text to show that Final Destination has been selected but not yet submit.
        It makes the select button interactable otherwise the user would be able to submit without anything selected.

    RETURNS
        Returns nothing
    */
    /**/
    public void FDSelect()
    {
        stageLabel.text = "Final Destination";
        //Makes it so the select button can be pressed
        select.interactable = true;
        stageSelected = "Final Destination";
    }

    /**/
    /*
    StageSelect::Select()

    NAME
        StageSelect::Select()

    Synopsis
        void StageSelect::Select();

    Description
        This function is called when the user clicks the select button. It sets the "stageSelected" in PlayerPrefs to whichever stage is selected. It sends the user to the Battle scene.

    RETURNS
        Returns nothing
    */
    /**/
    public void Select()
    {
        //Adds the stage selected to PlayerPrefs so the battle scene knows which stage to use
        PlayerPrefs.SetString("stageSelected", stageSelected);
        //Loads the battle scene
        SceneManager.LoadScene("BattleScene");
    }
}
