using System;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ProjectBlue.ArtNetRecorder
{

    public struct AcnRecordingPacket
    {
        public uint Sequence;
        public double Time;
        public byte[] Data;
    }
    
    public class AcnRecorder : RecorderBase
    {

        static volatile bool loopFlg = true;

        private ConcurrentQueue<AcnRecordingPacket> udpBuff = new ConcurrentQueue<AcnRecordingPacket>();
        
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
                udpBuff = new ConcurrentQueue<AcnRecordingPacket>();
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
        
            var multicastEndpoint = new IPEndPoint(IPAddress.Parse( "239.0.0.0" ), 64500);

            using (var multicastClient = new UdpClient(6454, AddressFamily.InterNetwork))
            {
                multicastClient.JoinMulticastGroup( multicastEndpoint.Address, 50 );
                
                var manualResetEvent = new ManualResetEvent(false);

                var receiving = multicastClient.BeginReceive((receive) => { }, manualResetEvent);

                do
                {
                    if (receiving.AsyncWaitHandle.WaitOne())
                    {
                        break;
                    }
                } while (!receiving.IsCompleted);

                IPEndPoint remotePoint = null;
                var receivedBytes = multicastClient.EndReceive(receiving, ref remotePoint);
                
                Debug.Log(BitConverter.ToString(receivedBytes));

                if (IsRecording)
                {
                    udpBuff.Enqueue(new AcnRecordingPacket(){Sequence = recordingSequenceNumber, Time = recordingStopWatch.ElapsedMilliseconds, Data = receivedBytes});
                    recordingSequenceNumber++;
                }
            }
            
        }
    }
}
