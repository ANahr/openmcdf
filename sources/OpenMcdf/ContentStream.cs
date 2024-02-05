using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OpenMcdf
{
    public sealed class ContentStream : Stream
    {
        private CompoundFile compoundFile;
        private IDirectoryEntry directoryEntry;
        private StreamView streamView;
        private long position = 0;

        internal ContentStream(CompoundFile compoundFile, IDirectoryEntry directoryEntry)
        {
            this.compoundFile = compoundFile;
            this.directoryEntry = directoryEntry;
            streamView = compoundFile.CreateStreamView(directoryEntry);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            // nothing to do;
        }

        public override long Length => directoryEntry.Size;

        public override long Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            count = (int)Math.Min((long)(buffer.Length - offset), (long)count);
            count = (int)Math.Min(Length - Position, count);
            if (count == 0) return 0;

            streamView.Seek(position, SeekOrigin.Begin);
            var readBytes = streamView.Read(buffer, offset, count);
            position += readBytes;
            return readBytes;
        }

        public override int Read(Span<byte> buffer)
        {
            streamView.Seek(position, SeekOrigin.Begin);
            var readBytes = streamView.Read(buffer);
            position += readBytes;
            return readBytes;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    position = offset;
                    break;
                case SeekOrigin.Current:
                    position += offset;
                    break;
                case SeekOrigin.End:
                    position -= offset;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(origin));
            }

            return position;
        }

        public override void SetLength(long value)
        {
            //this.cfStream.Resize(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            //this.cfStream.Write(buffer, position, offset, count);
            //position += count;
        }

        public override void Close()
        {
            // Do nothing
        }
    }
}
