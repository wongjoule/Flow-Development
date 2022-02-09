using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Player : MonoBehaviour
{
    // Generic Variables
    SpriteRenderer c_SpriteRenderer;
    Rigidbody2D c_Rigidbody;
    float f_Drag = 0;
    S_Level s_Level;



    void Awake()
    {
        c_SpriteRenderer = GetComponentInParent<SpriteRenderer>();
        c_Rigidbody = GetComponent<Rigidbody2D>();
        f_Drag = c_Rigidbody.drag;
        s_Level = GameObject.Find("LEVELSYSTEM").GetComponent<S_Level>();
    }



    void OnTriggerEnter2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "BlackInk":
                if (!S_Booster.mod_Invincible)
                {
                    // Black ink absorbs the player's blob and becomes larger
                    other.GetComponent<S_BlackInk>().Absorb();
                    // Disable colliders. Also slowly become black and fade out when being absorbed
                    StartCoroutine(Coroutine_Absorb(other.transform));
                    // Activate Respawn System
                    s_Level.Respawn();
                }
                break;


            case "Tunnel":
                // Disable the collider and pass through the border
                other.GetComponent<S_Tunnel>().Enter();
                break;


            case "Booster":
                // Activate the booster
                // Note: Will destroy the object when the coroutine completes
                other.GetComponent<S_Booster>().Activate();
                // Disable its collider component immediately
                other.enabled = false;
                break;



            case "Border":
                // Spawn Particle system when collided with map borders


                break;



            case "Part":
                // Get the sorting order of the collided masked part
                int i_SortingOrder = other.GetComponent<SpriteRenderer>().sortingOrder;
                // Start the revealing process
                s_Level.Reveal(i_SortingOrder, other.transform, transform.position);
                // Add progress and call the progression inner mechanism
                s_Level.Progress();
                // Disable its collider component immediately
                other.enabled = false;
                break;

        }
    }



    void OnTriggerExit2D(Collider2D other)
    {
        switch (other.tag)
        {
            case "Tunnel":
                // Recover the collider after reaching the other side of the tunnel
                other.GetComponent<S_Tunnel>().Exit();
                break;
        }

    }



    // ------------------------------------------------------------ //



    // Used in [S_Controller]
    public void Slow(int i_Input)
    {
        switch (i_Input)
        {
            case 0: // No Slow. Default speed
                c_Rigidbody.drag = f_Drag;
                break;
            case 1: // Extra Slow. Tap & Hold
                c_Rigidbody.drag = f_Drag * 20;
                break;
            case 2: // Super Slow. Time freeze
                c_Rigidbody.drag = f_Drag * 100;
                break;
        }
    }



    // Used in [S_Controller]
    public void Move(Vector2 v_Input)
    {
        c_Rigidbody.AddForce(v_Input);
    }



    IEnumerator Coroutine_Absorb(Transform t_Ink)
    {
        // Disable all colliders owned by the player
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            CircleCollider2D c_Collider = transform.parent.GetChild(i).GetComponent<CircleCollider2D>();
            c_Collider.enabled = false;
        }
        // Local Variables
        // Note: FixedUpdate() has a stable value of 50 frames per second
        int i_Frame = 0;
        float f_Color = 1f;
        float f_Alpha = 1f;
        // Slowly fade out and move to the center of the Black Ink
        // Note: This will be a 4 seconds loop and also it acts like FixedUpdate()
        while (i_Frame <= 200)
        {
            // Calculate the current vector direction to that Black Ink
            // Note: Direction = Destination - Source
            Vector3 v_Direction = t_Ink.position - transform.position;
            v_Direction *= 0.01f;
            // Move to the center of the Black Ink
            // Note: MovePosition moves to an absolute position, so we make it relative
            c_Rigidbody.MovePosition(transform.position + v_Direction);
            // Note: f_Color will take 67 fixed update frames, hence it is 1.34 secs
            f_Color -= 0.015f;
            // Note: f_Alpha will take 125 fixed update frames, hence it is 2.5 secs 
            if (i_Frame > 75)
            {
                f_Alpha -= 0.008f;
            }
            // To clamp both of the values within 0 and 1
            f_Color = Mathf.Clamp(f_Color, 0f, 1f);
            f_Alpha = Mathf.Clamp(f_Alpha, 0f, 1f);
            // Fade out and slowly become black
            c_SpriteRenderer.color = new Color(f_Color, f_Color, f_Color, f_Alpha);
            // Lastly, Increment by one per fixed update frame
            i_Frame += 1;
            yield return new WaitForFixedUpdate();
        }
    }



}
