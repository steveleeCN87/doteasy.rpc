using System;
using System.IO;

namespace DotEasy.Rpc.ApiGateway.AuthMiddleware
{
    public class MemoryWrappedHttpResponseStream : MemoryStream
    {
        private Stream _innerStream;
        public MemoryWrappedHttpResponseStream(Stream innerStream)
        {
            _innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
        }
        public override void Flush()
        {
            _innerStream.Flush();
            base.Flush();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            base.Write(buffer, offset, count);
            _innerStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                _innerStream.Dispose();
            }
        }

        public override void Close()
        {
            base.Close();
            _innerStream.Close();
        }
    }
}