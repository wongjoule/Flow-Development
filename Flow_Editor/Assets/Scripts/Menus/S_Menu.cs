using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// S_Menu will contain all functions related to S_Controller, S_Camera 

public class S_Menu : MonoBehaviour
{
    // General Variables
    static string MenuType;
    float f_OpeningInterval;
    Camera c_Camera;
    S_SubMenu s_SubMenu;

    // Variables - Panels
    Transform t_PanelRoot;
    Transform t_Panel_A, t_Panel_B, t_Panel_C;
    TextMeshPro tmp_PanelIndex_A, tmp_PanelIndex_B, tmp_PanelIndex_C;

    // Variables - Panel Bars
    Transform t_Panel_Line;
    TextMeshPro tmp_Panel_Stories;
    TextMeshPro tmp_Panel_Arts;
    TextMeshPro tmp_Panel_Events;

    // Variables - Level Title Bars
    TextMeshPro tmp_Level_Index;
    TextMeshPro tmp_Level_Name;

    // Variables - Star Bars
    int[] i_StarBars;
    Transform t_StarBarsParent;
    SpriteRenderer[] c_Stars = new SpriteRenderer[3];

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
    GameObject prefab_FullPanel; // A full-screen panel



    void Awake()
    {
        // Priorities
        s_SubMenu = GameObject.Find("SUBMENUS").GetComponent<S_SubMenu>();
        s_SubMenu.Initialize("Menu");
        // General Variables
        c_Camera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
        f_OpeningInterval = Time.time + .5f;
        // Variables - Level Title Bars
        if (GameObject.Find("LEVELTITLEBARS") != null)
        {
            tmp_Level_Index = GameObject.Find("Text_LevelTitle_Level").GetComponent<TextMeshPro>();
            tmp_Level_Name = GameObject.Find("Text_LevelTitle_LevelName").GetComponent<TextMeshPro>();
        }
        // Variables - Star Bars
        if (GameObject.Find("STARBARS") != null)
        {
            t_StarBarsParent = GameObject.Find("STARBARS").transform;
            for (int i = 0; i < 3; i++)
                c_Stars[i] = t_StarBarsParent.GetChild(i).GetComponent<SpriteRenderer>();
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
        Initialize_AspectRatio();
        Initialize_Panels();
        Initialize_Levels();
        StartCoroutine(Coroutine_SceneTransition("Opening", null));
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
                    Snap(1, "Vertical");
                else if (touch.deltaPosition.y < -70)
                    Snap(-1, "Vertical");
                else if (touch.deltaPosition.x > 40)
                    Snap(-1, "Horizontal");
                else if (touch.deltaPosition.x < -40)
                    Snap(1, "Horizontal");
            }
        }

        // -------------------- ! EDITOR INPUT ! -------------------- //

