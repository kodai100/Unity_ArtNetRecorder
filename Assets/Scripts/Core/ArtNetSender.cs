using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ArtNetSender : MonoBehaviour
{
    
    [SerializeField] private DmxRecordData dmxRecordData;

    [SerializeField] private DataVisualizer visualizer;

    [SerializeField] private int maxUniverseNum = 64;

    [SerializeField] private Slider slider;
    
    
    private byte[][] dmx;
    private float[] dmxRaw;
    
    private double header = 0;
    private double endTime;
    
    private void Start()
    {
        
        visualizer.Initialize(maxUniverseNum);
        
        dmx = new byte[maxUniverseNum][];
        for(var i = 0; i < maxUniverseNum; i++)
        {
            dmx[i] = new byte[512];
        }
        
        dmxRaw = new float[maxUniverseNum * 512];
        
        Play();
    }

    public void Play()
    {
        header = 0;
        endTime = dmxRecordData.Data.Last().time;
    }

    private void Update()
    {

        header += Time.deltaTime * 1000;    // millisec

        foreach (var packet in dmxRecordData.Data)
        {
                
            if (packet.time >= header)
            {
                
                Debug.Log($"{packet.sequence} : {packet.time}");
                    
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

        slider.value = (float) (header / endTime);

    }
    
}
