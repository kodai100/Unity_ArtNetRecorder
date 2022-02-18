using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[UnityEditor.AssetImporters.ScriptedImporter(1, "dmx")]
public class DmxRecordDataImporter : UnityEditor.AssetImporters.ScriptedImporter
{
    #region ScriptedImporter implementation

    public override void
        OnImportAsset(UnityEditor.AssetImporters.AssetImportContext context)
    {
        var data = ImportDmxRecorderData(context.assetPath);
        if (data == null) return;

        context.AddObjectToAsset("data", data);
        context.SetMainObject(data);
    }

    #endregion

    #region Reader implementation

    private DmxRecordData ImportDmxRecorderData(string path)
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

            return DmxRecordData.CreateAsset(finalPaketTime/1000, list);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed importing {path}. {e.Message}");
            return null;
        }
    }

    #endregion
}