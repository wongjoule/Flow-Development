using UnityEngine;

public class S_DebugLog : MonoBehaviour
{
    // Color Settings for all the log types
    static string text_Color_Log = "white";
    static string text_Color_LevelLog = "#b1dae7ff"; // 80% Light Blue
    static string text_Color_SuccessLog = "#33ff33ff"; // 60% Lime
    static string text_Color_WarningLog = "orange";
    static string text_Color_ErrorLog = "#ff3333ff"; // 60% Red
    static string text_Color_TestingLog = "magenta";



    // ------------------------------------------------------------ //



    // "Log" can be used for anything
    public static void Log(string description, object value)
    {
        Debug.Log($"<b><color={text_Color_Log}>   [LOG]   </color></b>" +
        $"<color={text_Color_Log}>{description}</color>" +
        $"<b><color={text_Color_Log}>{value}</color></b>");
    }



    // "Level Log" should be used only for level initialization data
    // Note: This log should be used for calling once, like in the Start() function
    public static void LevelLog(string description, object value)
    {
        Debug.Log($"<b><color={text_Color_LevelLog}>   [LEVELLOG]   </color></b>" +
        $"<color={text_Color_LevelLog}>{description}</color>" +
        $"<b><color={text_Color_LevelLog}>{value}</color></b>");
    }



    // "Success Log" should be used for functions if it did not fail
    public static void SuccessLog(string description, object value)
    {
        Debug.Log($"<b><color={text_Color_SuccessLog}>   [SUCCESSLOG]   </color></b>" +
        $"<color={text_Color_SuccessLog}>{description}</color>" +
        $"<b><color={text_Color_SuccessLog}>{value}</color></b>");
    }



    // "Warning Log" should be used for functions that requires attention
    // Note: Only description is accepted, and all text will be bold
    public static void WarningLog(string description)
    {
        Debug.Log($"<b><color={text_Color_WarningLog}>   [WARNINGLOG]   {description}</color></b>");
    }



    // "Error Log" should be used for functions that impacts the most when it fails
    // Note: Only description is accepted, and all text will be bold
    public static void ErrorLog(string description)
    {
        Debug.Log($"<b><color={text_Color_ErrorLog}>   [ERRORLOG]   {description}</color></b>");
    }



    // "Testing log" should be used for temporary testing
    // Note: You should completely avoid letting this log appear
    public static void TestingLog(string description, object value)
    {
        Debug.Log($"<b><color={text_Color_TestingLog}>   [TESTINGLOG]   </color></b>" +
        $"<color={text_Color_TestingLog}>{description}</color>" +
        $"<b><color={text_Color_TestingLog}>{value}</color></b>");
    }

}
