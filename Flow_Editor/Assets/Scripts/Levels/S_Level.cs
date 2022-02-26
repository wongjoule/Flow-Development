using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public enum InkType { Idle, Spreading, Fading, Moving, Malicious }
public enum BoosterType { Offensive, Defensive, Others }

public class S_Level : MonoBehaviour
{
    [Header("Black Ink Pools")]
    [Range(0, 9)] public int i_IdleInkPool = 0;
    [Range(0, 9)] public int i_SpreadingInkPool = 0;
    [Range(0, 9)] public int i_FadingInkPool = 0;
    [Range(0, 9)] public int i_MovingInkPool = 0;
    [Range(0, 9)] public int i_MaliciousInkPool = 0;

    [Header("Properties - Spreading Ink")]
    public Vector2Int i_CloneCount = new Vector2Int(2, 3);

    [Header("Properties - Fading Ink")]
    public Vector2Int i_HoldTime = new Vector2Int(7, 15); // In seconds

    // Generic Variables
    Transform t_Player;
    Transform t_Camera;
    Camera c_Camera;
    S_Camera s_Camera;
    S_Controller s_Controller;
    S_SubMenu s_SubMenu;

    // Variables - View Modes
    [HideInInspector] public bool b_FullView = false; // Unified management of the control of the Full View
    [HideInInspector] public List<SpriteRenderer> list_ViewModeSprite = new List<SpriteRenderer>();
    float f_ViewModeAlpha = 1f;
    SpriteRenderer c_BackgroundPause, c_BackgroundShadow; // 'Pause and Observe' background sprite that fades in when zoomed out

    // Variables - Joysticks
    Transform t_JoystickParent;
    SpriteRenderer c_JoystickKnob, c_JoystickBase;

    // Variables - Splashes
    float f_SplashCooldown = 0f;

    // Variables - Black Inks
    Transform t_BlackInkParent;
    int i_TotalInkPool = 0;
    int i_TotalInkSlot = 0;
    List<int> list_InkOrder = new List<int>();
    [HideInInspector] public List<Vector3> list_InkTeleportSlot = new List<Vector3>();

    // Variables - Parts
    Transform t_PartsParent;

    // Variables - Revealers
    Transform t_RevealerParent;
    int i_CurrentRevealer = 0;

    // Variables - Respawns
    Transform t_RespawnParent;
    Transform t_PlayerParent;

    // Variables - Hints
    Transform t_HintParent;
    SpriteRenderer c_HintSprite;

    // Variables - Menu Bars (Indicators - Lives, Progress, Hint)
    Transform t_MenuBarParent, t_ProgressBar, t_HintButton;
    TextMeshPro tmp_Lives;
    bool[] b_BoosterIcons; // Default = All false
    SpriteRenderer[] c_BoosterIcons;
    int i_Lives = 3; // Note: Your third death = game over
    int i_Progress = 0;
    int i_CompleteProgress = 0;

    // Variables - Animations
    Transform t_AnimationParent;

    // Variables - Cards
    Transform t_CardParent;

    // Variables - Summaries
    Transform t_SummaryParent;

    // Variables - Resources Path
    [HideInInspector] public GameObject prefab_Player;
    [HideInInspector] public GameObject prefab_BlackInk;
    [HideInInspector] public GameObject prefab_Revealer;
    [HideInInspector] public GameObject prefab_FullPanel; // A full-screen panel made with sprite
    [HideInInspector] public GameObject prefab_Splash;



    void Awake()
    {
        // Priorities
        // if (!Application.isEditor)
        {
            s_SubMenu = GameObject.Find("SUBMENUS").GetComponent<S_SubMenu>();
            s_SubMenu.Initialize("Level");
        }
        S_Booster.ResetToDefault();
        // General Variables
        t_Player = GameObject.FindWithTag("Player").transform;
        t_Camera = GameObject.FindWithTag("MainCamera").transform;
        c_Camera = t_Camera.GetComponent<Camera>();
        s_Camera = t_Camera.GetComponent<S_Camera>();
        s_Controller = GameObject.Find("CONTROLLERS").GetComponent<S_Controller>();
        // Variables - View Modes
        c_BackgroundPause = GameObject.Find("Background_Pause").GetComponent<SpriteRenderer>();
        c_BackgroundShadow = GameObject.Find("Background_Shadow").GetComponent<SpriteRenderer>();
        // Variables - Joysticks
        t_JoystickParent = GameObject.Find("JOYSTICKS").transform;
        c_JoystickKnob = t_JoystickParent.Find("Joystick_Knob").GetComponent<SpriteRenderer>();
        c_JoystickBase = t_JoystickParent.Find("Joystick_Base").GetComponent<SpriteRenderer>();
        // Variables - Black Inks
        t_BlackInkParent = GameObject.Find("BLACKINKS").transform;
        // Variables - Parts
        t_PartsParent = GameObject.Find("PARTS").transform;
        // Variables - Revealers
        t_RevealerParent = GameObject.Find("REVEALERS").transform;
        // Variables - Respawns
        t_RespawnParent = GameObject.Find("RESPAWNS").transform;
        t_PlayerParent = GameObject.Find("PLAYERS").transform;
        // Variables - Hints
        t_HintParent = GameObject.Find("HINTS").transform;
        c_HintSprite = t_HintParent.GetComponentInChildren<SpriteRenderer>();
        // Variables - Menu Bars
        t_MenuBarParent = GameObject.Find("MENUBARS").transform;
        tmp_Lives = t_MenuBarParent.Find("LIVES/Text_Lives").GetComponent<TextMeshPro>();
        t_ProgressBar = t_MenuBarParent.Find("PROGRESSBARS/IMG_ProgressBar");
        t_HintButton = t_MenuBarParent.Find("Button_Hint");
        // Variables - Animations
        t_AnimationParent = GameObject.Find("ANIMATIONS").transform;
        // Variables - Cards
        t_CardParent = GameObject.Find("CARDS").transform;
        // Variables - Summaries
        t_SummaryParent = GameObject.Find("SUMMARIES").transform;
        // Variables - Resources Path
        prefab_Player = Resources.Load<GameObject>("Prefabs/Player");
        prefab_BlackInk = Resources.Load<GameObject>("Prefabs/BlackInk");
        prefab_Revealer = Resources.Load<GameObject>("Prefabs/Revealer");
        prefab_FullPanel = Resources.Load<GameObject>("Prefabs/FullPanel");
        prefab_Splash = Resources.Load<GameObject>("Prefabs/Splash");
    }



