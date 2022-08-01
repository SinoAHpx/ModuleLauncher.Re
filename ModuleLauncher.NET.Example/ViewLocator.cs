using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using ModuleLauncher.NET.Example.ViewModels;

namespace ModuleLauncher.NET.Example
{
    public class ViewLocator : IDataTemplate
    {
        public IControl Build(object data)
        {
            var name = data.GetType().FullName!.Replace("ViewModels", "View")
                .Replace("VM", "View");
            var type = Type.GetType(name);

            if (type != null)
            {
                return (Control)Activator.CreateInstance(type)!;
            }

            return new TextBlock { Text = "Not Found: " + name };
        }

        public bool Match(object data)
        {
            return data is ViewModelBase;
        }
    }
}