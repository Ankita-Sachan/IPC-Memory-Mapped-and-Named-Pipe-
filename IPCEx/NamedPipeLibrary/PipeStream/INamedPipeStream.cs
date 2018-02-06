using System;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace NamedPipeLibrary
{
    public interface INamedPipeStream :IDisposable
    {
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler Disconnected;

        PipeStream PipeStream { get; set; }
        void BeginRead(MessageInformation messageInformation);
        Task BeginWrite(string message);
        bool IsConnected();
    }
}
