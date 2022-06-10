using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class ApplicationManager : MonoBehaviour
{
    [SerializeField] private Tab applicationTab;

    [SerializeField] private List<ApplicationBase> applications;
    
    private void Start()
    {
        applicationTab.OnSelected.Subscribe(ApplicationChanged).AddTo(this);
    }
    
    private void ApplicationChanged(int index)
    {
        
        applications.ForEach(app =>
        {
            app.OnClose();
        });

        if (index < applications.Count)
        {
            applications[index]?.OnOpen();
        }

    }

}
