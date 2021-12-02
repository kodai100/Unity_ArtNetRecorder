using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class Tab : MonoBehaviour
{

    [SerializeField] private Color disabledBackgroundColor;
    [SerializeField] private Color enabledBackgroundColor;

    [SerializeField] private List<Button> buttons;
    
    private Subject<int> onSelected = new Subject<int>();

    public IObservable<int> OnSelected => onSelected;

    private async void Start()
    {


        for (var i = 0; i < buttons.Count; i++)
        {
            var i1 = i;    // capture
            buttons[i1].OnClickAsObservable().Subscribe(_ =>
            {
                onSelected.OnNext(i1);
                buttons.ForEach(b =>
                {
                    b.targetGraphic.color = disabledBackgroundColor;
                });

                buttons[i1].targetGraphic.color = enabledBackgroundColor;

            }).AddTo(this);
        }

        await Task.Delay(TimeSpan.FromSeconds(0.1f));
        
        buttons[0].onClick.Invoke();
        
    }


    public void Disable()
    {
        buttons.ForEach(b =>
        {
            b.interactable = false;
        });
    }

    public void Enable()
    {
        buttons.ForEach(b =>
        {
            b.interactable = true;
        });
    }
    
    private void OnDestroy()
    {
        onSelected.Dispose();
    }
}
