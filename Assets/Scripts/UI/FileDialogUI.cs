using System;
using System.Collections;
using System.Collections.Generic;
using SFB;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class FileDialogUI : MonoBehaviour
{

    [SerializeField] private InputField inputField;
    [SerializeField] private Button browseButton;
    [SerializeField] private string extensionFilter = "jpg";
    
    public IObservable<string> OnFileNameChanged => onFileNameChanged;
    
    private Subject<string> onFileNameChanged = new Subject<string>();
    
    private void Awake()
    {

        inputField.OnValueChangedAsObservable().Subscribe(text =>
        {
            onFileNameChanged.OnNext(text);
        }).AddTo(this);
        
        browseButton.onClick.AddListener(Open);
    }
    
    private void Open()
    {

        var extensions = new [] {
            new ExtensionFilter("DMX Record Files", extensionFilter ),
        };
        
        StandaloneFileBrowser.OpenFilePanelAsync("Open File", "", extensions, false, (string[] paths) =>
        {
            inputField.text = paths[0];
            
            // オープン成功したら上流に通知する方が良さそう。
            // リセットかける
            onFileNameChanged.OnNext(paths[0]);
        });
        
    }

    private void OnDestroy()
    {
        onFileNameChanged.Dispose();
    }
}
