using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Booster : MonoBehaviour
{
    // General Variables
    [HideInInspector] public BoosterType Type;
    [HideInInspector] public int i_Duration = 0; // Default Value
    SpriteRenderer c_Sprite;
    S_Level s_Level;

    // Variables - Menu Bars
    int i_BoosterIconIndex;
    GameObject o_BoosterIcon;

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



    void Awake()
    {
        c_Sprite = GetComponent<SpriteRenderer>();
        s_Level = GameObject.Find("SYSTEMS").GetComponent<S_Level>();
    }



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
                c_Sprite.sprite = Resources.Load<Sprite>("Sprites/IMG_Booster_Offensives");
                break;

            case 2:
                Type = BoosterType.Defensive;
                c_Sprite.sprite = Resources.Load<Sprite>("Sprites/IMG_Booster_Defensives");
                break;

            case 3:
                Type = BoosterType.Others;
                c_Sprite.sprite = Resources.Load<Sprite>("Sprites/IMG_Booster_Others");
                break;
        }

    }



    // ------------------------------------------------------------ //



    // Used in [S_Level]
    public static void ResetToDefault()
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



    // Used in [S_Player]
    public void Activate()
    {
        // The player absorb the booster
        StartCoroutine(Coroutine_Absorb());
        // Show the icon on the menu bar, and get the returned index
        i_BoosterIconIndex = s_Level.BoosterIcon_Start(Type);
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
                yield return new WaitForSeconds(10);
                break;
        }
        // Lastly, Destroy this object and its icon indicator on the menu bar
        s_Level.BoosterIcon_End(i_BoosterIconIndex);
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
                yield return new WaitForSeconds(10);
                // --- REMEMBER TO RESET TO ITS DEFAULT VALUES ---

                break;
        }
        // Lastly, Destroy this object and its icon indicator on the menu bar
        s_Level.BoosterIcon_End(i_BoosterIconIndex);
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
                S_Level s_Level = GameObject.Find("SYSTEMS").GetComponent<S_Level>();
                SpriteRenderer c_BorderSprite = GameObject.Find("Background_Border").GetComponent<SpriteRenderer>();
                S_DebugLog.Log("General Booster Activated - ", "Torchlight Booster");
                while (f_Time <= i_Duration)
                {
                    // Functions
                    if (s_Level.b_FullView)
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
                // Temp
                GameObject o_Border = GameObject.Find("Border_Collider_Interior");
                i_Duration = 30;
                S_DebugLog.Log("General Booster Activated - ", "Invisible Booster");
                while (f_Time <= i_Duration)
                {
                    // Functions
                    o_Border.SetActive(false);
                    // Timers
                    f_Time += .2f;
                    yield return new WaitForSeconds(.2f);
                }
                // --- REMEMBER TO RESET TO ITS DEFAULT VALUES ---
                o_Border.SetActive(true);
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
                yield return new WaitForSeconds(10);
                break;
        }
        // Lastly, Destroy this object and its icon indicator on the menu bar
        s_Level.BoosterIcon_End(i_BoosterIconIndex);
        Destroy(gameObject);
    }



    IEnumerator Coroutine_Absorb()
    {
        // Local Variables
        float f_Time = 0f;
        Transform t_Player = GameObject.FindWithTag("Player").transform;
        Vector3 v_Velocity = Vector3.zero;
        // A quick absorption animation after interacting with the booster
        // Note: This will be a 2 seconds loop and also it acts like Update()
        while (f_Time < 2)
        {
            // Move this booster's position
            transform.position = Vector3.SmoothDamp(transform.position, t_Player.position, ref v_Velocity, .1f);
            // Scale this booster's local scale
            float f_Scale = Mathf.SmoothStep(transform.localScale.x, 0f, .1f);
            transform.localScale = new Vector3(f_Scale, f_Scale, f_Scale);
            // Wait for the next frame before looping over
            f_Time += Time.deltaTime;
            yield return null;
        }
        // Lastly, Disable its sprite renderer component
        c_Sprite.enabled = false;
    }

}
