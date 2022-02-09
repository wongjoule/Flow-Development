using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Booster : MonoBehaviour
{
    // General Variables
    [HideInInspector] public BoosterType Type;
    [HideInInspector] public int i_Duration = 0; // Default Value

    // Note: All modifiers should act like a multiplication factor
    // Modifiers - Offensive Boosters
    public static float mod_MoveSpeed = 1f;
    public static float mod_SplashArea = 1f;

    // Modifiers - Defensive Boosters
    public static float mod_InkSize = 1f;
    public static float mod_InkSpeed = 1f;

    // Modifiers - General Boosters
    public static float mod_PlayerEye = 1f;
    public static bool mod_Invincible = false;




    void Start()
    {
        Initialize();
    }



    // ------------------------------------------------------------ //



    void Initialize()
    {
        // 1st, Generate a random number ranged from 1 to 3
        // Note: Range.Max is exclusive, so add 1
        int i_RandomType = Random.Range(1, 4);
        // 2nd, Define this booster as this type
        switch (i_RandomType)
        {
            case 1:
                Type = BoosterType.Offensive;
                break;

            case 2:
                Type = BoosterType.Defensive;
                break;

            case 3:
                Type = BoosterType.Others;
                break;
        }

    }



    // ------------------------------------------------------------ //



    public static void ResetModifiers()
    {
        // Offensive Boosters
        mod_MoveSpeed = 1f;
        mod_SplashArea = 1f;
        // Defensive Boosters
        mod_InkSize = 1f;
        mod_InkSpeed = 1f;
        // General Boosters
        mod_PlayerEye = 1f;
        mod_Invincible = false;
    }



    public void Activate()
    {
        // Disable its sprite renderer component

        // Start coroutine based on the type
        // Note: SubType '0' means it will generate a random subtype by itself
        switch (Type)
        {
            case BoosterType.Offensive:
                StartCoroutine(Activate_Offensive(0));
                break;

            case BoosterType.Defensive:
                StartCoroutine(Activate_Defensive(0));
                break;

            case BoosterType.Others:
                StartCoroutine(Activate_Others(0));
                break;
        }
    }



    // ------------------------------------------------------------ //



    IEnumerator Activate_Offensive(int i_SubType)
    {
        // Local Variables
        float f_Time = 0f;
        // Check if SubType is predefined or to be randomly generated
        if (i_SubType == 0)
        {
            // Note: Range.Max is exclusive, so add 1
            i_SubType = Random.Range(1, 3 + 1);
        }
        // Next, Apply the boost based on the subtype index
        // Note: This will be a temporary loop with a variable timer
        switch (i_SubType)
        {
            // 1. Speed Booster
            case 1:
                i_Duration = 10;
                S_DebugLog.Log("Offensive Booster Activated - ", "Speed Booster");
                while (f_Time <= i_Duration)
                {
                    // Functions
                    S_Booster.mod_MoveSpeed = 2f;
                    // Timers
                    f_Time += .2f;
                    yield return new WaitForSeconds(.2f);
                }
                // --- REMEMBER TO RESET TO ITS DEFAULT VALUES ---
                S_Booster.mod_MoveSpeed = 1f;
                break;

            // 2. Splash Booster
            case 2:
                i_Duration = 10;
                S_DebugLog.Log("Offensive Booster Activated - ", "Splash Booster");
                while (f_Time <= i_Duration)
                {
                    // Functions
                    S_Booster.mod_SplashArea = 3f;
                    // Timers
                    f_Time += .2f;
                    yield return new WaitForSeconds(.2f);
                }
                // --- REMEMBER TO RESET TO ITS DEFAULT VALUES ---
                S_Booster.mod_SplashArea = 1f;
                break;

            // 3. Powerup Booster
            case 3:
                S_DebugLog.Log("Offensive Booster Activated - ", "Duration Booster");
                // One-time booster doesn't really need a loop (value += 10)
                break;
        }
        // Lastly, Destroy this object
        Destroy(gameObject);
    }



    IEnumerator Activate_Defensive(int i_SubType)
    {
        // Local Variables
        float f_Time = 0f;
        // Check if SubType is predefined or to be randomly generated
        if (i_SubType == 0)
        {
            // Note: Range.Max is exclusive, so add 1
            i_SubType = Random.Range(1, 3 + 1);
        }
        // Next, Apply the boost based on the subtype index
        // Note: This will be a temporary loop with a variable timer
        switch (i_SubType)
        {
            // 1. SmallInk Booster
            case 1:
                i_Duration = 30;
                S_DebugLog.Log("Defensive Booster Activated - ", "SmallInk Booster");
                while (f_Time <= i_Duration)
                {
                    // Functions
                    S_Booster.mod_InkSize = .4f;
                    // Timers
                    f_Time += .2f;
                    yield return new WaitForSeconds(.2f);
                }
                // --- REMEMBER TO RESET TO ITS DEFAULT VALUES ---
                S_Booster.mod_InkSize = 1f;
                break;

            // 2. SlowInk Booster
            case 2:
                i_Duration = 30;
                S_DebugLog.Log("Defensive Booster Activated - ", "SlowInk Booster");
                while (f_Time <= i_Duration)
                {
                    // Functions
                    S_Booster.mod_InkSpeed = .4f;
                    // Timers
                    f_Time += .2f;
                    yield return new WaitForSeconds(.2f);
                }
                // --- REMEMBER TO RESET TO ITS DEFAULT VALUES ---
                S_Booster.mod_InkSpeed = 1f;
                break;

            // 3. Tornado Booster
            case 3:
                i_Duration = 30;
                S_DebugLog.Log("Defensive Booster Activated - ", "Tornado Booster");
                while (f_Time <= i_Duration)
                {
                    // Functions

                    // Timers
                    f_Time += .2f;
                    yield return new WaitForSeconds(.2f);
                }
                // --- REMEMBER TO RESET TO ITS DEFAULT VALUES ---

                break;
        }
        // Lastly, Destroy this object
        Destroy(gameObject);
    }



    IEnumerator Activate_Others(int i_SubType)
    {
        // Local Variables
        float f_Time = 0f;
        // Check if SubType is predefined or to be randomly generated
        if (i_SubType == 0)
        {
            // Note: Range.Max is exclusive, so add 1
            i_SubType = Random.Range(1, 5 + 1);
        }
        // Next, Apply the boost based on the subtype index
        // Note: This will be a temporary loop with a variable timer
        switch (i_SubType)
        {
            // 1. Torchlight Booster
            case 1:
                i_Duration = 30;
                float f_WhiteValue = 1f;
                S_Level s_Level = GameObject.Find("LEVELSYSTEM").GetComponent<S_Level>();
                SpriteRenderer c_BorderSprite = GameObject.Find("Background_Border").GetComponent<SpriteRenderer>();
                S_DebugLog.Log("General Booster Activated - ", "Torchlight Booster");
                while (f_Time <= i_Duration)
                {
                    // Functions
                    if (s_Level.b_IsFullView)
                    {
                        f_WhiteValue += .01f;
                        f_WhiteValue = Mathf.Clamp(f_WhiteValue, 0.9f, 1f);
                        c_BorderSprite.color = new Color(f_WhiteValue, f_WhiteValue, f_WhiteValue, 1);
                    }
                    else
                    {
                        f_WhiteValue -= .01f;
                        f_WhiteValue = Mathf.Clamp(f_WhiteValue, 0.9f, 1f);
                        c_BorderSprite.color = new Color(f_WhiteValue, f_WhiteValue, f_WhiteValue, 1);
                    }
                    // Timers
                    f_Time += .2f;
                    yield return new WaitForSeconds(.2f);
                }
                // --- REMEMBER TO RESET TO ITS DEFAULT VALUES ---
                c_BorderSprite.color = new Color(1, 1, 1, 1);
                break;

            // 2. Invincible Booster
            case 2:
                i_Duration = 30;
                S_DebugLog.Log("General Booster Activated - ", "Invincible Booster");
                while (f_Time <= i_Duration)
                {
                    // Functions
                    S_Booster.mod_Invincible = true;
                    // Timers
                    f_Time += .2f;
                    yield return new WaitForSeconds(.2f);
                }
                // --- REMEMBER TO RESET TO ITS DEFAULT VALUES ---
                S_Booster.mod_Invincible = false;
                break;

            // 3. Invisible Booster
            case 3:
                i_Duration = 30;
                S_DebugLog.Log("General Booster Activated - ", "Invisible Booster");
                while (f_Time <= i_Duration)
                {
                    // Functions

                    // Timers
                    f_Time += .2f;
                    yield return new WaitForSeconds(.2f);
                }
                // --- REMEMBER TO RESET TO ITS DEFAULT VALUES ---

                break;

            // 4. EnhancedView Booster
            case 4:
                i_Duration = 10;
                S_DebugLog.Log("General Booster Activated - ", "EnhancedView Booster");
                while (f_Time <= i_Duration)
                {
                    // Functions
                    S_Booster.mod_PlayerEye = 1.33f;
                    // Timers
                    f_Time += .2f;
                    yield return new WaitForSeconds(.2f);
                }
                // --- REMEMBER TO RESET TO ITS DEFAULT VALUES ---
                S_Booster.mod_PlayerEye = 1f;
                break;

            // 5. TimeFreeze Booster
            case 5:
                S_DebugLog.Log("General Booster Activated - ", "TimeFreeze Booster");


                break;
        }
        // Lastly, Destroy this object
        Destroy(gameObject);
    }
}
