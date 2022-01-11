using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Revealer : MonoBehaviour
{
    // Generic Variables
    SpriteMask c_SpriteMask;
    SpriteRenderer c_SpriteRenderer;



    void Awake()
    {
        c_SpriteMask = GetComponent<SpriteMask>();
        c_SpriteRenderer = GetComponent<SpriteRenderer>();
    }



    void Start()
    {
        // To make sure everything is in the default state
        c_SpriteMask.sprite = null;
        c_SpriteRenderer.enabled = false;
    }



    void OnEnable()
    {
        // Reset the sprite of Sprite Mask to None when enabled
        c_SpriteMask.sprite = null;

    }



    void LateUpdate()
    {
        if (c_SpriteMask.sprite != c_SpriteRenderer.sprite)
        {
            c_SpriteMask.sprite = c_SpriteRenderer.sprite;
        }
    }



    // ------------------------------------------------------------ //



    // Used in [S_Level]
    public void SetTarget(int i_Target)
    {
        c_SpriteMask.frontSortingOrder = i_Target;
        c_SpriteMask.backSortingOrder = i_Target - 1;
    }





}
