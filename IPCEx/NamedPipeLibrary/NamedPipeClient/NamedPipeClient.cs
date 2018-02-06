using System;
using System.IO.Pipes;
using System.Threading.Tasks;
using Common;

namespace NamedPipeLibrary
{
    public class NamedPipeClient : INamedPipeClient
    {
        #region Variable
        private NamedPipeClientStream _pipeClient;
        private INamedPipeStream _pipeStream;
        private readonly int _timeOut;
        protected bool _isConnected;
        #endregion

        #region Events

        public event EventHandler Connect;
        public event EventHandler Disconnected;

        #endregion

        #region Constructor

        public NamedPipeClient(int timeOut, INamedPipeStream namedPipeStream)
        {
            _pipeClient = namedPipeStream.PipeStream as NamedPipeClientStream;
            _pipeStream = namedPipeStream;
            _timeOut = timeOut;
            registerEventHandlers();
        }

        #endregion

        #region Public Methods
        public void Start()
        {
            try
            {
                if (!_isConnected)
                {
                    _pipeClient.Connect(_timeOut);
                    Logger.Info(System.Reflection.MethodBase.GetCurrentMethod().Name + " : Client Connected.");

                    if (_pipeStream.IsConnected())
                    {
                        Connect?.Invoke(this, null);
                    }
                }
                _isConnected = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        public void Stop()
        {
            finalizeClient();
        }
        public Task SendMessage(string message)
        {
            Task task=null;
            try
            {
               task= _pipeStream.BeginWrite(message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return task;
        }
        public void Dispose()
        {
            _pipeClient?.Dispose();
        }

        #endregion

        #region Private Methods
        private void registerEventHandlers()
        {
            _pipeStream.Disconnected += StreamOperation_Disconnected;
        }
        private void finalizeClient()
        {
            try
            {
                finalizeEventHandlers();
                _pipeStream?.Dispose();
                _isConnected = false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                _pipeClient?.Close();
                _pipeClient?.Dispose();
            }
        }
        private void finalizeEventHandlers()
        {
            _pipeStream.Disconnected -= StreamOperation_Disconnected;
        }
        private void StreamOperation_Disconnected(object sender, EventArgs e)
        {
            finalizeClient();

            if (Disconnected != null)
            {
                Disconnected.Invoke(sender, e);
            }
        }
       
        #endregion

    }
}
