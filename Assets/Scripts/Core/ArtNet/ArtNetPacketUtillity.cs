using System.Linq;
using Unity.Collections.LowLevel.Unsafe;

namespace ProjectBlue
{
    public static class ArtNetPacketUtillity
    {
        public static ArtNetOpCodes GetOpCode(byte[] buffer)
        {
            return (ArtNetOpCodes)buffer[9] + (buffer[8] << 8);
        }
        public static short GetUniverse(byte[] buffer)
        {
            var offset = 14;
            return ByteConvertUtility.ToShort(buffer, ref offset);
        }
        public static unsafe void GetDmx(byte[] src, ref byte[] dst)
        {
            fixed (byte* bp = src, dp = dst)
            {
                UnsafeUtility.MemCpy(dp, bp + 18, dst.Length);
            }
        }
        
    }
}
