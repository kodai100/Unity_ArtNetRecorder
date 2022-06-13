using UnityEngine;


// Thread safe
public class Logger : SingletonMonoBehaviour<Logger>
{

    [SerializeField] private StatusTextUI statusTextUI;

    public static void Log(string message)
    {
        Instance.statusTextUI.Log(message);
    }

    public static void Error(string message)
    {
        Instance.statusTextUI.Error(message);
    }
    

}
