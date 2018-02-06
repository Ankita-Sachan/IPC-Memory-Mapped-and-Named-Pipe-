using System.IO.Pipes;
using LightInject;
using MemoryMappedFileManager;
using MemoryMappedFileManagerWindowsApp;
using NamedPipeLibrary;

namespace Client.IOCContainer
{
    public class Bootstrap
    {
        public static ServiceContainer container;
        public static void Start()
        {
            container = new ServiceContainer();
            container.Register<string, long, IMemoryMapFileCommunicator>((factory, mapName, capacity) => new MemoryMapFileCommunicator(mapName, capacity), "MemoryMapFileCommunicator");

            container.Register<PipeStream, INamedPipeStream>((factory, pipeStream) =>
            new NamedPipeStream(pipeStream), "NamedPipeStream");

            container.Register<int,INamedPipeClient>((factory,timeout) =>
            new NamedPipeClient(timeout,
                 factory.GetInstance<PipeStream,INamedPipeStream>(ClientPipe.NamedPipeStream, "NamedPipeStream")), "NamedPipeClient");
        }
        public static TService GetInstance<TService>(string serviceName)
        {
            return container.GetInstance<TService>(serviceName);
        }

        public static TService GetInstance<T1, T2, TService>(T1 arg1, T2 arg2, string serviceName)
        {
            return container.GetInstance<T1, T2, TService>(arg1, arg2, serviceName);

        }
        
        public static TService GetInstance<T1, TService>(T1 arg1, string serviceName)
        {
            return container.GetInstance<T1, TService>(arg1, serviceName);
        }
    }
}
