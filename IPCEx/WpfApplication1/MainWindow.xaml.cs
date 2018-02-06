using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using IOCContainer;
using MemoryMappedFileManager;
using NamedPipeLibrary;
using WpfApplication1.ViewModel;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static INamedPipeServer _namedPipeServerStream;
        private static IMemoryMapFileCommunicator _memoryMapFileCommunicator;
        public MainWindow()
        {
            InitializeComponent();
            _namedPipeServerStream = Bootstrap.GetInstance<int, INamedPipeServer>
                (10000, "NamedPipeServer");

            _memoryMapFileCommunicator = Bootstrap.GetInstance<string, long, IMemoryMapFileCommunicator>("testMMP", 4096, "MemoryMapFileCommunicator");

            DataContext = new MainWindowViewModel(_memoryMapFileCommunicator, _namedPipeServerStream);
        }
    }
}
