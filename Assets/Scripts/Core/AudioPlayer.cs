using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class AudioPlayer : MonoBehaviour
{

    [SerializeField] private AudioSource source;

    private void Start()
    {
        source.loop = false;
        source.playOnAwake = false;
    }
    
    public async void Play(double delayMillisec)
    {
        await Task.Delay(TimeSpan.FromMilliseconds(delayMillisec));
        
        source.Play();
    }

    public void Resume(float positionMillisec)
    {
        source.time = positionMillisec * 0.001f;
        source.Play();
    }

    public void Pause()
    {
        source.Pause();
    }
    
    public async UniTaskVoid LoadClipFromPath(string path)
    {
        var url = $"file://{path}";

        var r = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.MPEG);

        await r.SendWebRequest();
        
        if (r.result == UnityWebRequest.Result.Success){
            source.clip = DownloadHandlerAudioClip.GetContent(r);
        }
    }
}
