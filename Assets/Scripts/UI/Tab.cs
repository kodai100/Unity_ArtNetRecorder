using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class ButtonAndCanvasGroupPair
{
    public Button button;
    public CanvasGroup canvasGroup;
}

public class Tab : MonoBehaviour
{

    [SerializeField] private Color disabledBackgroundColor;
    [SerializeField] private Color enabledBackgroundColor;

    [SerializeField] private List<ButtonAndCanvasGroupPair> pairs;
    
    private Subject<int> onSelected = new Subject<int>();

    public IObservable<int> OnSelected => onSelected;

    private async void Start()
    {


        for (var i = 0; i < pairs.Count; i++)
        {
            var i1 = i;    // capture
            pairs[i1].button.OnClickAsObservable().Subscribe(_ =>
            {
                onSelected.OnNext(i1);
                pairs.ForEach(pair =>
                {
                    if (pair.button != null)
                    {
                        pair.button.targetGraphic.color = disabledBackgroundColor;
                    }
                    
                    if (pair.canvasGroup != null)
                    {
                        pair.canvasGroup.alpha = 0;
                        pair.canvasGroup.interactable = false;
                        pair.canvasGroup.blocksRaycasts = false;
                    }
                    
                });
                
                if (pairs[i1].button != null)
                {
                    pairs[i1].button.targetGraphic.color = enabledBackgroundColor;
                }

                if (pairs[i1].canvasGroup != null)
                {
                    pairs[i1].canvasGroup.alpha = 1;
                    pairs[i1].canvasGroup.interactable = true;
                    pairs[i1].canvasGroup.blocksRaycasts = true;
                }

            }).AddTo(this);
        }

        await Task.Delay(TimeSpan.FromSeconds(0.1f));
        
        pairs[0].button.onClick.Invoke();
        
    }


    public void Disable()
    {
        pairs.ForEach(pair =>
        {
            pair.button.interactable = false;
        });
    }

    public void Enable()
    {
        pairs.ForEach(pair =>
        {
            pair.button.interactable = true;
        });
    }
    
    private void OnDestroy()
    {
        onSelected.Dispose();
    }
}
