using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NamedPipeLibrary
{
    public interface INamedPipeServerStream
    {
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler OnDisconnect;

        void SendMessage(string message);
        void Start();
        void Stop();
    }
}
