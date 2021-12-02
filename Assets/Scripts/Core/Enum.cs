namespace ProjectBlue
{
    public enum ArtNetOpCodes
    {
        None = 0,
        Poll = 0x20,
        PollReply = 0x21,
        Dmx = 0x50,
        TodRequest = 0x80,
        TodData = 0x81,
        TodControl = 0x82,
        Rdm = 0x83,
        RdmSub = 0x84,
    }
}
