using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using Cysharp.Threading.Tasks;
using Mono.Cecil.Cil;
using ProjectBlue;
using UnityEngine;

public class ArtNetPlayer : MonoBehaviour
{

    [SerializeField] private ArtNetResendUI artNetResendUI;
    
    private DmxRecordData dmxRecordData;
    
    private byte[][] dmx;
    private float[] dmxRaw;
    
    UdpClient udpClient = new UdpClient();

    public async UniTask<DmxRecordData> Load(string path)
    {
        dmxRecordData = await ReadFile(path);
        return dmxRecordData;
    }
    
    private static async UniTask<DmxRecordData> ReadFile(string path)
    {

        var result = await UniTask.Run(() => DmxRecordData.ReadFromFilePath(path));

        return result;
    }

    public double GetDuration()
    {
        return dmxRecordData.Data.Last().time;
    }

    public void Initialize(int maxUniverseNum)
    {
        dmx = new byte[maxUniverseNum][];
        for(var i = 0; i < maxUniverseNum; i++)
        {
            dmx[i] = new byte[512];
        }
        
        dmxRaw = new float[maxUniverseNum * 512];
    }

    public float[] ReadAndSend(double header)
    {
        foreach (var packet in dmxRecordData.Data)
        {
                
            if (packet.time >= header)
            {

                foreach (var universeData in packet.data)
                {
                    
                    Buffer.BlockCopy(universeData.data, 0, dmx[universeData.universe],0, universeData.data.Length);

                    if (artNetResendUI.IsEnabled)
                    {
                        var artNetPacket = new ArtNetDmxPacket
                        {
                            Universe = (short) universeData.universe, DmxData = dmx[universeData.universe]
                        };

                        var artNetPacketBytes = artNetPacket.ToArray();

                        udpClient.Send(artNetPacketBytes, artNetPacketBytes.Length, artNetResendUI.IPAddress.ToString(), artNetResendUI.Port);
                    }
                    
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
                    
                return dmxRaw;
            }
        }
        
        return dmxRaw;
    }
}
