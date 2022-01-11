using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Tunnel : MonoBehaviour
{
    // Generic Variables
    Collider2D c_BorderCollider;
    GameObject o_TunnelChild;



    void Start()
    {
        Initialize_Phase_1_AssignCollider();
        Initialize_Phase_2_PrepareBlocker();
    }



    // ------------------------------------------------------------ //



    void Initialize_Phase_1_AssignCollider()
    {
        // 1st, Get all the overlapped colliders regardless of its type
        Collider2D[] hits = Physics2D.OverlapPointAll(transform.position);
        // 2nd, Check if it is an border's inner collider
        if (hits != null)
        {
            foreach (Collider2D hit in hits)
            {
                if (hit.tag == "Border")
                {
                    if (hit.name == "Border_Collider_Interior")
                    {
                        c_BorderCollider = hit;
                    }
                }
            }
        }
        // Lastly, Print the output
        if (c_BorderCollider != null)
        {
            S_DebugLog.LevelLog(gameObject.name +
            " has successfully registered an overlapping Border's interior collider.", "");
        }
        else
        {
            S_DebugLog.ErrorLog(gameObject.name +
            " cannot register any overlapping colliders. Please try to relocate its position.");
        }
    }



    void Initialize_Phase_2_PrepareBlocker()
    {
        // 1st, Find and assign it into a global variable
        o_TunnelChild = transform.GetChild(0).gameObject;
        // 2nd, Reset to its default state
        o_TunnelChild.SetActive(false);
    }



    // ------------------------------------------------------------ //



    // Used in [S_Player]
    public void Enter()
    {
        // Disable the overlapped border's inner collider
        c_BorderCollider.enabled = false;
        // Activate the tunnel blocker object
        o_TunnelChild.SetActive(true);
    }



    // Used in [S_Player]
    public void Exit()
    {
        // Recover the overlapped border's inner collider
        c_BorderCollider.enabled = true;
        // Deactivate the tunnel blocker object
        o_TunnelChild.SetActive(false);
    }
}
