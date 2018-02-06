using NamedPipeLibrary;

namespace MemoryMappedFileManagerWindowsApp
{
    class NamedPipeBasedClient
    {

        public NamedPipeBasedClient()
        {

        }

        public void SendMessage(string message)
        {
            if (!string.IsNullOrEmpty(message))
            {
                //INamedPipeClientStream client = new NamedPipeClient("Test-Pipe1");

                //client.Start();
                //client.SendMessage(message);
                //client.Stop();
            }
        }
    }
}