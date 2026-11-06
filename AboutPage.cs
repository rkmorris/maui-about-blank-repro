namespace MauiAboutBlankRepro;

public class AboutPage : ContentPage
{
    private readonly Label _status;
    private readonly WebView _webView;
    private int _appearanceCount;
    private bool _htmlAssigned;

    public AboutPage()
    {
        Title = "About";
        BackgroundColor = Colors.Black;
        DiagnosticLog.Write("AboutPage constructed");

        _status = new Label
        {
            Text = "WebView created — diagnostics retained on device",
            TextColor = Colors.Orange,
            FontSize = 12
        };

        _webView = new WebView
        {
            BackgroundColor = Colors.Black
        };

        _webView.HandlerChanged += (_, _) => DiagnosticLog.Write($"WebView HandlerChanged: {DescribeWebView()}");
        _webView.SizeChanged += (_, _) => DiagnosticLog.Write($"WebView SizeChanged: {DescribeWebView()}");
        _webView.Navigating += (_, e) =>
        {
            _status.Text = "Navigating";
            DiagnosticLog.Write($"WebView Navigating: {e.Url}");
        };
        _webView.Navigated += (_, e) =>
        {
            _status.Text = $"Navigated: {e.Result}";
            DiagnosticLog.Write($"WebView Navigated: {e.Result}, URL={e.Url}; {DescribeWebView()}");
        };

        var recordButton = new Button { Text = "Record Blank Screen", BackgroundColor = Colors.DarkOrange, TextColor = Colors.Black };
        recordButton.Clicked += (_, _) =>
        {
            DiagnosticLog.Write($"USER REPORTED BLANK SCREEN: appearances={_appearanceCount}; {DescribeWebView()}; pageVisible={IsVisible}");
            _status.Text = "Blank screen observation saved to log";
        };

        var exportButton = new Button { Text = "Export Diagnostics" };
        exportButton.Clicked += async (_, _) =>
        {
            DiagnosticLog.Write($"User requested diagnostic export: {DescribeWebView()}");
            try
            {
                await Share.Default.RequestAsync(new ShareFileRequest
                {
                    Title = "About WebView reproduction diagnostics",
                    File = new ShareFile(DiagnosticLog.FilePath, "text/plain")
                });
            }
            catch (Exception ex)
            {
                DiagnosticLog.Write($"Export failed: {ex}");
                await DisplayAlertAsync("Export error", ex.Message, "OK");
            }
        };

        var actions = new VerticalStackLayout { Spacing = 6 };
        actions.Children.Add(_status);
        actions.Children.Add(recordButton);
        actions.Children.Add(exportButton);

        var layout = new Grid
        {
            Padding = new Thickness(12),
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition(GridLength.Star),
                new RowDefinition(GridLength.Auto)
            }
        };
        layout.Add(_webView, 0, 0);
        layout.Add(actions, 0, 1);
        Content = layout;
    }

    private string DescribeWebView() =>
        $"handler={_webView.Handler?.GetType().FullName ?? "null"}, " +
        $"platformView={_webView.Handler?.PlatformView?.GetType().FullName ?? "null"}, " +
        $"size={_webView.Width:0.#}x{_webView.Height:0.#}, " +
        $"visible={_webView.IsVisible}, loaded={_webView.IsLoaded}";

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        DiagnosticLog.Write($"AboutPage OnAppearing #{++_appearanceCount}: {DescribeWebView()}");

        if (_htmlAssigned)
            return;

        try
        {
            using var stream = await FileSystem.OpenAppPackageFileAsync("about-repro.html");
            using var reader = new StreamReader(stream);
            var html = await reader.ReadToEndAsync();
            DiagnosticLog.Write($"HTML loaded from bundle: {System.Text.Encoding.UTF8.GetByteCount(html)} bytes");
            _webView.Source = new HtmlWebViewSource { Html = html };
            _htmlAssigned = true;
            DiagnosticLog.Write("HTML assigned to WebView once");
        }
        catch (Exception ex)
        {
            DiagnosticLog.Write($"HTML loading failed: {ex}");
            _status.Text = "HTML load failed — export diagnostics";
        }
    }

    protected override void OnDisappearing()
    {
        DiagnosticLog.Write($"AboutPage OnDisappearing: {DescribeWebView()}");
        base.OnDisappearing();
    }
}
