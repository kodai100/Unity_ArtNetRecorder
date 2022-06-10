public class ArtNetDmxPacket : ArtNetPacket
{
    public ArtNetDmxPacket()
        : base(ArtNetOpCodes.Dmx)
    {
    }

    #region Packet Properties

    public byte Sequence { get; set; } = 0;

    public byte Physical { get; set; } = 0;

    public short Universe { get; set; } = 0;

    public short Length
    {
        get
        {
            if (DmxData == null)
                return 0;
            return (short)DmxData.Length;
        }
    }

    public byte[] DmxData { get; set; } = null;
    
    public override void WriteData(ArtNetBinaryWriter data)
    {
        base.WriteData(data);

        data.Write(Sequence);
        data.Write(Physical);
        data.Write(Universe);
        data.WriteNetwork(Length);
        data.Write(DmxData);
    }

    #endregion
}
