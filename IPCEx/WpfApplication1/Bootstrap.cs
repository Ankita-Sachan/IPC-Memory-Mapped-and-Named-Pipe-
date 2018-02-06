using System.IO.Pipes;
using LightInject;
using MemoryMappedFileManager;
using NamedPipeLibrary;
using WpfApplication1;

namespace IOCContainer
{
    public sealed class Bootstrap
    {
        public static ServiceContainer container;
        public static void Start()
        {
            container = new ServiceContainer();
            container.Register<string, long, IMemoryMapFileCommunicator>((factory, mapName, capacity) => new MemoryMapFileCommunicator(mapName, capacity), "MemoryMapFileCommunicator");
            PipeStream obj= ServerPipe.NamedPipeStream;

            container.Register<PipeStream, INamedPipeStream>((factory, pipeStream) => 
            new NamedPipeStream(pipeStream), "NamedPipeStream");

           
            container.Register<INamedPipeServer>((factory) => 
            new NamedPipeServer(
                factory.GetInstance<PipeStream, INamedPipeStream>(obj, "NamedPipeStream")),
                "NamedPipeServer");

            container.Register<MainWindow>();
        }
        public static TService GetInstance<TService>()
        {
            return container.GetInstance<TService>();
        }
        public static TService GetInstance<TService>(string serviceName)
        {
            return container.GetInstance<TService>(serviceName);
        }
        public static TService GetInstance<T1, TService>(T1 arg1, string serviceName)
        {
            return container.GetInstance<T1, TService>(arg1, serviceName);
        }

        public static TService GetInstance<T1, T2, TService>(T1 arg1, T2 arg2, string serviceName)
        {
            return container.GetInstance<T1, T2, TService>(arg1, arg2, serviceName);

        }


    }
}
