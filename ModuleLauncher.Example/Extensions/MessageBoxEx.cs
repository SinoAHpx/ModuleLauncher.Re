using System.Threading.Tasks;
using Avalonia.Controls;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Enums;

namespace ModuleLauncher.Example.Extensions
{
    public static class MessageBoxEx
    {
        public static async Task<ButtonResult> Show(string text, string title = "Message", ButtonEnum button = ButtonEnum.Ok, Icon icon = Icon.None)
        {
            var param = new MessageBoxStandardParams
            {
                ContentHeader = title,
                ContentMessage = text,
                ContentTitle = "Message",
                MaxWidth = 500,
                MaxHeight = 800,
                ButtonDefinitions = button,
                Icon = icon,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            var dialog =
                MessageBoxManager.GetMessageBoxStandardWindow(param);

            return await dialog.ShowDialog(GlobalUtility.GetMainWindow());
        }
    }
}