using System.Threading.Tasks;
using Avalonia.Controls;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;

namespace ModuleLauncher.Example.Extensions
{
    public static class MessageBoxEx
    {
        public static async Task<ButtonResult> Show(string text, string title = "Message", ButtonEnum button = ButtonEnum.Ok, Icon icon = Icon.None)
        {
            var dialog = MessageBoxManager.GetMessageBoxStandardWindow(title, text, button, icon, WindowStartupLocation.CenterOwner);

            return await dialog.ShowDialog(GlobalUtility.GetMainWindow());
        }
    }
}