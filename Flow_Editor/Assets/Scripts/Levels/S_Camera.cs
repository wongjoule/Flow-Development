using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Camera : MonoBehaviour
{
    // Generic Variables
    Camera c_Camera;
    Transform t_Player;
    S_Level s_Level;

    // Variables - Full Viewing Area
    float f_PlayerEye = 1.5f;
    float f_FullEye = 12.0f;
    float f_MoveDuration = 0.5f;
    float f_SizeDuration = 0.5f;

    // Variables - SmoothDamp Calculations
    Vector3 v_MoveVelocity = Vector3.zero;
    float f_SizeVelocity = 0f;



    void Awake()
    {
        c_Camera = GetComponent<Camera>();
        t_Player = GameObject.FindWithTag("Player").transform;
        s_Level = GameObject.Find("LEVELSYSTEM").GetComponent<S_Level>();
    }



    void Start()
    {
        Initialize_AspectRatio();
    }



    // Note: Used in Respawn System. This will cache the newly instantiated player object
    void OnEnable()
    {
        t_Player = GameObject.FindWithTag("Player").transform;
    }



    void Update()
    {
        if (s_Level.b_IsFullView)
        {
            // Move to the center of the world
            transform.position = Vector3.SmoothDamp(transform.position, Vector3.zero, ref v_MoveVelocity, f_MoveDuration);
            // Increase to reach the full orthographic size
            c_Camera.orthographicSize = Mathf.SmoothDamp(c_Camera.orthographicSize, f_FullEye, ref f_SizeVelocity, f_SizeDuration);
        }
        else
        {
            // Move to the player current position
            transform.position = Vector3.SmoothDamp(transform.position, t_Player.position, ref v_MoveVelocity, f_MoveDuration);
            // Decrease to reach the player orthographic size
            c_Camera.orthographicSize = Mathf.SmoothDamp(c_Camera.orthographicSize, f_PlayerEye * S_Booster.mod_PlayerEye, ref f_SizeVelocity, f_SizeDuration);
        }

    }



    // ------------------------------------------------------------ //



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


}