    // Level Initialization will require a loading scene
    void Start()
    {
        // If Prerequisite returns 'true', enter the next initialization phase
        // Else, Throw an error log and immediately stop the initialization process
        if (Initialize_Prerequisite())
        {
            Initialize_BlackInks();
            Initialize_Parts();
            Initialize_Revealers();
            Initialize_Respawns();
            Initialize_MenuBars();
            Initialize_Hints();
            Initialize_Animations();
            Initialize_Cards();
            Initialize_Summaries();
        }
        else
        {
            S_DebugLog.ErrorLog("The level initialization process has been terminated.");
        }
    }



    void Update()
    {
        Update_ViewModeSprites();
    }



    // ------------------------------------------------------------ //



    void Update_ViewModeSprites()
    {
        switch (b_FullView)
        {
            case true:
                // Note: Decrease the alpha value from '1' to '0'
                if (f_ViewModeAlpha > 0)
                {
                    // The fade-out duration is 1 sec
                    f_ViewModeAlpha -= Time.deltaTime;
                    f_ViewModeAlpha = Mathf.Clamp(f_ViewModeAlpha, 0f, 1f);
                    // Fade out everything in the list_SpriteRenderer
                    foreach (SpriteRenderer c_Sprite in list_ViewModeSprite)
                        c_Sprite.color = new Color(1, 1, 1, f_ViewModeAlpha);
                    // Fade in the silhouette hint image
                    // Note: Here f_HintAlpha is inverted, and its maximum value is '0.5'
                    float f_HintAlpha = (1f - f_ViewModeAlpha) - 0.5f; // Half second duration
                    c_HintSprite.color = new Color(1, 1, 1, f_HintAlpha);
                    // Fade in the background sprites when in Full View mode
                    c_BackgroundPause.color = new Color(0.95f, 0.95f, 0.95f, 1f - f_ViewModeAlpha);
                    c_BackgroundShadow.color = new Color(1, 1, 1, 1f - f_ViewModeAlpha);
                }
                break;
            case false:
                // Note: Increase the alpha value from '0' to '1'
                if (f_ViewModeAlpha < 1)
                {
                    // The fade-in duration is 1 sec
                    f_ViewModeAlpha += Time.deltaTime;
                    f_ViewModeAlpha = Mathf.Clamp(f_ViewModeAlpha, 0f, 1f);
                    // Fade in everything in the list_SpriteRenderer
                    foreach (SpriteRenderer c_Sprite in list_ViewModeSprite)
                        c_Sprite.color = new Color(1, 1, 1, f_ViewModeAlpha);
                    // Fade out the silhouette hint image
                    // Note: Here f_Alpha is inverted, and its minimum value is '0'
                    float f_HintAlpha = (1f - f_ViewModeAlpha) - 0.5f; // Half second duration
                    c_HintSprite.color = new Color(1, 1, 1, f_HintAlpha);
                    // Fade out the background sprites when in Default View mode
                    c_BackgroundPause.color = new Color(0.95f, 0.95f, 0.95f, 1f - f_ViewModeAlpha);
                    c_BackgroundShadow.color = new Color(1, 1, 1, 1f - f_ViewModeAlpha);
                }
                break;
        }
        // Joystick visuals
        float f_JoystickAlpha = (5.5f - c_Camera.orthographicSize) / 4f;
        c_JoystickKnob.color = new Color(1, 1, 1, f_JoystickAlpha);
        c_JoystickBase.color = new Color(1, 1, 1, f_JoystickAlpha);
    }



