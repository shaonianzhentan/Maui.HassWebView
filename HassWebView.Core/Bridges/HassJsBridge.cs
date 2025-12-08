using System;
#if ANDROID
using Android.Webkit;
using Java.Lang;
#endif

namespace HassWebView.Core.Bridges
{
#if ANDROID
    public class HassJsBridge : Java.Lang.Object
#else
    public class HassJsBridge
#endif
    {
#if ANDROID
        [JavascriptInterface]
#endif
        public void Test(string message)
        {
            Console.WriteLine($"HassJsBridge.Test called with message: {message}");
        }
    }
}
