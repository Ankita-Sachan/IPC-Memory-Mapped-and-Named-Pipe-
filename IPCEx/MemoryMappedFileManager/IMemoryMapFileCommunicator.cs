using System;

namespace MemoryMappedFileManager
{
    public interface IMemoryMapFileCommunicator
    {
        event EventHandler<MemoryMappedDataReceivedEventArgs> DataReceived;
        int ReadPosition { get; set; }
        int WritePosition { get; set; }

        void StartReader();
        void Write(byte[] data);
        void Write(string message);
       
        void CloseReader();
        void Dispose();
    }
}
