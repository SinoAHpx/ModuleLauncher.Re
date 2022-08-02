using System.IO;
using ModuleLauncher.NET.Utilities;

namespace ModuleLauncher.NET.Example.ViewModels
{
    public class MainWindowVM : ViewModelBase
    {
        public ObservableCollection<Node> Items { get; }
        public ObservableCollection<Node> SelectedItems { get; }

        public MainWindowVM()
        {
            var dir = new DirectoryInfo("/home/ahpx/Documents/Minecrafts/.minecraft");

            Items = new ObservableCollection<Node>();

            var rootNode = new Node(dir);
            rootNode.SubNodes.Add(new Node(dir.Dive("versions")));

            Items.Add(rootNode);
        }

        public class Node
        {
            public ObservableCollection<Node> SubNodes { get; set; } = new();

            public DirectoryInfo Dir { get; set; }

            public Node(DirectoryInfo dir)
            {
                Dir = dir;
            }
        }
    }
}