using System;
using System.Threading.Tasks;
using UniRx;
using ProjectBlue.ArtNetRecorder;
using UnityEngine;

public class DmxRecorderApplication : ApplicationBase
{

    [SerializeField] private RecorderUI recorderUI;
    
    [SerializeField] private RecorderBase currentRecorder;

    private int prevRecorderIndex = 0;
    
    public override void OnClose()
    {
        if (currentRecorder != null)
        {
            DestroyImmediate(currentRecorder.gameObject);
        }
    }

    public override void OnOpen()
    {
        RecorderChanged(prevRecorderIndex);
    }
    
    
    private void Start()
    {
        // OnOpenが最初に呼ばれるので、最初の小タブのイベントはスキップして重複を防止
        recorderUI.TabChangedAsObservable.Skip(1).Subscribe(RecorderChanged).AddTo(this);
    }

    private void InstantiateRecorder(int index)
    {
        var recorder = new GameObject();

        if (index == 0)
        {
            currentRecorder = recorder.AddComponent<UdpRecorder>();
            recorder.name = "UDP Recorder";
            Logger.Log("Changed to UDP Recorder");
        }
        else if (index == 1)
        {
            currentRecorder = recorder.AddComponent<ArtNetRecorder>();
            recorder.name = "DMX Recorder";
            Logger.Log("Changed to ArtNet Recorder");
        }

        prevRecorderIndex = index;
    }

    private async void RecorderChanged(int index)
    {

        if (currentRecorder != null)
        {
            DestroyImmediate(currentRecorder.gameObject);
        }
        
        // Destroy and release udp port first
        await Task.Delay(TimeSpan.FromSeconds(0.1f));

        InstantiateRecorder(index);

        recorderUI.RecordButton.Button.OnClickAsObservable().Subscribe(_ =>
        {

            if (!currentRecorder.IsRecording)
            {
                recorderUI.RecordButton.SetRecord();
                Logger.Log("Recording...");
                recorderUI.TimeCodeText.color = Color.red;
                currentRecorder.RecordStart();

                recorderUI.Tab.Disable();
            }
            else
            {
                recorderUI.RecordButton.SetStop();
                currentRecorder.RecordEnd();

                recorderUI.TimeCodeText.color = Color.white;

                recorderUI.Tab.Enable();
            }
            
        }).AddTo(currentRecorder);
        
        
        recorderUI.IndicatorUI.ResetIndicator();

        currentRecorder.OnIndicatorUpdate = tuple =>
        {
            recorderUI.IndicatorUI.SetScale(tuple.Item2);
            recorderUI.IndicatorUI.Set(tuple.Item1, tuple.Item3);
        };
        
        
        currentRecorder.OnUpdateTime = (ms) =>
        {
            var t = TimeSpan.FromMilliseconds(ms);
            recorderUI.TimeCodeText.text = $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2};{t.Milliseconds:D3}";
        };

        currentRecorder.OnSaved = (result) =>
        {
            // Record中にQuitするとTextがDestroy済なのにアクセスしてしまうのを防止
            if (!Application.isPlaying) return;
            
            
            string size;
                
            if (result.Size > 1024)
            {
                size =  Mathf.CeilToInt(result.Size/1024f) + "KB";
            } else if (result.Size > 1024 * 1024)
            {
                size = Mathf.CeilToInt(result.Size/(1024f*1024f)) + "MB";
            }
            else
            {
                size = result.Size + "Bytes";
            }
                
            Logger.Log($"Saved - Packets: {result.PacketNum}, DataSize: {size} : {result.DataPath}");
        };
    }

   
}
