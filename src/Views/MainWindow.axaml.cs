using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;
using AvaloniaEdit;
using AvaloniaEdit.CodeCompletion;
using AvaloniaEdit.Document;
using AvaloniaEdit.TextMate;
using AvaloniaEdit.Utils;
using SimpleWebStudio.Controllers;
using SimpleWebStudio.Models;
using TextMateSharp.Grammars;
using Xilium.CefGlue.Avalonia;
using Path = System.IO.Path;

namespace SimpleWebStudio.Views;

public partial class MainWindow : Window
{
    private readonly IWebStudioController _controller;
    private readonly TreeView _explorer;
    private readonly ObservableCollection<TreeViewItem> _explorerItems;
    private readonly AvaloniaCefBrowser _webView;
    private readonly TextEditor _editor;
    private readonly RegistryOptions _registryOptions;
    private readonly TextMate.Installation _textMate;
    private CompletionWindow? _completionWindow;

    public MainWindow(IWebStudioController controller)
    {
        InitializeComponent();

        _controller = controller;

        _explorer = this.FindControl<TreeView>("Explorer")!;
        _explorer.ItemsSource = _explorerItems = new ObservableCollection<TreeViewItem>();

        _webView = new AvaloniaCefBrowser();
        this.FindControl<Decorator>("WebView")!.Child = _webView;

        _editor = this.FindControl<TextEditor>("Editor")!;
        _editor.DocumentChanged += Editor_OnDocumentChanged;
        _editor.TextArea.KeyDown += (sender, e) =>
        {
            if (e is { Key: Key.S, KeyModifiers: KeyModifiers.Control })
                _ = HandleEditorSaveMenuItemClickedAsync();
        };
        _editor.TextArea.TextEntering += Editor_OnTextEntering;
        _editor.TextArea.TextEntered += Editor_OnTextEntered;

        _registryOptions = new RegistryOptions(ThemeName.LightPlus);
        _textMate = _editor.InstallTextMate(_registryOptions);

        this.FindControl<Button>("ThemeButton")!.ContextMenu = new ContextMenu
        {
            Placement = PlacementMode.RightEdgeAlignedTop,
            ItemsSource = Enum.GetNames(typeof(ThemeName))
                .Select(themeName =>
                {
                    var item = new MenuItem { Header = themeName };
                    item.Click += (sender, args) => UpdateTheme(themeName);
                    return item;
                })
        };
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        _textMate.Dispose();
    }

    private async Task HandleEditorSaveMenuItemClickedAsync()
    {
        var path = _editor.Document.FileName;
        if (string.IsNullOrWhiteSpace(path)) return;

        await File.WriteAllTextAsync(path, _editor.Document.Text);

        if (Path.GetExtension(path).ToLower() is ".html" or ".css" or ".js")
            _webView.Reload();
    }

    [Obsolete("Obsolete")]
    private void OpenFileMenuItem_OnClick(object? sender, RoutedEventArgs e)
        => _ = HandleOpenFileMenuItemClickedAsync();

    [Obsolete("Obsolete")]
    private void OpenFolderMenuItem_OnClick(object? sender, RoutedEventArgs e)
        => _ = HandleOpenFolderMenuItemClickedAsync();

    private void Explorer_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
        => _ = HandleExplorerSelectionChangedAsync();

