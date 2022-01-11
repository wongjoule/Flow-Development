using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Controller : MonoBehaviour
{
    // Generic Variables
    S_Player s_Player;
    Transform t_Camera;
    S_Camera s_Camera;
    Camera c_Camera;
    S_Level s_Level;

    // Variables - Move Player
    float f_SpeedModifier = 1f;
    Vector2 v_Movement = Vector2.zero;

    // Variables - Joystick
    Transform t_Joystick;
    Vector3 v_JoystickOffset = new Vector3(0, -1, 0); // Match the settings
    bool b_IsJoystickActivated = false;

    // Variables - Double Tap
    float f_TapInterval = 0f;
    Vector3 v_TapPosition = Vector3.zero;

    // Editor Variables - Keyboard Mapping
    KeyCode key_Player_Slow = KeyCode.LeftShift;
    KeyCode key_Level_FullView = KeyCode.LeftBracket;
    KeyCode key_Level_PlayerView = KeyCode.RightBracket;

    // Editor Variables - Cheat Mapping
    KeyCode key_Cheat_Completion = KeyCode.Tab;
    KeyCode key_Cheat_TurboSpeed = KeyCode.Alpha1; // [Shift] + [1]



    void Awake()
    {
        // Generic Variables
        s_Player = GameObject.FindWithTag("Player").GetComponent<S_Player>();
        t_Camera = GameObject.FindWithTag("MainCamera").transform;
        s_Camera = t_Camera.GetComponent<S_Camera>();
        c_Camera = t_Camera.GetComponent<Camera>();
        s_Level = GameObject.Find("LEVELSYSTEM").GetComponent<S_Level>();
        // Variables - Joystick
        t_Joystick = GameObject.FindWithTag("Joystick").transform;
    }



    // Note: Used in Respawn System. This will cache the newly instantiated player object
    void OnEnable()
    {
        s_Player = GameObject.FindWithTag("Player").GetComponent<S_Player>();

        // Temp
        v_Movement = Vector2.zero;
        t_Joystick.position = t_Camera.position + v_JoystickOffset;

    }



    void Update()
    {
        // Generic Functions
        Controller_Level_ViewMode();
        Controller_Menu_Button();
        // Functions that restricted to only the player view
        if (!s_Level.b_IsFullView)
        {
            Controller_Player_Slow();
            Controller_Skill_Splash();
        }
        // Editor-only Cheat Functions
        if (Application.isEditor)
        {
            Cheats();
        }
    }



    void FixedUpdate()
    {
        Controller_Player_Move();
    }



    void LateUpdate()
    {
        // Have a if-function here - gyro & joystick
        if (true)
        {
            Controller_Joystick_Control();
            // Return the joystick to its origin position when not in use
            if (!b_IsJoystickActivated)
            {
                Controller_Joystick_Origin();
            }
        }
    }



    // ------------------------------------------------------------ //



    void Controller_Player_Slow()
    {
        // -------------------- ! MOBILE INPUT ! -------------------- //

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    s_Player.Slow(1);
                    break;

                case TouchPhase.Ended:
                    s_Player.Slow(0);
                    break;
            }
        }

        // -------------------- ! EDITOR INPUT ! -------------------- //

        if (Application.isEditor)
        {
            // Hold down [LeftShift] to slow down the player
            if (Input.GetKey(key_Player_Slow))
            {
                s_Player.Slow(1);
            }
            else if (Input.GetKeyUp(key_Player_Slow))
            {
                s_Player.Slow(0);
            }
        }
    }



    void Controller_Skill_Splash()
    {
        // -------------------- ! MOBILE INPUT ! -------------------- //

        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            switch (touch.phase)
            {
                case TouchPhase.Ended:
                    // Splash skill is only allowed to be executed within the time interval
                    f_TapInterval = Time.time + 0.66f;
                    // Record the first touch position
                    v_TapPosition = touch.position;
                    break;

                case TouchPhase.Began:
                    // Check the interval time and whether both taps are in the same place
                    if (Time.time < f_TapInterval && Vector3.Distance(v_TapPosition, touch.position) < 30)
                    {
                        // Convert the user input from screen-space to world-space
                        Vector3 v_TapWorld = c_Camera.ScreenToWorldPoint(v_TapPosition);
                        // Execute the skill function - Splash
                        s_Level.Splash(v_TapWorld);
                    }
                    break;
            }
        }

        // -------------------- ! EDITOR INPUT ! -------------------- //

        if (Application.isEditor)
        {
            if (Input.GetMouseButtonUp(0))
            {
                // Splash skill is only allowed to be executed within the time interval
                f_TapInterval = Time.time + 0.66f;
                // Record the first mouse position
                v_TapPosition = Input.mousePosition;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                // Check the interval time and whether both taps are in the same place
                if (Time.time < f_TapInterval && Vector3.Distance(v_TapPosition, Input.mousePosition) < 30)
                {
                    // Convert the user input from screen-space to world-space
                    Vector3 v_TapWorld = c_Camera.ScreenToWorldPoint(v_TapPosition);
                    // Execute the skill function - Splash
                    s_Level.Splash(v_TapWorld);
                }
            }
        }
    }



    void Controller_Joystick_Origin()
    {
        // Calculate the joystick's origin point at the current frame
        Vector3 v_Origin = t_Camera.position + v_JoystickOffset;
        // Return the joystick to its origin position when not in use
        t_Joystick.position = v_Origin;

    }



    void Controller_Joystick_Control()
    {
        // -------------------- ! MOBILE INPUT ! -------------------- //

        // -------------------- ! EDITOR INPUT ! -------------------- //

        // if (Application.isEditor)
        if (true)
        {
            // Activate the joystick if the first tap is within the joystick's detection area
            if (Input.GetMouseButtonDown(0))
            {
                // 1st, Convert the user input from screen-space to world-space
                Vector3 v_InputWorld = c_Camera.ScreenToWorldPoint(Input.mousePosition);
                // 2nd, Calculate the joystick's origin point at the current frame
                Vector3 v_Origin = t_Camera.position + v_JoystickOffset;
                // 3rd, Check whether the user input is within the joystick's detection area
                if ((v_InputWorld - v_Origin).sqrMagnitude < 0.1f)
                {
                    b_IsJoystickActivated = true;
                }
            }
            // Start to move the player only when the boolean is true and held down
            else if (Input.GetMouseButton(0) && b_IsJoystickActivated)
            {
                // 1st, Convert the user input from screen-space to world-space
                Vector3 v_InputWorld = c_Camera.ScreenToWorldPoint(Input.mousePosition);
                // 2nd, Calculate the joystick's origin point at the current frame
                Vector3 v_Origin = t_Camera.position + v_JoystickOffset;
                // 3rd, Calculate the normalized vector of direction
                // Note: Vector3's normalized value has been square rooted
                Vector3 v_Direction = (v_InputWorld - v_Origin).normalized;
                // 4th, Apply to the player's movement, have to wait for FixedUpdate
                // Note: Don't add any speed boost value here since it will be done in the Move function
                v_Movement = v_Direction;
                // 5th, Check whether the user input is within the joystick's detection area
                if ((v_InputWorld - v_Origin).sqrMagnitude < 0.16f)
                {
                    t_Joystick.position = v_InputWorld;
                }
                // Lastly, If the touch exceeds, clamp the joystick's visual within that area
                else
                {
                    Vector3 v_Clamped = v_Origin + (v_Direction * 0.4f);
                    t_Joystick.position = v_Clamped;
                }
            }
            // Deactivate the joystick when the user's finger is lifted
            else if (Input.GetMouseButtonUp(0) && b_IsJoystickActivated)
            {
                b_IsJoystickActivated = false;
                // Reset to its default value
                v_Movement = Vector2.zero;
            }
        }
    }



    void Controller_Player_Move()
    {
        // -------------------- ! MOBILE INPUT ! -------------------- //

        // The Accelerometer mechanism will be used to move the player
        //  v_MobileInput = new Vector2(Input.acceleration.x, Input.acceleration.y);

        // -------------------- ! EDITOR INPUT ! -------------------- //

        if (Application.isEditor)
        {
            // Using [W], [A], [S], [D] to move the player
            // v_Movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        }

        // -------------------- ! LOCAL FUNCTIONS ! -------------------- //


        // Note: Default value is 4
        v_Movement *= 4f * f_SpeedModifier;
        // Use the FixedUpdate method of this script to move the player
        s_Player.Move(v_Movement);
    }



    void Controller_Level_ViewMode()
    {
        // -------------------- ! MOBILE INPUT ! -------------------- //

        if (Input.touchCount == 2)
        {
            if (PinchDistance() < -30)
            {
                // Set the boolean to true in S_Level
                // Note: This also ignore the 'Tap & Hold' function during the Full View
                // Note: This also change some of the sprite behavior in S_Level
                s_Level.b_IsFullView = true;
                // The player will be frozen at its current position
                s_Player.Slow(2);
            }
            else if (PinchDistance() > 30)
            {
                // Set the boolean to false in S_Level
                // Note: Restore the 'Tap & Hold' function during the Player View
                // Note: This also change some of the sprite behavior in S_Level
                s_Level.b_IsFullView = false;
                // Restore the player's movement speed
                s_Player.Slow(0);
            }
        }

        // -------------------- ! EDITOR INPUT ! -------------------- //

        if (Application.isEditor)
        {
            if (Input.GetKeyDown(key_Level_FullView))
            {
                // Set the boolean to true in S_Level
                // Note: This also ignore the 'Tap & Hold' function during the Full View
                // Note: This also toggle the viewing mode behavior in S_Level
                s_Level.b_IsFullView = true;
                // The player will be frozen at its current position
                s_Player.Slow(2);
            }
            else if (Input.GetKeyDown(key_Level_PlayerView))
            {
                // Set the boolean to false in S_Level
                // Note: Restore the 'Tap & Hold' function during the Player View
                // Note: This also toggle the viewing mode behavior in S_Level
                s_Level.b_IsFullView = false;
                // Restore the player's movement speed
                s_Player.Slow(0);
            }
        }
    }



    void Controller_Menu_Button()
    {
        // -------------------- ! MOBILE INPUT ! -------------------- //

        if (Input.touchCount == 1)
        {
            // Get the data of the first touch input
            Touch touch = Input.GetTouch(0);
            // Note: This is similar to 'Get Mouse Button Down'
            if (touch.phase == TouchPhase.Began)
            {
                // Firstly, Convert the user input from screen-space to world-space
                Vector3 v_TapScreen = new Vector3(touch.position.x, touch.position.y);
                Vector3 v_TapWorld = c_Camera.ScreenToWorldPoint(v_TapScreen);
                // Secondly, Return a collider if it overlapped with the user input
                Collider2D hit = Physics2D.OverlapPoint(new Vector2(v_TapWorld.x, v_TapWorld.y));
                // Thirdly, Check if it is a button or not if a collider is hit
                if (hit != null)
                {
                    // Lastly, Find and execute the specific button function
                    PressButton(hit);
                }
            }
        }

        // -------------------- ! EDITOR INPUT ! -------------------- //

        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // Get the data of the current mouse position
                Vector3 v_Mouse = Input.mousePosition;
                // Firstly, Convert the user input from screen-space to world-space
                Vector3 v_TapScreen = new Vector3(v_Mouse.x, v_Mouse.y);
                Vector3 v_TapWorld = c_Camera.ScreenToWorldPoint(v_TapScreen);
                // Secondly, Return a collider if it overlapped with the user input
                Collider2D hit = Physics2D.OverlapPoint(new Vector2(v_TapWorld.x, v_TapWorld.y));
                // Thirdly, Check if it is a button or not if a collider is hit
                if (hit != null)
                {
                    // Lastly, Find and execute the specific button function
                    PressButton(hit);
                }
            }
        }
    }



    // ------------------------------------------------------------ //



    void Cheats()
    {
        // ------------- ! Activate Turbo Speed ! ------------- //

        if (Input.GetKeyDown(KeyCode.LeftShift) && Input.GetKeyDown(key_Cheat_TurboSpeed))
        {
            f_SpeedModifier = 2.5f;
            // Print a warning log every time a cheat function has been executed
            S_DebugLog.WarningLog("Successfully Executed Cheat - Turbo Speed");
        }

        // ------------- ! Instant Level Completion ! ------------- //

        if (Input.GetKeyDown(key_Cheat_Completion))
        {
            // Get the total amount of Parts
            int i_Total = GameObject.Find("PARTS").transform.childCount;
            // Repeat adding progress until it reaches the total amount of Parts
            for (int i = 1; i < i_Total; i++)
            {
                s_Level.Progress();
            }
            // Print warning log to show that a cheat function has been executed
            S_DebugLog.WarningLog("Successfully Executed Cheat - Level Completion");
        }
    }



    // ------------------------------------------------------------ //



    void PressButton(Collider2D hit)
    {
        switch (hit.name)
        {
            case "Button_Hint":
                s_Level.Hint();
                break;

            case "Button_Back":


                break;

        }
    }



    float PinchDistance()
    {
        // Get the current frame's touches' position
        Touch touch_0 = Input.GetTouch(0);
        Touch touch_1 = Input.GetTouch(1);
        // Get the previous frame's touches' position
        Vector2 pretouch_0_position = touch_0.position - touch_0.deltaPosition;
        Vector2 pretouch_1_position = touch_1.position - touch_1.deltaPosition;
        // Calculate the current & previous distances, to get an offset value 
        float f_Current = Vector2.Distance(touch_0.position, touch_1.position);
        float f_Previous = Vector2.Distance(pretouch_0_position, pretouch_1_position);
        float f_Distance = f_Current - f_Previous;
        // Return either negative or positive value
        return f_Distance;
    }







}
