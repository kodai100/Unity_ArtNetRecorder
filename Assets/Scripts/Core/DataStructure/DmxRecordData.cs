using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DmxRecordData
{
    private double duration;
    private List<DmxRecordPacket> data;

    public double Duration => duration;
    public IEnumerable<DmxRecordPacket> Data => data;

    public DmxRecordData(double duration, List<DmxRecordPacket> data)
    {
        this.duration = duration;
        this.data = data;
    }
    
    public static DmxRecordData ReadFromFilePath(string path)
    {
        try
        {
            var list = new List<DmxRecordPacket>();

            double finalPaketTime = 0;
            
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var reader = new BinaryReader(stream);
                
                // loop to the end
                var baseStream = reader.BaseStream;
                while ( baseStream.Position != baseStream.Length )
                {
                    var sequence = (int)reader.ReadUInt32();
                    var time = reader.ReadDouble();

                    finalPaketTime = time;
                    
                    var numUniverses = (int)reader.ReadUInt32();
                
                    var data = new List<UniverseData>();
                
                    for (var i = 0; i < numUniverses; i++)
                    {
                        var universe = (int)reader.ReadUInt32();
                        data.Add(new UniverseData{universe=universe, data=reader.ReadBytes( 512).ToArray()});
                    }
                
                    list.Add(new DmxRecordPacket
                    {
                        sequence = sequence, time = time, numUniverses = numUniverses, data = data
                    });
                }
            }

            return new DmxRecordData(finalPaketTime, list);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed importing {path}. {e.Message}");
            return null;
        }
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