using System;
using System.Threading.Tasks;

namespace NamedPipeLibrary
{
    public interface INamedPipeServer : IDisposable
    {
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler ServerPipeDisconnected;
        void ReadMessage();
        void Start();
        void Stop();
    }
}
