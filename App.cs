using Microsoft.Maui.Controls;

namespace MauiAboutBlankRepro;

public class App : Application
{
    public App()
    {
        DiagnosticLog.Write("Application constructed");
    }

    protected override Window CreateWindow(
        IActivationState? activationState)
    {
        DiagnosticLog.Write("Application window created");

        return new Window(new AppShell());
    }
}
