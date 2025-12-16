using HassWebView.Core.Events;
using HassWebView.Core.Services;
using System.Diagnostics;

namespace HassWebView.Demo;

[QueryProperty(nameof(Url), "Url")]
public partial class MediaPage : ContentPage
{
    private readonly KeyService _keyService;
    public string Url { get; set; }

	public MediaPage(KeyService keyService)
	{
		InitializeComponent();
        _keyService = keyService;
        /*
        wv.Navigated += (s, e) =>
        {
            Debug.WriteLine($"WebView navigated to: {e.Url}");
            MainThread.BeginInvokeOnMainThread(() =>
            {
                wv.EvaluateJavaScriptAsync($@"
var meta = document.createElement('meta');
meta.name = 'viewport';
meta.content = 'width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no';
document.getElementsByTagName('head')[0].appendChild(meta);

document.documentElement.style.height = '{Height}px';
document.body.style.height = '{Height}px';
document.body.style.margin = '0';
document.body.style.backgroundColor = 'black';

document.body.innerHTML = `<video controls autoplay src='{Url}' style='width: 100%; height: 100%; object-fit: contain; position:fixed; top:0; left:0; background:black;'></video>`
                ");
            });
        };
        */
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        var uri = new Uri(Url);
        //wv.Source = $"{uri.Scheme}://{uri.Host}/";
        LoadUrl(Url); 
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _keyService.KeyDown += OnKeyDown;
        _keyService.SingleClick += OnSingleClick;
        _keyService.DoubleClick += OnDoubleClick;
        _keyService.LongClick += OnLongClick;
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        _keyService.KeyDown -= OnKeyDown;
        _keyService.SingleClick -= OnSingleClick;
        _keyService.DoubleClick -= OnDoubleClick;
        _keyService.LongClick -= OnLongClick;
    }

    void LoadUrl(string videoUrl)
    {
        if (string.IsNullOrEmpty(videoUrl)) return;
        Debug.WriteLine($"Loading video URL: {videoUrl}");

        string htmlContent = $@"
                <html>
                <head>
                    <meta name='viewport' content='width=device-width, initial-scale=1.0, maximum-scale=1.0'>
                    <style>
                        html,body {{ margin: 0; padding: 0; height: {wv.Height}px; background-color: black; }}
                        video {{ width: 100%; height: 100%; object-fit: contain;  }}
                    </style>
                </head>
                <body>
                    <video controls autoplay src='{videoUrl}'></video>
                </body>
                </html>";

        var uri = new Uri(videoUrl);
        var htmlSource = new HtmlWebViewSource
        {
            BaseUrl = $"{uri.Scheme}://{uri.Host}/",
            Html = htmlContent
        };
        Debug.WriteLine("Setting WebView source with HTML content.");
        wv.Source = htmlSource;
    }

    void VideoSeek(int sencond)
    {
        wv.EvaluateJavaScriptAsync($@"(function() {{
                var video = document.querySelector('video');
                if (video) video.currentTime += {sencond};
            }})()");
    }

    void PlayPause()
    {
        wv.EvaluateJavaScriptAsync(@"(function() {
                var video = document.querySelector('video');
                if (video) {
                    if (video.paused) {
                        video.play();
                    } else {
                        video.pause();
                    }
                }
            })()");
    }

    public bool OnKeyDown(object sender, RemoteKeyEventArgs args)
    {
        if (args.KeyName == "VolumeUp" || args.KeyName == "VolumeDown")
        {
            return false; // Let the system handle volume keys
        }
        return true; // We will handle all other keys
    }

    public void OnSingleClick(object sender, RemoteKeyEventArgs e)
    {
        MainThread.InvokeOnMainThreadAsync(async () =>
        {
            switch (e.KeyName)
            {
                case "Enter":
                case "DpadCenter":
                    PlayPause();
                    break;

                case "Escape":
                case "Back":
                    await Shell.Current.GoToAsync("..");
                    break;

                case "Left":
                case "DpadLeft":
                    VideoSeek(-5); 
                    break;

                case "Right":
                case "DpadRight":
                    VideoSeek(5); 
                    break;
            }
        });
    }
    
    public void OnDoubleClick(object sender, RemoteKeyEventArgs args)
    {
        // No action 
    }

    public void OnLongClick(object sender, RemoteKeyEventArgs e)
    {
        int repeatInterval = 100;
        switch (e.KeyName)
        {
            case "Left":
            case "DpadLeft":
                _keyService.StartRepeatingAction(() => VideoSeek(-15), repeatInterval);
                break;
            case "Right":
            case "DpadRight":
                _keyService.StartRepeatingAction(() => VideoSeek(15), repeatInterval);
                break;
        }
    }
}