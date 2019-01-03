using DotNetty.Buffers;
using DotNetty.Common.Utilities;

namespace DotEasy.Rpc.Core.Transport.InternalAdaper
{
    public static class ByteBufferExtend
    {
        public static byte[] ToArray(this IByteBuffer byteBuffer)
        {
            int readableBytes = byteBuffer.ReadableBytes;
            if (readableBytes == 0)
            {
                return ArrayExtensions.ZeroBytes;
            }

            if (byteBuffer.HasArray)
            {
                return byteBuffer.Array.Slice(byteBuffer.ArrayOffset + byteBuffer.ReaderIndex, readableBytes);
            }

            var bytes = new byte[readableBytes];
            byteBuffer.GetBytes(byteBuffer.ReaderIndex, bytes);
            return bytes;
        }
    }
}