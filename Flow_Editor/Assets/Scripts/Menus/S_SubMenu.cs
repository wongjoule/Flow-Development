using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class S_SubMenu : MonoBehaviour
{
    // General Variables
    public static bool b_SubMenu = false;
    Camera c_Camera;

    // Variables - SubMenu Panels
    Transform t_Panel_A;
    Transform t_Panel_B;
    Transform t_Panel_C;

    // Variables - SubMenu Bars
    Transform t_Panel_Line;
    TextMeshPro tmp_Panel_Store;
    TextMeshPro tmp_Panel_Settings;
    TextMeshPro tmp_Panel_Credits;
    float f_Target_Line = -1.5f; // Default Value

    // Variables - Activations
    float f_ActivationInterval;
    Collider2D c_BackButton;
    float f_Closed = 10f; // CLosed. This value is specifically used for Menu scene
    float f_Opened = 1.2f; // Opened. This value is specifically used for Menu scene

    // Variables - Full Screen Panels
    SpriteRenderer c_FullPanel_Black_Sprite; // Full Screen Black Panel
    float f_FullPanel_Alpha = 0f;

    // Variables - Button Detections
    float f_TapInterval;
    Vector2 v_BeganTouch;
    Vector2 v_EndedTouch;

    // Variables - Settings
    bool b_SlidingBGM = false;
    bool b_SlidingSFX = false;
    Transform t_Slider_BGM;
    Transform t_Slider_SFX;
    Transform t_Toggle_Mechanic;
    float f_Target_Toggle = 1.67f; // Default to Joystick
    GameObject o_Joystick;


    float Temp_MouseX;



    void Awake()
    {
        // Priorities
        Initialize_Singleton();
        // Variables - SubMenu Panels
        t_Panel_A = GameObject.Find("Panel_Stores").transform;
        t_Panel_B = GameObject.Find("Panel_Settings").transform;
        t_Panel_C = GameObject.Find("Panel_Credits").transform;
        // Variables - SubMenu Bars
        t_Panel_Line = GameObject.Find("IMG_Panel_SubMenuLine").transform;
        tmp_Panel_Store = GameObject.Find("Button_SubMenu_Store").GetComponent<TextMeshPro>();
        tmp_Panel_Settings = GameObject.Find("Button_SubMenu_Settings").GetComponent<TextMeshPro>();
        tmp_Panel_Credits = GameObject.Find("Button_SubMenu_Credits").GetComponent<TextMeshPro>();
        // Variables - Activations
        c_FullPanel_Black_Sprite = GameObject.Find("FullPanel_SubMenu").GetComponent<SpriteRenderer>();
        c_BackButton = GameObject.Find("Button_SubMenu_Back").GetComponent<Collider2D>();
        // Variables - Settings
        t_Slider_BGM = GameObject.Find("Slider_BGM").transform;
        t_Slider_SFX = GameObject.Find("Slider_SFX").transform;
        t_Toggle_Mechanic = GameObject.Find("Toggle_Mechanic").transform;
    }



    void Start()
    {
        Initialize();
    }



    void Update()
    {
        Update_SubMenuPanels();
        Update_BlackScreenPanels();
        Update_Toggles();

        if (b_SubMenu)
        {
            // Related to all controls - Buttons, Sliders
            Controller_Touches();
        }

    }



    // ------------------------------------------------------------ //



    void Update_SubMenuPanels()
    {
        // Activation and Deactivation
        if (b_SubMenu)
        {
            transform.position = new Vector3(Mathf.SmoothStep(transform.position.x, f_Opened, .25f), 0, 0);
            t_Panel_Line.localPosition = new Vector3(Mathf.SmoothStep(t_Panel_Line.localPosition.x, f_Target_Line, .2f), 7.5f, 0);
        }
        else
        {
            transform.position = new Vector3(Mathf.SmoothStep(transform.position.x, f_Closed, .25f), 0, 0);
        }
    }



    void Update_BlackScreenPanels()
    {
        c_FullPanel_Black_Sprite.color = new Color(0, 0, 0,
        Mathf.SmoothStep(c_FullPanel_Black_Sprite.color.a, f_FullPanel_Alpha, .17f));
    }



    void Update_Toggles()
    {
        t_Toggle_Mechanic.localPosition = new Vector3(Mathf.SmoothStep(t_Toggle_Mechanic.localPosition.x, f_Target_Toggle, .2f), -1.18f, -2);
    }



    void Controller_Touches()
    {
        // -------------------- ! MOBILE INPUT ! -------------------- //

        if (Input.touchCount == 1)
        {
            // Get the data of the first finger's touch input
            Touch touch = Input.GetTouch(0);
            // Note: A dedicated tap detector so it won't conflict with others
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    v_BeganTouch = touch.position;
                    // Set the interval time limit for button detection
                    f_TapInterval = Time.time + 0.66f;
                    // Note: Slider Pre Functions 
                    // Convert the user input from screen-space to world-space
                    Vector3 v_InputScreen = new Vector3(touch.position.x, touch.position.y);
                    Vector3 v_InputWorld = c_Camera.ScreenToWorldPoint(v_InputScreen);
                    // Return a nullable collider if it overlapped with the user input
                    Collider2D hit = Physics2D.OverlapPoint(new Vector2(v_InputWorld.x, v_InputWorld.y));
                    // Lastly, Check if a collider is hit
                    if (hit != null)
                        PressSlider(hit.name);
                    break;

                case TouchPhase.Moved:
                    // Note: Slider Move Functions
                    float f_InputX = (touch.position.x - touch.deltaPosition.x) * .01f;
                    MoveSlider(f_InputX);
                    break;

                case TouchPhase.Ended:
                    v_EndedTouch = touch.position;
                    // Note: Slider End Functions
                    PressSlider(null);
                    // Note: The distance value have to be squared
                    // Note: Button Tapping is only allowed to be executed within the time interval 
                    if ((v_EndedTouch - v_BeganTouch).sqrMagnitude < 150 &&
                    Time.time < f_TapInterval && IsIntervalFinished())
                    {
                        // Note: Button Functions
                        // Convert the user input from screen-space to world-space
                        v_InputScreen = new Vector3(touch.position.x, touch.position.y);
                        v_InputWorld = c_Camera.ScreenToWorldPoint(v_InputScreen);
                        // Return a nullable collider if it overlapped with the user input
                        hit = Physics2D.OverlapPoint(new Vector2(v_InputWorld.x, v_InputWorld.y));
                        // Lastly, Check if a collider is hit
                        if (hit != null)
                        {
                            PressButton(hit);

                            S_Audio.Play("SFX_Test");
                        }
                    }
                    break;
            }
        }

        // -------------------- ! EDITOR INPUT ! -------------------- //

        if (Application.isEditor)
        {
            if (Input.GetMouseButtonDown(0) && IsIntervalFinished())
            {
                Vector3 v_InputWorld = Vector3.zero;
                v_InputWorld = c_Camera.ScreenToWorldPoint(Input.mousePosition);
                // Return a nullable collider if it overlapped with the user input
                Collider2D hit = Physics2D.OverlapPoint(new Vector2(v_InputWorld.x, v_InputWorld.y));
                // Lastly, Check if a collider is hit
                if (hit != null)
                {
                    PressButton(hit);
                    PressSlider(hit.name);
                    Temp_MouseX = Input.mousePosition.x;

                    S_Audio.Play("SFX_Test");
                }
            }
            else if (Input.GetMouseButton(0))
            {
                float f_InputX = (Input.mousePosition.x - Temp_MouseX) * .01f;
                MoveSlider(f_InputX);
                Temp_MouseX = Input.mousePosition.x;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                b_SlidingBGM = false;
                b_SlidingSFX = false;
            }
        }
    }



    void Initialize_Singleton()
    {
        Application.targetFrameRate = 60; // Temp
        DontDestroyOnLoad(gameObject);
    }



    void Initialize()
    {
        c_BackButton.enabled = false;
        t_Panel_B.gameObject.SetActive(false);
        t_Panel_C.gameObject.SetActive(false);
    }



    // ------------------------------------------------------------ //



    bool IsIntervalFinished()
    {
        if (Time.time > f_ActivationInterval)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    // ------------------------------------------------------------ //



    // Used in [S_Menu], [S_Level]
    public void Initialize(string mode)
    {
        // Find and assign the main camera each time the scene is loaded
        // Note: MainCamera is mainly used for the button tapping detection
        c_Camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        // Change the position and scale
        if (mode == "Menu")
        {
            // Set the default transformation value. Inactivated SubMenu
            transform.position = new Vector3(10f, 0, 0);
            transform.localScale = Vector3.one;
            // Set the target positions for smoothsteps
            f_Closed = 10f;
            f_Opened = 1.2f;
        }
        else if (mode == "Level")
        {
            // Set the default transformation value. Inactivated SubMenu
            transform.position = new Vector3(12.2f, 0, 0);
            transform.localScale = new Vector3(1.263f, 1.263f, 1.263f);
            // Set the target positions for smoothsteps
            f_Closed = 12.2f;
            f_Opened = 1.5f;
            // Locate the Joystick object and set its active state based on b_Gyro 
            o_Joystick = GameObject.Find("JOYSTICKS");
            if (S_Controller.b_Gyro)
                o_Joystick.SetActive(false);
            else
                o_Joystick.SetActive(true);
        }
    }



    // Used in [S_Menu]
    public void Open()
    {
        b_SubMenu = true;
        f_ActivationInterval = Time.time + .5f;
        c_BackButton.enabled = true;
        f_FullPanel_Alpha = .33f;
    }



    // Not used
    public void Close()
    {
        b_SubMenu = false;
        f_ActivationInterval = Time.time + .5f;
        c_BackButton.enabled = false;
        f_FullPanel_Alpha = 0f;
    }



    void PressButton(Collider2D hit)
    {
        switch (hit.name)
        {
            case "Button_SubMenu_Back":
                Close();
                break;
            case "Button_SubMenu_Store":
                DeactivateAll();
                tmp_Panel_Store.color = new Color(0, 0, 0, 1);
                t_Panel_A.gameObject.SetActive(true);
                f_Target_Line = -1.5f;
                break;
            case "Button_SubMenu_Settings":
                DeactivateAll();
                tmp_Panel_Settings.color = new Color(0, 0, 0, 1);
                t_Panel_B.gameObject.SetActive(true);
                f_Target_Line = 1.25f;
                break;
            case "Button_SubMenu_Credits":
                DeactivateAll();
                tmp_Panel_Credits.color = new Color(0, 0, 0, 1);
                t_Panel_C.gameObject.SetActive(true);
                f_Target_Line = 4f;
                break;

            // -------------------- ! Settings ! -------------------- //

            case "Toggle_Mechanic":
                if (S_Controller.b_Gyro)
                {
                    f_Target_Toggle = 1.67f;
                    S_Controller.b_Gyro = false;
                    // Activate Joystick visuals
                    if (o_Joystick != null)
                        o_Joystick.SetActive(true);
                }
                else
                {
                    f_Target_Toggle = 0.525f;
                    S_Controller.b_Gyro = true;
                    // Deactivate Joystick visuals
                    if (o_Joystick != null)
                        o_Joystick.SetActive(false);
                }
                break;
        }
    }



    void PressSlider(string name)
    {
        switch (name)
        {
            // -------------------- ! Settings ! -------------------- //

            case "Slider_BGM":
                b_SlidingBGM = true;
                break;
            case "Slider_SFX":
                b_SlidingSFX = true;
                break;
            case null:
                b_SlidingBGM = false;
                b_SlidingSFX = false;
                break;
        }
    }



    void MoveSlider(float f_InputX)
    {
        // Local Variables
        float f_PosX = 0f;
        // SubMenu - Settings
        if (b_SlidingBGM)
        {
            f_PosX = t_Slider_BGM.localPosition.x + f_InputX;
            f_PosX = Mathf.Clamp(f_PosX, 0.645f, 3.52f);
            t_Slider_BGM.localPosition = new Vector3(f_PosX, 2.73f, -2);
            // Actual Functions
            float f_Volume = (f_PosX - 0.645f) / 2.875f;
            S_Audio.BGMVolume(f_Volume);
        }
        else if (b_SlidingSFX)
        {
            f_PosX = t_Slider_SFX.localPosition.x + f_InputX;
            f_PosX = Mathf.Clamp(f_PosX, 0.645f, 3.52f);
            t_Slider_SFX.localPosition = new Vector3(f_PosX, 1.85f, -2);
            // Actual Functions
            float f_Volume = (f_PosX - 0.645f) / 2.875f;
            S_Audio.SFXVolume(f_Volume);
        }
    }



    void DeactivateAll()
    {
        // Panel Title Alpha
        tmp_Panel_Store.color = new Color(0, 0, 0, .25f);
        tmp_Panel_Settings.color = new Color(0, 0, 0, .25f);
        tmp_Panel_Credits.color = new Color(0, 0, 0, .25f);
        // Panel Visibility
        t_Panel_A.gameObject.SetActive(false);
        t_Panel_B.gameObject.SetActive(false);
        t_Panel_C.gameObject.SetActive(false);
    }

}
