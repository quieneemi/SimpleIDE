using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace SimpleWebStudio.Views;

public partial class InputDialog : Window
{
    private readonly TextBox _textBox;

    public InputDialog()
    {
        InitializeComponent();

        _textBox = this.FindControl<TextBox>("TextBox")!;
    }

    public InputDialog(string initText)
    {
        InitializeComponent();

        _textBox = this.FindControl<TextBox>("TextBox")!;
        _textBox.Text = initText;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void OkButton_OnClick(object? sender, RoutedEventArgs e)
        => Close(_textBox.Text);

    private void CancelButton_OnClick(object? sender, RoutedEventArgs e)
        => Close(string.Empty);
}