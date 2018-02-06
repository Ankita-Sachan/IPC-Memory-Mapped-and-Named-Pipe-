using System;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace NamedPipeLibrary
{
    public class NamedPipeServer : INamedPipeServer
    {
        #region Variables

        private NamedPipeServerStream _pipeServer;
        private INamedPipeStream _pipeStream;
        private bool _isClosing;
        protected bool _isWaiting;
        private readonly SynchronizationContext _synchronizationContext;

        #endregion

        #region Events

        public event EventHandler ServerPipeDisconnected;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        #endregion
        #region Consructor
        public NamedPipeServer(INamedPipeStream namedPipeStream)
        {
            _pipeServer = namedPipeStream.PipeStream as NamedPipeServerStream;
            _pipeStream = namedPipeStream;
            registerEventHandlers();
            _synchronizationContext = SynchronizationContext.Current;
        } 
        #endregion

        #region Public Method       
        public void Start()
        {
            try
            {
                if (!_pipeStream.IsConnected())
                {
                    _isClosing = false;
                    _isWaiting = true;
                    _pipeServer.BeginWaitForConnection(WaitForConnectionCallBack, _pipeServer);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        public void Stop()
        {
            _isClosing = true;
            _isWaiting = false;
            finalizeServer();
            
        }
        public void ReadMessage()
        {
            _pipeStream.BeginRead(new MessageInformation(6));
        }
        public void Dispose()
        {
            _pipeServer?.Dispose();
        }
        #endregion

        #region Private Methods

        private void WaitForConnectionCallBack(IAsyncResult result)
        {
            try
            {
                if (_isClosing)
                {
                    return;
                }
                Logger.Info(System.Reflection.MethodBase.GetCurrentMethod().Name + " : Server Connected.");

                _pipeServer.EndWaitForConnection(result);
                ReadMessage();
                _isWaiting = false;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }

        }        
        private void registerEventHandlers()
        {
            _pipeStream.Disconnected += StreamOperation_Disconnected;
            _pipeStream.MessageReceived += StreamOperation_MessageReceived;
        }
        private void finalizeServer()
        {
            try
            {
                if (_pipeStream.IsConnected())
                {
                    _pipeServer?.Disconnect();
                    ServerPipeDisconnected?.Invoke(this, null);
                }
                finalizeEventHandlers();
                _pipeStream.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            finally
            {
                _pipeServer?.Close();
                _pipeServer?.Dispose();
            }
        }
        private void finalizeEventHandlers()
        {
            _pipeStream.MessageReceived -= StreamOperation_MessageReceived;
            _pipeStream.Disconnected -= StreamOperation_Disconnected;
        }
        private void StreamOperation_Disconnected(object sender, EventArgs e)
        {
            finalizeServer();
            ServerPipeDisconnected?.Invoke(this, null);
        }
        private void StreamOperation_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            SendOrPostCallback callBack = new SendOrPostCallback(OnDataRecieved);
            _synchronizationContext.Post(callBack, e);
            
        }
        public void OnDataRecieved(object state) {
            MessageReceived.Invoke(this, (MessageReceivedEventArgs)state);
        }
        #endregion
    }
}
