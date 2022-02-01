// S_Menu will contain all functions related to S_Controller, S_Camera 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class S_Menu : MonoBehaviour
{
    // General Variables
    static string text_SceneType;
    float f_LoadSceneInterval;
    Camera c_Camera;
    S_SubMenu s_SubMenu;

    // Variables - LevelTitle Bars
    TextMeshPro tmp_Level_Index;
    TextMeshPro tmp_Level_Name;

    // Variables - Panels
    Transform t_PanelRoot;
    Transform t_Panel_A;
    Transform t_Panel_B;
    Transform t_Panel_C;

    // Variables - Panel Bars
    Transform t_Panel_Line;
    TextMeshPro tmp_Panel_Stories;
    TextMeshPro tmp_Panel_Arts;
    TextMeshPro tmp_Panel_Events;

    // Variables - Snap Control
    bool b_SnapController = false;
    float f_SnapInterval = 0; // Temporary hold more time when a new scene is loaded
    int i_PanelIndex_Horizontal = 1; // Only used for two panel groups and above
    int i_PanelIndex_Vertical_A = 1;
    int i_PanelIndex_Vertical_B = 1;
    int i_PanelIndex_Vertical_C = 1;
    int i_Target_Horizontal; // For Horizontal Panel
    int i_Target_A; // For Vertical Panel A
    int i_Target_B; // For Vertical Panel B
    int i_Target_C; // For Vertical Panel C
    float f_Target_Line = -3.5f; // Default Value, For Panel Line Indicator

    // Variables - Button Detections
    float f_TapInterval;
    Vector2 v_BeganTouch;
    Vector2 v_EndedTouch;

    // Variables - Resources Path
    [HideInInspector] public GameObject prefab_FullPanel; // A full-screen panel



    void Awake()
    {
        // Priorities
        s_SubMenu = GameObject.Find("SUBMENUS").GetComponent<S_SubMenu>();
        s_SubMenu.Initialize("Menu");
        // General Variables
        c_Camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        f_LoadSceneInterval = Time.time + .5f;
        // Variables - SubTitle Bars
        if (GameObject.Find("LEVELTITLEBARS") != null)
        {
            tmp_Level_Index = GameObject.Find("Text_LevelTitle_Level").GetComponent<TextMeshPro>();
            tmp_Level_Name = GameObject.Find("Text_LevelTitle_LevelName").GetComponent<TextMeshPro>();
        }
        // Variables - Panel Bars
        // Note: Panel_Line variable will only be used when the scene got three panels
        if (GameObject.Find("Button_Panel_Stories") != null)
        {
            tmp_Panel_Stories = GameObject.Find("Button_Panel_Stories").GetComponent<TextMeshPro>();
            tmp_Panel_Arts = GameObject.Find("Button_Panel_Arts").GetComponent<TextMeshPro>();
            tmp_Panel_Events = GameObject.Find("Button_Panel_Events").GetComponent<TextMeshPro>();
            t_Panel_Line = GameObject.Find("IMG_Panel_Line").transform;
        }
        // Variables - Resources Path
        prefab_FullPanel = Resources.Load<GameObject>("Prefabs/FullPanel");
    }



    void Start()
    {
        Initialize_Panel();
        Initialize_AspectRatio();
        StartCoroutine(Coroutine_SceneTransition());
    }



    void Update()
    {
        if (!S_SubMenu.b_SubMenu)
        {
            Controller_Button();
            // Note: If Snap Control is true, Scroll Control have to be disabled 
            if (b_SnapController)
            {
                Controller_Snap();
                Update_Panels();
            }
        }
    }



    // ------------------------------------------------------------ //



    void Update_Panels()
    {
        // Move Panel_A smoothly
        t_Panel_A.localPosition = new Vector3(0, Mathf.SmoothStep(t_Panel_A.localPosition.y, i_Target_A, .2f), 0);
        // Move Horizontal, Panel_B and Panel_C if they are exist
        if (IsThreePanels())
        {
            // Vertical Panels
            t_Panel_B.localPosition = new Vector3(15, Mathf.SmoothStep(t_Panel_B.localPosition.y, i_Target_B, .2f), 0);
            t_Panel_C.localPosition = new Vector3(30, Mathf.SmoothStep(t_Panel_C.localPosition.y, i_Target_C, .2f), 0);
            // Horizontal Panels
            t_PanelRoot.position = new Vector3(Mathf.SmoothStep(t_PanelRoot.position.x, i_Target_Horizontal, .2f), 0, 0);
            t_Panel_Line.position = new Vector3(Mathf.SmoothStep(t_Panel_Line.position.x, f_Target_Line, .2f), -8.7f, 0);
        }
    }



    void Controller_Snap()
    {
        // -------------------- ! MOBILE INPUT ! -------------------- //

        if (Input.touchCount == 1)
        {
            // Get the data of the first finger's touch input
            Touch touch = Input.GetTouch(0);
            // Detect the scroll gesture in only y-axis
            if (Time.time > f_SnapInterval)
            {
                if (touch.deltaPosition.y > 70)
                {
                    Snap(1, "Vertical");
                }
                else if (touch.deltaPosition.y < -70)
                {
                    Snap(-1, "Vertical");
                }
                else if (touch.deltaPosition.x > 40)
                {
                    Snap(-1, "Horizontal");
                }
                else if (touch.deltaPosition.x < -40)
                {
                    Snap(1, "Horizontal");
                }
            }
        }

        // -------------------- ! EDITOR INPUT ! -------------------- //

        if (Application.isEditor)
        {
            if (Time.time > f_SnapInterval)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                {
                    Snap(-1, "Vertical");
                }
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                {
                    Snap(1, "Vertical");
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    Snap(1, "Horizontal");
                }
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    Snap(-1, "Horizontal");
                }
            }
        }
    }



    void Snap(int value, string type)
    {
        // Set the interval time limit after each scroll snapping
        f_SnapInterval = Time.time + .5f;
        // Increment or decrement the value based on the user's scroll gesture 
        if (type == "Vertical")
        {
            switch (i_PanelIndex_Horizontal)
            {
                case 1:
                    i_PanelIndex_Vertical_A += value;
                    // Then, Limit all Snap Index within the available content
                    i_PanelIndex_Vertical_A = Mathf.Clamp(i_PanelIndex_Vertical_A, 1, t_Panel_A.childCount);
                    // Lastly, Calculate the target values
                    i_Target_A = (i_PanelIndex_Vertical_A * 20) - 20;
                    break;

                case 2:
                    i_PanelIndex_Vertical_B += value;
                    // Then, Limit all Snap Index within the available content
                    i_PanelIndex_Vertical_B = Mathf.Clamp(i_PanelIndex_Vertical_B, 1, t_Panel_B.childCount);
                    // Lastly, Calculate the target values
                    i_Target_B = (i_PanelIndex_Vertical_B * 20) - 20;
                    break;

                case 3:
                    i_PanelIndex_Vertical_C += value;
                    // Then, Limit all Snap Index within the available content
                    i_PanelIndex_Vertical_C = Mathf.Clamp(i_PanelIndex_Vertical_C, 1, t_Panel_C.childCount);
                    // Lastly, Calculate the target values
                    i_Target_C = (i_PanelIndex_Vertical_C * 20) - 20;
                    break;
            }
            // Change the level's title only in the chapter's level selection
            if (tmp_Level_Index != null)
            {
                tmp_Level_Index.text = "Level " + i_PanelIndex_Vertical_A + "  -";
            }
        }
        else if (type == "Horizontal")
        {
            if (IsThreePanels())
            {
                i_PanelIndex_Horizontal += value;
                // Then, Limit all Snap Index within the available content
                i_PanelIndex_Horizontal = Mathf.Clamp(i_PanelIndex_Horizontal, 1, 3);
                // Then, Calculate the target values
                i_Target_Horizontal = (i_PanelIndex_Horizontal * -15) + 15;
                f_Target_Line = (i_PanelIndex_Horizontal * 3.5f) - 7f;
                // Then, Set all texts in the panel bar to the inactive color
                tmp_Panel_Stories.color = new Color(0, 0, 0, .25f);
                tmp_Panel_Arts.color = new Color(0, 0, 0, .25f);
                tmp_Panel_Events.color = new Color(0, 0, 0, .25f);
                // Lastly, Set one of the texts to be fully opaque when selected
                switch (i_PanelIndex_Horizontal)
                {
                    case 1:
                        tmp_Panel_Stories.color = new Color(0, 0, 0, 1);
                        break;
                    case 2:
                        tmp_Panel_Arts.color = new Color(0, 0, 0, 1);
                        break;
                    case 3:
                        tmp_Panel_Events.color = new Color(0, 0, 0, 1);
                        break;
                }
            }
        }
    }



    void Controller_Button()
    {
        // -------------------- ! MOBILE INPUT ! -------------------- //

        if (Input.touchCount == 1)
        {
            Vector3 v_InputWorld = Vector3.zero;
            // Get the data of the first finger's touch input
            Touch touch = Input.GetTouch(0);
            // Note: A dedicated tap detector so it won't conflict with others
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    v_BeganTouch = touch.position;
                    // Set the interval time limit for button detection
                    f_TapInterval = Time.time + 0.66f;
                    break;

                case TouchPhase.Ended:
                    v_EndedTouch = touch.position;
                    // Note: The distance value have to be squared
                    // Note: Button Tapping is only allowed to be executed within the time interval 
                    if ((v_EndedTouch - v_BeganTouch).sqrMagnitude < 150 &&
                    Time.time < f_TapInterval && IsIntervalFinished())
                    {
                        // Convert the user input from screen-space to world-space
                        Vector3 v_InputScreen = new Vector3(touch.position.x, touch.position.y);
                        v_InputWorld = c_Camera.ScreenToWorldPoint(v_InputScreen);
                        // Return a nullable collider if it overlapped with the user input
                        Collider2D hit = Physics2D.OverlapPoint(new Vector2(v_InputWorld.x, v_InputWorld.y));
                        // Lastly, Check if a collider is hit
                        if (hit != null)
                        {
                            PressButton(hit);
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
                }
            }
        }
    }



    // ------------------------------------------------------------ //



    bool IsIntervalFinished()
    {
        if (Time.time > f_LoadSceneInterval &&
        Time.time > f_SnapInterval)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    bool IsThreePanels()
    {
        if (t_PanelRoot.childCount == 3)
        {
            return true;
        }
        else
        {
            return false;
        }
    }



    // ------------------------------------------------------------ //



    IEnumerator Coroutine_SceneTransition()
    {
        // Create a full-screen panel when loading a new scene
        GameObject o_FullPanel = Instantiate(prefab_FullPanel);
        SpriteRenderer c_FullPanel_Sprite = o_FullPanel.GetComponent<SpriteRenderer>();
        c_FullPanel_Sprite.sortingLayerName = "Transition";
        yield return null;
        // Local Variables
        float f_Alpha = 1f;
        // Fade out that full-screen panel's sprite
        // Note: This will be a 1 second loop and also it acts like Update()
        while (f_Alpha > 0)
        {
            c_FullPanel_Sprite.color = new Color(1, 1, 1, f_Alpha);
            // Slowly reduce the alpha value every frame
            f_Alpha -= Time.deltaTime;
            // Wait for the next frame before looping over
            yield return null;
        }
        // Lastly, Destroy that useless full-screen panel
        Destroy(o_FullPanel);
    }



    void Initialize_Panel()
    {
        // Try to locate the gameobject named PANELS
        if (GameObject.Find("PANELS") != null)
        {
            // Assign its transform component to a variable
            t_PanelRoot = GameObject.Find("PANELS").transform;
            // Note: If gameobject PANELS is detected, it will always have at least one panel
            t_Panel_A = t_PanelRoot.GetChild(0);
            // Get the offset value of panel A in the editor
            float f_Offset_A = t_Panel_A.GetChild(0).localPosition.y;
            // Set the spacing for each element in this panel
            for (int i = 0; i < t_Panel_A.childCount; i++)
            {
                t_Panel_A.GetChild(i).localPosition = new Vector3(0, (i * -20) + f_Offset_A, 0);
            }
            // Only access to the other two panels if they are detected
            if (IsThreePanels())
            {
                // Assign the other two values to the local variables
                t_Panel_B = t_PanelRoot.GetChild(1);
                t_Panel_C = t_PanelRoot.GetChild(2);
                // Get the offset value of panel B and C in the editor
                float f_Offset_B = t_Panel_B.GetChild(0).localPosition.y;
                float f_Offset_C = t_Panel_C.GetChild(0).localPosition.y;
                // Set the spacing of each element in each panel
                for (int i = 0; i < t_Panel_B.childCount; i++)
                {
                    t_Panel_B.GetChild(i).localPosition = new Vector3(0, (i * -20) + f_Offset_B, 0);
                }
                for (int i = 0; i < t_Panel_C.childCount; i++)
                {
                    t_Panel_C.GetChild(i).localPosition = new Vector3(0, (i * -20) + f_Offset_C, 0);
                }
                // Set the panel line's position so it can be moved when the scene is loaded
                t_Panel_Line.position = new Vector3(-5f, -8.7f, 0);
            }
            // Note: This boolean allows to control both vertical and horizontal types
            // Note: However, the horizontal movement will be clamped if only one panel is detected
            b_SnapController = true;
        }
        else
        {
            b_SnapController = false;
        }
    }



    void Initialize_AspectRatio()
    {
        // Calculate the target screen aspect ratio (0.5625:1)
        float f_Target = 9f / 16f;
        // Get the user device's screen aspect ratio
        float f_User = (float)Screen.width / (float)Screen.height;
        // Get the difference between the user ratio and the target ratio
        // Note: If the value = 1, the user's screen matched with the target's screen
        // Note: If the value < 1, it means that the user's screen is larger than the target
        // Note: (If the user’s screen aspect ratio is 2:1) 0.5 / (Target) 0.5625 = 0.8889
        float f_Offset = f_User / f_Target;
        // Adjust the camera rect to match with the user's screen aspect ratio
        if (f_Offset != 1)
        {
            // Local Variables
            Rect rect = c_Camera.rect;
            // Inner calculations
            // Note: 'y' represents the new position of the rect
            // Note: 'height' represents the scale of the rect on the y-axis
            rect.y = (1f - f_Offset) / 2f;
            rect.height = f_Offset;
            // Apply it back to the camera
            c_Camera.rect = rect;
        }
    }



    public static void BackButton()
    {
        // Note: 'case' records what the current scene is
        // Note: You have to think about what the previous scene was
        // Note: Take note that you have to specify the scene type as well
        switch (text_SceneType)
        {
            case "CATEGORY":
                // Home Page has no return button, no need to specify the scene type
                SceneManager.LoadScene("MENU_HOME");
                break;

            case "LEVEL_ROOT":
            case "CARD_ROOT":
            case "ARTIST_ROOT":
                SceneManager.LoadScene("MENU_CATEGORY");
                text_SceneType = "CATEGORY";
                break;

            case "LEVEL_CHAPTER":
                SceneManager.LoadScene("MENU_LEVELS");
                text_SceneType = "LEVEL_ROOT";
                break;

            case "ARTIST_PROFILE":
                SceneManager.LoadScene("MENU_ARTISTS");
                text_SceneType = "ARTIST_ROOT";
                break;

            case "GAMEPLAY_CH1":
                S_SceneSelector.Levels(1, 1, 0, 0);
                text_SceneType = "LEVEL_CHAPTER";
                break;
        }

    }



    void PressButton(Collider2D hit)
    {
        // Note: When defining the scene type, just reference the line above
        switch (hit.name)
        {
            // -------------------- ! ALL ! -------------------- //

            case "Button_Back":
                S_Menu.BackButton();
                break;

            case "Button_SubMenu":
                s_SubMenu.Open();
                break;

            // -------------------- ! PANELBARS ! -------------------- //

            case "Button_Panel_Stories":
                i_PanelIndex_Horizontal = 1;
                Snap(0, "Horizontal"); // Just to process the snap function
                break;

            case "Button_Panel_Arts":
                i_PanelIndex_Horizontal = 2;
                Snap(0, "Horizontal"); // Just to process the snap function
                break;

            case "Button_Panel_Events":
                i_PanelIndex_Horizontal = 3;
                Snap(0, "Horizontal"); // Just to process the snap function
                break;

            // -------------------- ! MENU_HOME ! -------------------- //

            case "Button_Explore":
                SceneManager.LoadScene("MENU_CATEGORY");
                text_SceneType = "CATEGORY";
                break;

            // -------------------- ! MENU_CATEGORY ! -------------------- //

            case "Button_Category_Stories":
                SceneManager.LoadScene("MENU_LEVELS");
                text_SceneType = "LEVEL_ROOT";
                break;

            case "Button_Category_Cards":
                SceneManager.LoadScene("MENU_CARDS");
                text_SceneType = "CARD_ROOT";
                break;

            case "Button_Category_Artists":
                SceneManager.LoadScene("MENU_ARTISTS");
                text_SceneType = "ARTIST_ROOT";
                break;

            // -------------------- ! MENU_LEVELS ! -------------------- //

            case "Button_Levels":
                S_SceneSelector.Levels(i_PanelIndex_Horizontal, i_PanelIndex_Vertical_A,
                i_PanelIndex_Vertical_B, i_PanelIndex_Vertical_C);
                text_SceneType = "LEVEL_CHAPTER";
                break;


            // -------------------- ! MENU_LEVELS_STORIES ! -------------------- //

            case "Button_Levels_Stories_CH1":
                S_SceneSelector.Levels_Stories(1, i_PanelIndex_Vertical_A);
                text_SceneType = "GAMEPLAY_CH1";
                break;

            // -------------------- ! MENU_CARDS ! -------------------- //

            case "Button_Cards":

                break;



            // -------------------- ! MENU_ARTISTS ! -------------------- //

            case "Button_Artists":
                S_SceneSelector.Artists(i_PanelIndex_Vertical_A);
                text_SceneType = "ARTIST_PROFILE";
                break;


        }
    }
}
