using System.Threading;
using UnityEngine;


// Thread safe
public class Logger : SingletonMonoBehaviour<Logger>
{

    [SerializeField] private StatusTextUI statusTextUI;

    private SynchronizationContext synchronizationContext;

    private void Awake()
    {
        synchronizationContext = SynchronizationContext.Current;
    }

    public static void Log(string message)
    {
        Instance.synchronizationContext.Post( _ =>
        {
            Instance.statusTextUI.Log(message);
        }, null);
    }

    public static void Error(string message)
    {
        Instance.synchronizationContext.Post( _ =>
        {
            Instance.statusTextUI.Error(message);
        }, null);
    }
    

}
