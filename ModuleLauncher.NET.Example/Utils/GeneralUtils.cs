using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Models;

namespace ModuleLauncher.NET.Example.Utils;

public class GeneralUtils
{
    //https://stackoverflow.com/a/43232486/12167919
    public static void OpenUrl(string url)
    {
        try
        {
            Process.Start(url);
        }
        catch
        {
            // hack because of this: https://github.com/dotnet/corefx/issues/10361
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
            else
            {
                throw;
            }
        }
    }
    
    public static Window GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;

        throw new ApplicationException("Internal error");
    }

    public static async Task PromptExceptionDialog(Exception e)
    {
        var result = await MessageBoxManager.GetMessageBoxCustomWindow(new MessageBoxCustomParams
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ContentHeader = "An exception occurred",
                ContentTitle = "Oops",
                ContentMessage = e.ToString(),
                MaxHeight = 300,
                MaxWidth = 600,
                SizeToContent = SizeToContent.Height,
                ButtonDefinitions = new[]
                {
                    new ButtonDefinition {Name = "Copy"},
                    new ButtonDefinition {Name = "OK"},
                }
            })
            .ShowDialog(GetMainWindow());

        if (result == "Copy")
        {
            await Application.Current!.Clipboard!.SetTextAsync(e.ToString());
        }
    }
}