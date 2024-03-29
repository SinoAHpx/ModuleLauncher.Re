using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using MessageBox.Avalonia;
using MessageBox.Avalonia.DTO;
using MessageBox.Avalonia.Models;

namespace ModuleLauncher.NET.Example.Utils;

public static class GeneralUtils
{

    public static Window GetMainWindow()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            return desktop.MainWindow;

        throw new ApplicationException("Internal error");
    }

    public static async Task<string> Dialog(string content, params string[] buttons)
    {
        var buttonList = buttons.ToList();
        if (buttonList.Count == 0)
        {
            buttonList.Add("Fine");
        }
        var buttonDefinitions = buttonList.Select(x => new ButtonDefinition { Name = x });
        return await MessageBoxManager.GetMessageBoxCustomWindow(new MessageBoxCustomParams
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ContentHeader = "Example said: ",
                ContentTitle = "Prompt",
                ContentMessage = content,
                MaxHeight = 600,
                Height = 400,
                CanResize = true,
                MaxWidth = 700,
                SizeToContent = SizeToContent.Manual,
                ButtonDefinitions = buttonDefinitions
            })
            .ShowDialog(GetMainWindow());
    }

    public static async Task Exception(Exception e)
    {
        var result = await MessageBoxManager.GetMessageBoxCustomWindow(new MessageBoxCustomParams
            {
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ContentHeader = "An exception occurred",
                ContentTitle = "Oops",
                ContentMessage = e.ToString(),
                MaxHeight = 400,
                MaxWidth = 600,
                SizeToContent = SizeToContent.Height,
                CanResize = true,
                ButtonDefinitions = new[]
                {
                    new ButtonDefinition { Name = "Copy" },
                    new ButtonDefinition { Name = "OK" },
                }
            })
            .ShowDialog(GetMainWindow());

        if (result == "Copy")
        {
            await Application.Current!.Clipboard!.SetTextAsync(e.ToString());
        }
    }

    public static async Task<string?> Input(string? message = null, string? content = null)
    {
        var dialog = MessageBoxManager.GetMessageBoxInputWindow(new MessageBoxInputParams
        {
            ButtonDefinitions = new[]
            {
                new ButtonDefinition { Name = "Confirm" },
                new ButtonDefinition { Name = "Cancel" }
            },
            ContentTitle = "Additional information required",
            ContentHeader = "Input",
            ContentMessage = message ?? "Value: ",
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        });

        var result = await dialog.ShowDialog(GetMainWindow());

        if (result.Button == "Cancel")
        {
            return null;
        }

        return result.Message;
    }

    public static async Task<string?> DirectoryBrowser(string title)
    {
        var dialog = new OpenFolderDialog
        {
            Title = title
        };
        
        var result = await dialog.ShowAsync(GetMainWindow());
        return result;
    }

    public static async Task<string?> FileBrowser(string title, List<FileDialogFilter>? filters = null)
    {
        var dialog = new OpenFileDialog
        {
            Title = title,
            AllowMultiple = false,
            Filters = filters ?? new()
            {
                new() { Name = "java" },
                new() { Name = "javaw" }
            }
        };

        var result = await dialog.ShowAsync(GetMainWindow());
        return result?[0];
    }
}