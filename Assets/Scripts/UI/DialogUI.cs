using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class DialogImageData
{
    public Sprite infoImage;
    public Sprite errorImage;
}

public class DialogUI : MonoBehaviour
{

    [SerializeField] private RectTransform paneTransform;
    
    [SerializeField] private Button okButton;
    [SerializeField] private Text dialogText;
    [SerializeField] private Button closeButton;

    [SerializeField] private Image backgroundImage;
    [SerializeField] private Image dialogImage;

    [SerializeField] private DialogImageData dialogImageData;
    
    private void Awake()
    {
        var color = backgroundImage.color;
        color.a = 0;
        backgroundImage.color = color;
        
        paneTransform.localScale = Vector3.zero;
    }

    private void Start()
    {

        DOTween.ToAlpha(
            ()=> backgroundImage.color,
            color => backgroundImage.color = color,0.4f,0.5f);
        
    }

    public async UniTask<bool> OpenInfo(string str)
    {
        
        paneTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBounce);
        
        closeButton.gameObject.SetActive(false);
        
        dialogImage.sprite = dialogImageData.infoImage;
        
        dialogText.text = str;

        var buttonEvent = okButton.onClick.GetAsyncEventHandler(this.GetCancellationTokenOnDestroy());

        await buttonEvent.OnInvokeAsync();
        
        Destroy(gameObject);

        return true;
    }
    
    public async UniTask<bool> OpenError(string str)
    {
        
        paneTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        
        closeButton.gameObject.SetActive(false);

        dialogImage.sprite = dialogImageData.errorImage;
        
        dialogText.text = str;

        var buttonEvent = okButton.onClick.GetAsyncEventHandler(this.GetCancellationTokenOnDestroy());

        await buttonEvent.OnInvokeAsync();

        Destroy(gameObject);
        
        return true;
    }
    

}
