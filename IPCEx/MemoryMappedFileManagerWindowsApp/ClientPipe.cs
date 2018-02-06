using System.IO.Pipes;

namespace MemoryMappedFileManagerWindowsApp
{
    public sealed class ClientPipe
    {
        private static readonly object padlock = new object();
        private static PipeStream _namedPipeStream = null;

        public static PipeStream initializeNamedPipeStream()
        {
            PipeStream pipeStream= new NamedPipeClientStream(".", "TestPipe",
                PipeDirection.InOut, 
                PipeOptions.Asynchronous | PipeOptions.WriteThrough);
            return pipeStream;
        }


        public static PipeStream NamedPipeStream
        {
            get {
                lock (padlock)
                {
                    if (_namedPipeStream == null)
                    {
                        _namedPipeStream = initializeNamedPipeStream();
                    }
                    return _namedPipeStream;
                }
            }
        }
    }
}
