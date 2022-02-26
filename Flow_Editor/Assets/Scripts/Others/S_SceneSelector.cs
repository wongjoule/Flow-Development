using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Dedicated menu script to select scenes based on the snap index.
/// </summary>
public class S_SceneSelector : MonoBehaviour
{


    public static void Levels(int i_Horizontal, int i_Vertical_A, int i_Vertical_B, int i_Vertical_C)
    {
        if (i_Horizontal == 1)
        {
            switch (i_Vertical_A)
            {
                case 1:
                    SceneManager.LoadScene("MENU_LEVELS_STORIES_CH1");
                    break;

                case 2:

                    break;
            }
        }
        else if (i_Horizontal == 2)
        {
            switch (i_Vertical_B)
            {
                case 1:

                    break;

                case 2:

                    break;
            }
        }
        else if (i_Horizontal == 3)
        {
            switch (i_Vertical_C)
            {
                case 1:

                    break;

                case 2:

                    break;
            }
        }
    }



    public static void Levels_Stories(int i_Chapter, int i_Vertical)
    {
        if (i_Chapter == 1)
        {
            switch (i_Vertical)
            {
                case 1:
                    SceneManager.LoadScene("STORIES_CH1_L1");
                    break;
            }
        }
    }



    public static void Artists(int i_Vertical)
    {
        switch (i_Vertical)
        {
            case 1:
                SceneManager.LoadScene("MENU_ARTISTS_NAME1");
                break;
        }
    }


}
