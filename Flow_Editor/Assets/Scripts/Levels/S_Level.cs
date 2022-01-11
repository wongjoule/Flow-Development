using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InkType { Idle, Spreading, Fading, Moving, Malicious }


public class S_Level : MonoBehaviour
{
    // Inspector Variables for level settings
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
    S_Camera s_Camera;
    S_Controller s_Controller;

    // Variables - View Mode
    [HideInInspector] public bool b_IsFullView = false; // Unified management of the control of the Full View
    [HideInInspector] public List<SpriteRenderer> list_ViewModeSprite = new List<SpriteRenderer>();
    float f_ViewModeAlpha = 1f;
    SpriteRenderer c_BackgroundSprite;

    // Variables - Splash
    float f_SplashCooldown = 0f;

    // Variables - Black Ink
    Transform t_BlackInkParent;
    int i_TotalInkPool = 0;
    int i_TotalInkSlot = 0;
    List<int> list_InkOrder = new List<int>();
    [HideInInspector] public List<Vector3> list_InkTeleportSlot = new List<Vector3>();

    // Variables - Parts
    Transform t_PartsParent;

    // Variables - Revealer
    Transform t_RevealerParent;
    int i_CurrentRevealer = 0;

    // Variables - Respawn
    Transform t_RespawnParent;
    Transform t_PlayerParent;

    // Variables - Progress
    int i_Progress = 0;
    int i_CompleteProgress = 0;

    // Variables - Hint
    Transform t_HintParent;
    SpriteRenderer c_HintSprite;

    // Variables - Menu Bar
    Transform t_MenuBarParent;
    int i_Life = 3; // Note: It need to reach '-1' to count as death

    // Variables - Animation
    Transform t_AnimationParent;

    // Variables - Card
    Transform t_CardParent;

    // Variables - Resources Path
    [HideInInspector] public GameObject prefab_Player;
    [HideInInspector] public GameObject prefab_BlackInk;
    [HideInInspector] public GameObject prefab_Revealer;
    [HideInInspector] public GameObject prefab_Panel; // A full-screen panel made with sprite
    [HideInInspector] public GameObject prefab_Splash;



    void Awake()
    {
        // Generic Variables
        t_Player = GameObject.FindWithTag("Player").transform;
        t_Camera = GameObject.FindWithTag("MainCamera").transform;
        s_Camera = t_Camera.GetComponent<S_Camera>();
        s_Controller = GameObject.Find("CONTROLLERS").GetComponent<S_Controller>();
        // Variables - Black Ink
        t_BlackInkParent = GameObject.Find("BLACKINKS").transform;
        // Variables - Parts
        t_PartsParent = GameObject.Find("PARTS").transform;
        // Variables - Revealer
        t_RevealerParent = GameObject.Find("REVEALERS").transform;
        // Variables - Respawn
        t_RespawnParent = GameObject.Find("RESPAWNS").transform;
        t_PlayerParent = GameObject.Find("PLAYERS").transform;
        // Variables - Hint
        t_HintParent = GameObject.Find("HINTS").transform;
        c_HintSprite = t_HintParent.GetComponentInChildren<SpriteRenderer>();
        // Variables - Menu Bar
        t_MenuBarParent = GameObject.Find("MENUBARS").transform;
        // Variables - Animation
        t_AnimationParent = GameObject.Find("ANIMATIONS").transform;
        // Variables - Card
        t_CardParent = GameObject.Find("CARDS").transform;
        // Variables - Resources Path
        prefab_Player = Resources.Load<GameObject>("Prefabs/Player");
        prefab_BlackInk = Resources.Load<GameObject>("Prefabs/BlackInk");
        prefab_Revealer = Resources.Load<GameObject>("Prefabs/Revealer");
        prefab_Panel = Resources.Load<GameObject>("Prefabs/Panel");
        prefab_Splash = Resources.Load<GameObject>("Prefabs/Splash");


        c_BackgroundSprite = GameObject.Find("Background_FullView").GetComponent<SpriteRenderer>();






        // Temporary
        Application.targetFrameRate = 60;

    }



