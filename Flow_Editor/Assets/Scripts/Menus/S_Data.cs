using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Data : MonoBehaviour
{
    // General Variables
    public static S_Data Instance;

    // Note: All modifiers should act like a multiplication factor (For In-App Store)



    void Awake()
    {
        // Priorities
        Initialize_Singleton();
    }



    void Start()
    {
        // Maybe can have an Initialize_Load function here. Maybe not now
    }



    // ------------------------------------------------------------ //



    void Initialize_Singleton()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }



    // ------------------------------------------------------------ //



    // Used in [S_Level]
    public void SetScore(string SceneName, int i_Score)
    {
        PlayerPrefs.SetInt(SceneName, i_Score);
        // Lastly, Save the PlayerPrefs
        PlayerPrefs.Save();
    }



    // Used in [S_Menu]
    public int[] GetScore(string SceneName)
    {
        // Local Variables
        string Category = "";
        int i_Chapter = 0;
        int[] i_Scores = new int[5];
        // Set the category and chapter index based on the active scene
        switch (SceneName)
        {
            case "MENU_LEVELS_STORIES_CH1":
                Category = "STORIES";
                i_Chapter = 1;
                break;
        }
        // Load the level's scores based on the category and chapter index
        for (int i = 1; i <= 5; i++)
        {
            string key = $"{Category}_CH{i_Chapter}_L{i}";
            i_Scores[i - 1] = PlayerPrefs.GetInt(key, 0);
        }
        return i_Scores;
    }



}
