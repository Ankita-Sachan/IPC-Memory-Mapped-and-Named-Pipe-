using System;
using System.IO.Pipes;
using Common;
using MemoryMappedFileManager;
using NamedPipeLibrary;

namespace IOCContainer
{
    class Program
    {
        private static INamedPipeServer _namedPipeServerStream;
        private static IMemoryMapFileCommunicator _memoryMappedFileCommunicator;

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();
            Console.WriteLine("Server is Up.");
            Console.WriteLine("---Press enter to disconnect the server---");
            Bootstrap.Start();

            setMemoryMapFileCommunicator();
            startPipeServer();
            Console.ReadLine();

            _memoryMappedFileCommunicator.CloseReader();
            _namedPipeServerStream.Stop();
        }

        private static void startPipeServer()
        {
            initializePipeServer();
            initializeHandlers();
            _namedPipeServerStream.Start();
        }

        private static void initializePipeServer()
        {
            PipeStream pipeStreamobject = ServerPipe.NamedPipeStream;

            _namedPipeServerStream = Bootstrap.GetInstance<INamedPipeServer>
                ("NamedPipeServer");
        }

        private static void initializeHandlers()
        {

            _namedPipeServerStream.ServerPipeDisconnected += server_OnDisconnected;
            _namedPipeServerStream.MessageReceived += server_OnMessageReceived;
        }

        private static void server_OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Console.WriteLine(e.Message.ToString());
        }

        private static void server_OnDisconnected(object sender, EventArgs e)
        {
            Logger.Error("Server disconnected.");
        }

        private static void setMemoryMapFileCommunicator()
        {
            _memoryMappedFileCommunicator = Bootstrap.GetInstance<string, long, IMemoryMapFileCommunicator>("testMMP", 4096, "MemoryMapFileCommunicator");


            _memoryMappedFileCommunicator.ReadPosition = 0;
            _memoryMappedFileCommunicator.WritePosition = 2000;


            _memoryMappedFileCommunicator.DataReceived += new EventHandler<MemoryMappedDataReceivedEventArgs>(communicator_DataReceived);
            _memoryMappedFileCommunicator.StartReader();
        }

        private static void communicator_DataReceived(object sender, MemoryMappedDataReceivedEventArgs e)
        {
            var receivedMessage = System.Text.Encoding.UTF8.GetString(e.Data);
            Console.WriteLine(receivedMessage);

            Logger.Error(System.Reflection.MethodBase.GetCurrentMethod() + " : Data Recieved.");
        }
    }
}