    bool Initialize_Prerequisite()
    {
        // Get the total amount of Black Inks at the current level pool
        i_TotalInkPool = i_IdleInkPool + i_SpreadingInkPool + i_FadingInkPool + i_MovingInkPool + i_MaliciousInkPool;
        S_DebugLog.LevelLog("Total amount of Black Ink Slots required (Pool + Teleport) = ", $"{i_TotalInkPool} + {i_FadingInkPool}");
        // Get the total amount of Black Inks on the current level map
        i_TotalInkSlot = t_BlackInkParent.childCount;
        S_DebugLog.LevelLog("Total amount of Black Ink Slots (Map) = ", i_TotalInkSlot);
        // Check if total Black Ink on map is sufficient for the pool and teleport slots
        if (i_TotalInkSlot >= (i_TotalInkPool + i_FadingInkPool))
        {
            S_DebugLog.SuccessLog("", "All prerequisites for level initialization have been met.");
            // Return 'true' and the level initialization process continues
            return true;
        }
        else
        {
            S_DebugLog.ErrorLog("Total amount of Black Ink Slots is insufficient. Please add at least " +
            ((i_TotalInkPool + i_FadingInkPool) - i_TotalInkSlot) + "more Black Ink Slots on the map.");
            // Return 'false' and stop the level initialization 
            return false;
        }
    }



    void Initialize_BlackInks()
    {
        Initialize_BlackInk_Phase_1_CreatingOrder();
        Initialize_BlackInk_Phase_2_AssigningType();
        Initialize_BlackInk_Phase_3_TeleportSlot();
        Initialize_BlackInk_Phase_4_CleaningUp();
        Initialize_BlackInk_Phase_5_StartCoroutine();
    }



