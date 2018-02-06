using System;
using System.IO.Pipes;
using Common;
using NamedPipeLibrary.StreamOperation;

namespace NamedPipeLibrary.BasePipe
{
    internal interface INamedPipe : IDisposable
    {
        event EventHandler<MessageReceivedEventArgs> MessageReceived;
        event EventHandler OnDisconnect;

        void SendMessage(string message);
        void Start();
        void Stop();
    }
    public interface IPipeStream : IDisposable
    {
        event EventHandler<MessageReceivedEventArgs> PipeMessageReceived;

        void BeginRead(MessageInformation messageInformation);

        void BeginWrite(string message);

    }
    internal interface IClientNamedPipe : INamedPipe
    {
        event EventHandler Connect;
    }

    internal interface IServerNamedPipe : INamedPipe
    {
        event EventHandler OnConnected;
    }

    internal abstract class BaseNamedPipe : INamedPipe,IPipeStream
    {
        #region Variables

        protected PipeStream _pipeStream;
        protected string _pipeName;
       

        #endregion

        #region Constructor
        public BaseNamedPipe(string pipeName,PipeStream pipeStream)
        {
            _pipeName = pipeName;
            _pipeStream = pipeStream;
        }
        #endregion


        #region Public Methods
        public void Dispose()
        {
            CloseStreamInternal();
            _pipeStream.Dispose();
        }
        public void SendMessage(string message)
        {
            try
            {
                _pipeStream.Write(message);
            }
            catch (Exception ex)
            {
                Logger.DoLogEntry(ex.ToString());
            }
        }

        public abstract void Start();
        public abstract void Stop();
        #endregion

        #region Protected Methods

        protected abstract void IntializeStream();
        protected abstract void FinalizeStream();
        protected void UnRegisterPipe_streamOperationsEvents()
        {
            _streamOperation.PipeDisconnected -= streamOperation_Disconnected;
            _streamOperation.MessageReceived -= streamOperation_MessageReceived;
        }
        protected void RegisterPipe_streamOperationsEvents()
        {
            _streamOperation.PipeDisconnected += streamOperation_Disconnected;
            _streamOperation.MessageReceived += streamOperation_MessageReceived;
        }
        protected void InitializePipeStreamAndEvent()
        {
            _streamOperation = new NamedPipeStream(_pipeStream);

            RegisterPipe_streamOperationsEvents();
        }
        protected void CloseStreamInternal()
        {
            if (_pipeStream != null)
            {
                _pipeStream.Close();
                _pipeStream.Dispose();
            }
        }
        #endregion

        #region Private Methods
        private void streamOperation_Disconnected(object sender, EventArgs e)
        {
            FinalizeStream();

            if (OnDisconnect != null)
            {
                OnDisconnect.Invoke(sender, e);
            }
        }

        private void streamOperation_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            if (MessageReceived != null)
            {
                MessageReceived.Invoke(this, e);
            }
        }

        public void BeginRead(MessageInformation messageInformation)
        {
            throw new NotImplementedException();
        }

        public void BeginWrite(string message)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
