using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

public enum PlayState
{
    Playing, Pausing
}

public class ArtNetSender : MonoBehaviour
{

    [SerializeField] private DataVisualizer visualizer;

    [SerializeField] private PlayerUI playerUI;
    
    [SerializeField] private FileDialogUI dmxFileDialogUI;
    [SerializeField] private FileDialogUI audioDialogUI;
    
    [SerializeField] private AudioPlayer audioPlayer;

    private DmxRecordData dmxRecordData;

    private bool initialized = false;
    
    private byte[][] dmx;
    private float[] dmxRaw;
    
    private double header = 0;
    private double endTime;

    private PlayState playState = PlayState.Pausing;
    
    private void Start()
    {

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

    private async Task<DmxRecordData> Read(string path)
    {

        var result = await Task.Run(() => DmxRecordData.ReadFromFilePath(path));

        return result;
    }
    
    private async void Initialize(string path)
    {
        
        initialized = false;
        
        // read file
        dmxRecordData = await Read(path);
        endTime = dmxRecordData.Duration;
        
        if (dmxRecordData != null)
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
            dmx = new byte[maxUniverseNum][];
            for(var i = 0; i < maxUniverseNum; i++)
            {
                dmx[i] = new byte[512];
            }
        
            dmxRaw = new float[maxUniverseNum * 512];

            initialized = true;
        }
        
        // ローディング画面から開ける
    }
    
    public void Resume()
    {

        if (!initialized) return;
        
        // ここでヘッダ読んでくる
        
        header = 0;
        endTime = dmxRecordData.Data.Last().time;
        
        playerUI.SetAsPauseVisual();

        playState = PlayState.Playing;
        
        audioPlayer.Play();
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

        foreach (var packet in dmxRecordData.Data)
        {
                
            if (packet.time >= header)
            {

                foreach (var universeData in packet.data)
                {
                    
                    Buffer.BlockCopy(universeData.data, 0, dmx[universeData.universe],0, universeData.data.Length);
                    
                    // universe
                    for (var universe = 0; universe < dmx.Length; universe++)
                    {
                        // channel
                        for (var channel = 0; channel < dmx[universe].Length; channel++)
                        {
                            dmxRaw[universe * dmx[universe].Length + channel] = dmx[universe][channel];
                        }
                    
                    }
                    
                }

                visualizer.Exec(dmxRaw);
                    
                break;
            }
        }

        playerUI.SetHeader(header, endTime);

    }
    
}
