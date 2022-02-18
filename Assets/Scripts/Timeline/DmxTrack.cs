using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace inc.stu.SyncSystem
{
    [TrackColor(1, 0, 0)]
    [TrackClipType(typeof(DmxClip))]
    // [TrackBindingType(typeof(ArtNetManager))]
    public class DmxTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var playable = ScriptPlayable<DmxMixerBehaviour>.Create(graph, inputCount);
            
            playable.GetBehaviour().Clips = GetClips().ToArray();
            playable.GetBehaviour().Director = go.GetComponent<PlayableDirector>();
            
            return playable;
        }
        
        protected override Playable CreatePlayable(PlayableGraph graph, GameObject gameObject, TimelineClip clip)
        {
            var dmxClip = clip.asset as DmxClip;
            if (dmxClip != null)
            {
                clip.duration = dmxClip.RecordData.Duration;
            }
            return base.CreatePlayable(graph, gameObject, clip);
        }
    }
}
