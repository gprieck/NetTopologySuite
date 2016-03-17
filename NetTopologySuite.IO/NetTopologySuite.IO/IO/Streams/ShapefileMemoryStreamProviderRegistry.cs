using System;
using System.IO;

namespace NetTopologySuite.IO.Streams
{
    /// <summary>
    /// A stream provider registry for an ESRI Shapefile dataset
    /// </summary>
    public class ShapefileMemoryStreamProviderRegistry : IStreamProviderRegistry, IDisposable
    {
        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="shapeStream">A Stream for the shape stream</param>
        /// <param name="dataStream">A Stream for the data stream</param>
        /// <param name="indexStream">A Stream for the index stream</param>
        public ShapefileMemoryStreamProviderRegistry(Stream shapeStream, Stream dataStream, Stream indexStream)
        {
            ShapeStream = new MemoryStreamProvider(StreamTypes.Shape, shapeStream);
            DataStream = new MemoryStreamProvider(StreamTypes.Data, dataStream);
            IndexStream = new MemoryStreamProvider(StreamTypes.Index, indexStream);
        }

        /// <summary>
        /// Creates an instance of this class
        /// </summary>
        /// <param name="shapeStream">A stream provider for the shape stream</param>
        /// <param name="dataStream">A stream provider for the data stream</param>
        /// <param name="indexStream">A stream provider for the shape index stream</param>
        /// <param name="validateShapeProvider">A value indicating that the <paramref name="shapeStream"/> must be validated</param>
        /// <param name="validateDataProvider">A value indicating that the <paramref name="dataStream"/> must be validated</param>
        /// <param name="validateIndexProvider">A value indicating that the <paramref name="indexStream"/> must be validated</param>
        public ShapefileMemoryStreamProviderRegistry(IStreamProvider shapeStream, IStreamProvider dataStream, IStreamProvider indexStream,
            bool validateShapeProvider = false, bool validateDataProvider = false, bool validateIndexProvider = false)
        {
            if (validateShapeProvider && shapeStream == null)
                throw new ArgumentNullException("shapeStream");
            if (validateDataProvider && dataStream == null)
                throw new ArgumentNullException("dataStream");
            if (validateIndexProvider && indexStream == null)
                throw new ArgumentNullException("indexStream");

            ShapeStream = shapeStream;
            DataStream = dataStream;
            IndexStream = indexStream;
        }

        private IStreamProvider DataStream { get; set; }

        private IStreamProvider ShapeStream { get; set; }

        private IStreamProvider IndexStream { get; set; }

        public IStreamProvider this[string streamType]
        {
            get
            {
                switch (streamType)
                {
                    case StreamTypes.Data:
                        return DataStream;
                    case StreamTypes.Index:
                        return IndexStream;
                    case StreamTypes.Shape:
                        return ShapeStream;
                    default:
                        throw new ArgumentException(
                            string.Format("Unknown stream type: '{0}'", streamType),
                            "streamType");
                }
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    ((MemoryStreamProvider)this.ShapeStream).Dispose();
                    ((MemoryStreamProvider)this.DataStream).Dispose();
                    ((MemoryStreamProvider)this.IndexStream).Dispose();
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
