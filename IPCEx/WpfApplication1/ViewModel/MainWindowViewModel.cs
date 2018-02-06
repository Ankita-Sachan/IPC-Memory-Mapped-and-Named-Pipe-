using System;
using System.Collections.ObjectModel;
using System.IO.Pipes;
using Common;
using IOCContainer;
using MemoryMappedFileManager;
using NamedPipeLibrary;

namespace WpfApplication1.ViewModel
{
    class MainWindowViewModel : ViewModelBase
    {
        private static INamedPipeServer _namedPipeServerStream;
        private static IMemoryMapFileCommunicator _memoryMappedFileCommunicator;
        private ObservableCollection<string> _data;
        public MainWindowViewModel(IMemoryMapFileCommunicator memoryMapFileCommunicator, INamedPipeServer namedPipeServer)
        {
            _namedPipeServerStream = namedPipeServer;
            _memoryMappedFileCommunicator = memoryMapFileCommunicator;
            setMemoryMapFileCommunicator();
            startPipeServer();
            _data = new ObservableCollection<string>();
        }

        public ObservableCollection<string> Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                this.SetPropertyChanged("Data");
            }
        }
        private void startPipeServer()
        {
            initializePipeServer();
            initializeHandlers();
            _namedPipeServerStream.Start();
        }

        private void initializePipeServer()
        {
            PipeStream pipeStreamobject = ServerPipe.NamedPipeStream;

            _namedPipeServerStream = Bootstrap.GetInstance<INamedPipeServer>
                ("NamedPipeServer");
        }

        private void initializeHandlers()
        {

            _namedPipeServerStream.ServerPipeDisconnected += server_OnDisconnected;
            _namedPipeServerStream.MessageReceived += server_OnMessageReceived;
        }

        private void server_OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            _data.Add("Named Pipe : " +e.Message.ToString());
            Data = _data;
        }

        private void server_OnDisconnected(object sender, EventArgs e)
        {
            Logger.Error("Server disconnected.");
        }

        private void setMemoryMapFileCommunicator()
        {
            _memoryMappedFileCommunicator = Bootstrap.GetInstance<string, long, IMemoryMapFileCommunicator>("testMMP", 4096, "MemoryMapFileCommunicator");


            _memoryMappedFileCommunicator.ReadPosition = 0;
            _memoryMappedFileCommunicator.WritePosition = 2000;


            _memoryMappedFileCommunicator.DataReceived += new EventHandler<MemoryMappedDataReceivedEventArgs>(communicator_DataReceived);
            _memoryMappedFileCommunicator.StartReader();
        }

        private void communicator_DataReceived(object sender, MemoryMappedDataReceivedEventArgs e)
        {
            Logger.Info(System.Reflection.MethodBase.GetCurrentMethod() + " : Data Recieved.");

            var receivedMessage = System.Text.Encoding.UTF8.GetString(e.Data);
            _data.Add("Memory Mapped : " +receivedMessage);
            Data = _data;
        }
    }
}
