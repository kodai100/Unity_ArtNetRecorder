using System.IO;

public class ArtNetPacket
{
    public ArtNetPacket(ArtNetOpCodes opCode)
    {
        OpCode = opCode;
    }
    
    public byte[] ToArray()
    {
        var stream = new MemoryStream();
        WriteData(new ArtNetBinaryWriter(stream));
        return stream.ToArray();
    }

    #region Packet Properties

    private string protocol = "Art-Net";

    public string Protocol
    {
        get { return protocol; }
        protected set
        {
            if (value.Length > 8)
                protocol = value.Substring(0, 8);
            else
                protocol = value;
        }
    }


    private short version = 14;

    public short Version
    {
        get { return version; }
        protected set { version = value; }
    }

    private ArtNetOpCodes opCode = ArtNetOpCodes.None;

    public virtual ArtNetOpCodes OpCode
    {
        get { return opCode; }
        protected set { opCode = value; }
    }

    #endregion
    
    public virtual void WriteData(ArtNetBinaryWriter data)
    {
        data.WriteNetwork(Protocol, 8);
        data.WriteNetwork((short)OpCode);

        //For some reason the poll packet header does not include the version.
        if (OpCode != ArtNetOpCodes.PollReply)
            data.WriteNetwork(Version);

    }
    
}
