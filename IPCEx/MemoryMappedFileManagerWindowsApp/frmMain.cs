using System;
using System.Windows.Forms;
using Common;
using MemoryMappedFileManager;
using NamedPipeLibrary;

namespace MemoryMappedFileManagerWindowsApp
{
    public partial class frmMain : Form
    {
        #region Variables

        private IMemoryMapFileCommunicator _communicator;
        private INamedPipeClient _namedPipeClientStream;
       
        #endregion

        #region Constructors

        public frmMain(INamedPipeClient namedPipeClientStream, IMemoryMapFileCommunicator communicator)
        {
            InitializeComponent();
            _namedPipeClientStream = namedPipeClientStream;
            _namedPipeClientStream.Connect += client_OnConnect;
            _namedPipeClientStream.Disconnected += client_OnDisconnected;

            _communicator = communicator;
        }

        private void client_OnDisconnected(object sender, EventArgs e)
        {
            Logger.Error("Message recieved by client ");
        }

        private void client_OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Logger.Error("Message recieved by client " + e.Message.ToString());
        }

        private void client_OnConnect(object sender, EventArgs e)
        {
            Logger.Error("client_OnConnect : Client connected.");
        }
        #endregion

        private void frmMain_Load(object sender, EventArgs e)
        {

            InitializeMemoryMapFileCommunicator();
            InitializeNamedPipeClient();
        }

        private void InitializeMemoryMapFileCommunicator()
        {
            
            _communicator.ReadPosition = 2000;
            _communicator.WritePosition = 0;

            _communicator.DataReceived += new EventHandler<MemoryMappedDataReceivedEventArgs>(communicator_DataReceived);
            _communicator.StartReader();
        }

        private void InitializeNamedPipeClient()
        {
            _namedPipeClientStream.Start();
           
        }

        private void communicator_DataReceived(object sender, MemoryMappedDataReceivedEventArgs e)
        {
            Logger.Error("Memory Map File recieved data.");
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            _communicator?.CloseReader();
            _communicator?.Dispose();
            _communicator = null;

            _namedPipeClientStream.Stop();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                string message = txtData.Text;
                if (!string.IsNullOrEmpty(message))
                {
                    if (rdbNamedPipe.Checked)
                    {
                        _namedPipeClientStream.SendMessage(message);
                    }
                    if (rdbMemoryMap.Checked)
                    {
                        var data = System.Text.Encoding.UTF8.GetBytes(message);
                        _communicator.Write(data);

                    }
                }
            }
            catch (Exception ex) {
                Logger.Error(ex); }
        }
    }
}