    void Initialize_Parts()
    {
        // Iterate through all of parts and initialize their sprite renderer component
        for (int i = 0; i < t_PartsParent.childCount; i++)
        {
            // Get the current child's sprite renderer component
            SpriteRenderer c_Sprite = t_PartsParent.GetChild(i).GetComponent<SpriteRenderer>();
            // Initialize its sorting order according to the current loop index
            // Note: Make it starts from index 1 instead of zero
            c_Sprite.sortingOrder = i + 1;
            // Initialize its object's tag
            c_Sprite.tag = "Part";
            // Initialize its mask interaction behavior to be visible only inside the mask
            c_Sprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
    }



    void Initialize_Revealers()
    {
        // Find the revealers' parent and store it into a local variable
        Transform t_RevealerParent = GameObject.Find("REVEALERS").transform;
        // Check the current map's parts amount then instantiate itself
        int i_Count = 10;
        // Iterate through all of revealers and deactivate them
        for (int i = 1; i <= i_Count; i++)
        {
            // Get the current revealer's game object
            GameObject o_Revealer = Instantiate(prefab_Revealer, t_RevealerParent);
            o_Revealer.name = "Revealer_" + i;
            o_Revealer.SetActive(false);
        }
    }



    void Initialize_Respawns()
    {
        // Local Variables
        int i_TotalPoint = t_RespawnParent.childCount;
        // Print out the log
        if (i_TotalPoint > 0)
        {
            S_DebugLog.LevelLog("Total amount of Respawn Points = ", i_TotalPoint);
        }
        else
        {
            S_DebugLog.WarningLog("There are no respawn points available on the map.");
        }
    }



    void Initialize_MenuBars()
    {
        // Set its default values
        tmp_Lives.text = "3";
        t_ProgressBar.position = new Vector3(-3.9f, t_ProgressBar.position.y, 0);
        t_HintButton.gameObject.SetActive(false);
        b_BoosterIcons = new bool[3];
        c_BoosterIcons = new SpriteRenderer[b_BoosterIcons.Length];
        // Then, Locate all BoosterIcons' Sprite Renderers
        Transform t_BoosterIconParent = t_MenuBarParent.Find("BOOSTERICONS");
        for (int i = 0; i < b_BoosterIcons.Length; i++)
        {
            c_BoosterIcons[i] = t_BoosterIconParent.GetChild(i).GetComponent<SpriteRenderer>();
            c_BoosterIcons[i].gameObject.SetActive(false);
        }
        // Get the total amount of Parts in this level
        i_CompleteProgress = t_PartsParent.childCount;
        // Print out the log
        S_DebugLog.LevelLog("Total amount of Parts = ", i_CompleteProgress);
    }



    void Initialize_Hints()
    {
        // Default value
        c_HintSprite.enabled = false;
    }



    void Initialize_Animations()
    {
        // Deactivate the completion animation object at the beginning
        t_AnimationParent.gameObject.SetActive(false);
    }



    void Initialize_Cards()
    {
        // Then, Deactivate the parent object at the beginning
        t_CardParent.gameObject.SetActive(false);
    }



    void Initialize_Summaries()
    {
        t_SummaryParent.gameObject.SetActive(false);
    }



    // ------------------------------------------------------------ //
    // Phase functions used in the level initialization. It should not be used alone



    void Initialize_BlackInk_Phase_1_CreatingOrder()
    {
        // Create a placeholder to store a random number
        int i_RandomIndex;
        // Generate a list of unique random numbers
        for (int i = 0; i < i_TotalInkSlot; i++)
        {
            // 1st, Generate the first random number
            i_RandomIndex = Random.Range(0, i_TotalInkSlot);
            // 2nd, Check and repeat if the list already contains the current generated number
            while (list_InkOrder.Contains(i_RandomIndex))
                i_RandomIndex = Random.Range(0, i_TotalInkSlot);
            // 3rd, Add the generated unique number to the list
            list_InkOrder.Add(i_RandomIndex);
        }
        S_DebugLog.LevelLog("Black Ink's Order List = ", string.Join(", ", list_InkOrder.ConvertAll(i => i.ToString()).ToArray()));
    }



    void Initialize_BlackInk_Phase_2_AssigningType()
    {
        // Local Variables
        int i_CurrentIndex = 0; // Default is zero
        Vector3 v_CurrentPosition;
        GameObject o_BlackInk;
        S_BlackInk s_BlackInk;
        // 1st, Define as Idle Ink Type
        while (i_IdleInkPool > 0)
        {
            v_CurrentPosition = t_BlackInkParent.GetChild(list_InkOrder[i_CurrentIndex]).position;
            o_BlackInk = Instantiate(prefab_BlackInk, v_CurrentPosition, Quaternion.identity, t_BlackInkParent);
            s_BlackInk = o_BlackInk.GetComponent<S_BlackInk>();
            s_BlackInk.Type = InkType.Idle;
            o_BlackInk.name = "BlackInk_" + list_InkOrder[i_CurrentIndex] + "_" + s_BlackInk.Type;
            i_IdleInkPool -= 1;
            i_CurrentIndex += 1;
        }
        // 2nd, Define as Spreading Ink Type
        while (i_SpreadingInkPool > 0)
        {
            v_CurrentPosition = t_BlackInkParent.GetChild(list_InkOrder[i_CurrentIndex]).position;
            o_BlackInk = Instantiate(prefab_BlackInk, v_CurrentPosition, Quaternion.identity, t_BlackInkParent);
            s_BlackInk = o_BlackInk.GetComponent<S_BlackInk>();
            s_BlackInk.Type = InkType.Spreading;
            o_BlackInk.name = "BlackInk_" + list_InkOrder[i_CurrentIndex] + "_" + s_BlackInk.Type;
            i_SpreadingInkPool -= 1;
            i_CurrentIndex += 1;
        }
        // 3rd, Define as Fading Ink Type
        while (i_FadingInkPool > 0)
        {
            v_CurrentPosition = t_BlackInkParent.GetChild(list_InkOrder[i_CurrentIndex]).position;
            o_BlackInk = Instantiate(prefab_BlackInk, v_CurrentPosition, Quaternion.identity, t_BlackInkParent);
            s_BlackInk = o_BlackInk.GetComponent<S_BlackInk>();
            s_BlackInk.Type = InkType.Fading;
            o_BlackInk.name = "BlackInk_" + list_InkOrder[i_CurrentIndex] + "_" + s_BlackInk.Type;
            i_FadingInkPool -= 1;
            i_CurrentIndex += 1;
            list_InkTeleportSlot.Add(v_CurrentPosition);
        }
        // 4th, Define as Moving Ink Type
        while (i_MovingInkPool > 0)
        {
            v_CurrentPosition = t_BlackInkParent.GetChild(list_InkOrder[i_CurrentIndex]).position;
            o_BlackInk = Instantiate(prefab_BlackInk, v_CurrentPosition, Quaternion.identity, t_BlackInkParent);
            s_BlackInk = o_BlackInk.GetComponent<S_BlackInk>();
            s_BlackInk.Type = InkType.Moving;
            o_BlackInk.name = "BlackInk_" + list_InkOrder[i_CurrentIndex] + "_" + s_BlackInk.Type;
            i_MovingInkPool -= 1;
            i_CurrentIndex += 1;
        }
        // 5th, Define as Malicious Ink Type
        while (i_MaliciousInkPool > 0)
        {
            v_CurrentPosition = t_BlackInkParent.GetChild(list_InkOrder[i_CurrentIndex]).position;
            o_BlackInk = Instantiate(prefab_BlackInk, v_CurrentPosition, Quaternion.identity, t_BlackInkParent);
            s_BlackInk = o_BlackInk.GetComponent<S_BlackInk>();
            s_BlackInk.Type = InkType.Malicious;
            o_BlackInk.name = "BlackInk_" + list_InkOrder[i_CurrentIndex] + "_" + s_BlackInk.Type;
            i_MaliciousInkPool -= 1;
            i_CurrentIndex += 1;
        }
    }



    void Initialize_BlackInk_Phase_3_TeleportSlot()
    {
        // Note: Fading Inks have already recorded its positions in the Teleport list
        // Record the empty Black Ink objects as the teleportation's target positions
        // Note: i_TotalInkPool value will be the same as i_CurrentIndex
        for (int i = i_TotalInkPool; i < i_TotalInkSlot; i++)
            list_InkTeleportSlot.Add(t_BlackInkParent.GetChild(list_InkOrder[i]).position);
        // Print out all stored teleport slots' details
        S_DebugLog.LevelLog("Total amount of Black Ink's Teleport Slots = ", list_InkTeleportSlot.Count);
        for (int i = 0; i < list_InkTeleportSlot.Count; i++)
            // Note: 'F2' means to show the float value in fixed-point 2 decimals format
            S_DebugLog.LevelLog($"Black Ink's Teleport Slot [{i}] = ", list_InkTeleportSlot[i].ToString("F2"));
    }



    void Initialize_BlackInk_Phase_4_CleaningUp()
    {
        // Note: Destroy() function will take place after an update frame
        for (int i = 0; i < i_TotalInkSlot; i++)
        {
            // Get the current empty Black Ink object
            GameObject o_EmptyInk = t_BlackInkParent.GetChild(i).gameObject;
            // Destroy all the empty Black Ink objects
            Destroy(o_EmptyInk);
        }
    }



    void Initialize_BlackInk_Phase_5_StartCoroutine()
    {
        // Note: Check distance between the player and black inks 
        StartCoroutine(Coroutine_BlackInk());
    }



    // ------------------------------------------------------------ //



    // Used in [S_Controller]
    public void Splash(Vector3 v_TapWorld)
    {
        // Check if the cooldown timer has been completed
        // Note: Not sure if total splash charges are limited too
        if (Time.time > f_SplashCooldown)
        {
            StartCoroutine(Coroutine_Splash(v_TapWorld));
            // Note: You can define the cooldown duration here (in seconds)
            f_SplashCooldown = Time.time + 10f;
        }
    }



    // Used in [S_Player], [S_Level]
    public void Reveal(int i_SortingOrder, Transform t_Parts, Vector3 v_Position)
    {
        StartCoroutine(Coroutine_Reveal(i_SortingOrder, t_Parts, v_Position));
    }



    // Used in [S_Player]
    public void Progress()
    {
        // Increase the progress by 1 each time a masked part is revealed
        i_Progress += 1;
        // Calculate the percentage of current progress
        float f_Percent = ((float)i_Progress / (float)i_CompleteProgress) * 100;
        // Print out the log
        S_DebugLog.Log($"Current Progress = {i_Progress} / {i_CompleteProgress} ------- ",
        f_Percent.ToString("F1") + "%");
        // Move the progress bar indicator on the menu bar
        float f_PosX = (.046f * f_Percent) - 3.9f;
        t_ProgressBar.position = new Vector3(f_PosX, t_ProgressBar.position.y, 0);
        // Increase Hint button's alpha value after passing 70% progress
        // Note: Absolute path may become a dependency problem in the future
        if (f_Percent >= 70)
            t_HintButton.gameObject.SetActive(true);
        // Proceed to level completion sequences
        if (i_Progress == i_CompleteProgress)
            StartCoroutine(Coroutine_Completion());
    }



    // Used in [S_Controller]
    public void Hint()
    {
        // c_HintSprite.enabled = !c_HintSprite.enabled;
    }



    // Used in [S_Booster], It contains both functions and a return method
    public int BoosterIcon_Start(BoosterType Type)
    {
        // Local Variables
        int i_Index = 0;
        // Iterate through the BoosterIcon array to find an available slot
        for (int i = 0; i < b_BoosterIcons.Length; i++)
        {
            if (b_BoosterIcons[i] == false)
            {
                i_Index = i;
                b_BoosterIcons[i] = true;
                break;
            }
        }
        // Then, Assign a sprite to the selected BoosterIcon object
        switch (Type)
        {
            case BoosterType.Offensive:
                c_BoosterIcons[i_Index].sprite = Resources.Load<Sprite>("Sprites/IMG_Booster_Offensives");
                break;
            case BoosterType.Defensive:
                c_BoosterIcons[i_Index].sprite = Resources.Load<Sprite>("Sprites/IMG_Booster_Defensives");
                break;
            case BoosterType.Others:
                c_BoosterIcons[i_Index].sprite = Resources.Load<Sprite>("Sprites/IMG_Booster_Others");
                break;
        }
        // Then, Activate the selected sprite gameobject
        c_BoosterIcons[i_Index].gameObject.SetActive(true);

        S_DebugLog.TestingLog("BoosterIcons Availability = ", b_BoosterIcons[0].ToString() + b_BoosterIcons[1] + b_BoosterIcons[2]);
        // Lastly, Return the index back to its S_Booster
        return i_Index;
    }



    // Used in [S_Booster]
    public void BoosterIcon_End(int i_Index)
    {
        // Set the slot state to available as 'false'. Taken as 'true'
        b_BoosterIcons[i_Index] = false;
        // Turn off the selected sprite gameobject
        c_BoosterIcons[i_Index].gameObject.SetActive(false);
    }



    // Used in [S_Player]
    public void Respawn()
    {
        // Proceed to destroy the current player and create a new one
        StartCoroutine(Coroutine_Respawn());
        // Lose one life. 1st Death = Revive. 2nd Death = Revive. 3rd Death = Game Over
        // Note: Have to reach a value of '0' to count as death
        i_Lives -= 1;
        // Change its text indicator on the menu bar
        tmp_Lives.text = $"{i_Lives}";
        // Check if it is equal to zero. If so, level failed
        if (i_Lives <= 0)
        {
            S_DebugLog.TestingLog("You died! Current life value is = ", i_Lives);
        }
    }



    // ------------------------------------------------------------ //



    IEnumerator Coroutine_BlackInk()
    {
        // Local Variables
        List<S_BlackInk> list_InkScript = new List<S_BlackInk>();
        yield return null;
        // 1st, Get all the black ink objects and assign their scripts into a list
        for (int i = 0; i < t_BlackInkParent.childCount; i++)
        {
            S_BlackInk s_BlackInk = t_BlackInkParent.GetChild(i).GetComponent<S_BlackInk>();
            // Add them into the list
            list_InkScript.Add(s_BlackInk);
        }
        // 2nd, Check the distance between the player and black inks
        // Note: This will be a long-term loop and also it acts like Update()
        while (list_InkScript.Count > 0)
        {
            for (int i = 0; i < list_InkScript.Count; i++)
            {
                S_BlackInk s_BlackInk = list_InkScript[i];
                if (!s_BlackInk.b_IsAppeared)
                {
                    // Local Variables
                    float f_Distance = (t_Player.position - s_BlackInk.transform.position).sqrMagnitude;
                    // Note: You can specify the required distance here
                    if (f_Distance < 1.5f)
                    {
                        s_BlackInk.Appear();
                        yield return null;
                        // Remove this item from the list, no longer need to track this item
                        list_InkScript.Remove(s_BlackInk);
                    }
                }
            }
            // Note: Five computations per second
            yield return new WaitForSeconds(.2f);
        }
    }



    IEnumerator Coroutine_Splash(Vector3 v_TapWorld)
    {
        // Local Variables
        Vector2 v_CircleCenter = new Vector2(v_TapWorld.x, v_TapWorld.y);
        // 1st, Instantiate a new splash object to represent visuals
        GameObject o_Splash = Instantiate(prefab_Splash, v_TapWorld, Quaternion.identity);
        yield return null;
        // 2nd, Scale modifiers
        o_Splash.transform.localScale = Vector3.one * S_Booster.mod_SplashArea;
        // 3rd, Return an array of colliders if it overlapped with the user input
        Collider2D[] hits = Physics2D.OverlapCircleAll(v_CircleCenter, 0.2f * S_Booster.mod_SplashArea);
        S_DebugLog.TestingLog("Total hits = ", hits.Length);
        // 4th, Iterate through all of them and find the objects tagged as "Part"
        foreach (Collider2D hit in hits)
        {
            if (hit.tag == "Part")
            {
                // Get the sorting order of the collided masked part
                int i_SortingOrder = hit.GetComponent<SpriteRenderer>().sortingOrder;
                // Start the revealing process
                Reveal(i_SortingOrder, hit.transform, v_TapWorld);
                // Add progress and call the progression inner mechanism
                Progress();
                // Disable its collider component immediately
                hit.enabled = false;
                // Wait for the next 0.2 seconds before looping over
                yield return new WaitForSeconds(.2f);
            }
        }
        // Lastly, Wait for the splash animation to complete and then destroy it
        yield return new WaitForSeconds(1.5f);
        Destroy(o_Splash);
    }



    IEnumerator Coroutine_Reveal(int i_SortingOrder, Transform t_Parts, Vector3 v_Position)
    {
        // 1st, Get the child's transform from the current revealer index
        Transform t_Revealer = t_RevealerParent.GetChild(i_CurrentRevealer);
        // 2nd, Set its sorting order based on the player collider data
        t_Revealer.GetComponent<S_Revealer>().SetTarget(i_SortingOrder);
        // 3rd, Place this revealer at the given position
        t_Revealer.position = v_Position;
        // 4th, Activate the current selected revealer
        t_Revealer.gameObject.SetActive(true);
        // 5th, Loop the revealer index and reset it if it reaches the end
        if (i_CurrentRevealer < t_RevealerParent.childCount - 1)
            i_CurrentRevealer += 1;
        else
            i_CurrentRevealer = 0;
        // Lastly, After four seconds, End the revealing process
        // The number of seconds here is defined by the animation duration 
        yield return new WaitForSeconds(4);
        t_Parts.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
        yield return null;
        t_Revealer.gameObject.SetActive(false);
    }



    IEnumerator Coroutine_Respawn()
    {
        // 1st, Disable all player's inputs
        // Note: This will also stop returning to the player's rigidbody default drag
        s_Controller.enabled = false;
        // 2nd, Make sure the camera stays in the player view
        b_FullView = false;
        // 3rd, Set the target respawn point for the new player object
        // Check to see which respawn point is closest to the player
        // And only if they have accessed to there. If not, go to the second closest
        Vector3 v_RespawnPoint = t_RespawnParent.GetChild(0).position;
        // 4th, Wait for the absorption to complete before respawning
        yield return new WaitForSeconds(4);

        // -------------------- ! START RESPAWN ! -------------------- //

        // 5th, Deactivate all scripts connected to the current player object
        s_Camera.enabled = false;
        // 6th, Destroy the current dead player object
        Destroy(t_Player.parent.gameObject);
        yield return null;
        // 7th, Instantiate a new player object to a specific respawn point
        GameObject o_NewPlayer = Instantiate(prefab_Player, v_RespawnPoint, Quaternion.identity, t_PlayerParent);
        yield return null;
        o_NewPlayer.name = "Player_Life_" + i_Lives;
        // 8th, Re-cache this new player's transform into t_Player
        t_Player = GameObject.FindWithTag("Player").transform;
        yield return null;

        // -------------------- ! END RESPAWN ! -------------------- //

        // Lastly, Activate all disabled scripts
        // Note: Re-enable [S_Camera] and [S_Controller] will cache the new player object
        s_Camera.enabled = true;
        yield return new WaitForSeconds(1);
        s_Controller.enabled = true;
    }



    IEnumerator Coroutine_Completion()
    {
        // Local Variables 
        float f_Time = 0f;
        // 1st, Block all possible disturbing factors
        t_Player.GetComponent<CircleCollider2D>().enabled = false;
        // 2nd, Wait for awhile and let the last masked part be revealed
        yield return new WaitForSeconds(2);
        // 3rd, Disable all player's inputs
        s_Controller.enabled = false;

        // -------------------- ! PREPARE ANIMATION ! -------------------- //

        // 4th, Create a full-screen panel to block other visual elements
        // Note: The animation object's sorting order is '1'
        // Note: Currently, Panel's sprite alpha is zero
        GameObject o_FullPanel_Completion = Instantiate(prefab_FullPanel, t_AnimationParent);
        SpriteRenderer c_FullPanel_Completion_Sprite = o_FullPanel_Completion.GetComponent<SpriteRenderer>();
        o_FullPanel_Completion.name = "FullPanel_Completion";
        c_FullPanel_Completion_Sprite.sortingLayerName = "Completion";
        c_FullPanel_Completion_Sprite.sortingOrder = 0;
        c_FullPanel_Completion_Sprite.color = Color.clear;
        // 5th, Activate the completion animation object
        // Note: Currently, Animator is paused and Animation's sprite alpha is zero
        GameObject o_Animation = t_AnimationParent.GetChild(0).gameObject;
        Animator c_Animation_Animator = o_Animation.GetComponent<Animator>();
        SpriteRenderer c_Animation_Sprite = o_Animation.GetComponent<SpriteRenderer>();
        t_AnimationParent.gameObject.SetActive(true);
        c_Animation_Animator.speed = 0;
        c_Animation_Sprite.color = Color.clear;
        yield return null;

        // -------------------- ! START ANIMATION ! -------------------- //

        // 6th, Enter the full view mode
        b_FullView = true;
        // 7th, Fade in both Panel and Animation's sprite
        // Note: This will be a 1 second loop and also it acts like Update()
        while (f_Time < 1)
        {
            c_FullPanel_Completion_Sprite.color = new Color(0.89f, 0.89f, 0.89f, f_Time);
            c_Animation_Sprite.color = new Color(1, 1, 1, f_Time);
            // Calculate the time spent in seconds
            f_Time += Time.deltaTime;
            // Wait for the next frame before looping over
            yield return null;
        }
        // 8th, Start the animation
        c_Animation_Animator.speed = 1;
        // 9th, Wait for awhile until the animation reaches a specific keyframe
        yield return new WaitForSeconds(4);

        // -------------------- ! PREPARE SCREENSHOT ! -------------------- //

        // 10th, Create another full-screen panel to darken the background
        // Note: The card object's sorting order is '3'
        // Note: Currently, Panel_Black's sprite alpha is zero
        GameObject o_FullPanel_Black = Instantiate(prefab_FullPanel, t_AnimationParent);
        SpriteRenderer c_FullPanel_Black_Sprite = o_FullPanel_Black.GetComponent<SpriteRenderer>();
        o_FullPanel_Black.name = "FullPanel_Black";
        c_FullPanel_Black_Sprite.sortingLayerName = "Completion";
        c_FullPanel_Black_Sprite.sortingOrder = 2;
        c_FullPanel_Black_Sprite.color = Color.clear;
        // 11th, Create the last full-screen panel to simulate the screenshot effect
        // Note: [0] Panel_Completion; [1] Animation; [2] Panel_Black; [3] Card; [4] Panel_Screenshot 
        // Note: Currently, Panel_Screenshot's sprite alpha is zero
        GameObject o_FullPanel_Screenshot = Instantiate(prefab_FullPanel, t_AnimationParent);
        SpriteRenderer c_FullPanel_Screenshot_Sprite = o_FullPanel_Screenshot.GetComponent<SpriteRenderer>();
        o_FullPanel_Screenshot.name = "FullPanel_Screenshot";
        c_FullPanel_Screenshot_Sprite.sortingLayerName = "Completion";
        c_FullPanel_Screenshot_Sprite.sortingOrder = 4;
        c_FullPanel_Screenshot_Sprite.color = Color.clear;

        // -------------------- ! SCREENSHOT TIME ! -------------------- //

        // 12th, Pause the animation in the middle
        c_Animation_Animator.speed = 0;
        // 13th, Make both panels appear 
        c_FullPanel_Screenshot_Sprite.color = Color.white;
        c_FullPanel_Black_Sprite.color = new Color(0, 0, 0, 0.6f);
        yield return null;
        // 14th, Prepare the card objects
        SpriteRenderer c_Card_Sprite = t_CardParent.GetChild(0).GetComponent<SpriteRenderer>();
        SpriteRenderer c_Card_Blank = t_CardParent.GetChild(1).GetComponent<SpriteRenderer>();
        t_CardParent.gameObject.SetActive(true);
        c_Card_Sprite.color = Color.clear;
        c_Card_Blank.color = Color.clear;
        yield return null;
        // 15th, Fade out Panel_Screenshot's sprite but fade in Card's sprite
        // Note: This will be a 2 seconds loop and also it acts like Update()
        // Note: Have to adjust the delta time calculation to slow down the timer 
        f_Time = 0f;
        while (f_Time < 1)
        {
            c_FullPanel_Screenshot_Sprite.color = new Color(1, 1, 1, 1f - f_Time);
            c_Card_Sprite.color = new Color(1, 1, 1, 0.4f + f_Time);
            // Wait for the next frame before looping over
            f_Time += Time.deltaTime / 2f;
            yield return null;
        }
        // 16th, Wait for a while before continuing the animation
        yield return new WaitForSeconds(4);

        // -------------------- ! SUMMARY TIME ! -------------------- //

        // 17th, Fade in the blank card sprite
        // Note: This will be a 1 second loop and also it acts like Update()
        f_Time = 0f;
        while (f_Time < 1)
        {
            c_Card_Blank.color = new Color(1, 1, 1, f_Time);
            // Wait for the next frame before looping over
            f_Time += Time.deltaTime;
            yield return null;
        }
        c_Card_Sprite.gameObject.SetActive(false);
        // Local Variables
        SpriteRenderer c_Star_1 = t_SummaryParent.GetChild(0).GetComponent<SpriteRenderer>();
        SpriteRenderer c_Star_2 = t_SummaryParent.GetChild(1).GetComponent<SpriteRenderer>();
        SpriteRenderer c_Star_3 = t_SummaryParent.GetChild(2).GetComponent<SpriteRenderer>();
        TextMeshPro tmp_TitleChapter = t_SummaryParent.GetChild(3).GetComponent<TextMeshPro>();
        TextMeshPro tmp_TitleLevel = t_SummaryParent.GetChild(4).GetComponent<TextMeshPro>();
        SpriteRenderer c_Line = t_SummaryParent.GetChild(5).GetComponent<SpriteRenderer>();
        TextMeshPro tmp_TitleSummary = t_SummaryParent.GetChild(6).GetComponent<TextMeshPro>();
        TextMeshPro tmp_ValueSummary = t_SummaryParent.GetChild(7).GetComponent<TextMeshPro>();
        int[] i_StarColors = new int[] { 0, 0, 0 };
        // 18th, Set the color of the stars
        for (int i = 1; i <= 3; i++)
            if (i_Lives >= i)
                i_StarColors[i - 1] = 1;
        // 19th, Fade in all summary' objects in ascending order
        // Note: This will be a x seconds loop and also it acts like Update()
        t_SummaryParent.gameObject.SetActive(true);
        f_Time = 0f;
        while (f_Time < 7)
        {
            c_Star_1.color = new Color(i_StarColors[0], i_StarColors[0], i_StarColors[0], f_Time);
            c_Star_2.color = new Color(i_StarColors[1], i_StarColors[1], i_StarColors[1], f_Time - 0.5f);
            c_Star_3.color = new Color(i_StarColors[2], i_StarColors[2], i_StarColors[2], f_Time - 1.0f);
            tmp_TitleChapter.color = new Color(0, 0, 0, f_Time - 1.25f);
            tmp_TitleLevel.color = new Color(0, 0, 0, f_Time - 1.5f);
            c_Line.color = new Color(0, 0, 0, Mathf.Clamp(f_Time - 1.75f, 0, 0.2f));
            tmp_TitleSummary.color = new Color(.2f, .2f, .2f, f_Time - 2.0f);
            tmp_ValueSummary.color = new Color(.2f, .2f, .2f, f_Time - 2.5f);
            // Wait for the next frame before looping over
            f_Time += Time.deltaTime;
            yield return null;
        }
        // 20th, Instantiate and fade in another blank card sprite
        GameObject o_Card_Blank2 = Instantiate(c_Card_Blank.gameObject, t_CardParent);
        SpriteRenderer c_Card_Blank2 = o_Card_Blank2.GetComponent<SpriteRenderer>();
        c_Card_Blank2.color = Color.clear;
        c_Card_Blank2.sortingOrder = 7;
        yield return null;
        f_Time = 0f;
        while (f_Time < 1)
        {
            c_Card_Blank2.color = new Color(1, 1, 1, Mathf.SmoothStep(0, 1, f_Time));
            // Wait for the next frame before looping over
            f_Time += Time.deltaTime;
            yield return null;
        }
        // 21st, Then, Deactivate the summary parent object and first blank card
        t_SummaryParent.gameObject.SetActive(false);
        c_Card_Blank.gameObject.SetActive(false);

        // -------------------- ! CONTINUE ANIMATION ! -------------------- //

        // 22nd, Fade out both Panel_Black and Card's sprite, also slowly resume animation
        // Note: This will be a 1.5 seconds loop and also it acts like Update()
        // Note: Have to adjust the delta time calculation to slow down the timer 
        f_Time = 0f;
        while (f_Time < 1)
        {
            c_Card_Blank2.color = new Color(1, 1, 1, 1f - f_Time);
            c_FullPanel_Black_Sprite.color = new Color(0, 0, 0, 0.6f - f_Time);
            c_Animation_Animator.speed = f_Time;
            // Wait for the next frame before looping over
            f_Time += Time.deltaTime / 1.5f;
            yield return null;

        }
        // Note: Make sure that the animation's speed is the exact value of 1
        c_Animation_Animator.speed = 1;

        // -------------------- ! END ANIMATION ! -------------------- //

        // Send this level score back to the S_Data
        string SceneName = SceneManager.GetActiveScene().name;
        S_Data.Instance.SetScore(SceneName, i_Lives);
        // Go to the next scene, transitioning




    }
}
