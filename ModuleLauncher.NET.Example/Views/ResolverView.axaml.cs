﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace ModuleLauncher.NET.Example.Views;

public partial class ResolverView : UserControl
{
    public ResolverView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}