using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ProjectBlue.ArtNetRecorder
{

    public struct DmxRecordingPacket
    {
        public uint Sequence;
        public double Time;
        public byte[][] Data;
    }
    
    public class ArtNetRecorder : RecorderBase
    {

        static volatile bool loopFlg = true;

        private static byte[][] dmx = new byte[Const.MaxUniverse][];
        
        private ConcurrentQueue<DmxRecordingPacket> dmxBuff = new ConcurrentQueue<DmxRecordingPacket>();

        private const double FixedDeltaTime = 1.0d / 60.0d;
        
        private Stopwatch recordingStopWatch = new Stopwatch();
        private uint recordingSequenceNumber = 0;

        private void OnEnable()
        {
            dmx = new byte[Const.MaxUniverse][];

            loopFlg = true;
            ReceiveDmxTaskRun(this.GetCancellationTokenOnDestroy());
        }

        private void Update()
        {

            if (IsRecording)
            {
                OnUpdateTime?.Invoke(recordingStopWatch.ElapsedMilliseconds);
            }
        }

        private void OnDisable()
        {
            if (!loopFlg) return;
            
            loopFlg = false;
            IsRecording = false;
        }
        
        public override async void RecordStart()
        {
            if (dmxBuff.Count == 0)
            {
                dmxBuff = new ConcurrentQueue<DmxRecordingPacket>();
                IsRecording = true;

                recordingStopWatch.Start();

                var path = CreateRecordingFile();
                
                
                var total = 0L;
                var bytesLen = 0L;
                
                await Task.Run(() =>
                {
                    
                    using (var file = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                    {
                        while (IsRecording || dmxBuff.Count > 0)
                        {
                            if (dmxBuff.TryDequeue(out var dmxRecordingPacket))
                            {
                                
                                // ここで時刻を取り出す
                                var sequence = ByteConvertUtility.GetBytes(dmxRecordingPacket.Sequence);
                                var milliseconds = ByteConvertUtility.GetBytes(dmxRecordingPacket.Time);

                                uint numUniverses = 0;
                                var dataSegment = new List<byte[]>();
                                
                                // all universes
                                for (short i = 0; i < dmxRecordingPacket.Data.Length; i++)
                                {
                                    
                                    if (dmxRecordingPacket.Data[i] != null)
                                    {
                                        var myUniverse = ByteConvertUtility.GetBytes((uint)i);
                                        numUniverses++;
                                        dataSegment.Add(ByteConvertUtility.Join(myUniverse, dmxRecordingPacket.Data[i]));
                                    }
                                }
                                
                                var bytes = ByteConvertUtility.Join(sequence, milliseconds, ByteConvertUtility.GetBytes(numUniverses), dataSegment.SelectMany(a => a).ToArray());
                                file.Write(bytes, 0, bytes.Length);
                                bytesLen += bytes.Length;
                                
                                total++;
                            }
                        }
                        
                        
                    }
                    
                    
                });
                
                
                if (total > 0)
                {
                    OnSaved?.Invoke(new SaveResult{DataPath = path, PacketNum = total, Size = bytesLen});
                }
                else
                {
                    File.Delete(path);
                    Debug.Log("Zero!!");
                }
            }
            else
            {
                Debug.Log($"DmxBuffCount:{dmxBuff.Count}");
            }
        }

        private static string CreateRecordingFile()
        {
            var fileName = "Dmx_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".dmx";
            Directory.CreateDirectory(Application.streamingAssetsPath);
            return  Path.Combine(Application.streamingAssetsPath, fileName);
        }
        
        public override void RecordEnd()
        {
            IsRecording = false;
            recordingSequenceNumber = 0;
            recordingStopWatch.Stop();
            recordingStopWatch.Reset();
        }
        
        // cannot use async keyword with unsafe context
        private unsafe void ReceiveDmxTaskRun(CancellationToken cancellationToken = default)
        {
            var ip = new IPEndPoint(IPAddress.Any, Const.ArtNetServerPort);
            
            Task.Run(() =>
            {
                using (var udpClient = new UdpClient(ip))
                {
                    var fixedFramerateStopwatch = new Stopwatch();
                    var dt = 0.0d;

                    while (loopFlg)
                    {
                        
                        try
                        {
                            
                            Debug.Log("Loop");
                            
                            // DMXのレコードのために一時バッファに格納するプロセス
                            if (IsRecording)
                            {
                                fixedFramerateStopwatch.Stop();
                                if (dt <= fixedFramerateStopwatch.Elapsed.TotalSeconds)
                                {
                                    
                                    fixedFramerateStopwatch.Start();
                                    dt += FixedDeltaTime;
                                    
                                    // Create packet
                                    
                                    var buff = new byte[Const.MaxUniverse][];
                                    for (var i = 0; i < Const.MaxUniverse; i++)
                                    {
                                        if (dmx[i] != null)
                                        {
                                            buff[i] = new byte[512];
                                            fixed (byte* src = dmx[i], dst = buff[i])
                                            {
                                                UnsafeUtility.MemCpy(dst, src, buff[i].Length);
                                            }
                                        }
                                    }
                                    dmxBuff.Enqueue(new DmxRecordingPacket{Sequence = recordingSequenceNumber, Time = recordingStopWatch.ElapsedMilliseconds, Data = buff});
                                    recordingSequenceNumber++;
                                }
                                else
                                {
                                    fixedFramerateStopwatch.Start();
                                }
                            }
                            
                            
                            // DMXの受信プロセス
                            var result = udpClient.ReceiveAsync().WithCancellation(cancellationToken);
                            
                            if (result.Result.Buffer.Length > 0)
                            {
                                var buffer = result.Result.Buffer;
                                if (ArtNetPacketUtillity.GetOpCode(buffer) == ArtNetOpCodes.Dmx)
                                {
                                    var universe = ArtNetPacketUtillity.GetUniverse(buffer);
                                    dmx[universe] ??= new byte[512];    // 新しいUniverseが飛んできた場合はバッファに新規Universeの配列分を足す
                                    ArtNetPacketUtillity.GetDmx(buffer, ref dmx[universe]);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            if (e is AggregateException)
                            {

                                if (e.InnerException is TaskCanceledException)
                                {
                                    Debug.Log("Task canceled");
                                }
                            
                            }
                            else
                            {
                                Debug.LogException(e);
                            }

                            loopFlg = false;
                        }

                    }
                    
                    Debug.Log("DMX Server finished");
                    
                    fixedFramerateStopwatch.Stop();
                }
            }, cancellationToken);
        }
    }
}
