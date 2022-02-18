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

    private double endTimeMillisec; 
    
    public IObservable<Unit> OnPlayButtonPressedAsObservable => playButton.OnClickAsObservable;

    private void Awake()
    {
        SetAsPlayVisual();

        slider.OnValueChangedAsObservable().Subscribe(value =>
        {

            var headerMillisec = value * endTimeMillisec;
            
            var sec = headerMillisec * 0.001d;
            var min = (int) (sec / 60);
            sec = sec - (min * 60);

            headerText.text = $"{min:D2}:{(int)sec:D2};{(int)headerMillisec % 1000:D3}";
        }).AddTo(this);
    }
    
    public void SetAsPauseVisual()
    {
        playButton.SetAsPauseVisual();
    }

    public void SetAsPlayVisual()
    {
        playButton.SetAsPlayVisual();
    }

    public float GetSliderPosition()
    {
        return slider.value;
    }
    
    public void Initialize(double endTimeMillisec)
    {
        var sec = endTimeMillisec / 1000d;
        var min = (int) (sec / 60);
        sec = sec - (min * 60);

        endText.text = $"{min:D2}:{(int)sec:D2}";

        this.endTimeMillisec = endTimeMillisec;
        
        slider.value = 0;
    }
    
    public void SetHeader(double headerMillisec)
    {
        slider.value = (float)(headerMillisec / endTimeMillisec);
    }
    
}
