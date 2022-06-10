using UnityEngine;

public class LoadingUI : MonoBehaviour
{

    [SerializeField] private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup.interactable = false;
    }

    public void Display()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }

}
