using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    [SerializeField] private PlayToggleButton playButton;
    [SerializeField] private Slider slider;
    [SerializeField] private Text headerText;
    [SerializeField] private Text endText;

    public IObservable<Unit> OnPlayButtonPressedAsObservable => playButton.OnClickAsObservable;

    private void Awake()
    {
        SetAsPlayVisual();
    }
    
    public void SetAsPauseVisual()
    {
        playButton.SetAsPauseVisual();
    }

    public void SetAsPlayVisual()
    {
        playButton.SetAsPlayVisual();
    }

    public void Initialize(double endTimeMillisec)
    {
        var sec = endTimeMillisec / 1000d;
        var min = (int) (sec / 60);
        sec = sec - (min * 60);

        endText.text = $"{min:D2}:{(int)sec:D2}";
        
        slider.value = 0;
    }
    
    public void SetHeader(double headerMillisec, double endTimeMillisec)
    {
        slider.value = (float)(headerMillisec / endTimeMillisec);
        
        var sec = headerMillisec * 0.001d;
        var min = (int) (sec / 60);
        sec = sec - (min * 60);

        headerText.text = $"{min:D2}:{(int)sec:D2};{(int)headerMillisec % 1000:D3}";
    }
    
}
