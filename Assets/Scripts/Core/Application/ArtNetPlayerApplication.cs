using System;
using System.IO;
using UniRx;
using UnityEngine;

public enum PlayState
{
    Playing, Pausing
}

public class ArtNetPlayerApplication : MonoBehaviour
{

    [SerializeField] private DataVisualizer visualizer;

    [SerializeField] private PlayerUI playerUI;
    
    [SerializeField] private FileDialogUI dmxFileDialogUI;
    [SerializeField] private FileDialogUI audioDialogUI;
    
    [SerializeField] private AudioPlayer audioPlayer;

    [SerializeField] private ArtNetPlayer artNetPlayer;

    [SerializeField] private LoadingUI loadingUI;

    private bool initialized = false;
    
    private double header = 0;
    private double endTime;

    private PlayState playState = PlayState.Pausing;
    
    private void Start()
    {

        loadingUI.Hide();

        playerUI.OnPlayButtonPressedAsObservable.Subscribe(_ =>
        {
            if (playState == PlayState.Playing)
            {
                Pause();
            }
            else
            {
                Resume();
            }
            
        }).AddTo(this);


        dmxFileDialogUI.OnFileNameChanged.Subscribe(path =>
        {
            if (File.Exists(path))
            {
                Initialize(path);
            }
        }).AddTo(this);

        audioDialogUI.OnFileNameChanged.Subscribe(async path =>
        {
            if (File.Exists(path))
            {
                audioPlayer.LoadClipFromPath(path).Forget();
            }
        }).AddTo(this);

    }


    
    private async void Initialize(string path)
    {
        
        initialized = false;
        
        // read file
        loadingUI.Display();
        
        var data = await artNetPlayer.Load(path);

        loadingUI.Hide();
        
        endTime = data.Duration;
        
        if (data != null)
        {
            // initialize visualizer
            // TODO: 今後ファイルに使用Universe数を格納するようにする。
            const int maxUniverseNum = 32;
            visualizer.Initialize(maxUniverseNum);
            
            // initialize player
            playerUI.Initialize(endTime);
            playerUI.SetAsPlayVisual();
            playState = PlayState.Pausing;
        
            // initialize buffers
            artNetPlayer.Initialize(maxUniverseNum);

            initialized = true;
        }
        
        // ローディング画面から開ける
    }
    
    public void Resume()
    {

        if (!initialized) return;
        
        // ここでヘッダ読んでくる
        
        header = playerUI.GetSliderPosition() * endTime;
        endTime = artNetPlayer.GetDuration();
        
        playerUI.SetAsPauseVisual();

        playState = PlayState.Playing;
        
        audioPlayer.Resume(2566.667f + (float)header);
    }

    public void Pause()
    {
        playerUI.SetAsPlayVisual();
        playState = PlayState.Pausing;
        
        audioPlayer.Pause();
    }

    private void Update()
    {

        if (!initialized) return;

        if (playState == PlayState.Pausing) return;

        if (header > endTime)
        {
            Pause();
        }
        
        header += Time.deltaTime * 1000;    // millisec

        visualizer.Exec(artNetPlayer.ReadAndSend(header));

        playerUI.SetHeader(header);

    }
    
}
