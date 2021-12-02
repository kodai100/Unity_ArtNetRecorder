using System;
using System.Collections.Generic;
using System.Text;

namespace ProjectBlue
{
    using unsafety;

    public static class ByteConvertUtility
    {
        const int boolSize = sizeof(bool);
        const int shortSize = sizeof(short);
        const int intSize = sizeof(int);
        const int longSize = sizeof(long);
        const int floatSize = sizeof(float);
        const int doubleSize = sizeof(double);
        const int ushortSize = sizeof(ushort);
        const int uintSize = sizeof(uint);
        const int ulongSize = sizeof(ulong);
        const int charSize = sizeof(char);

        static ByteConvertUtility()
        {
            BitConverter.IsLittleEndian = true;
        }
        public static byte[] GetBytes(params bool[] values)
        {
            var length = values.Length;
            var bytes = new byte[boolSize * length];
            for (var i = 0; i < length; i++)
            {
                var b = BitConverter.GetBytes(values[i]);
                Buffer.BlockCopy(b, 0, bytes, i * boolSize, boolSize);
            }
            return bytes;
        }
        public static bool ToBool(byte[] bytes, ref int offset)
        {
            var value = BitConverter.ToBoolean(bytes, offset);
            offset += boolSize;
            return value;
        }
        public static byte[] GetBytes(params short[] values)
        {
            var length = values.Length;
            var bytes = new byte[shortSize * length];
            for (var i = 0; i < length; i++)
            {
                var b = BitConverter.GetBytes(values[i]);
                Buffer.BlockCopy(b, 0, bytes, i * shortSize, shortSize);
            }
            return bytes;
        }
        public static short ToShort(byte[] bytes, ref int offset)
        {
            var value = BitConverter.ToInt16(bytes, offset);
            offset += shortSize;
            return value;
        }
        public static byte[] GetBytes(params int[] values)
        {
            var length = values.Length;
            var bytes = new byte[intSize * length];
            for (var i = 0; i < length; i++)
            {
                var b = BitConverter.GetBytes(values[i]);
                Buffer.BlockCopy(b, 0, bytes, i * intSize, intSize);
            }
            return bytes;
        }
        public static int ToInt(byte[] bytes, ref int offset)
        {
            var value = BitConverter.ToInt32(bytes, offset);
            offset += intSize;
            return value;
        }
        public static byte[] GetBytes(params long[] values)
        {
            var length = values.Length;
            var bytes = new byte[longSize * length];
            for (var i = 0; i < length; i++)
            {
                var b = BitConverter.GetBytes(values[i]);
                Buffer.BlockCopy(b, 0, bytes, i * longSize, longSize);
            }
            return bytes;
        }
        public static long ToLong(byte[] bytes, ref int offset)
        {
            var value = BitConverter.ToInt64(bytes, offset);
            offset += longSize;
            return value;
        }
        public static byte[] GetBytes(params float[] values)
        {
            var length = values.Length;
            var bytes = new byte[floatSize * length];
            for (var i = 0; i < length; i++)
            {
                var b = BitConverter.GetBytes(values[i]);
                Buffer.BlockCopy(b, 0, bytes, i * floatSize, floatSize);
            }
            return bytes;
        }
        public static float ToFloat(byte[] bytes, ref int offset)
        {
            var value = BitConverter.ToSingle(bytes, offset);
            offset += floatSize;
            return value;
        }
        public static byte[] GetBytes(params double[] values)
        {
            var length = values.Length;
            var bytes = new byte[doubleSize * length];
            for (var i = 0; i < length; i++)
            {
                var b = BitConverter.GetBytes(values[i]);
                Buffer.BlockCopy(b, 0, bytes, i * doubleSize, doubleSize);
            }
            return bytes;
        }
        public static double ToDouble(byte[] bytes, ref int offset)
        {
            var value = BitConverter.ToDouble(bytes, offset);
            offset += doubleSize;
            return value;
        }
        public static byte[] GetBytes(params ushort[] values)
        {
            var length = values.Length;
            var bytes = new byte[ushortSize * length];
            for (var i = 0; i < length; i++)
            {
                var b = BitConverter.GetBytes(values[i]);
                Buffer.BlockCopy(b, 0, bytes, i * ushortSize, ushortSize);
            }
            return bytes;
        }
        public static ushort ToUShort(byte[] bytes, ref int offset)
        {
            var value = BitConverter.ToUInt16(bytes, offset);
            offset += ushortSize;
            return value;
        }
        public static byte[] GetBytes(params uint[] values)
        {
            var length = values.Length;
            var bytes = new byte[uintSize * length];
            for (var i = 0; i < length; i++)
            {
                var b = BitConverter.GetBytes(values[i]);
                Buffer.BlockCopy(b, 0, bytes, i * uintSize, uintSize);
            }
            return bytes;
        }
        public static uint ToUInt(byte[] bytes, ref int offset)
        {
            var value = BitConverter.ToUInt32(bytes, offset);
            offset += uintSize;
            return value;
        }
        public static byte[] GetBytes(params ulong[] values)
        {
            var length = values.Length;
            var bytes = new byte[ulongSize * length];
            for (var i = 0; i < length; i++)
            {
                var b = BitConverter.GetBytes(values[i]);
                Buffer.BlockCopy(b, 0, bytes, i * ulongSize, ulongSize);
            }
            return bytes;
        }
        public static float ToUlong(byte[] bytes, ref int offset)
        {
            var value = BitConverter.ToUInt64(bytes, offset);
            offset += ulongSize;
            return value;
        }
        public static byte[] GetBytes(params char[] values)
        {
            var length = values.Length;
            var bytes = new byte[charSize * length];
            for (var i = 0; i < length; i++)
            {
                var b = BitConverter.GetBytes(values[i]);
                Buffer.BlockCopy(b, 0, bytes, i * charSize, charSize);
            }
            return bytes;
        }
        public static char ToChar(byte[] bytes, ref int offset)
        {
            var value = BitConverter.ToChar(bytes, offset);
            offset += charSize;
            return value;
        }
        public static byte[] GetBytes(string values, Encoding encoding = null)
        {
            if(encoding == null) encoding = Encoding.Default;
            return encoding.GetBytes(values);
        }
        public static string ToString(byte[] bytes, ref int offset, int length, int startIndex = 0, Encoding encoding = null)
        {
            if(encoding == null) encoding = Encoding.Default;
            offset += length;
            return encoding.GetString(bytes, startIndex, length);
        }
        public static byte[] Join(params byte[][] values)
        {
            var length = 0;
            for(var i = 0; i < values.Length; i++)
            {
                length += values[i].Length;
            }
            var bytes = new byte[length];
            var prev = 0;
            for(var i = 0; i < values.Length; i++)
            {
                Buffer.BlockCopy(values[i], 0, bytes, prev, values[i].Length);
                prev += values[i].Length;
            }
            return bytes;
        }
        public static byte[] Join(List<byte[]> values)
        {
            var count = 0;
            values.ForEach(e => count += e.Length);
            var bytes = new byte[count];
            var prev = 0;
            for (var i = 0; i < values.Count; i++)
            {
                Buffer.BlockCopy(values[i], 0, bytes, prev, values[i].Length);
                prev += values[i].Length;
            }
            return bytes;
        }
    }
}
