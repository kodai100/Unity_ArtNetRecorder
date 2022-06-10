using UnityEngine;
using UnityEngine.UI;

public class StatusTextUI : MonoBehaviour
{
    [SerializeField] private Text text;


    public void Log(string message)
    {
        text.color = Color.white;
        text.text = message;
    }

    public void Error(string message)
    {
        text.color = Color.red;
        text.text = message;
    }
}
