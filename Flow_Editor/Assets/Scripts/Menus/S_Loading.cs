using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class S_Loading : MonoBehaviour
{
    // Variables - Tips
    TextMeshPro tmp_Tips;

    // Variables - Resources Path
    GameObject prefab_FullPanel; // A full-screen panel




    void Awake()
    {
        // Variables - Tips
        tmp_Tips = GameObject.Find("Text_Tips").GetComponent<TextMeshPro>();
        // Variables - Resources Path
        prefab_FullPanel = Resources.Load<GameObject>("Prefabs/FullPanel");
    }



    void Start()
    {
        StartCoroutine(Coroutine_SceneTransition());
        Tips();
    }



    // ------------------------------------------------------------ //



    void Tips()
    {
        // 1st, Have a list of available tips
        string[] messages = new string[]
        {
            // Black Inks
            "Beware of the Black Inks! They can swallow you and gets bigger.",
            // Boosters
            "Boosters, are the powerups given to the blob to defend themselves against the Black Inks."
        };
        // Lastly, Pass the value and display it on the screen
        tmp_Tips.text = "Tips: " + messages[Random.Range(0, messages.Length)];
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
        float f_Factor = 0f;
        // Fade out that full-screen panel's sprite
        // Note: This will be a 1 second loop and also it acts like Update()
        while (f_Factor < 1)
        {
            // Slowly reduce the alpha value every frame
            c_FullPanel_Sprite.color = new Color(.96f, .96f, .96f, Mathf.SmoothStep(1, 0, f_Factor));
            // Wait for the next frame before looping over
            f_Factor += Time.deltaTime;
            yield return null;
        }
    }



}
