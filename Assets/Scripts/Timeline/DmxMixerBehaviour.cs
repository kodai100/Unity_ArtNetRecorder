using System;
using System.ComponentModel;
using System.Linq;
using inc.stu;
using inc.stu.SyncSystem;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class DmxMixerBehaviour : PlayableBehaviour
{
    
    public TimelineClip[] Clips { get; set; }
    public PlayableDirector Director { get; set; }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {

        // var trackBinding = playerData as ArtNetManager;

        // if (trackBinding == null)
        //     return;

        var clipTime = Director.time;
        
        for (var i = 0; i < playable.GetInputCount(); i++)
        {
            var inputWeight = playable.GetInputWeight(i);
            if (!(inputWeight > 0)) continue;
            
            var inputPlayable = (ScriptPlayable<DmxBehaviour>) playable.GetInput(i);
            // var inputBehaviour = inputPlayable.GetBehaviour();

            var dmxClip = Clips[i].asset as DmxClip;
            if (dmxClip == null || dmxClip.RecordData == null) continue;
            
            var offsetTimeMilliseconds = (clipTime - Clips[i].start) * 1000;
            
            //var dmx = new byte[Const.MaxUniverse][];
            
            
            foreach (var packet in dmxClip.RecordData.Data)
            {
                
                if (packet.time >= offsetTimeMilliseconds)
                {
                    
                    // Debug.Log($"{packet.sequence} : {packet.time}");
                    
                    foreach (var universeData in packet.data)
                    {
                        // dmx[universeData.universe] = new byte[512];

                        // Buffer.BlockCopy(universeData.data, 0, dmx[universeData.universe],0, universeData.data.Length);

                    }
                    
                    break;
                }
            }
            
            // trackBinding.ForceUpdateFromInstance(dmx);
        }
        
        
    }
    
}