    // Level Initialization will require a loading scene
    void Start()
    {
        // If Prerequisite returns 'true', enter the next initialization phase
        // Else, Throw an error log and immediately stop the initialization process
        if (Initialize_Prerequisite())
        {
            Initialize_BlackInk();
            Initialize_Parts();
            Initialize_Revealer();
            Initialize_Respawn();
            Initialize_Progression();
            Initialize_Hint();
            Initialize_Animation();
            Initialize_Card();
        }
        else
        {
            S_DebugLog.ErrorLog("The level initialization process has been terminated.");
        }
    }



    void Update()
    {
        switch (b_IsFullView)
        {
            case true:
                // Note: Decrease the alpha value from '1' to '0'
                if (f_ViewModeAlpha > 0)
                {
                    // The fade-out duration is 1 sec
                    f_ViewModeAlpha -= Time.deltaTime;
                    // Clamp the alpha value within 0 and 1
                    f_ViewModeAlpha = Mathf.Clamp(f_ViewModeAlpha, 0f, 1f);
                    // Fade out everything in the list_SpriteRenderer
                    foreach (SpriteRenderer c_Sprite in list_ViewModeSprite)
                    {
                        c_Sprite.color = new Color(1, 1, 1, f_ViewModeAlpha);
                    }
                    // Fade in the silhouette hint image
                    // Note: Here f_Alpha is inverted, its maximum value is '0.5'
                    float f_HintAlpha = (1f - f_ViewModeAlpha) - 0.5f; // Half second duration
                    c_HintSprite.color = new Color(1, 1, 1, f_HintAlpha);

                    c_BackgroundSprite.color = new Color(1, 1, 1, 1f - f_ViewModeAlpha);
                }

                break;


            case false:
                // Note: Increase the alpha value from '0' to '1'
                if (f_ViewModeAlpha < 1)
                {
                    // The fade-in duration is 1 sec
                    f_ViewModeAlpha += Time.deltaTime;
                    // Clamp the alpha value within 0 and 1
                    f_ViewModeAlpha = Mathf.Clamp(f_ViewModeAlpha, 0f, 1f);
                    // Fade in everything in the list_SpriteRenderer
                    foreach (SpriteRenderer c_Sprite in list_ViewModeSprite)
                    {
                        c_Sprite.color = new Color(1, 1, 1, f_ViewModeAlpha);
                    }
                    // Fade out the silhouette hint image
                    // Note: Here f_Alpha is inverted, its minimum value is '0'
                    float f_HintAlpha = (1f - f_ViewModeAlpha) - 0.5f; // Half second duration
                    c_HintSprite.color = new Color(1, 1, 1, f_HintAlpha);

                    c_BackgroundSprite.color = new Color(1, 1, 1, 1f - f_ViewModeAlpha);
                }

                break;
        }
    }



    // ------------------------------------------------------------ //



