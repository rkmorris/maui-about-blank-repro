namespace MauiAboutBlankRepro;

public class AppShell : Shell
{
    public AppShell()
    {
        Shell.SetNavBarIsVisible(this, true);
        Items.Add(new ShellContent
        {
            Title = "Home",
            Route = "home",
            ContentTemplate = new DataTemplate(typeof(HomePage))
        });
        Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
    }
}
