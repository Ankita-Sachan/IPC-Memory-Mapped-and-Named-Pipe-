using System;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using Common;
using NamedPipeLibrary.Utilities;

namespace NamedPipeLibrary
{
    public class NamedPipeStream : INamedPipeStream
    {
        #region Variables

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;
        public event EventHandler Disconnected;
        private PipeStream _pipeStream;

        #endregion

        #region Constructor
        public NamedPipeStream(PipeStream stream)
        {
            _pipeStream = stream;
        }
        #endregion

        #region Property

        public PipeStream PipeStream
        {
            get
            {
                return _pipeStream;
            }

            set
            {
                _pipeStream = value;
            }
        }
        #endregion

        #region Public Methods

        public void BeginRead(MessageInformation message)
        {
            try
            {
                _pipeStream.BeginRead(message.Buffer, 0, message.BufferSize, EndReadCallBack, message);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        private void EndReadCallBack(IAsyncResult result)
        {
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
                    var message = messageInfo.Message.ToString().TrimEnd('\0');

                    MessageReceived?.Invoke(this, new MessageReceivedEventArgs(message));
                    BeginRead(new MessageInformation(6));
                }
            }
            else // When no bytes were read, it can mean that the client have been disconnected
            {
                if (!_pipeStream.IsConnected)
                {
                    Disconnected?.Invoke(this, null);
                }
            }
        }
        public Task BeginWrite(string message)
        {
            var taskCompletionSource = new TaskCompletionSource<TaskResult>();
            try
            {
                if (_pipeStream.IsConnected)
                {
                    var buffer = Encoding.UTF8.GetBytes(message);
                    _pipeStream.BeginWrite(buffer, 0, buffer.Length, asyncCallback =>
                     {
                         try
                         {
                             taskCompletionSource.SetResult(EndWriteCallBack(asyncCallback));
                         }
                         catch (Exception ex)
                         {
                             taskCompletionSource.SetException(ex);
                         }
                     }, null);
                }
                else
                {
                    Disconnected?.Invoke(this, null);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
            return taskCompletionSource.Task;
        }

        private TaskResult EndWriteCallBack(IAsyncResult result)
        {
            _pipeStream.EndWrite(result);
            _pipeStream.Flush();
            return new TaskResult { IsSuccess = true };
        }

        public void Dispose()
        {
            try
            {
                if (_pipeStream != null)
                {
                    _pipeStream.Close();
                    _pipeStream.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
            }
        }
        public virtual bool IsConnected()
        {
            if (_pipeStream.IsConnected)
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}
