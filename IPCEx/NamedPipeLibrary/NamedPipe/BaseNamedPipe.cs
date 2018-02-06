using System;
using System.IO;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using Common;
using NamedPipeLibrary.StreamOperation;

namespace NamedPipeLibrary.NamedPipe
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
        event EventHandler<MessageReceivedEventArgs> MessageReceived;

        event EventHandler Disconnected;

        void BeginRead(MessageInformation messageInformation);

        void Write(string message);

    }
    internal interface IClientNamedPipe : INamedPipe
    {
        event EventHandler OnConnect;
    }

    internal interface IServerNamedPipe : INamedPipe
    {
    }

    internal abstract class BaseNamedPipe : INamedPipe
    {
        #region Variables

        protected PipeStream _pipeStream;
        protected IPipeStream _streamOperation;
        protected string _pipeName;
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler OnDisconnect;

        #endregion

        #region Constructor
        public BaseNamedPipe(string pipeName,IPipeStream pipestream)
        {
            _pipeName = pipeName;
            _streamOperation = pipestream;
        }
        #endregion


        #region Public Methods
        public void Dispose()
        {
            CloseStreamInternal();
            _streamOperation.Dispose();
        }
        public void SendMessage(string message)
        {
            try
            {
                _streamOperation.Write(message);
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
            _streamOperation.Disconnected -= streamOperation_Disconnected;
            _streamOperation.MessageReceived -= streamOperation_MessageReceived;
        }
        protected void RegisterPipe_streamOperationsEvents()
        {
            _streamOperation.Disconnected += streamOperation_Disconnected;
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

        #endregion
    }
    public class NamedPipeStream : IPipeStream, IDisposable
    {
        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public event EventHandler Disconnected;

        private readonly PipeStream _pipeStream;

        public NamedPipeStream(PipeStream stream)
        {
            _pipeStream = stream;
        }

        public void BeginRead(MessageInformation message)
        {
            try
            {
                _pipeStream.BeginRead(message.Buffer, 0, Convert.ToInt32(message.Buffer), EndReadCallBack, message);

            }
            catch (Exception ex)
            {
                Logger.DoLogEntry(ex.ToString());
            }
        }

        public void Write(string message)
        {
            try
            {
                Logger.DoLogEntry("NamedPipeStream : Write and Message : " + message);
                if (_pipeStream.IsConnected)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    _pipeStream.Write(buffer, 0, buffer.Length);
                }
                else
                {
                    throw new Exception("pipe is not connected");
                }
            }
            catch (Exception ex)
            {
                Logger.DoLogEntry(ex.ToString());
            }
        }

        private void EndReadCallBack(IAsyncResult result)
        {
            Logger.DoLogEntry("NamedPipeStream :Start EndReadCallBack");
            var readBytes = _pipeStream.EndRead(result);

            if (readBytes > 0)
            {
                var messageInfo = (MessageInformation)result.AsyncState;
                messageInfo.Message.Append(Encoding.UTF8.GetString(messageInfo.Buffer, 0, readBytes));

                if (!_pipeStream.IsMessageComplete) // Message is not complete, continue reading
                {
                    BeginRead(messageInfo);
                }
                else
                {
                    Logger.DoLogEntry("NamedPipeStream : EndReadCallBack");
                    var message = messageInfo.Message.ToString().TrimEnd('\0');

                    if (MessageReceived != null)
                    {
                        MessageReceived.Invoke(this, new MessageReceivedEventArgs(message));
                    }
                    BeginRead(new MessageInformation(6));
                }
            }
            else // When no bytes were read, it can mean that the client have been disconnected
            {
                if (Disconnected != null)
                {
                    Disconnected.Invoke(this, null);
                }
            }
        }

        public void Dispose()
        {
            if (_pipeStream != null)
            {
                _pipeStream.Dispose();
                _pipeStream.Flush();
            }
        }
    }
    internal class ClientNamePipe : BaseNamedPipe, IClientNamedPipe
    {
        public event EventHandler OnConnect;

        public ClientNamePipe(string pipeName,IPipeStream pipeStream)
        : base(pipeName, pipeStream)
        {

        }

        public override void Start()
        {
            try
            {
                _pipeStream = new NamedPipeClientStream(".", _pipeName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
                _pipeStream.ReadMode = PipeTransmissionMode.Message;
                InitializePipeStreamAndEvent();
                (_pipeStream as NamedPipeClientStream).Connect();

                if (OnConnect != null)
                {
                    OnConnect.BeginInvoke(this, null, null, null);
                }
            }
            catch (InvalidOperationException ex) { Logger.DoLogEntry("Client Already Connected : " + ex.ToString()); }
            catch (IOException ex) { Logger.DoLogEntry("The server is connected to another client and the time-out period has expired." + ex.ToString()); }
            catch (Exception ex)
            {
                Logger.DoLogEntry(ex.ToString());
            }
        }

        public override void Stop()
        {
            FinalizeStream();
        }

        protected override void FinalizeStream()
        {
            try
            {
                UnRegisterPipe_streamOperationsEvents();
            }
            catch (Exception ex)
            {
                Logger.DoLogEntry("Server Pipe " + ex.ToString());
            }
            finally
            {
                CloseStreamInternal();
            }
        }

        protected override void IntializeStream()
        {
            _pipeStream = new NamedPipeClientStream(".", _pipeName, PipeDirection.InOut, PipeOptions.Asynchronous | PipeOptions.WriteThrough);
            InitializePipeStreamAndEvent();
        }


    }

    internal class ServerNamePipe :NamedPipeStream, BaseNamedPipe, IServerNamedPipe
    {

        private bool _isClosing;

        public ServerNamePipe(string pipeName)
            : base(pipeName)
        {

        }

        public override void Start()
        {
            createServerStream();
        }

        public override void Stop()
        {
            FinalizeStream();
        }

        protected override void IntializeStream()
        {
            try
            {
                _isClosing = false;
                createServerStream();
            }
            catch (InvalidOperationException)
            {
                Logger.DoLogEntry("The pipe was not opened asynchronously.-or-A pipe connection has already been established.-or-The pipe handle has not been set.");
            }
            catch (Exception ex) { Logger.DoLogEntry(ex.ToString()); }
        }

        private void createServerStream()
        {

            _streamOperation = new NamedPipeStream(_pipeStream);

            RegisterPipe_streamOperationsEvents();

            PipeSecurity pipeSecurity = new PipeSecurity();
            pipeSecurity.AddAccessRule(new PipeAccessRule(WindowsIdentity.GetCurrent().User, PipeAccessRights.FullControl, AccessControlType.Allow));
            pipeSecurity.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow));
            var namedPipeServerStream = new NamedPipeServerStream(_pipeName, PipeDirection.InOut, 1, PipeTransmissionMode.Message, PipeOptions.Asynchronous | PipeOptions.WriteThrough, 6, 6, pipeSecurity);
            _pipeStream = namedPipeServerStream;
            (_pipeStream as NamedPipeServerStream).BeginWaitForConnection(WaitForConnectionCallBack, null);
        }

        private void WaitForConnectionCallBack(IAsyncResult result)
        {

            if (_isClosing)
            {
                return;
            }

            (_pipeStream as NamedPipeServerStream).EndWaitForConnection(result);
            Logger.DoLogEntry("NamedPipeServer : BeginRead called");
            _streamOperation.BeginRead(new MessageInformation(6));
        }

        protected override void FinalizeStream()
        {
            try
            {
                _isClosing = true;
                UnRegisterPipe_streamOperationsEvents();

                if (_pipeStream != null && _pipeStream.IsConnected)
                {
                    (_pipeStream as NamedPipeServerStream).Disconnect();
                }
            }
            catch (Exception ex)
            {
                Logger.DoLogEntry("Server Pipe " + ex.ToString());
            }
            finally
            {
                CloseStreamInternal();
            }
        }
    }
}
