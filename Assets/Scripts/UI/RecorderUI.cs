using System;
using System.Threading.Tasks;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectBlue.ArtNetRecorder
{


    public class RecorderUI : MonoBehaviour
    {

        [SerializeField] private Tab tab;

        [SerializeField] private Text recordingStatusText;

        [SerializeField] private Text timeCodeText;

        [SerializeField] private RecordButton recordButton;

        [SerializeField] private RecorderBase currentRecorder;

        [SerializeField] private IndicatorUI indicatorUI;

        private void Start()
        {

            tab.OnSelected.Subscribe(RecorderChanged).AddTo(this);
            
        }

        private async void RecorderChanged(int index)
        {

            if (currentRecorder != null)
            {
                DestroyImmediate(currentRecorder.gameObject);
            }
            
            // Destroy and release udp port first
            await Task.Delay(TimeSpan.FromSeconds(0.1f));
            
            var recorder = new GameObject();

            if (index == 0)
            {
                currentRecorder = recorder.AddComponent<UdpRecorder>();
                recorder.name = "UDP Recorder";
                recordingStatusText.text = "Changed to UDP Recorder";
            }
            else if(index == 1)
            {
                currentRecorder = recorder.AddComponent<ArtNetRecorder>();
                recorder.name = "DMX Recorder";
                recordingStatusText.text = "Changed to ArtNet Recorder";
            }
            
            
            
            
            
            recordButton.Button.OnClickAsObservable().Subscribe(_ =>
            {

                if (!currentRecorder.IsRecording)
                {
                    recordButton.SetRecord();
                    recordingStatusText.text = "Recording...";
                    timeCodeText.color = Color.red;
                    currentRecorder.RecordStart();

                    tab.Disable();
                }
                else
                {
                    recordButton.SetStop();
                    currentRecorder.RecordEnd();

                    timeCodeText.color = Color.white;

                    tab.Enable();
                }
                
            }).AddTo(currentRecorder);
            
            
            indicatorUI.ResetIndicator();

            currentRecorder.OnIndicatorUpdate = tuple =>
            {
                indicatorUI.SetScale(tuple.Item2);
                indicatorUI.Set(tuple.Item1, tuple.Item3);
            };
            
            
            currentRecorder.OnUpdateTime = (ms) =>
            {
                var t = TimeSpan.FromMilliseconds(ms);
                timeCodeText.text = $"{t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2};{t.Milliseconds:D3}";
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
                    
                recordingStatusText.text = $"Saved - Packets: {result.PacketNum}, DataSize: {size} : {result.DataPath}";
            };
        }
    }

}