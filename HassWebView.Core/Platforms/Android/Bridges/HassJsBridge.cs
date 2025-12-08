using Android.Webkit;

namespace HassWebView.Core.Bridges
{
    // This is the Android-specific implementation of the HassJsBridge partial class.
    public partial class HassJsBridge : Java.Lang.Object
    {

        [JavascriptInterface]
        public void OpenVideoPlayer(string url)
        {
            VideoService.HtmlWebView(wv, url);
        }
    }
}
