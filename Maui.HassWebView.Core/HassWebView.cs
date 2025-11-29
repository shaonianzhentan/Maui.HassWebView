using System.Collections.Generic;

namespace Maui.HassWebView.Core;

public class HassWebView : WebView
{
    public HassWebView()
    {
        JsBridges = new Dictionary<string, object>();
    }

    public static readonly BindableProperty JsBridgesProperty =
        BindableProperty.Create(nameof(JsBridges), typeof(IDictionary<string, object>), typeof(HassWebView), null);

    public IDictionary<string, object> JsBridges
    {
        get => (IDictionary<string, object>)GetValue(JsBridgesProperty);
        set => SetValue(JsBridgesProperty, value);
    }

    public static readonly BindableProperty EnableZoomProperty =
        BindableProperty.Create(nameof(EnableZoom), typeof(bool), typeof(HassWebView), true);

    public bool EnableZoom
    {
        get => (bool)GetValue(EnableZoomProperty);
        set => SetValue(EnableZoomProperty, value);
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