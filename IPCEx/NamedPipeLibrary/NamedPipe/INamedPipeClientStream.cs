using System;

namespace NamedPipeLibrary.NamedPipe
{
    public interface INamedPipeClientStream : IDisposable
    {
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler OnConnect;
        
        void SendMessage(string message);
        void Start();
        void Stop();
    }
}
