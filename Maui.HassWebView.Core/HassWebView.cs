namespace Maui.HassWebView.Core;

public class HassWebView : WebView
{
    public HassWebView()
    {
    }

    public static readonly BindableProperty CanGoBackProperty =
        BindableProperty.Create(nameof(CanGoBack), typeof(bool), typeof(HassWebView), false);

    public bool CanGoBack
    {
        get => (bool)GetValue(CanGoBackProperty);
        internal set => SetValue(CanGoBackProperty, value);
    }

    public static readonly BindableProperty CanGoForwardProperty =
        BindableProperty.Create(nameof(CanGoForward), typeof(bool), typeof(HassWebView), false);

    public bool CanGoForward
    {
        get => (bool)GetValue(CanGoForwardProperty);
        internal set => SetValue(CanGoForwardProperty, value);
    }


    public event EventHandler<WebNavigatingEventArgs> Navigating;
    public event EventHandler<WebNavigatedEventArgs> Navigated;

    internal void SendNavigating(WebNavigatingEventArgs args)
    {
        Navigating?.Invoke(this, args);
    }

    internal void SendNavigated(WebNavigatedEventArgs args)
    {
        Navigated?.Invoke(this, args);
    }
}