    private void ExplorerButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var splitter = this.FindControl<GridSplitter>("ExplorerSplitter")!;
        var grid = this.FindControl<Grid>("LayoutGrid")!;
        if (grid.ColumnDefinitions[0].Width == GridLength.Auto)
        {
            grid.ColumnDefinitions[0].Width = GridLength.Parse("0");
            splitter.IsVisible = false;
        }
        else
        {
            grid.ColumnDefinitions[0].Width = GridLength.Auto;
            splitter.IsVisible = true;
        }
    }

    private void WebViewButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var splitter = this.FindControl<GridSplitter>("WebViewSplitter")!;
        var grid = this.FindControl<Grid>("LayoutGrid")!;
        if (grid.ColumnDefinitions[4].Width == GridLength.Star)
        {
            grid.ColumnDefinitions[4].Width = GridLength.Parse("0");
            splitter.IsVisible = false;
        }
        else
        {
            grid.ColumnDefinitions[4].Width = GridLength.Star;
            splitter.IsVisible = true;
        }
    }

    private void ThemeButton_OnClick(object? sender, RoutedEventArgs e)
    {
        if (Application.Current!.RequestedThemeVariant == ThemeVariant.Dark)
        {
            _textMate.SetTheme(_registryOptions.LoadTheme(ThemeName.LightPlus));
            Application.Current.RequestedThemeVariant = ThemeVariant.Light;
        }
        else
        {
            _textMate.SetTheme(_registryOptions.LoadTheme(ThemeName.DarkPlus));
            Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
        }
    }

    private void Editor_OnDocumentChanged(object? sender, DocumentChangedEventArgs e)
    {
        var filePath = _editor.Document.FileName;
        var fileExtension = Path.GetExtension(filePath);

        if (string.IsNullOrWhiteSpace(fileExtension)) return;

        var scope = _registryOptions.GetScopeByExtension(fileExtension);
        _textMate.SetGrammar(scope);

        if (fileExtension.ToLower().Equals(".html"))
            _webView.Address = filePath;
    }

    [Obsolete("Obsolete")]
    private async Task HandleOpenFileMenuItemClickedAsync()
    {
        var dialog = new OpenFileDialog();
        var path = await dialog.ShowAsync(this);

        if (path is null || string.IsNullOrWhiteSpace(path[0])) return;

        var explorerItem = CreateFileTreeViewItem(path[0]);
        _explorerItems.Clear();
        _explorerItems.Add(explorerItem);
        _explorer.SelectedItem = explorerItem;
    }

    [Obsolete("Obsolete")]
    private async Task HandleOpenFolderMenuItemClickedAsync()
    {
        var dialog = new OpenFolderDialog();
        var path = await dialog.ShowAsync(this);

        if (path is null) return;

        var root = CreateFolderTreeViewItem(path);
        root.IsExpanded = true;
        PopulateTreeView(path, root);
        _explorerItems.Clear();
        _explorerItems.Add(root);
    }

    private async Task HandleExplorerSelectionChangedAsync()
    {
        var item = (TreeViewItem)_explorer.SelectedItem!;
        var node = (ExplorerNode)item.Tag!;

        await LoadDocumentToEditor(node.FilePath);
    }

    private async Task LoadDocumentToEditor(string path)
    {
        var text = await File.ReadAllTextAsync(path);
        _editor.Document = new TextDocument(text) { FileName = path };
    }

    private void UpdateTheme(string themeName)
    {
        var theme = (ThemeName)Enum.Parse(typeof(ThemeName), themeName);
        _textMate.SetTheme(_registryOptions.LoadTheme(theme));

        var currentTheme = Application.Current!.RequestedThemeVariant!;
        if (themeName.Contains("Light"))
        {
            if (currentTheme == ThemeVariant.Dark)
                Application.Current.RequestedThemeVariant = ThemeVariant.Light;
        }
        else
        {
            if (currentTheme == ThemeVariant.Light)
                Application.Current.RequestedThemeVariant = ThemeVariant.Dark;
        }
    }

    private void PopulateTreeView(string path, TreeViewItem root)
    {
        try
        {
            var children = new List<TreeViewItem>();

            var directories = Directory.GetDirectories(path);
            foreach (var directory in directories)
            {
                if (directory.StartsWith('.')) continue;

                var item = CreateFolderTreeViewItem(directory);
                children.Add(item);

                PopulateTreeView(directory, item);
            }

            children.AddRange(Directory.GetFiles(path)
                .Select(CreateFileTreeViewItem));

            root.ItemsSource = children;
        }
        catch
        {
            // ignored
        }
    }

    private TreeViewItem CreateFileTreeViewItem(string path)
    {
        var node = new ExplorerNode(path, NodeType.File);
        var item = new TreeViewItem { Header = node.FileName, Tag = node };

        var renameFileMenuItem = new MenuItem { Header = "Rename" };
        renameFileMenuItem.Click += (sender, args) => _ = HandleRenameFileMenuItemClickedAsync(item);

        item.ContextMenu = new ContextMenu { ItemsSource = new[] { renameFileMenuItem } };

        return item;
    }

    private async Task HandleRenameFileMenuItemClickedAsync(TreeViewItem item)
    {
        var node = (ExplorerNode)item.Tag!;
        var newFileName = await new InputDialog(node.FileName).ShowDialog<string>(this);
        if (string.IsNullOrWhiteSpace(newFileName)) return;

        var newFilePath = _controller.RenameFile(node.FilePath, newFileName);
        if (string.IsNullOrWhiteSpace(newFilePath)) return;

        if (_editor.Document.FileName == node.FilePath)
            await LoadDocumentToEditor(newFilePath);

        item.Header = newFileName;
        node.FilePath = newFilePath;
        node.FileName = newFileName;
        item.Tag = node;
    }

    private TreeViewItem CreateFolderTreeViewItem(string path)
    {
        var node = new ExplorerNode(path, NodeType.Directory);
        var item = new TreeViewItem { Header = node.FileName, Tag = node };

        var newFileMenuItem = new MenuItem { Header = "New File" };
        newFileMenuItem.Click += (sender, args) => _ = HandleNewFileMenuItemClickedAsync(item);

        var newFolderMenuItem = new MenuItem { Header = "New Folder" };
        newFolderMenuItem.Click += (sender, args) => _ = HandleNewFolderMenuItemClickedAsync(item);

        item.ContextMenu = new ContextMenu { ItemsSource = new[] { newFileMenuItem, newFolderMenuItem } };

        return item;
    }

    private async Task HandleNewFileMenuItemClickedAsync(TreeViewItem root)
    {
        var node = (ExplorerNode)root.Tag!;
        var newFileName = await new InputDialog().ShowDialog<string>(this);
        if (string.IsNullOrWhiteSpace(newFileName)) return;

        if (_controller.CreateFile(node.FilePath, newFileName))
            PopulateTreeView(node.FilePath, root);
    }

    private async Task HandleNewFolderMenuItemClickedAsync(TreeViewItem root)
    {
        var node = (ExplorerNode)root.Tag!;
        var newFolderName = await new InputDialog().ShowDialog<string>(this);
        if (string.IsNullOrWhiteSpace(newFolderName)) return;

        if (_controller.CreateDirectory(node.FilePath, newFolderName))
            PopulateTreeView(node.FilePath, root);
    }

    private void RefreshExplorerMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (_explorerItems.Count == 0) return;

        var root = _explorerItems[0];
        var node = (ExplorerNode)root.Tag!;
        PopulateTreeView(node.FilePath, root);
    }

    private void RefreshWebViewMenuItem_OnClick(object? sender, RoutedEventArgs e)
        => _webView.Reload();

    private void Editor_OnTextEntering(object? sender, TextInputEventArgs e)
    {
        if (e.Text is not { Length: > 0 } || _completionWindow is null) return;

        if (!char.IsLetterOrDigit(e.Text[0]))
            _completionWindow.CompletionList.RequestInsertion(e);
    }

    private void Editor_OnTextEntered(object? sender, TextInputEventArgs e)
    {
        if (_completionWindow is not null || string.IsNullOrWhiteSpace(e.Text)) return;

        var suggestions = _controller
            .GetSuggestions(e.Text, _editor.Document.FileName)
            .ToArray();
        if (suggestions.Length == 0) return;

        _completionWindow = new CompletionWindow(_editor.TextArea);
        _completionWindow.Closed += (o, args) => _completionWindow = null;
        _completionWindow.CompletionList.CompletionData.AddRange(suggestions);
        _completionWindow.Show();
    }

    private void HelpButton_OnClick(object? sender, RoutedEventArgs e)
        => _ = HandleHelpButtonClicked();

    private async Task HandleHelpButtonClicked()
        => await new HelpWindow().ShowDialog(this);
}