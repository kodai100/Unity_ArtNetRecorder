using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ProjectBlue.ArtNetRecorder
{

    public struct UdpRecordingPacket
    {
        public uint Sequence;
        public double Time;
        public byte[] Data;
    }

    public struct SaveResult
    {
        public string DataPath;
        public long PacketNum;
        public long Size;
    }

    public abstract class RecorderBase : MonoBehaviour
    {
        public Action<double> OnUpdateTime;
        public Action<SaveResult> OnSaved;
        public bool IsRecording { get; protected set; }
        public abstract void RecordStart();
        public abstract void RecordEnd();
    }
    
    public class UdpRecorder : RecorderBase
    {

        static volatile bool loopFlg = true;

        private ConcurrentQueue<UdpRecordingPacket> udpBuff = new ConcurrentQueue<UdpRecordingPacket>();
        
        private Stopwatch recordingStopWatch = new Stopwatch();
        private uint recordingSequenceNumber = 0;

        private void OnEnable()
        {
            loopFlg = true;
            ReceiveUdpTaskRun();
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
            if (udpBuff.Count == 0)
            {
                udpBuff = new ConcurrentQueue<UdpRecordingPacket>();
                IsRecording = true;
                recordingStopWatch.Start();

                var path = CreateRecordingFile();
                
                var total = 0L;
                var bytesLen = 0L;
                
                await Task.Run(() =>
                {
                   
                    using (var file = new FileStream(path, FileMode.Create, FileAccess.ReadWrite))
                    {
                        while (IsRecording || udpBuff.Count > 0)
                        {
                            if (udpBuff.TryDequeue(out var udpRecordingPacket))
                            {
                                
                                // ここで時刻を取り出す
                                var sequence = ByteConvertUtility.GetBytes(udpRecordingPacket.Sequence);
                                var milliseconds = ByteConvertUtility.GetBytes(udpRecordingPacket.Time);
                                var dataLength = ByteConvertUtility.GetBytes(udpRecordingPacket.Data.Length);

                                var bytes = ByteConvertUtility.Join(sequence, milliseconds, dataLength, udpRecordingPacket.Data);
                                
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
                }
                
            }
            else
            {
                Debug.Log($"BuffCount:{udpBuff.Count}");
            }
        }

        private static string CreateRecordingFile()
        {
            var fileName = "Udp_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".udp";
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
        
        private void ReceiveUdpTaskRun()
        {
            var ip = new IPEndPoint(IPAddress.Any, Const.ArtNetServerPort);
            Task.Run(() =>
            {
                using (var udpClient = new UdpClient(ip))
                {

                    while (loopFlg)
                    {
                        try
                        {
                            // DMXの受信プロセス
                            var result = udpClient.ReceiveAsync();
                            if (result.Result.Buffer.Length > 0)
                            {
                                var buffer = result.Result.Buffer;

                                if (IsRecording)
                                {
                                    udpBuff.Enqueue(new UdpRecordingPacket(){Sequence = recordingSequenceNumber, Time = recordingStopWatch.ElapsedMilliseconds, Data = buffer});
                                    recordingSequenceNumber++;
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                    }
                    
                    Debug.Log("UDP Server finished");
                }
            });
            
        }
    }
}