        if (Application.isEditor)
        {
            if (Time.time > f_SnapInterval)
            {
                if (Input.GetKeyDown(KeyCode.UpArrow))
                    Snap(-1, "Vertical");
                else if (Input.GetKeyDown(KeyCode.DownArrow))
                    Snap(1, "Vertical");
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                    Snap(1, "Horizontal");
                else if (Input.GetKeyDown(KeyCode.LeftArrow))
                    Snap(-1, "Horizontal");
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
                    // Then, Change the Index Letter
                    tmp_PanelIndex_A.text = IndexLetter(i_PanelIndex_Vertical_A) + ".";
                    // Lastly, Calculate the target values
                    i_Target_A = (i_PanelIndex_Vertical_A * 20) - 20;
                    break;
                case 2:
                    i_PanelIndex_Vertical_B += value;
                    // Then, Limit all Snap Index within the available content
                    i_PanelIndex_Vertical_B = Mathf.Clamp(i_PanelIndex_Vertical_B, 1, t_Panel_B.childCount);
                    // Then, Change the Index Letter
                    tmp_PanelIndex_B.text = IndexLetter(i_PanelIndex_Vertical_B) + ".";
                    // Lastly, Calculate the target values
                    i_Target_B = (i_PanelIndex_Vertical_B * 20) - 20;
                    break;
                case 3:
                    i_PanelIndex_Vertical_C += value;
                    // Then, Limit all Snap Index within the available content
                    i_PanelIndex_Vertical_C = Mathf.Clamp(i_PanelIndex_Vertical_C, 1, t_Panel_C.childCount);
                    // Then, Change the Index Letter
                    tmp_PanelIndex_C.text = IndexLetter(i_PanelIndex_Vertical_C) + ".";
                    // Lastly, Calculate the target values
                    i_Target_C = (i_PanelIndex_Vertical_C * 20) - 20;
                    break;
            }
            // Variables - Level Title Bars
            if (tmp_Level_Index != null)
                tmp_Level_Index.text = "Level " + i_PanelIndex_Vertical_A + "  -";
            // Variables - Star Bars
            if (t_StarBarsParent != null)
                for (int i = 1; i <= 3; i++)
                    if (i_StarBars[i_PanelIndex_Vertical_A - 1] >= i)
                        c_Stars[i - 1].color = Color.white;

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
                            f_SnapInterval = Time.time + .5f;
                            StartCoroutine(Coroutine_SceneTransition("Ending", hit.name));
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
                    f_SnapInterval = Time.time + .5f;
                    StartCoroutine(Coroutine_SceneTransition("Ending", hit.name));
                    S_Audio.Play("SFX_Test");
                }
            }
        }
    }



    // ------------------------------------------------------------ //



    bool IsIntervalFinished()
    {
        if (Time.time > f_OpeningInterval && Time.time > f_SnapInterval)
            return true;
        else
            return false;
    }



    bool IsThreePanels()
    {
        if (t_PanelRoot.childCount == 6)
            return true;
        else
            return false;
    }



    // ------------------------------------------------------------ //



    IEnumerator Coroutine_SceneTransition(string Phase, string Button)
    {
        if (Button != "Button_SubMenu" && Button != "Button_Panel_Stories" && Button != "Button_Panel_Arts" && Button != "Button_Panel_Events")
        {
            // Create a brand new full-screen panel
            GameObject o_FullPanel = Instantiate(prefab_FullPanel);
            SpriteRenderer c_FullPanel_Sprite = o_FullPanel.GetComponent<SpriteRenderer>();
            c_FullPanel_Sprite.sortingLayerName = "Transition";
            // Local Variables
            float f_Factor = 0f;
            // Fade out that full-screen panel's sprite
            // Note: This will be a 0.5 second loop and also it acts like Update()
            while (f_Factor < 1)
            {
                switch (Phase)
                {
                    case "Opening":
                        c_FullPanel_Sprite.color = new Color(.96f, .96f, .96f, Mathf.SmoothStep(1, 0, f_Factor));
                        break;
                    case "Ending":
                        c_FullPanel_Sprite.color = new Color(.96f, .96f, .96f, Mathf.SmoothStep(0, 1, f_Factor));
                        break;
                }
                // Wait for the next frame before looping over
                f_Factor += Time.deltaTime * 2f;
                yield return null;
            }
        }
        PressButton(Button);
    }



    // ------------------------------------------------------------ //



    string IndexLetter(int value)
    {
        switch (value)
        {
            case 1: return "i";
            case 2: return "ii";
            case 3: return "iii";
            case 4: return "iv";
            case 5: return "v";
            case 6: return "vi";
            case 7: return "vii";
            case 8: return "viii";
            case 9: return "ix";
            case 10: return "x";
            default: return "-";
        }
    }



    void Initialize_Levels()
    {
        if (MenuType == "LEVEL" || MenuType == "CARD")
        {
            string SceneName = SceneManager.GetActiveScene().name;
            i_StarBars = S_Data.Instance.GetScore(SceneName);
        }
    }



    void Initialize_Panels()
    {
        // Try to locate the gameobject named PANELS
        if (GameObject.Find("PANELS") != null)
        {
            // Assign its transform component to a variable
            t_PanelRoot = GameObject.Find("PANELS").transform;
            // Note: If gameobject PANELS is detected, it will always have at least one panel
            t_Panel_A = t_PanelRoot.GetChild(0);
            tmp_PanelIndex_A = GameObject.Find("Text_PanelIndex_A").GetComponent<TextMeshPro>();
            // Get the offset value of panel A in the editor
            float f_Offset_A = t_Panel_A.GetChild(0).localPosition.y;
            // Set the spacing for each element in this panel
            for (int i = 0; i < t_Panel_A.childCount; i++)
                t_Panel_A.GetChild(i).localPosition = new Vector3(0, (i * -20) + f_Offset_A, 0);
            // Only access to the other two panels if they are detected
            if (IsThreePanels())
            {
                // Assign the other two values to the local variables
                t_Panel_B = t_PanelRoot.GetChild(1);
                t_Panel_C = t_PanelRoot.GetChild(2);
                tmp_PanelIndex_B = GameObject.Find("Text_PanelIndex_B").GetComponent<TextMeshPro>();
                tmp_PanelIndex_C = GameObject.Find("Text_PanelIndex_C").GetComponent<TextMeshPro>();
                // Get the offset value of panel B and C in the editor
                float f_Offset_B = t_Panel_B.GetChild(0).localPosition.y;
                float f_Offset_C = t_Panel_C.GetChild(0).localPosition.y;
                // Set the spacing of each element in each panel
                for (int i = 0; i < t_Panel_B.childCount; i++)
                    t_Panel_B.GetChild(i).localPosition = new Vector3(0, (i * -20) + f_Offset_B, 0);
                for (int i = 0; i < t_Panel_C.childCount; i++)
                    t_Panel_C.GetChild(i).localPosition = new Vector3(0, (i * -20) + f_Offset_C, 0);
                // Set the panel line's position so it can be moved when the scene is loaded
                t_Panel_Line.position = new Vector3(-5f, -8.7f, 0);
            }
            // Note: This boolean allows to control both vertical and horizontal types
            // Note: However, the horizontal movement will be clamped if only one panel is detected
            b_SnapController = true;
        }
        else
            b_SnapController = false;
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



    // ------------------------------------------------------------ //



    public static void BackButton()
    {
        // Note: Home Page has no return button, no need to specify the scene type
        switch (MenuType)
        {
            case "CATEGORY":
                SceneManager.LoadScene("MENU_HOME");
                break;
            case "LEVEL_ROOT":
            case "CARD_ROOT":
            case "ARTIST_ROOT":
                MenuType = "CATEGORY";
                SceneManager.LoadScene("MENU_CATEGORY");
                break;
            case "LEVEL":
                MenuType = "LEVEL_ROOT";
                SceneManager.LoadScene("MENU_LEVELS");
                break;
            case "CARD":
                MenuType = "CARD_ROOT";
                SceneManager.LoadScene("MENU_CARDS");
                break;
            case "ARTIST":
                MenuType = "ARTIST_ROOT";
                SceneManager.LoadScene("MENU_ARTISTS");
                break;
            case "LEVEL_CH1": // The playable levels
                MenuType = "LEVEL";
                S_SceneSelector.Levels(1, 1, 0, 0);
                break;
        }
    }



    void PressButton(string Button)
    {
        // Note: When defining the scene type, just reference the line above
        switch (Button)
        {
            case "Button_Back":
                S_Menu.BackButton();
                break;
            case "Button_SubMenu":
                s_SubMenu.Open();
                break;
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

            // -------------------- | SCENES | -------------------- //

            case "Button_Explore":
                MenuType = "CATEGORY";
                SceneManager.LoadScene("MENU_CATEGORY");
                break;
            case "Button_Category_Stories":
                MenuType = "LEVEL_ROOT";
                SceneManager.LoadScene("MENU_LEVELS");
                break;
            case "Button_Category_Cards":
                MenuType = "CARD_ROOT";
                SceneManager.LoadScene("MENU_CARDS");
                break;
            case "Button_Category_Artists":
                MenuType = "ARTIST_ROOT";
                SceneManager.LoadScene("MENU_ARTISTS");
                break;



            case "Button_Levels":
                MenuType = "LEVEL";
                S_SceneSelector.Levels(i_PanelIndex_Horizontal, i_PanelIndex_Vertical_A,
                i_PanelIndex_Vertical_B, i_PanelIndex_Vertical_C);
                break;
            case "Button_Levels_Stories_CH1": // Maybe can have a loop to iterate through Chapters one by one
                MenuType = "LEVEL_CH1";
                S_SceneSelector.Levels_Stories(1, i_PanelIndex_Vertical_A);
                break;



            case "Button_Cards":

                break;



            case "Button_Artists":
                MenuType = "ARTIST";
                S_SceneSelector.Artists(i_PanelIndex_Vertical_A);
                break;


        }
    }
}
