/*
   Keeps track of the percent damage the characters have
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Percent : MonoBehaviour
{
    //The text that shows what percent the character is at
    public Text percentText;
    //The actual number of percent the character is at
    private float percent = 0;

    /**/
    /*
    Percent::Start()

    NAME
        Percent::Start() - Start is called before the first frame update

    Synopsis
        void Percent::Start();

    Description
        Calls UpdateHealth(). Sets the characters percent to 0.

    RETURNS
        Returns nothing
    */
    /**/
    void Start()
    {
        UpdateHealth();
        percent = 0;
    }

    /**/
    /*
    Percent::UpdateHealth()

    NAME
        Percent::UpdateHealth() - updates the characters health value

    Synopsis
        void Percent::UpdateHealth();

    Description
        Sets the text object to the current percent of the character

    RETURNS
        Returns nothing
    */
    /**/

    public void UpdateHealth()
    {
        percentText.text = percent.ToString() + '%';
    }

    /**/
    /*
    Percent::TakeDamage()

    NAME
        Percent::TakeDamage() - updates the characters health value

    Synopsis
        void Percent::TakeDamage(float damage);
            damage     --> the amount of damage to be added to the characters percent

    Description
        Adds the damage to the players percent without letting it exceed 999. Calls UpdateHealth();

    RETURNS
        Returns nothing
    */
    /**/
    private void TakeDamage(float damage)
    {
        percent += damage;
        //Stops percent from going higher than 999
        if(percent > 999)
        {
            percent = 999;
        }

        UpdateHealth();
    }

    /**/
    /*
    Percent::GetPercent()

    NAME
        Percent::GetPercent() - updates the characters health value

    Synopsis
        void Percent::GetPercent();

    Description
        Returns the percent the character is at

    RETURNS
        Returns percent the character is at
    */
    /**/
    public float GetPercent()
    {
        return percent;
    }

    /**/
    /*
    Percent::ResetPercent()

    NAME
        Percent::ResetPercent() - Resets characters percent

    Synopsis
        void Percent::ResetPercent();

    Description
        Resets the characters percent to 0 when they are KO'ed. Gets rid of the number on the UI while they are gone.

    RETURNS
        Returns nothing
    */
    /**/
    public void ResetPercent()
    {
        percent = 0;
        percentText.text = "";
    }
}
