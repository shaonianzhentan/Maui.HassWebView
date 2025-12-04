using System.Diagnostics;

#if ANDROID
using Android.Webkit;
using Java.Interop;
#endif

namespace HassWebView.Core
{
#if ANDROID
    public class HassJsBridge : Java.Lang.Object
#else
    public class HassJsBridge
#endif
    {
        public const string BridgeName = "HassJsBridge";

#if ANDROID
        [JavascriptInterface]
        [Export("OpenVideoPlayer")]
#endif
        public void OpenVideoPlayer(string url)
        {
            Debug.WriteLine($"[{GetPlatform()}] OpenVideoPlayer with url: {url}");
        }

        private string GetPlatform()
        {
#if ANDROID
            return "Android";
#elif WINDOWS
            return "Windows";
#else
            return "Generic";
#endif
        }
    }
}
