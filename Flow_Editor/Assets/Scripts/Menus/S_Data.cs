using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Data : MonoBehaviour
{
    // Note: All modifiers should act like a multiplication factor (Store)



    void Awake()
    {
        // Priorities
        Initialize_Singleton();
    }



    // ------------------------------------------------------------ //



    void Initialize_Singleton()
    {
        DontDestroyOnLoad(gameObject);

    }
}
