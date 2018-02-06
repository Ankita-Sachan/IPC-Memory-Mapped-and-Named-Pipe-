using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO.Pipes;
using NamedPipeLibrary;

namespace NamedPipeLibraryTest
{
    /// <summary>
    /// Summary description for NamedPipeStream
    /// </summary>
    [TestClass]
    public class NamedPipeStreamTest
    {
        private PipeStream _mockPipeStream;
        NamedPipeStream _namedPipeStream;
        public NamedPipeStreamTest()
        {
            
            _namedPipeStream =new NamedPipeStream(_mockPipeStream);
        }
        public void BeginRead()
        {
            
            _namedPipeStream.BeginRead(new MessageInformation(6));
        }
    }
}
