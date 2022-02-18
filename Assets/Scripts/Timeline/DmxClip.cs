using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace inc.stu.SyncSystem
{
    public class DmxClip : PlayableAsset, ITimelineClipAsset
    {

        [SerializeField] private DmxRecordData recordData;

        public DmxRecordData RecordData => recordData;
        
        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable  = ScriptPlayable<DmxBehaviour>.Create(graph);
            return playable ;
        }
    }
}
