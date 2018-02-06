using System;

namespace MemoryMappedFileManager
{
    public class MemoryMappedDataReceivedEventArgs : EventArgs
    {
        public byte[] Data { get; private set; }

        internal MemoryMappedDataReceivedEventArgs(byte[] data, long length)
        {
            if (data != null)
            {
                Data = new byte[length];
                Array.Copy(data, Data, length);
            }
        }
    }
}
