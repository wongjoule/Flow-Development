using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Audio : MonoBehaviour
{
    // General Variables
    static AudioSource c_SourceBGM;
    static AudioSource c_SourceSFX;

    // Variables - BGMs
    static AudioClip BGM_Test;

    // Variables - SFXs
    static AudioClip SFX_Test;



    void Awake()
    {
        // General Variables
        // Note: Two AudioSource components are added when the game is launched
        c_SourceBGM = gameObject.AddComponent<AudioSource>();
        c_SourceSFX = gameObject.AddComponent<AudioSource>();
        // Variables - BGMs
        BGM_Test = Resources.Load<AudioClip>("Audios/BGM_Test");
        // Variables - SFXs
        SFX_Test = Resources.Load<AudioClip>("Audios/SFX_Test");
    }



    void Start()
    {
        // Note: To correctly setup the added AudioSource components
        // Audio Source - BGM
        c_SourceBGM.playOnAwake = false;
        c_SourceBGM.loop = true;
        // Audio Source - SFX
        c_SourceSFX.playOnAwake = false;
        // Lastly, To start playing the BGM when the game is launched
        S_Audio.Play("BGM_Test");
    }



    // ------------------------------------------------------------ //



    // Used in [S_SubMenu]
    public static void BGMVolume(float f_Volume)
    {
        c_SourceBGM.volume = f_Volume;
    }



    // Used in [S_SubMenu]
    public static void SFXVolume(float f_Volume)
    {
        c_SourceSFX.volume = f_Volume;
    }



    // Used in 
    public static void Play(string text_Clip)
    {
        switch (text_Clip)
        {
            // Variables - BGMs
            // Note: Make sure the audio clip slot is inserted before playback
            case "BGM_Test":
                c_SourceBGM.clip = BGM_Test;
                c_SourceBGM.Play();
                break;

            // Variables - SFXs
            case "SFX_Test":
                c_SourceSFX.PlayOneShot(SFX_Test);
                break;
        }
    }




}