    bool Initialize_Prerequisite()
    {
        // Check whether all stuffs can be tracked in the Resources folder
        // If one of them cannot be tracked, stop the level initialization
        if (!Initialize_Prerequisite_ResourcesPath())
        {
            return false;
        }

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



    void Initialize_BlackInk()
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
            // Note: Add a value of 1 so it starts from index 1
            c_Sprite.sortingOrder = i + 1;
            // Initialize its object's tag
            c_Sprite.tag = "Part";
            // Initialize its mask interaction behavior to be visible only inside the mask
            c_Sprite.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        }
    }



    void Initialize_Revealer()
    {
        // Find the revealers' parent and store it into a local variable
        Transform t_RevealerParent = GameObject.Find("REVEALERS").transform;
        // Check the current map's parts amount then instantiate itself

        // Iterate through all of revealers and deactivate them
        for (int i = 0; i < t_RevealerParent.childCount; i++)
        {
            // Get the current revealer's game object
            GameObject o_Revealer = t_RevealerParent.GetChild(i).gameObject;
            // Deactivate all revealers. Default state
            o_Revealer.SetActive(false);
        }
    }



    void Initialize_Respawn()
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



    void Initialize_Progression()
    {
        // Get the total amount of Parts 
        i_CompleteProgress = t_PartsParent.childCount;
        // Print out the log
        S_DebugLog.LevelLog("Total amount of Parts = ", i_CompleteProgress);
    }



    void Initialize_Hint()
    {
        // Default value
        c_HintSprite.enabled = false;
    }



    void Initialize_Animation()
    {
        // Deactivate the completion animation object at the beginning
        t_AnimationParent.GetChild(0).gameObject.SetActive(false);
    }



    void Initialize_Card()
    {
        // Deactivate the completion card object at the beginning
        t_CardParent.GetChild(0).gameObject.SetActive(false);
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
            // Firstly, Generate the first random number
            i_RandomIndex = Random.Range(0, i_TotalInkSlot);

            // Secondly, Check and repeat if the list already contains the current generated number
            while (list_InkOrder.Contains(i_RandomIndex))
            {
                i_RandomIndex = Random.Range(0, i_TotalInkSlot);
            }

            // Lastly, Add the generated unique number to the list
            list_InkOrder.Add(i_RandomIndex);
        }

        S_DebugLog.LevelLog("Black Ink's Order List = ", string.Join(", ", list_InkOrder.ConvertAll(i => i.ToString()).ToArray()));
    }



    void Initialize_BlackInk_Phase_2_AssigningType()
    {
        // Create and to record current index. Starts from zero
        int i_CurrentIndex = 0;

        // Create local variables to store various current data
        Vector3 v_CurrentPosition;
        GameObject o_BlackInk;
        S_BlackInk s_BlackInk;

        // Firstly, Define as Idle Ink Type
        while (i_IdleInkPool > 0)
        {
            // Get the current Black Ink's child position based on the ordered list 
            v_CurrentPosition = t_BlackInkParent.GetChild(list_InkOrder[i_CurrentIndex]).position;
            // Instantiate a real Black Ink game object to that child position, and cache to a local variable
            o_BlackInk = Instantiate(prefab_BlackInk, v_CurrentPosition, Quaternion.identity, t_BlackInkParent);
            // Get access to this newly instantiated Black Ink's script
            s_BlackInk = o_BlackInk.GetComponent<S_BlackInk>();
            // Define this specific Black Ink as this ink type
            s_BlackInk.Type = InkType.Idle;
            // Change the name of this instantiated Black Ink object with the Order Index and Ink Type
            o_BlackInk.name = "BlackInk_" + list_InkOrder[i_CurrentIndex] + "_" + s_BlackInk.Type;
            // Decrease the available slot for this ink type
            i_IdleInkPool -= 1;
            // Increase the current order index
            i_CurrentIndex += 1;
        }

        // Secondly, Define as Spreading Ink Type
        while (i_SpreadingInkPool > 0)
        {
            // Get the current Black Ink's child position based on the ordered list 
            v_CurrentPosition = t_BlackInkParent.GetChild(list_InkOrder[i_CurrentIndex]).position;
            // Instantiate a real Black Ink game object to that child position
            o_BlackInk = Instantiate(prefab_BlackInk, v_CurrentPosition, Quaternion.identity, t_BlackInkParent);
            // Get access to this newly instantiated Black Ink's script
            s_BlackInk = o_BlackInk.GetComponent<S_BlackInk>();
            // Define this specific Black Ink as this ink type
            s_BlackInk.Type = InkType.Spreading;
            // Change the name of this instantiated Black Ink object with the Order Index and Ink Type
            o_BlackInk.name = "BlackInk_" + list_InkOrder[i_CurrentIndex] + "_" + s_BlackInk.Type;
            // Decrease the available slot for this ink type
            i_SpreadingInkPool -= 1;
            // Increase the current order index
            i_CurrentIndex += 1;
        }

        // Thirdly, Define as Fading Ink Type
        while (i_FadingInkPool > 0)
        {
            // Get the current Black Ink's child position based on the ordered list 
            v_CurrentPosition = t_BlackInkParent.GetChild(list_InkOrder[i_CurrentIndex]).position;
            // Instantiate a real Black Ink game object to that child position, and cache to a local variable
            o_BlackInk = Instantiate(prefab_BlackInk, v_CurrentPosition, Quaternion.identity, t_BlackInkParent);
            // Get access to this newly instantiated Black Ink's script
            s_BlackInk = o_BlackInk.GetComponent<S_BlackInk>();
            // Define this specific Black Ink as this ink type
            s_BlackInk.Type = InkType.Fading;
            // Change the name of this instantiated Black Ink object with the Order Index and Ink Type
            o_BlackInk.name = "BlackInk_" + list_InkOrder[i_CurrentIndex] + "_" + s_BlackInk.Type;
            // Decrease the available slot for this ink type
            i_FadingInkPool -= 1;
            // Increase the current order index
            i_CurrentIndex += 1;
            // Add this Black Ink object position as one of the teleportation's target positions
            list_InkTeleportSlot.Add(v_CurrentPosition);
        }

        // Fourthly, Define as Moving Ink Type
        while (i_MovingInkPool > 0)
        {
            // Get the current Black Ink's child position based on the ordered list 
            v_CurrentPosition = t_BlackInkParent.GetChild(list_InkOrder[i_CurrentIndex]).position;
            // Instantiate a real Black Ink game object to that child position, and cache to a local variable
            o_BlackInk = Instantiate(prefab_BlackInk, v_CurrentPosition, Quaternion.identity, t_BlackInkParent);
            // Get access to this newly instantiated Black Ink's script
            s_BlackInk = o_BlackInk.GetComponent<S_BlackInk>();
            // Define this specific Black Ink as this ink type
            s_BlackInk.Type = InkType.Moving;
            // Change the name of this instantiated Black Ink object with the Order Index and Ink Type
            o_BlackInk.name = "BlackInk_" + list_InkOrder[i_CurrentIndex] + "_" + s_BlackInk.Type;
            // Decrease the available slot for this ink type
            i_MovingInkPool -= 1;
            // Increase the current order index
            i_CurrentIndex += 1;
        }

        // Fifthly, Define as Malicious Ink Type
        while (i_MaliciousInkPool > 0)
        {
            // Get the current Black Ink's child position based on the ordered list 
            v_CurrentPosition = t_BlackInkParent.GetChild(list_InkOrder[i_CurrentIndex]).position;
            // Instantiate a real Black Ink game object to that child position, and cache to a local variable
            o_BlackInk = Instantiate(prefab_BlackInk, v_CurrentPosition, Quaternion.identity, t_BlackInkParent);
            // Get access to this newly instantiated Black Ink's script
            s_BlackInk = o_BlackInk.GetComponent<S_BlackInk>();
            // Define this specific Black Ink as this ink type
            s_BlackInk.Type = InkType.Malicious;
            // Change the name of this instantiated Black Ink object with the Order Index and Ink Type
            o_BlackInk.name = "BlackInk_" + list_InkOrder[i_CurrentIndex] + "_" + s_BlackInk.Type;
            // Decrease the available slot for this ink type
            i_MaliciousInkPool -= 1;
            // Increase the current order index
            i_CurrentIndex += 1;
        }

    }



    void Initialize_BlackInk_Phase_3_TeleportSlot()
    {
        // Note: Fading Inks have already recorded its positions in the Teleport list
        // Record the empty Black Ink objects as the teleportation's target positions
        // Note: i_TotalInkPool value will be the same as i_CurrentIndex
        for (int i = i_TotalInkPool; i < i_TotalInkSlot; i++)
        {
            list_InkTeleportSlot.Add(t_BlackInkParent.GetChild(list_InkOrder[i]).position);
        }

        // Print out all stored teleport slots' details
        S_DebugLog.LevelLog("Total amount of Black Ink's Teleport Slots = ", list_InkTeleportSlot.Count);

        for (int i = 0; i < list_InkTeleportSlot.Count; i++)
        {
            // Note: 'F2' means to show the float value in fixed-point 2 decimals format
            S_DebugLog.LevelLog($"Black Ink's Teleport Slot [{i}] = ", list_InkTeleportSlot[i].ToString("F2"));
        }

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



    bool Initialize_Prerequisite_ResourcesPath()
    {
        // Note: If one of them cannot be loaded, stop the level initialization

        if (prefab_Player == null)
            S_DebugLog.ErrorLog("'Resources/Prefabs/Player.prefab' cannot be located.");

        if (prefab_BlackInk == null)
            S_DebugLog.ErrorLog("'Resources/Prefabs/BlackInk.prefab' cannot be located.");

        if (prefab_Revealer == null)
            S_DebugLog.ErrorLog("'Resources/Prefabs/Revealer.prefab' cannot be located.");

        if (prefab_Panel == null)
            S_DebugLog.ErrorLog("'Resources/Prefabs/Panel.prefab' cannot be located.");

        if (prefab_Splash == null)
            S_DebugLog.ErrorLog("'Resources/Prefabs/Splash.prefab' cannot be located.");

        // If every conditions above are met, return 'true' and the level initialization process continues
        return true;
    }



    // ------------------------------------------------------------ //

    // Used in [S_Controller]
    public void Splash(Vector3 v_TapWorld)
    {
        // Check if the cooldown timer has been completed and there is still ammunition
        // Note: During the cooldown period, f_SplashCooldown is always greater than the current time
        if (f_SplashCooldown < Time.time)
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
        float f_Percentage = ((float)i_Progress / (float)i_CompleteProgress) * 100;
        // Print out the log
        S_DebugLog.Log($"Current Progress = {i_Progress} / {i_CompleteProgress} ------- ",
        f_Percentage.ToString("F1") + "%");

        // -------------------- ! PROGRESS BAR ADJUSTMENT ! -------------------- //

        // Local Variables
        // Note: Absolute path may become a dependency problem in the future
        Transform t_ProgressBar = t_MenuBarParent.GetChild(3);
        // Adjust the progress bar according to the player's current progress
        // Note: The maximum value of scale is '1', which is equal to 100% progress
        t_ProgressBar.localScale = new Vector3(f_Percentage / 100f, 1, 1);

        // -------------------- ! HINT BUTTON APPEARANCE ! -------------------- //

        // Increase Hint button's alpha value after passing 70% progress
        // Note: Absolute path may become a dependency problem in the future
        if (f_Percentage >= 70)
        {
            SpriteRenderer c_HintButtonSprite = t_MenuBarParent.GetChild(4).GetComponent<SpriteRenderer>();
            c_HintButtonSprite.color = new Color(0, 0, 0, 1);
        }

        // -------------------- ! COMPLETION REQUIREMENT ! -------------------- //

        // Proceed to level completion sequences
        if (i_Progress == i_CompleteProgress)
        {
            StartCoroutine(Coroutine_Completion());
        }
    }



    // Used in [S_Controller]
    public void Hint()
    {
        // Calculate the percentage of current progress
        float f_Percentage = ((float)i_Progress / (float)i_CompleteProgress) * 100;
        // Check if the player managed to pass 70% progress
        if (f_Percentage >= 70)
        {
            c_HintSprite.enabled = !c_HintSprite.enabled;
        }
    }



    // Used in [S_Player]
    public void Respawn()
    {
        // Proceed to destroy the current player and create a new one
        StartCoroutine(Coroutine_Respawn());
        // Lose one life. You can still continue even with 'zero' lives
        // Note: More like "Chances", 'zero' means you have no more chances
        // Note: It need to reach a value of '-1' to count as death
        i_Life -= 1;
        // Check if it is less than zero (reaches '-1'). If so, level failed
        if (i_Life < 0)
        {
            S_DebugLog.TestingLog("You died! Current life value is = ", i_Life);
            // Do return for now, until I have implemented Scene Manager
            return;
        }

        // -------------------- ! LIFE BAR DEACTIVATION ! -------------------- //

        // Deactivate one life object on the life bar
        // Note: Absolute path may become a dependency problem in the future
        // Note: Reverse the order of getchild so that it disappears from the left
        int i_Order = 2 - i_Life;
        t_MenuBarParent.GetChild(i_Order).gameObject.SetActive(false);
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
                // Local Variables
                S_BlackInk s_BlackInk = list_InkScript[i];
                //
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
            yield return new WaitForSeconds(0.2f);
        }

        S_DebugLog.TestingLog("You have reached the end of the loop.", "");

    }



    IEnumerator Coroutine_Splash(Vector3 v_TapWorld)
    {
        // Local Variables
        Vector2 v_CircleCenter = new Vector2(v_TapWorld.x, v_TapWorld.y);
        // 1st, Instantiate a new splash object to represent visuals
        GameObject o_Splash = Instantiate(prefab_Splash, v_TapWorld, Quaternion.identity);
        yield return null;
        // 2nd, Return an array of colliders if it overlapped with the user input
        Collider2D[] hits = Physics2D.OverlapCircleAll(v_CircleCenter, 0.2f);
        S_DebugLog.TestingLog("Total hits = ", hits.Length);
        // 3rd, Iterate through all of them and find the objects tagged as "Part"
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
                yield return new WaitForSeconds(0.2f);
            }
        }
        // Lastly, Wait for the splash animation to complete and then destroy it
        yield return new WaitForSeconds(1.5f);
        Destroy(o_Splash);
    }



    IEnumerator Coroutine_Reveal(int i_SortingOrder, Transform t_Parts, Vector3 v_Position)
    {
        // Firstly, Get the child's transform from the current revealer index
        Transform t_Revealer = t_RevealerParent.GetChild(i_CurrentRevealer);
        // Secondly, Set its sorting order based on the player collider data
        t_Revealer.GetComponent<S_Revealer>().SetTarget(i_SortingOrder);
        // Thirdly, Place this revealer at the given position
        t_Revealer.position = v_Position;
        // Fourthly, Activate the current selected revealer
        t_Revealer.gameObject.SetActive(true);
        // Fifthly, Loop the revealer index and reset it if it reaches the end
        if (i_CurrentRevealer < t_RevealerParent.childCount - 1)
        {
            i_CurrentRevealer += 1;
        }
        else
        {
            i_CurrentRevealer = 0;
        }

        // Lastly, After four seconds, End the revealing process
        // The number of seconds here is defined by the animation duration 
        yield return new WaitForSeconds(4);
        t_Parts.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.None;
        yield return null;
        t_Revealer.gameObject.SetActive(false);
    }



    IEnumerator Coroutine_Respawn()
    {
        // Firstly, Disable all player's inputs
        // Note: This will also stop returning to the player's rigidbody default drag
        s_Controller.enabled = false;
        // Secondly, Make sure the camera stays in the player view
        b_IsFullView = false;
        // Thirdly, Set the target respawn point for the new player object
        // // Check to see which respawn point is closest to the player
        // // And only if they have accessed to there. If not, go to the second closest
        Vector3 v_RespawnPoint = t_RespawnParent.GetChild(0).position;
        // Fourthly, Wait for the absorption to complete before respawning
        yield return new WaitForSeconds(4);

        // -------------------- ! START RESPAWN ! -------------------- //

        // Fifthly, Deactivate all scripts connected to the current player object
        s_Camera.enabled = false;
        // Sixthly, Destroy the current dead player object
        Destroy(t_Player.parent.gameObject);
        yield return null;
        // Seventhly, Instantiate a new player object to a specific respawn point
        GameObject o_NewPlayer = Instantiate(prefab_Player, v_RespawnPoint, Quaternion.identity, t_PlayerParent);
        yield return null;
        o_NewPlayer.name = "Player_Life_" + i_Life;
        // Eightly, Re-cache this new player's transform into t_Player
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
        // 1st, Disable all player's inputs
        s_Controller.enabled = false;
        // 2nd, Block all possible disturbing factors
        t_Player.GetComponent<CircleCollider2D>().enabled = false;
        // 3rd, Wait for awhile and let the last masked part be revealed
        yield return new WaitForSeconds(1);

        // -------------------- ! PREPARE ANIMATION ! -------------------- //

        // 4th, Create a full-screen panel to block other visual elements
        // Note: The animation object's sorting order is '1'
        // Note: Currently, Panel's sprite alpha is zero
        GameObject o_Panel_Completion = Instantiate(prefab_Panel, t_AnimationParent);
        SpriteRenderer c_Panel_Completion_Sprite = o_Panel_Completion.GetComponent<SpriteRenderer>();
        o_Panel_Completion.name = "Panel_Completion";
        c_Panel_Completion_Sprite.sortingLayerName = "Completion";
        c_Panel_Completion_Sprite.sortingOrder = 0;
        c_Panel_Completion_Sprite.color = Color.clear;
        // 5th, Activate the completion animation object
        // Note: Currently, Animator is paused and Animation's sprite alpha is zero
        GameObject o_Animation = t_AnimationParent.GetChild(0).gameObject;
        Animator c_Animation_Animator = o_Animation.GetComponent<Animator>();
        SpriteRenderer c_Animation_Sprite = o_Animation.GetComponent<SpriteRenderer>();
        o_Animation.SetActive(true);
        c_Animation_Animator.speed = 0;
        c_Animation_Sprite.color = Color.clear;
        yield return null;

        // -------------------- ! START ANIMATION ! -------------------- //

        // 6th, Enter the full view mode
        b_IsFullView = true;
        // Local Variables 
        float f_Time = 0f;
        // 7th, Fade in both Panel and Animation's sprite
        // Note: This will be a 1 second loop and also it acts like Update()
        while (f_Time <= 1)
        {
            c_Panel_Completion_Sprite.color = new Color(0.89f, 0.89f, 0.89f, f_Time);
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
        GameObject o_Panel_Black = Instantiate(prefab_Panel, t_AnimationParent);
        SpriteRenderer c_Panel_Black_Sprite = o_Panel_Black.GetComponent<SpriteRenderer>();
        o_Panel_Black.name = "Panel_Black";
        c_Panel_Black_Sprite.sortingLayerName = "Completion";
        c_Panel_Black_Sprite.sortingOrder = 2;
        c_Panel_Black_Sprite.color = Color.clear;
        // 11th, Create the last full-screen panel to simulate the screenshot effect
        // Note: [0] Panel_Completion; [1] Animation; [2] Panel_Black; [3] Card; [4] Panel_Screenshot 
        // Note: Currently, Panel_Screenshot's sprite alpha is zero
        GameObject o_Panel_Screenshot = Instantiate(prefab_Panel, t_AnimationParent);
        SpriteRenderer c_Panel_Screenshot_Sprite = o_Panel_Screenshot.GetComponent<SpriteRenderer>();
        o_Panel_Screenshot.name = "Panel_Screenshot";
        c_Panel_Screenshot_Sprite.sortingLayerName = "Completion";
        c_Panel_Screenshot_Sprite.sortingOrder = 4;
        c_Panel_Screenshot_Sprite.color = Color.clear;

        // -------------------- ! START SCREENSHOT ! -------------------- //

        // 12th, Pause the animation in the middle
        c_Animation_Animator.speed = 0;
        // 13th, Make both panels appear and stay for a while
        c_Panel_Screenshot_Sprite.color = Color.white;
        yield return null;
        c_Panel_Black_Sprite.color = new Color(0, 0, 0, 0.6f);
        yield return new WaitForSeconds(1);
        // 14th, Prepare the card object
        GameObject o_Card = t_CardParent.GetChild(0).gameObject;
        SpriteRenderer c_Card_Sprite = o_Card.GetComponent<SpriteRenderer>();
        o_Card.SetActive(true);
        c_Card_Sprite.color = Color.clear;
        yield return null;
        // 15th, Fade out Panel_Screenshot's sprite but fade in Card's sprite
        // Note: This will be a 2 seconds loop and also it acts like Update()
        // Note: Have to adjust the delta time calculation to slow down the timer 
        f_Time = 0f;
        while (f_Time <= 1)
        {
            c_Panel_Screenshot_Sprite.color = new Color(1, 1, 1, 1f - f_Time);
            c_Card_Sprite.color = new Color(1, 1, 1, 0.4f + f_Time);
            // Calculate the time spent in seconds
            f_Time += Time.deltaTime / 2f;
            // Wait for the next frame before looping over
            yield return null;
        }
        // 16th, Wait for a while before continuing the animation
        yield return new WaitForSeconds(4);

        // -------------------- ! CONTINUE ANIMATION ! -------------------- //

        // 17th, Fade out both Panel_Black and Card's sprite, also slowly resume animation
        // Note: This will be a 1.5 seconds loop and also it acts like Update()
        // Note: Have to adjust the delta time calculation to slow down the timer 
        f_Time = 0f;
        while (f_Time <= 1)
        {
            c_Card_Sprite.color = new Color(1, 1, 1, 1f - f_Time);
            c_Panel_Black_Sprite.color = new Color(0, 0, 0, 0.6f - f_Time);
            c_Animation_Animator.speed = f_Time;
            // Calculate the time spent in seconds
            f_Time += Time.deltaTime / 1.5f;
            // Wait for the next frame before looping over
            yield return null;

        }
        // Note: Make sure that the animation's speed is the exact value of 1
        c_Animation_Animator.speed = 1;

        // -------------------- ! END ANIMATION ! -------------------- //

        // Go to the next scene, transitioning




    }



}
