using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_BlackInk : MonoBehaviour
{
    // General Variables
    [HideInInspector] public InkType Type;
    [HideInInspector] public bool b_IsAppeared = false;
    Animator c_Animator;
    S_Level s_Level;

    // Variables - Absorb
    float f_InkScale;

    // Variables - Moving Ink
    Vector3 v_Lateral_Target = Vector3.zero;



    void Awake()
    {
        c_Animator = GetComponent<Animator>();
        s_Level = GameObject.FindWithTag("Level").GetComponent<S_Level>();
    }



    void Start()
    {
        f_InkScale = transform.localScale.x;
    }



    void Update()
    {
        float f_Scale = Mathf.SmoothStep(transform.localScale.x, f_InkScale * S_Booster.mod_InkSize, .1f);
        transform.localScale = new Vector3(f_Scale, f_Scale, f_Scale);
    }



    // ------------------------------------------------------------ //



    // Used in [S_Player]
    public void Absorb()
    {
        f_InkScale += .025f;
    }



    // Used in [S_Level]
    public void Appear()
    {
        // Appear when encountering black ink for the first time
        if (!b_IsAppeared)
        {
            // Firstly, Set the boolean to 'true'
            b_IsAppeared = true;
            // Secondly, Make it appear visually
            c_Animator.SetTrigger("Appear");
            // Thirdly, Include this Ink's Sprite Renderer into list_ViewMode
            s_Level.list_ViewModeSprite.Add(GetComponent<SpriteRenderer>());
            // Lastly, Activate specific coroutines based on its defined types
            // Note: Here is the place for Black Ink's initialization and activation
            switch (Type)
            {
                case InkType.Idle:
                    StartCoroutine(Activate_Idle_Ink());
                    break;

                case InkType.Spreading:
                    StartCoroutine(Activate_Spreading_Ink());
                    break;

                case InkType.Fading:
                    StartCoroutine(Activate_Fading_Ink());
                    break;

                case InkType.Moving:
                    StartCoroutine(Activate_Moving_Ink());
                    break;
            }
        }
    }



    // ------------------------------------------------------------ //



    IEnumerator Activate_Idle_Ink()
    {
        // Reserved for the future updates
        yield return null;
    }



    IEnumerator Activate_Spreading_Ink()
    {
        // Get the data from S_Level and store them into local variables
        Vector2Int i_CloneCount = s_Level.i_CloneCount;

        // Local Variables
        // Note: Need to include the maximum clone count value, so add 1
        int i_RandomClone = Random.Range(i_CloneCount.x, i_CloneCount.y + 1);
        List<Vector3> list_ClonePosition = new List<Vector3>();
        List<Vector3> list_CloneVelocity = new List<Vector3>();
        List<Transform> list_CloneTransform = new List<Transform>();
        GameObject prefab_BlackInk = s_Level.prefab_BlackInk;
        Vector3 v_ScaleTarget = new Vector3(0.05f, 0.05f, 0.05f);

        // Firstly, Create a new game object to record a random target position
        GameObject o_Empty = new GameObject("Empty Anchor Object");
        yield return null;

        // Secondly, Move o_Empty to the outer boundary of the area within the radius
        o_Empty.transform.position = transform.position;
        yield return null;
        o_Empty.transform.Translate(0.4f, 0, 0);
        yield return null;

        // Thirdly, Create target positions for clones and set up the list_Velocity
        for (int i = 0; i < i_RandomClone; i++)
        {
            o_Empty.transform.RotateAround(transform.position, Vector3.forward, (i * 120) + Random.Range(0, 60));
            yield return null;
            list_ClonePosition.Add(o_Empty.transform.position);
            list_CloneVelocity.Add(Vector3.zero);
            yield return null;
        }

        // Fourthly, Add an extra layer of variation to the defined positions
        for (int i = 0; i < list_ClonePosition.Count; i++)
        {
            Vector3 v_Variation = list_ClonePosition[i];

            v_Variation = new Vector3(v_Variation.x + Random.Range(-0.2f, 0.2f),
                v_Variation.y + Random.Range(-0.2f, 0.2f), 0);

            list_ClonePosition[i] = v_Variation;
        }

        // Fifthly, Instantiate new clones based on the given i_RandomClone value
        for (int i = 0; i < i_RandomClone; i++)
        {
            GameObject o_Clone = Instantiate(prefab_BlackInk, transform.position, Quaternion.identity, transform.parent);
            yield return null;
            // Configurations of newly made clone ink object
            // Note: The name index will start at one
            o_Clone.name = "BlackInk_Clone_" + (i + 1);
            o_Clone.transform.localScale = v_ScaleTarget;
            // Add to the list
            list_CloneTransform.Add(o_Clone.transform);
            o_Clone.GetComponent<Animator>().SetTrigger("Appear");
            // Also include all clone's Sprite Renderer into list_ViewMode
            s_Level.list_ViewModeSprite.Add(o_Clone.GetComponent<SpriteRenderer>());
            yield return null;
        }

        // Sixthly, Clean up and destroy that empty anchor object
        Destroy(o_Empty);
        yield return new WaitForSeconds(5);

        // Local Variables
        float f_TimeSpent = 0.0f;
        Vector3 v_ScaleVelocity = Vector3.zero;

        // Finally, move all clones to their target location while shrinking in size
        // Note: This will be a 7 seconds loop and also it acts like Update()
        while (f_TimeSpent <= 7)
        {
            for (int i = 0; i < i_RandomClone; i++)
            {
                // Temporary Variables
                Transform t_Clone = list_CloneTransform[i];
                Vector3 v_CloneVelocity = list_CloneVelocity[i];
                // Smooth Damp
                t_Clone.position = Vector3.SmoothDamp(t_Clone.position, list_ClonePosition[i], ref v_CloneVelocity, 0.4f);
                // Note: 'ref' The returned value must be assigned back to the list
                list_CloneVelocity[i] = v_CloneVelocity;
            }

            transform.localScale = Vector3.SmoothDamp(transform.localScale, v_ScaleTarget, ref v_ScaleVelocity, 0.1f);
            // Calculate the time
            f_TimeSpent += Time.deltaTime;
            // Wait for the next frame before looping over
            yield return null;
        }
    }



    IEnumerator Activate_Fading_Ink()
    {
        // Get the data from S_Level and store them into local variables
        Vector2Int i_HoldTime = s_Level.i_HoldTime;
        List<Vector3> list_TeleportSlot = s_Level.list_InkTeleportSlot;

        // Create local variables for later uses
        int i_RandomTime = 0;
        int i_RandomIndex = 0;

        // Start and permanently loop the Fading Ink function
        while (true)
        {
            // Firstly, Generate a random index and a random hold time
            i_RandomTime = Random.Range(i_HoldTime.x, i_HoldTime.y);
            i_RandomIndex = Random.Range(0, list_TeleportSlot.Count);

            // Print out the log
            S_DebugLog.Log($"{gameObject.name}'s current Hold Time = ", i_RandomTime);

            // Secondly, Hold until it reaches the randomly generated time
            yield return new WaitForSeconds(i_RandomTime);

            // Thirdly, Make it disappear visually
            c_Animator.SetTrigger("Disappear");
            yield return new WaitForSeconds(3);

            // Fourthly, Teleport to a random target location specified in teleport slot
            transform.position = list_TeleportSlot[i_RandomIndex];
            yield return new WaitForSeconds(2);

            // Lastly, Make it re-appear visually
            c_Animator.SetTrigger("Appear");
        }
    }



    IEnumerator Activate_Moving_Ink()
    {
        // Generate a random number with value either 0 or 1
        int i_Subtype = Random.Range(0, 2);

        // Local variables
        GameObject o_Empty;

        // Activate this Moving Ink functions based on its defined subtype
        switch (i_Subtype)
        {
            // -------------------- ! LATERAL ! -------------------- //
            case 1:
                // Print this Moving's Subtype info out
                S_DebugLog.LevelLog($"{gameObject.name}'s Subtype is = ", "Lateral");

                // Firstly, Create a new game object to record a random pivot position
                o_Empty = new GameObject("Empty Anchor Object");
                yield return null;

                // Secondly, Move o_Empty to the outer boundary of the area within the radius
                // Note: You can define the radius at Translate() function
                o_Empty.transform.position = transform.position;
                yield return null;
                o_Empty.transform.Translate(1, 0, 0);
                yield return null;

                // Thirdly, Create the target position A at random angle
                o_Empty.transform.RotateAround(transform.position, Vector3.forward, Random.Range(0, 360));
                yield return null;
                Vector3 v_Target_A = o_Empty.transform.position;
                yield return null;

                // Fourthly, Create the target position B in the opposite direction of A
                o_Empty.transform.RotateAround(transform.position, Vector3.forward, 180);
                yield return null;
                Vector3 v_Target_B = o_Empty.transform.position;
                yield return null;

                // Fifthly, Clean up and destroy that empty anchor object
                Destroy(o_Empty);
                yield return new WaitForSeconds(5);

                // Sixthly, Start the coroutine to change current target for Lateral subtype
                StartCoroutine(Lateral_ChangeTarget(v_Target_A, v_Target_B));

                // Seventhly, Create a local variable for SmoothDamp calculations
                Vector3 v_Velocity = Vector3.zero;

                // Lastly, Do a lateral motion that change target every 10 seconds
                // Note: This will be a never-ending loop and also it acts like Update()
                while (true)
                {
                    transform.position = Vector3.SmoothDamp(transform.position, v_Lateral_Target, ref v_Velocity, 3 / S_Booster.mod_InkSpeed);
                    // Wait for the next frame before looping over
                    yield return null;
                }


            // -------------------- ! CIRCULAR ! -------------------- //
            case 0:
                // Print this Moving's Subtype info out
                S_DebugLog.LevelLog($"{gameObject.name}'s Subtype is = ", "Circular");

                // Firstly, Create a new game object to record a random pivot position
                o_Empty = new GameObject("Empty Anchor Object");
                yield return null;

                // Secondly, Move o_Empty to the outer boundary of the area within the radius
                // Note: You can define the radius at Translate() function
                o_Empty.transform.position = transform.position;
                yield return null;
                o_Empty.transform.Translate(1, 0, 0);
                yield return null;

                // Thirdly, Create a pivot for circular motion at random angle 
                o_Empty.transform.RotateAround(transform.position, Vector3.forward, Random.Range(0, 360));
                yield return null;
                Vector3 v_RandomPivot = o_Empty.transform.position;
                yield return null;

                // Fourthly, Clean up and destroy that empty anchor object
                Destroy(o_Empty);
                yield return new WaitForSeconds(5);

                // Lastly, Do a circular motion at a speed of 20 degrees per second
                // Note: This will be a never-ending loop and also it acts like Update()
                while (true)
                {
                    transform.RotateAround(v_RandomPivot, Vector3.forward, 20 * Time.deltaTime * S_Booster.mod_InkSpeed);
                    // Wait for the next frame before looping over
                    yield return null;
                }
        }
    }



    // ------------------------------------------------------------ //



    IEnumerator Lateral_ChangeTarget(Vector3 v_Target_A, Vector3 v_Target_B)
    {
        while (true)
        {
            v_Lateral_Target = v_Target_A;
            yield return new WaitForSeconds(10);

            v_Lateral_Target = v_Target_B;
            yield return new WaitForSeconds(10);
        }
    }
}
