using System;
using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;
using Moq;
using NamedPipeLibrary;
using Xunit;


namespace NamedPipeLibraryTest
{

    public class NamedPipeServerTest
    {
        #region Variables
        PipeSecurity _pipeSecurity;
        #endregion

        #region Constructor
        public NamedPipeServerTest()
        {
            _pipeSecurity = initializePipeSecurity();
        }
        #endregion


        #region Test Methods
        [Fact]
        public void Start_ServerIsNotUp_VerifyServerIsWaitingForConnection()
        {
            //Arrange
            string pipeName = Guid.NewGuid().ToString();
            Mock<INamedPipeStream> mockPipeStreamServer;
            NamedPipeServerStub namedPipeServer;
            //Act
            StartPipeServer(pipeName, out mockPipeStreamServer, out namedPipeServer);

            //Assert
            Assert.True(namedPipeServer.Waiting);
        }


        [Fact]
        public void Start_WaitingForConnection_VerifyConnectionDone()
        {
            //Arrange
            string pipeName = Guid.NewGuid().ToString();
            Mock<INamedPipeStream> mockPipeStreamServer;
            Mock<INamedPipeStream> mockPipeStreamClient;

            NamedPipeServerStub namedPipeServer;
            NamedPipeClientStub namedPipeClient;

            //Act
            StartPipeServer(pipeName, out mockPipeStreamServer, out namedPipeServer);
            StartPipeClient(pipeName, out mockPipeStreamClient, out namedPipeClient);

            //Assert
            Assert.False(namedPipeServer.Waiting);
        }

        [Fact]
        public void ReadMessage_ConnectedToClient_VerifyBeginReadCalled()
        {
            //Arrange
            string pipeName = Guid.NewGuid().ToString();
            Mock<INamedPipeStream> mockPipeStreamServer;
            Mock<INamedPipeStream> mockPipeStreamClient;

            NamedPipeServerStub namedPipeServer;
            NamedPipeClientStub namedPipeClient;


            StartPipeServer(pipeName, out mockPipeStreamServer, out namedPipeServer);
            mockPipeStreamServer.Setup(x => x.BeginRead(It.IsAny<MessageInformation>()));

            StartPipeClient(pipeName, out mockPipeStreamClient, out namedPipeClient);

            //Act
            namedPipeClient.SendMessage("test");
            //Assert
            mockPipeStreamServer.Verify(x => x.BeginRead(It.IsAny<MessageInformation>()), Times.Once);
        }

        [Fact]
        public void Stop_ServerConnectedWithClient_VerifyDisposeCalled()
        {
            //Arrange
            string pipeName = Guid.NewGuid().ToString();
            Mock<INamedPipeStream> mockPipeStreamServer;
            Mock<INamedPipeStream> mockPipeStreamClient;

            NamedPipeServerStub namedPipeServer;
            NamedPipeClientStub namedPipeClient;

           //Act
            StartPipeServer(pipeName, out mockPipeStreamServer, out namedPipeServer);
            StartPipeClient(pipeName, out mockPipeStreamClient, out namedPipeClient);

            mockPipeStreamServer.Setup(x => x.IsConnected()).Returns(true);

            //Act
            namedPipeServer.Stop();

            //Assert
            Assert.False(namedPipeServer.Waiting);
            mockPipeStreamServer.Verify(x => x.Dispose(), Times.Once);
        }
        [Fact]
        public void Stop_ServerIsUpClientNotConnected_VerifyDisposeCalled()
        {
            //Arrange
            string pipeName = Guid.NewGuid().ToString();
            Mock<INamedPipeStream> mockPipeStreamServer;
            NamedPipeServerStub namedPipeServer;

            StartPipeServer(pipeName, out mockPipeStreamServer, out namedPipeServer);
            mockPipeStreamServer.Setup(x => x.Dispose());
            
            //Act
            namedPipeServer.Stop();

            //Assert
            Assert.False(namedPipeServer.Waiting);
            mockPipeStreamServer.Verify(x => x.Dispose(), Times.Once);
        }
        #endregion

        #region Private Methods
        private void createServer(string pipeName, out Mock<INamedPipeStream> mockPipeStreamServer, out NamedPipeServerStub namedPipeServer)
        {
            PipeStream _namedPipeServerStream;

            _namedPipeServerStream = initializeServerStream(_pipeSecurity, pipeName);
            mockPipeStreamServer = setUpMockOfPipeStreamForServer(_namedPipeServerStream);
            namedPipeServer = startPipeServer(mockPipeStreamServer);
        }

