using Android.Graphics;
using Com.Tencent.Smtt.Export.External.Interfaces;
using Com.Tencent.Smtt.Sdk;

namespace HassWebView.Core.Platforms.Android;
public class WebViewClientHandler : WebViewClient
{
    private readonly HassWebView _webView;

    public WebViewClientHandler(HassWebView webView)
    {
        _webView = webView;
    }

    public override bool ShouldOverrideUrlLoading(global::Com.Tencent.Smtt.Sdk.WebView view, IWebResourceRequest request)
    {
        var url = request.Url.ToString();
        var args = new WebNavigatingEventArgs(WebNavigationEvent.NewPage, new UrlWebViewSource{ Url = url }, url);
        _webView.SendNavigating(args);

        if (args.Cancel)
        {
            return true;
        }

        view.LoadUrl(url);
        return true;
    }

    public override void OnPageFinished(global::Com.Tencent.Smtt.Sdk.WebView view, string url)
    {
        base.OnPageFinished(view, url);
        _webView.SendNavigated(new WebNavigatedEventArgs(WebNavigationEvent.NewPage, new UrlWebViewSource { Url = url }, url, WebNavigationResult.Success));
        _webView.CanGoBack = view.CanGoBack();
        _webView.CanGoForward = view.CanGoForward();
    }
}