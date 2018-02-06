using System;
using System.Windows.Forms;
using NamedPipeLibrary;
using MemoryMappedFileManager;
using Client.IOCContainer;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]

namespace MemoryMappedFileManagerWindowsApp
{
    static class Program
    {
        private static INamedPipeClient _namedPipeClientStream;
        private static IMemoryMapFileCommunicator _memoryMapFileCommunicator;

        [STAThread]
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Bootstrap.Start();
            _namedPipeClientStream = Bootstrap.GetInstance<int,INamedPipeClient>
                (10000,"NamedPipeClient");

            _memoryMapFileCommunicator = Bootstrap.GetInstance<string, long, IMemoryMapFileCommunicator>("testMMP", 4096, "MemoryMapFileCommunicator");

            Application.Run(new frmMain(_namedPipeClientStream, _memoryMapFileCommunicator));
        }

    }
}