        private void createClient(string pipeName, out Mock<INamedPipeStream> mockPipeStreamClient, out NamedPipeClientStub namedPipeClient)
        {
            PipeStream _namedPipeClientStream;
            _namedPipeClientStream = initializeClientStream(pipeName);
            mockPipeStreamClient = setUpMockOfPipeStreamForClient(_namedPipeClientStream);
            namedPipeClient = startPipeClient(mockPipeStreamClient);
        }
        private PipeStream initializeServerStream(PipeSecurity pipeSecurity, string pipeName)
        {
            return new NamedPipeServerStream(pipeName,
                                  PipeDirection.InOut, 1,
                                  PipeTransmissionMode.Message,
                                  PipeOptions.Asynchronous | PipeOptions.WriteThrough,
                                  6, 6,
                                  pipeSecurity);
        }
        private static PipeSecurity initializePipeSecurity()
        {
            PipeSecurity pipeSecurity = new PipeSecurity();
            pipeSecurity.AddAccessRule(new PipeAccessRule(WindowsIdentity.GetCurrent().User, PipeAccessRights.FullControl, AccessControlType.Allow));
            pipeSecurity.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow));
            return pipeSecurity;
        }
        private PipeStream initializeClientStream(string pipeName)
        {
            return new NamedPipeClientStream(".", pipeName,
               PipeDirection.InOut,
               PipeOptions.Asynchronous | PipeOptions.WriteThrough);
        }
        private static NamedPipeServerStub startPipeServer(Mock<INamedPipeStream> mockPipeStreamServer)
        {
            NamedPipeServerStub namedPipeServer = new NamedPipeServerStub(mockPipeStreamServer.Object);
            return namedPipeServer;
        }
        private static NamedPipeClientStub startPipeClient(Mock<INamedPipeStream> mockPipeStreamClient)
        {
            NamedPipeClientStub namedPipeClient = new NamedPipeClientStub(mockPipeStreamClient.Object);
            //Act
            return namedPipeClient;
        }
        private Mock<INamedPipeStream> setUpMockOfPipeStreamForServer(PipeStream _namedPipeServerStream)
        {
            Mock<INamedPipeStream> mockPipeStreamServer = new Mock<INamedPipeStream>(MockBehavior.Default);
            mockPipeStreamServer.Setup(x => x.PipeStream).Returns(_namedPipeServerStream);
            mockPipeStreamServer.Setup(x => x.IsConnected()).Returns(false);
            mockPipeStreamServer.Raise(x => x.Disconnected += null, new EventArgs());
            mockPipeStreamServer.Raise(x => x.MessageReceived += null, new EventArgs());
            return mockPipeStreamServer;
        }
        private Mock<INamedPipeStream> setUpMockOfPipeStreamForClient(PipeStream _namedPipeClientStream)
        {
            Mock<INamedPipeStream> mockPipeStreamClient = new Mock<INamedPipeStream>(MockBehavior.Strict);
            mockPipeStreamClient.Setup(x => x.PipeStream).Returns(_namedPipeClientStream);
            mockPipeStreamClient.Setup(x => x.IsConnected()).Returns(true).Callback(() => Thread.Sleep(500));
            mockPipeStreamClient.Raise(x => x.Disconnected += null, new EventArgs());
            return mockPipeStreamClient;
        }
        private void StartPipeClient(string pipeName, out Mock<INamedPipeStream> mockPipeStreamClient, out NamedPipeClientStub namedPipeClient)
        {
            createClient(pipeName, out mockPipeStreamClient, out namedPipeClient);

            namedPipeClient.Start();
        }

        private void StartPipeServer(string pipeName, out Mock<INamedPipeStream> mockPipeStreamServer, out NamedPipeServerStub namedPipeServer)
        {
            createServer(pipeName, out mockPipeStreamServer, out namedPipeServer);
            namedPipeServer.Start();
        }
        #endregion
    }

    public class NamedPipeServerStub : NamedPipeServer
    {
        public NamedPipeServerStub(INamedPipeStream namedPipeStream) : base(namedPipeStream)
        {
        }
        public bool Waiting { get { return _isWaiting; } }
    }
}
