using System;
using System.Collections.Generic;
using UnityEngine;

[PreferBinarySerialization]
public class DmxRecordData : ScriptableObject
{
    [SerializeField] private double duration;
    [SerializeField] private List<DmxRecordPacket> data;

    public double Duration => duration;
    public IEnumerable<DmxRecordPacket> Data => data;
    
    public static DmxRecordData CreateAsset(double duration, List<DmxRecordPacket> list)
    {
        var asset = CreateInstance<DmxRecordData>();
        asset.duration = duration;
        asset.data = list;
        
        return asset;
    }
}


[Serializable]
public class DmxRecordPacket
{
    public int sequence;
    public double time; // millisec
    public int numUniverses;
    public List<UniverseData> data;
}

[Serializable]
public class UniverseData
{
    public int universe;
    public byte[] data;
}