namespace MauiAboutBlankRepro;

public class HomePage : ContentPage
{
    public HomePage()
    {
        Title = "Home";
        BackgroundColor = Colors.Black;
        Content = new VerticalStackLayout
        {
            Padding = new Thickness(24),
            Spacing = 18,
            Children =
            {
                new Label { Text = "About WebView reproduction", FontSize = 22, TextColor = Colors.White },
                new Label { Text = "Open About, put the app in the background, use other apps, then return", TextColor = Colors.White },
                new Button
                {
                    Text = "Open About",
                    Command = new Command(async () => await Shell.Current.GoToAsync(nameof(AboutPage)))
                }
            }
        };
    }
}
