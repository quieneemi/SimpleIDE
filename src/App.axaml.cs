using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using SimpleWebStudio.Controllers;
using SimpleWebStudio.Services;
using MainWindow = SimpleWebStudio.Views.MainWindow;

namespace SimpleWebStudio;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var codeCompletionService = new CodeCompletionService();
            var fileService = new FileService();
            var controller = new WebStudioController(codeCompletionService, fileService);
            desktop.MainWindow = new MainWindow(controller);
        }

        base.OnFrameworkInitializationCompleted();
    }
}