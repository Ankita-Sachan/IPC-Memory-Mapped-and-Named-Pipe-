using System;
using System.Threading.Tasks;

namespace NamedPipeLibrary
{

    public interface INamedPipeClient : IDisposable
    {
        event EventHandler Connect;
        event EventHandler Disconnected;
        Task SendMessage(string message);
        void Start();
        void Stop();
    }
}
