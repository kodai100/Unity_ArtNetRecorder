using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PlayToggleButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image pauseImage;
    [SerializeField] private Image playImage;

    [SerializeField] private Color pauseColor = Color.red;
    [SerializeField] private Color playColor = Color.green;

    public IObservable<Unit> OnClickAsObservable => button.OnClickAsObservable();
    
    public void SetAsPauseVisual()
    {
        button.image.color = pauseColor;
        pauseImage.gameObject.SetActive(true);
        playImage.gameObject.SetActive(false);
    }

    public void SetAsPlayVisual()
    {
        button.image.color = playColor;
        pauseImage.gameObject.SetActive(false);
        playImage.gameObject.SetActive(true);
    }
}
