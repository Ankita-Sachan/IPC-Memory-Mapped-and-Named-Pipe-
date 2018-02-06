using System.IO.Pipes;
using System.Security.AccessControl;
using System.Security.Principal;

namespace IOCContainer
{
    public sealed class ServerPipe
    {
        private static readonly object padlock = new object();
        private static PipeStream _namedPipeStream;

        public static PipeStream initializeNamedPipeStream()
        {
            PipeStream namedPipeServerStream;
            PipeSecurity pipeSecurity = initializePipeSecurity();
            namedPipeServerStream = new NamedPipeServerStream("TestPipe",
                      PipeDirection.InOut, 2,
                      PipeTransmissionMode.Message,
                      PipeOptions.Asynchronous,
                      6, 6,
                      pipeSecurity);

            return namedPipeServerStream;
        }

        private static PipeSecurity initializePipeSecurity()
        {
            PipeSecurity pipeSecurity = new PipeSecurity();
            pipeSecurity.AddAccessRule(new PipeAccessRule(WindowsIdentity.GetCurrent().User, PipeAccessRights.FullControl, AccessControlType.Allow));
            pipeSecurity.AddAccessRule(new PipeAccessRule(new SecurityIdentifier(WellKnownSidType.AuthenticatedUserSid, null), PipeAccessRights.ReadWrite, AccessControlType.Allow));
            return pipeSecurity;
        }

        public static PipeStream NamedPipeStream
        {
            get
            {
                lock (padlock)
                {
                    if (_namedPipeStream == null)
                        _namedPipeStream = initializeNamedPipeStream();
                    return _namedPipeStream;
                }
            }
        }

    }
}
