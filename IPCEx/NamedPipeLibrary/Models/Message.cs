using System.Text;

namespace NamedPipeLibrary
{
    public class MessageInformation
    {
        public byte[] Buffer { get; private set; }
        public StringBuilder Message { get; private set; }
        public int BufferSize { get; private set; }

        public MessageInformation(int bufferSize)
        {
            BufferSize = bufferSize;
            Buffer = new byte[BufferSize];
            Message = new StringBuilder();
        }


    }
}
