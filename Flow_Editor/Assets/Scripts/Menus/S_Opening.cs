using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class S_Opening : MonoBehaviour
{


    void Start()
    {
        StartCoroutine(Coroutine_Opening());
    }



    // ------------------------------------------------------------ //



    IEnumerator Coroutine_Opening()
    {
        yield return new WaitForSeconds(.5f);
        SceneManager.LoadScene("MENU_HOME");
    }
}
