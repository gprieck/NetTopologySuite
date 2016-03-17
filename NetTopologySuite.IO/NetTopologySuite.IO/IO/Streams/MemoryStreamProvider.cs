using System;
using System.IO;

namespace NetTopologySuite.IO.Streams
{
    public class MemoryStreamProvider : IStreamProvider, IDisposable
    {
        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="kind">The kind of stream</param>
        /// <param name="stream">The stream</param>
        public MemoryStreamProvider(string kind, Stream stream)
        {
            Kind = kind;
            UnderlyingStream = stream;
            UnderlyingStreamBytes = ReadFully(stream);
        }
        /// <summary>
        /// Copy the stream to a byte array
        /// </summary>
        /// <param name="input">The stream to copy to a byte array</param>
        /// <returns>byte array</returns>
        private byte[] ReadFully(Stream input)
        {
            if (input is MemoryStream)
            {
                return ((MemoryStream)input).ToArray();
            }
            else
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    input.CopyTo(ms);
                    return ms.ToArray();
                }
            }
        }
        /// <summary>
        /// The stream passed as a parameter in the constructor
        /// </summary>
        public Stream UnderlyingStream { get; private set; }

        private byte[] UnderlyingStreamBytes;

        /// <summary>
        /// Gets a value indicating that the underlying stream is read-only
        /// </summary>
        public bool UnderlyingStreamIsReadonly
        {
            get { return true; }
        }

        /// <summary>
        /// Function to return a Stream of the bytes
        /// </summary>
        /// <returns>An opened stream</returns>
        public Stream OpenRead()
        {
            return new MemoryStream(UnderlyingStreamBytes);
        }

        /// <summary>
        /// Function to open the underlying stream for writing purposes
        /// </summary>
        /// <remarks>If <see cref="IStreamProvider.UnderlyingStreamIsReadonly"/> is not <value>true</value> this method shall fail</remarks>
        /// <returns>An opened stream</returns>
        public Stream OpenWrite(bool truncate)
        {
            return UnderlyingStream;
        }

        /// <summary>
        /// Gets a value indicating the kind of stream
        /// </summary>
        public string Kind { get; private set; }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.UnderlyingStream.Dispose();
                }
                disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion
    }
}
